using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.JobScheduling
{
    /// <summary>
    /// Represents data about a Job Execution.
    /// </summary>
    public class JobExecution
    {
        /// <summary>
        /// Gets or set a unique identifier of the job execution.
        /// </summary>
        [JsonProperty("executionId")]
        public string ExecutionId { get; set; }

        /// <summary>
        /// Gets or set the job name of the job execution.
        /// </summary>
        [JsonProperty("jobName")]
        public string JobName { get; set; }

        /// <summary>
        /// Gets or set the timestamp for the creation of the job execution.
        /// </summary>
        [JsonProperty("createDateTime")]
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// Gets or set the timestamp for the creation of the job execution, if it has started.
        /// </summary>
        [JsonProperty("startDateTime")]
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// Gets or set the assembly qualified type name of the job execution.
        /// </summary>
        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or set the type name of the job.
        /// </summary>
        [JsonProperty("state")]
        public JobExecutionState State { get; set; }
    }

    public enum JobExecutionState
    {
        Planned = 0,
        Executing = 1,
        Succeded = 2,
        Failed = 3
    }
}
