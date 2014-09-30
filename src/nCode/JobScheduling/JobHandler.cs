using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.JobScheduling
{
    public static class JobHandler
    {
        private static object syncRoot;
        private static bool isInitialized;
        private static JobEngine engine;

        static JobHandler()
        {
            syncRoot = new object();
            isInitialized = false;
        }

        public static bool IsInitialized { get { return isInitialized; } }

        public static void Initialize(JobEngine jobEngine)
        {
            if (jobEngine == null)
                throw new ArgumentNullException("jobEngine");

            Log.Info("Initializing Job Engine: " + jobEngine.GetType().FullName + ".");

            lock (syncRoot)
            {
                if (isInitialized)
                    throw new ApplicationException("The Job Engine has already been initialized.");

                engine = jobEngine;
                isInitialized = true;

                Log.Info("Job Engine Initialized.");
            }
        }

        public static JobEngine Engine
        {
            get { return engine; }
        }
    }
}
