using System.Data.Entity;

namespace nCode.CRM
{
    /// <summary>
    /// Entity Framework DbContext for the CRM module.
    /// </summary>
    public class CrmDbContext : DbContext
    {
        public CrmDbContext()
            : base(SqlUtilities.ConnectionString)
        { }

        public DbSet<CustomerGroup> CustomerGroups { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<CustomerContact> CustomerContacts { get; set; }

        public DbSet<CustomerProperty> CustomerProperties { get; set; }

    }
}
