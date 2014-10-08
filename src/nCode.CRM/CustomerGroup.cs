using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using nCode.Data;

namespace nCode.CRM
{
    /// <summary>
    /// Represenst a Customer Group.
    /// </summary>
    [Table("CRM_CustomerGroups")]
    public class CustomerGroup 
    {
        /// <summary>
        /// Gets or sets the code (primary key) for this Customer Group.
        /// </summary>
        [Key]
        [Required]
        [MaxLength(20)]
        [RegularExpression(@"^[a-zA-Z0-9\.]{1,20}$")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name of this Customer Group.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }      
    }
	
}
