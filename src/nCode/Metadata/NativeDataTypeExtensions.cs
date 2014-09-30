using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Compilation;

namespace nCode.Metadata
{
    public static class NativeDataTypeExtensions 
    {
        const string fileDescriptorEditControlPath = "~/Admin/UserControls/FileDataType.ascx";
        const string imageFileDescriptorEditControlPath = "~/Admin/UserControls/ImageFileDataType.ascx";
        const string fileListDescriptorEditControlPath = "~/Admin/UserControls/FileListDataType.ascx";
        const string textDescriptorEditControlPath = "~/Admin/UserControls/TextDataType.ascx";
        const string linkDescriptorEditControlPath = "~/Admin/UserControls/LinkDataType.ascx";

        public static void AddFileDescriptor(this MetadataDescriptorCollection descriptorCollection, string name, string displayText) 
        {
            descriptorCollection.Add(new MetadataDescriptor(name, fileDescriptorEditControlPath) { DisplayText = displayText });
        }

        public static void AddImageFileDescriptor(this MetadataDescriptorCollection descriptorCollection, string name, string displayText)
        {
            descriptorCollection.Add(new MetadataDescriptor(name, imageFileDescriptorEditControlPath) { DisplayText = displayText });
        }


        public static void AddFileListDescriptor(this MetadataDescriptorCollection descriptorCollection, string name, string displayText)
        {
            descriptorCollection.Add(new MetadataDescriptor(name, fileListDescriptorEditControlPath) { DisplayText = displayText });
        }

        public static void AddTextDescriptor(this MetadataDescriptorCollection descriptorCollection, string name, string displayText)
        {
            descriptorCollection.Add(new MetadataDescriptor(name, textDescriptorEditControlPath) { DisplayText = displayText });
        }

        public static void AddLinkDescriptor(this MetadataDescriptorCollection descriptorCollection, string name, string displayText)
        {
            descriptorCollection.Add(new MetadataDescriptor(name, linkDescriptorEditControlPath) { DisplayText = displayText });
        }
    }
}
