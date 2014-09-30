using Hangfire.Client;
using Hangfire.Common;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.JobScheduling.Hangfire
{
    /// <summary>
    /// This filter is used for the Integration between the nCode Job Scheduling Framework,
    /// and the Hangfire JobEngine implementation (<see cref="nCode.JobScheduling.Hangfire.HangfireJobEngine"/>).
    /// 
    /// The filter is automatically added to the Global Filters collection <see cref="Hangfire.GlobalJobFilters"/>
    /// when the Hangfire Job Engine is initialized.
    /// </summary>
    public class JobSchedulingIntegrationAttribute : JobFilterAttribute, IClientFilter
    {
        /// <summary>
        /// Called before the creation of the job.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnCreating(CreatingContext filterContext)
        {
            if (filterContext == null) throw new ArgumentNullException("filterContext");
            
            filterContext.SetJobParameter("JobName", filterContext.Job.Type.FullName);
        }

        /// <summary>
        /// Called after the creation of the job.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public void OnCreated(CreatedContext filterContext)
        {

        }
    }
}
