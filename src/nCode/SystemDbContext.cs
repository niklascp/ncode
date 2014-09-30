using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

using nCode.Metadata.Model;

namespace nCode
{
    public class SystemDbContext : DbContext
    {
        static SystemDbContext()
        {
            Database.SetInitializer<SystemDbContext>(null);
        }

        public SystemDbContext()
            : base(Settings.ConnectionString)
        {
        }

        public DbSet<MetadataDescriptorEntity> MetadataDescriptors { get; set; }

        public DbSet<MetadataPropertyEntity> MetadataProperties { get; set; }

    }
}
