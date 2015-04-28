using nCode.Data;
using nCode.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.CMS.Models
{
    /// <summary>
    /// Represents the CMS database.
    /// </summary>
    public class CmsDbContext : DbContext
    {
        static CmsDbContext()
        {
            // don't let EF modify the database schema...
            Database.SetInitializer<CmsDbContext>(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CmsDbContext"/> class.
        /// </summary>
        public CmsDbContext()
            : base("Web")
        {

        }

        /// <summary>
        /// Gets the set of Content Parts.
        /// </summary>
        [ForeignEntityAtrribute]        
        public DbSet<ContentBlock> ContentBlocks { get; set; }

        /// <summary>
        /// Gets the set of Content Pages.
        /// </summary>
        public DbSet<ContentPageEntity> ContentPages { get; set; }

        /// <summary>
        /// Gets the set of Content Parts.
        /// </summary>
        public DbSet<ContentPart> ContentParts { get; set; }

        /// <summary>
        /// Gets the set of Content Parts.
        /// </summary>
        public DbSet<ContentPartInstance> ContentPartInstances { get; set; }

        /// <summary>
        /// Gets the set of Content Part Properties.
        /// </summary>
        public DbSet<ContentPartProperty> ContentPartProperties { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

        }
    }

}
