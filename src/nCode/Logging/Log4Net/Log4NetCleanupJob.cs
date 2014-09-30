using nCode.JobScheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

using Common.Logging;

namespace nCode.Logging.Log4Net
{
    /// <summary>
    /// A Background Job that Deletes old log files
    /// </summary>
    public class Log4NetCleanupJob : IBackgroundJob<Log4NetCleanupJob.JobParameters>
    {
        public void Execute(Log4NetCleanupJob.JobParameters parameters)
        {
            Log.Info("Log4NetCleanupJob Starting with parameters:");
            Log.Info("    - PhysicalPath : " + parameters.PhysicalPath);
            Log.Info("    - MaxAge       : " + parameters.MaxAge);

            DirectoryInfo di = new DirectoryInfo(parameters.PhysicalPath);
            if (!di.Exists)
            {
                Log.Info(string.Format("Log4NetCleanupJob Aborted: Directory: '{0}' does not exists.", di.FullName));
                return;
            }

            var deleted = 0;
            var processed = 0;
            var logFilePattern = new Regex(@"Log(?<yyyy>\d{4})(?<MM>\d{2})(?<dd>\d{2}).log");
            foreach (var logFile in di.GetFiles("Log*.log"))
            {
                /* Skip the current log file. */
                if (logFile.Name.Equals("Log.log", StringComparison.OrdinalIgnoreCase))
                    continue;

                var match = logFilePattern.Match(logFile.Name);
                if (match.Success)
                {
                    processed++;

                    var timestamp = new DateTime(int.Parse(match.Groups["yyyy"].Value), int.Parse(match.Groups["MM"].Value), int.Parse(match.Groups["dd"].Value));
                    if (timestamp + parameters.MaxAge < DateTime.Now) {
                        logFile.Delete();
                        deleted++;
                    }
                }
                else
                {
                    Log.Warn(string.Format("Log4NetCleanupJob: Unexpected log file pattern: '{0}'.", logFile.Name));
                }
            }

            Log.Info(string.Format("Log4NetCleanupJob Completed: Processed: {0} log files, Deleted {1} log files.", processed, deleted));
        }

        public class JobParameters : IBackgroundJobParameters
        {
            public JobParameters()
            {
                MaxAge = TimeSpan.FromDays(30);
            }

            public string PhysicalPath { get; set; }
            public TimeSpan MaxAge { get; set; }
        }
    }

    public class SleepJob : IBackgroundJob<SleepJob.JobParameters>
    {
        public void Execute(JobParameters parameters)
        {
            Log.Info("SleepJob: Going to sleep for: " + parameters.SleepTimeMs + " ms.");
            System.Threading.Thread.Sleep(parameters.SleepTimeMs);
            Log.Info("SleepJob: Woke from sleep.");            
        }

        public class JobParameters : IBackgroundJobParameters
        {
            public int SleepTimeMs { get; set; }
        }
    }


}
