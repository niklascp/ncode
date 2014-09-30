using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Metadata
{
    public class MetadataDescriptorCollection : List<MetadataDescriptor>
    {
        /// <summary>
        /// Gets a list of Metadata Descriptors from the System Metadata Table.
        /// </summary>
        public static MetadataDescriptorCollection GetMetadataDescriptors(Guid objectTypeId)
        {
            using (var systemDbContext = new SystemDbContext())
            {
                var metadataDescriptors = new MetadataDescriptorCollection();
                var metadataDescriptorEntities = systemDbContext.MetadataDescriptors.Where(x => x.ObjectTypeID == objectTypeId).OrderBy(x => x.DisplayIndex).ToList();

                foreach (var e in metadataDescriptorEntities)
                {
                    var m = new MetadataDescriptor(e.Name, e.EditControlPath);
                    m.EditControlArguments = e.EditControlArguments;
                    m.DisplayText = e.DisplayText;
                    m.ViewMode = e.DisplayMode;
                    //TODO: Handle Arguments for Edit Control.
                    metadataDescriptors.Add(m);
                }

                return metadataDescriptors;
            }
        }
    }
}
