using System.Data.Entity;

namespace nCode.CRM
{
    /// <summary>
    /// Entity Framework DbContext for the CRM module.
    /// </summary>
    public class CrmDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new CRM DbContext with default connection string.
        /// </summary>
        public CrmDbContext()
            : base(SqlUtilities.ConnectionString)
        { }

        /// <summary>
        /// Gets the set of Customer Groups.
        /// </summary>
        public DbSet<CustomerGroup> CustomerGroups { get; set; }

        /// <summary>
        /// Gets the set of Customers.
        /// </summary>
        public DbSet<Customer> Customers { get; set; }


        /// <summary>
        /// Gets the set of Customer Contacts.
        /// </summary>
        public DbSet<CustomerContact> CustomerContacts { get; set; }


        /// <summary>
        /// Gets the set of Customer Properties.
        /// </summary>
        public DbSet<CustomerProperty> CustomerProperties { get; set; }

    }
}
