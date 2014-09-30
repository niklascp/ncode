using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data
{
    /// <summary>
    /// Contains custom extensions for the Entity Framework.
    /// </summary>
    public static class DbConfigurationExtensions
    {
        /// <summary>
        /// Updates a DbMigrationsConfiguration to its latest migrating, logging the process.
        /// </summary>
        public static void UpdateWithLog(this DbMigrationsConfiguration dbMigrationsConfiguration)
        {
            var migrator = new DbMigrator(dbMigrationsConfiguration);
            var migratorLoggingDecorator = new MigratorLoggingDecorator(migrator, new LogMigrationsLogger());
            migratorLoggingDecorator.Update();
        }
    }
}
