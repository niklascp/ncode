using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using nCode.JobScheduling;
using nCode.Search;

namespace nCode.Controllers
{
    /// <summary>
    /// Controller API for Job Scheduling.
    /// </summary>
    [RoutePrefix("admin/system/search")]
    public class SearchController : ApiController
    {
        /// <summary>
        /// Gets a list of Planned and Completed Job Executions.
        /// </summary>
        [Route("reindex/{sourceGuid}")]
        [HttpPost]
        public bool Reindex(Guid sourceGuid)
        {
            if (!JobHandler.IsInitialized)
                return false;

            var queue = JobHandler.Engine.LocalQueue;
            JobHandler.Engine.Queue<ReindexSourceJob, ReindexSourceJobParameters>(new ReindexSourceJobParameters
            {
                SourceGuid = sourceGuid
            }, queue);

            return true;
        }


    }
}
