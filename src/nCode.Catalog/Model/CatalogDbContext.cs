using nCode.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Model
{
    /// <summary>
    /// Entity Framework DbContext for the Catalog module.
    /// </summary>
    public class CatalogDbContext : DbContext
    {
        static CatalogDbContext()
        {
            // don't let EF modify the database schema...
            Database.SetInitializer<CatalogDbContext>(null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogDbContext"/> class.
        /// </summary>
        public CatalogDbContext()
            : base(SqlUtilities.ConnectionString)
        { }

        /// <summary>
        /// Gets a list of Sales Channes.
        /// </summary>
        public DbSet<SalesChannel> SalesChannels { get; set; }

        /// <summary>
        /// Gets or sets the price groups.
        /// </summary>
        /// <value>
        /// The price groups.
        /// </value>
        [ForeignEntityAtrribute]
        public DbSet<PriceGroup> PriceGroups { get; set; }

        [ForeignEntityAtrribute]
        public DbSet<VatGroup> VatGroups { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        [ForeignEntityAtrribute]
        public DbSet<CatalogItem> Items { get; set; }

        /// <summary>
        /// Gets or sets the item localizations.
        /// </summary>
        /// <value>
        /// The item localizations.
        /// </value>
        [ForeignEntityAtrribute]
        public DbSet<CatalogItemLocalization> ItemLocalizations { get; set; }

        /// <summary>
        /// Gets or sets the item properties.
        /// </summary>
        /// <value>
        /// The item properties.
        /// </value>
        [ForeignEntityAtrribute]
        public DbSet<CatalogItemProperty> ItemProperties { get; set; }

        [ForeignEntityAtrribute]
        public DbSet<Campaign> Campaigns { get; set; }
    }
}
