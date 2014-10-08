using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nCode.CRM
{
    /// <summary>
    /// Represenst a Customer Contact.
    /// </summary>
    [Table("CRM_CustomerContacts")]
    public class CustomerContact
	{
        /// <summary>
        /// Gets or sets the primary key for this Customer Contact.
        /// </summary>
		[Key]
        public Guid ID { get; set; }
		
        /// <summary>
        /// Gets or sets the date and time for the creation of this Customer Contact.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets or sets the date and time for the last modification of this Customer Contact.
        /// </summary>
        public DateTime Modified { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for Customer this Contact belongs to.
        /// </summary>
        public Guid CustomerID { get; set; }
		
        /// <summary>
        /// Gets or sets the Customer this Contact belongs to.
        /// </summary>
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        /// <summary>
        /// Gets or sets the name of this Customer Contact.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the phone number of this Customer Contact.
        /// </summary>
        [MaxLength(255)]
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the email address of this Customer Contact.
        /// </summary>
        [MaxLength(255)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the username this Customer Contact. Used if <see cref="CrmSettings.CredentialMode"/> is set to ContactUsername_ContactPassword.
        /// </summary>
        [MaxLength(255)]
        public string Username { get; set; }	

        /// <summary>
        /// Gets or sets the password this Customer Contact. Used if <see cref="CrmSettings.CredentialMode"/> is set to ContactEmail_ContactPassword.
        /// </summary>
        [MaxLength(255)]
        public string Password { get; set; }	
	}
}
