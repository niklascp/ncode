using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.JobScheduling
{
    /// <summary>
    /// Represents a Scheduled Recurring Job.
    /// </summary>
    public class ScheduledJob
    {
        /// <summary>
        /// Gets or set a unique identifier of the job.
        /// </summary>
        [JsonProperty("jobId")]
        public string JobId { get; set; }

        /// <summary>
        /// Gets or set the type name of the job.
        /// </summary>
        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        /// <summary>
        /// Gets or set the Cron-expression that represents the schedule of the job.
        /// </summary>
        [JsonProperty("cronExpression")]
        public string CronExpression { get; set; }
    }
}
