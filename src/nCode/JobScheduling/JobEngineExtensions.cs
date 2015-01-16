using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.JobScheduling
{
    public static class JobEngineExtensions
    {
        public static string Queue(this JobEngine jobEngine, Type backgroundJobType, string queue = null)
        {
            if (backgroundJobType == null)
                throw new ArgumentNullException("backgroundJobType");

            if (!(typeof(IBackgroundJob).IsAssignableFrom(backgroundJobType)))
                throw new ArgumentException(string.Format("Expecting type '{0}' to implement IBackgroundJob.", backgroundJobType), "backgroundJobType");

            var method = jobEngine.GetType().GetMethod("Queue", new Type[] { typeof(string) });
            var genericMethod = method.MakeGenericMethod(backgroundJobType);
            return (string)genericMethod.Invoke(jobEngine, new[] { queue });
        }
    }
}
