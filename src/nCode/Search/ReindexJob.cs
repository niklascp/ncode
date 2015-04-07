using Common.Logging;
using nCode.JobScheduling;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Search
{
    /// <summary>
    /// A Background Job that Reindexes a single search source, specified by the source id.
    /// </summary>
    public class ReindexSourceJob : IBackgroundJob<ReindexSourceJobParameters>
    {
        public void Execute(ReindexSourceJobParameters parameters)
        {
            var log = LogManager.GetLogger<ReindexSourceJob>();
            var searchSource = SearchHandler.Sources.Where(x => x.SourceGuid == parameters.SourceGuid).SingleOrDefault();

            if (searchSource == null)
            {
                log.Warn(string.Format("ReindexSourceJob failed for the SourceGuid '{0}': No source found with this Guid.", parameters.SourceGuid));
                return;
            }

            log.Info(string.Format("ReindexSourceJob starting update of Source with SourceGuid '{0}' (of type '{1}').", parameters.SourceGuid, searchSource.GetType().AssemblyQualifiedName));

            searchSource.UpdateIndex();

            log.Info(string.Format("ReindexSourceJob completed update of Source with SourceGuid '{0}' (of type '{1}').", parameters.SourceGuid, searchSource.GetType().AssemblyQualifiedName));
        }
    }

    /// <summary>
    /// Background Job Parameters for Reindex Source Jobs.
    /// </summary>
    public class ReindexSourceJobParameters : IBackgroundJobParameters
    {
        [JsonProperty("sourceGuid")]
        public Guid SourceGuid { get; set; }
    }
}
