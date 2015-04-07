using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.JobScheduling
{
    /// <summary>
    /// Represents the Interface for a Job Engine.
    /// </summary>
    public abstract class JobEngine
    {
        /// <summary>
        /// Enqueue a background job to run immidiately on any worker node.
        /// </summary>
        /// <typeparam name="T">The Type og the Background Job.</typeparam>
        /// <returns>A job id that identifies the job enqueued.</returns>
        public abstract string Queue<T>(string queue = null)
            where T : IBackgroundJob;

        public abstract string Queue<T, P>(P parameters, string queue = null)
            where T : IBackgroundJob<P>
            where P : IBackgroundJobParameters;

        public abstract void Schedule<T>(string jobId, string cronExpression, string queue = null)
            where T : IBackgroundJob;

        public abstract void Schedule<T, P>(string jobId, P parameters, string cronExpression, string queue = null)
            where T : IBackgroundJob<P>
            where P : IBackgroundJobParameters;

        /// <summary>
        /// Deletes the job given by the job id.
        /// </summary>
        public abstract bool Delete(string jobId);

        /// <summary>
        /// Gets a list of Scheduled Jobs.
        /// </summary>
        public abstract IEnumerable<ScheduledJob> GetScheduledJobs();
        
        /// <summary>
        /// Gets a list of planned and completed Job Executions.
        /// </summary>
        public abstract IEnumerable<JobExecution> GetJobExecutions(int offset, int count);

        public virtual string LocalQueue
        {
            get { return Environment.MachineName; }
        }
    }
}
