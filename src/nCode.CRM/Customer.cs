using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using nCode.Data;
using nCode.Metadata;
using System.Collections.ObjectModel;

namespace nCode.CRM
{
    /// <summary>
    /// Represenst a Customer.
    /// </summary>
    [Table("CRM_Customers")]
    public class Customer : IMetadataContext
    {
        /// <summary>
        /// Gets or sets the primary key for this Customer.
        /// </summary>
        [Key]
        public Guid ID { get ; set;}

        /// <summary>
        /// Gets or sets the date and time for the creation of this Customer.
        /// </summary>
        [Required]
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time for the last modification of this Customer.
        /// </summary>
        [Required]
        public System.DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the Customer Number of this Customer.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string CustomerNo { get; set; }

        [SetExistingRows("true")]
        public bool IsActive { get; set; }

        [MaxLength(20)]
        public string GroupCode { get; set; }

        [ForeignKey("GroupCode")]
        public CustomerGroup Group { get; set; }

        /// <summary>
        /// Gets or sets the Name of this Customer.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Address (line 1) of this Customer.
        /// </summary>
        [MaxLength(255)]
        public string Address1 { get; set; }

        /// <summary>
        /// Gets or sets the Address (line 2) of this Customer.
        /// </summary>
        [MaxLength(255)]
        public string Address2 { get; set; }

        /// <summary>
        /// Gets or sets the Postal Code of this Customer.
        /// </summary>
        [MaxLength(255)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the City of this Customer.
        /// </summary>
        [MaxLength(255)]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the Country Code (ISO 3166-1 alpha-2) of this Customer.
        /// </summary>
        [MaxLength(2)]
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the Phone Number of this Customer.
        /// </summary>
        [MaxLength(255)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the Email of this Customer.
        /// </summary>
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Url of this Customer's webpage.
        /// </summary>
        [MaxLength(255)]
        public string Web { get; set; }

        /// <summary>
        /// Gets or sets the Description Customer.
        /// </summary>
        [MaxLength]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }

        [MaxLength(255)]
        public string Username { get; set; }

        [MaxLength(255)]
        public string Password { get; set; }

        /* Inverse Foreign Relationships */

        private ICollection<CustomerContact> _Contacts;

        public virtual ICollection<CustomerContact> Contacts
        {
            get { return _Contacts ?? (_Contacts = new Collection<CustomerContact>()); }
            set { _Contacts = value; }
        }

        /* IMetadataContext Members */

        /// <summary>
        /// Gets a Customer specific property (given by the Customer ID) of type T.
        /// </summary>
        public static T GetProperty<T>(CrmDbContext context, Guid id, string key, T defaultValue)
        {
            var property = (from p in context.CustomerProperties
                            where p.CustomerID == id && p.Key == key
                            select p.Value).SingleOrDefault();

            if (property == null)
                return defaultValue;

            /* Copy the string data to a Memory Stream. */
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(property);
                sw.Flush();

                /* Reset the Memory Stream. */
                ms.Position = 0;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(ms);
            }
        }

        /// <summary>
        /// Sets a Customer specific property (given by the Customer ID) of type T.
        /// </summary>
        public static void SetProperty<T>(CrmDbContext context, Guid id, string key, T value)
        {
            /* Serialize the data and copy to string. */
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(ms, value);

                /* Reset the Memory Stream. */
                ms.Position = 0;

                using (StreamReader sr = new StreamReader(ms))
                {
                    var property = (from p in context.CustomerProperties
                                    where p.CustomerID == id && p.Key == key
                                    select p).SingleOrDefault();

                    /* Property does not exists. */
                    if (property == null)
                    {
                        property = context.CustomerProperties.Create();
                        property.ID = Guid.NewGuid();
                        property.CustomerID = id;
                        property.Key = key;
                        context.CustomerProperties.Add(property);
                    }

                    property.Value = sr.ReadToEnd();

                }
            }
        }

        /// <summary>
        /// Gets a Customer specific property of type T.
        /// </summary>
        public T GetProperty<T>(CrmDbContext context, string key, T defaultValue)
        {
            return GetProperty<T>(context, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets a Customer specific property of type T.
        /// </summary>
        public void SetProperty<T>(CrmDbContext context, string key, T value)
        {
            SetProperty<T>(context, ID, key, value);
        }

        /// <summary>
        /// Gets a Customer specific property of type T.
        /// </summary>
        public T GetProperty<T>(string key, T defaultValue)
        {
            using (var context = new CrmDbContext())
            {
                return GetProperty<T>(context, key, defaultValue);
            }
        }

        /// <summary>
        /// Sets a Customer specific property of type T.
        /// </summary>
        public void SetProperty<T>(string key, T value)
        {
            using (var context = new CrmDbContext())
            {
                SetProperty<T>(context, key, value);
                context.SaveChanges();
            }
        }

    }
	
}
