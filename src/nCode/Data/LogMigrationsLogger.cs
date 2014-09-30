using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace nCode.Data
{
    public class LogMigrationsLogger : MigrationsLogger
    {
        private readonly CompilationSection compilationSection;

        public LogMigrationsLogger()
        {
            compilationSection = ConfigurationManager.GetSection("system.web/compilation") as CompilationSection;
        }

        public override void Info(string message)
        {
            Log.Info(message);
        }

        public override void Verbose(string message)
        {
            if (compilationSection.Debug)
                Log.Info(message);
        }

        public override void Warning(string message)
        {
            Log.Warn(message);
        }
    }
}
