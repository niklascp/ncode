using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using nCode.Metadata;

namespace nCode.UI
{
    public interface IDataTypeEditControl
    {
        void LoadData(MetadataDescriptor descriptor, IMetadataContext context);

        void SaveData(MetadataDescriptor descriptor, IMetadataContext context);
    }
}
