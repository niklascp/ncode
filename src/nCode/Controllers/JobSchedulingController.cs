using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using nCode.JobScheduling;

namespace nCode.Controllers
{
    /// <summary>
    /// Controller API for Job Scheduling.
    /// </summary>
    [RoutePrefix("admin/system/jobs")]
    public class JobSchedulingController : ApiController
    {
        /// <summary>
        /// Gets a list of Planned and Completed Job Executions.
        /// </summary>
        [Route("executions")]
        public IEnumerable<JobExecution> GetJobExecutions(int offset = 0, int count = 50)
        {
            if (!JobHandler.IsInitialized)
                return null;

            return JobHandler.Engine.GetJobExecutions(offset, count);
        }

        /// <summary>
        /// Get a list of Scheduled Jobs.
        /// </summary>
        [Route("scheduled")]
        public IEnumerable<ScheduledJob> GetScheduledJobs()
        {
            if (!JobHandler.IsInitialized)
                return null;

            return JobHandler.Engine.GetScheduledJobs();
        }

        /// <summary>
        /// Get a list of Scheduled Jobs.
        /// </summary>
        [Route("{jobId}"), HttpDelete]
        public bool? DeleteJob(string jobId)
        {
            if (!JobHandler.IsInitialized)
                return null;

            return JobHandler.Engine.Delete(jobId);
        }

    }
}
