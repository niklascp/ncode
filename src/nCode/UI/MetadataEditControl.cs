using System.Web.UI;
using nCode.Metadata;

namespace nCode.UI
{
    /// <summary>
    /// Abstract class for User Controls that edits Metadata 
    /// </summary>
    public abstract class MetadataEditControl : UserControl
    {
        public MetadataDescriptor MetadataDescriptor { get; set; }

        public abstract void LoadData(IMetadataContext context);

        public abstract void SaveData(IMetadataContext context);
    }
}
