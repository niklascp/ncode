using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

using nCode.Metadata;

namespace nCode.UI
{
    public abstract class DataTypeEditControl : UserControl, IDataTypeEditControl
    {
        public virtual void Init(MetadataDescriptor descriptor)
        {
        }
        public abstract void LoadData(MetadataDescriptor descriptor, IMetadataContext context);
        public abstract void SaveData(MetadataDescriptor descriptor, IMetadataContext context);
    }
}
