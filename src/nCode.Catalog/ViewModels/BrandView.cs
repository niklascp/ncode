using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nCode.Metadata;
using nCode.UI;

namespace nCode.Catalog.ViewModels
{
    public class BrandView : IItemListContextView, ISeoData, IMetadataContext
    {
        public Guid ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }

        public T GetProperty<T>(string key, T defaultValue)
        {
            return GenericMetadataHelper.GetProperty(Brand.ObjectTypeId, ID, key, defaultValue);
        }

        public void SetProperty<T>(string key, T value)
        {
            GenericMetadataHelper.SetProperty(Brand.ObjectTypeId, ID, key, value);
        }
    }
}
