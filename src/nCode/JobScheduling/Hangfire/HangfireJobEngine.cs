using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;

using Hangfire;
using Hangfire.Common;
using Hangfire.SqlServer;
using Hangfire.Storage;

using Dapper;
using Newtonsoft.Json;
using Hangfire.States;
using System.Text.RegularExpressions;

namespace nCode.JobScheduling.Hangfire
{
    /// <summary>
    /// Represent a Job Engine implementation using the Hangfire Job Framework.
    /// </summary>
    public class HangfireJobEngine : JobEngine
    {
        private string localQueue;
        private JobStorage jobStorage;
        private BackgroundJobServer backgroundJobServer;
        private BackgroundJobClient backgroundJobClient;
        private RecurringJobManager jobManager;

        /*
        private static Job DeserializeJob(string invocationData, string arguments)
        {
            var data = JobHelper.FromJson<InvocationData>(invocationData);
            data.Arguments = arguments;

            try
            {
                return data.Deserialize();
            }
            catch (JobLoadException)
            {
                return null;
            }
        }

        private static JobList<TDto> DeserializeJobs<TDto>(
            ICollection<SqlJob> jobs,
            Func<SqlJob, Job, Dictionary<string, string>, TDto> selector)
        {
            var result = new List<KeyValuePair<string, TDto>>(jobs.Count);

            foreach (var job in jobs)
            {
                var stateData = JobHelper.FromJson<Dictionary<string, string>>(job.StateData);
                var dto = selector(job, DeserializeJob(job.InvocationData, job.Arguments), stateData);

                result.Add(new KeyValuePair<string, TDto>(
                    job.Id.ToString(), dto));
            }

            return new JobList<TDto>(result);
        }
        */

        /// <summary>
        /// Initializes a new Hangfire Job Engine.
        /// </summary>
        /// <param name="useServer"></param>
        /// <param name="queues"></param>
        public HangfireJobEngine(bool useServer = true, string[] queues = null)
        {
            localQueue = Regex.Replace(Environment.MachineName.ToLower(), @"[^a-z0-9_]", string.Empty);

            // TODO: Should we instead create a custom implementation of
            // IJobCreationProcess so we do not pollute the GlobalFilters. 
            GlobalJobFilters.Filters.Add(new JobSchedulingIntegrationAttribute());

            jobStorage = new SqlServerStorage(SqlUtilities.ConnectionString, new SqlServerStorageOptions()
            {
                PrepareSchemaIfNecessary = true
            });

            if (useServer)
            {
                if (queues == null) 
                    queues = new[] { EnqueuedState.DefaultQueue, localQueue };

                if (queues.Length == 0) throw new ArgumentException("You should specify at least one queue to listen.", "queues");

                backgroundJobServer = new BackgroundJobServer(new BackgroundJobServerOptions()
                {
                    Queues = queues
                }, jobStorage);

                backgroundJobServer.Start();
            }

            backgroundJobClient = new BackgroundJobClient(jobStorage);
            jobManager = new RecurringJobManager(jobStorage);
        }

        public override string Queue<T>(string queue = null)
        {
            var state = new EnqueuedState();
            if (queue != null)
                state.Queue = queue;

            return backgroundJobClient.Create<T>(x => x.Execute(), state);
        }

        public override string Queue<T, P>(P parameters, string queue = null)
        {
            var state = new EnqueuedState();
            if (queue != null)
                state.Queue = queue;

            return backgroundJobClient.Create<T>(x => x.Execute(parameters), state);
        }

        public override void Schedule<T>(string jobId, string cronExpression, string queue = null)
        {
            var job = Job.FromExpression<T>(x => x.Execute());
            jobManager.AddOrUpdate(jobId, job, cronExpression);
        }

        public override void Schedule<T, P>(string jobId, P parameters, string cronExpression, string queue = null)
        {
            var job = Job.FromExpression<T>(x => x.Execute(parameters));
            jobManager.AddOrUpdate(jobId, job, cronExpression);
        }

        public override bool Delete(string jobId)
        {
            return backgroundJobClient.Delete(jobId);
        }


        public override IEnumerable<ScheduledJob> GetScheduledJobs()
        {
            using (var connection = jobStorage.GetConnection())
            {
                var jobIds = connection.GetAllItemsFromSet("recurring-jobs");

                foreach (var recurringJobId in jobIds)
                {
                    var recurringJob = connection.GetAllEntriesFromHash(string.Format("recurring-job:{0}", recurringJobId));

                    var invocationData = JobHelper.FromJson<InvocationData>(recurringJob["Job"]);

                    yield return new ScheduledJob()
                    {
                        JobId = recurringJobId,
                        CronExpression = recurringJob["Cron"],
                        TypeName = invocationData.Type
                        //invocationData.Arguments
                    };
                }
            }
        }

        public override IEnumerable<JobExecution> GetJobExecutions(int offset, int count)
        {
            using (var connection = new SqlConnection(SqlUtilities.ConnectionString))
            {
                const string jobsSql = @"
select * from (
  select j.*, p.Value as JobName, s.Reason as StateReason, s.Data as StateData, row_number() over (order by j.Id desc) as row_num
  from HangFire.Job j
  left join HangFire.State s on j.StateId = s.Id
  left join HangFire.JobParameter p on p.JobId = j.Id and p.Name = 'JobName'
) as j where j.row_num between @start and @end
";

                var jobs = connection.Query<SqlJob>(
                            jobsSql,
                            new { start = offset + 1, end = offset + count })
                            .ToList();

                foreach (var job in jobs)
                {
                    var invocationData = JobHelper.FromJson<InvocationData>(job.InvocationData);

                    var execution = new JobExecution()
                    {
                        JobId = job.Id.ToString(),
                        JobName = job.JobName != null ? JsonConvert.DeserializeObject<string>(job.JobName) : null,
                        TypeName = invocationData.Type,
                        CreateDateTime = job.CreatedAt,
                    };

                    if (job.StateName == "Scheduled")
                    {
                        execution.State = JobExecutionState.Planned;
                    }
                    else if (job.StateName == "Enqueued")
                    {
                        execution.State = JobExecutionState.Planned;
                    }
                    else if (job.StateName == "Processing")
                    {
                        execution.State = JobExecutionState.Executing;
                    }
                    else if (job.StateName == "Succeeded")
                    {
                        execution.State = JobExecutionState.Succeded;
                        //{"SucceededAt":"2014-09-28T12:24:14.8920751Z","PerformanceDuration":"43","Latency":"301"}
                    }
                    else if (job.StateName == "Failed")
                    {
                        execution.State = JobExecutionState.Failed;
                    }
                    else if (job.StateName == "Deleted")
                    {
                        execution.State = JobExecutionState.Canceled;
                    }

                    yield return execution; 
                }
            }
        }

        public override string LocalQueue { get { return localQueue; } }
    }

    internal class SqlJob
    {
        public int Id { get; set; }
        public string InvocationData { get; set; }
        public string Arguments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpireAt { get; set; }

        public string JobName { get; set; }

        public string StateName { get; set; }
        public string StateReason { get; set; }
        public string StateData { get; set; }
    }
}
