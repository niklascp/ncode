using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using nCode.Metadata;
using nCode.UI;

using nCode.Catalog.Models;
using nCode.Catalog.UI;

namespace nCode.Catalog.ViewModels
{
    public class ItemDetailView : IMetadataContext, ISeoData
    {
        public Guid ID { get; set; }
        public string ItemNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public bool OnSale { get; set; }
        public bool IsAvailable { get; set; }
        public VariantMode VariantMode { get; set; }

        public string SeoKeywords { get; set;}
        public string SeoDescription { get; set; }

        public Guid? CategoryID { get; set; }
        public string CategoryTitle { get; set; }
        public int CategoryIndex { get; set; }

        public Guid? BrandID { get; set; }
        public string BrandName { get; set; }
        public int BrandIndex { get; set; }

        public T GetProperty<T>(string key, T defaultValue)
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                var property = (from p in model.ItemProperties
                                where p.ItemID == ID && p.Key == key
                                select p.Value).SingleOrDefault();

                if (property == null)
                    return defaultValue;

                return GenericMetadataHelper.Deserialize<T>(property, defaultValue);
            }
        }

        public void SetProperty<T>(string key, T value)
        {
            using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
            {
                var property = (from p in model.ItemProperties
                                where p.ItemID == ID && p.Key == key
                                select p).SingleOrDefault();

                /* Property does not exists. */
                if (property == null)
                {
                    property = new ItemProperty();
                    property.ID = Guid.NewGuid();
                    property.ItemID = ID;
                    property.Key = key;
                    model.ItemProperties.InsertOnSubmit(property);
                }

                property.Value = GenericMetadataHelper.Serialize<T>(value);

                model.SubmitChanges();
            }
        }

    }
}
