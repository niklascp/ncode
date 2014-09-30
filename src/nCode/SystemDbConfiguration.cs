using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode
{
    public sealed class SystemDbConfiguration : DbMigrationsConfiguration<SystemDbContext>
    {
        public SystemDbConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
