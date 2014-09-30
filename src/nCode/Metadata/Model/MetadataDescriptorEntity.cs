using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace nCode.Metadata.Model
{
    [Table("System_MetadataDescriptor")]
    public class MetadataDescriptorEntity
    {
        public Guid ID { get; set; }

        [Index("IX_ObjectTypeID_Name", Order = 1, IsUnique = true)]
        public Guid ObjectTypeID { get; set; }

        public int DisplayIndex  { get; set; }

        [Required, MaxLength(255)]
        [Index("IX_ObjectTypeID_Name", Order = 2, IsUnique = true)]
        public string Name { get; set; }

        [Required, MaxLength(255)]
        public string DisplayText { get; set; }

        [Column("DisplayMode")]
        public int DisplayModeInt { get; set; }

        [NotMapped]
        public MetadataEditControlViewMode DisplayMode
        {
            get
            { 
                return (MetadataEditControlViewMode)DisplayModeInt; 
            }
            set
            {
                DisplayModeInt = (int)value;
            }
        }

        [Required, MaxLength(255)]
        public string EditControlPath { get; set; }

        [MaxLength(255)]
        public string EditControlArguments { get; set; }
    }
}
