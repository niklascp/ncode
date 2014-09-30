using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nCode.Metadata.Model
{
    [Table("System_MetadataProperty")]
    public class MetadataPropertyEntity
    {
        public Guid ID { get; set; }

        [Index("IX_ObjectTypeID_ObjectID_Key", Order = 1, IsUnique = true)]
        public Guid ObjectTypeID { get; set; }

        [Index("IX_ObjectTypeID_ObjectID_Key", Order = 2, IsUnique = true)]
        public Guid ObjectID { get; set; }

        [Required, MaxLength(255)]
        [Index("IX_ObjectTypeID_ObjectID_Key", Order = 3, IsUnique = true)]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }        
    }
}
