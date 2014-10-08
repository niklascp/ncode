using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nCode.CRM
{
    [Table("CRM_CustomerProperties")]
    public class CustomerProperty
	{		
		[Key]
        public Guid ID { get; set; }
		
        public Guid CustomerID { get; set; }
		
        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        [Required]
        [MaxLength]
        [Column(TypeName = "ntext")]
        public string Value { get; set; }	
	}
}
