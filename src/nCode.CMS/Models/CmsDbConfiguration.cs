using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.CMS.Models
{
    public sealed class CmsDbConfiguration : DbMigrationsConfiguration<CmsDbContext>
    {
        public CmsDbConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }
    }
}
