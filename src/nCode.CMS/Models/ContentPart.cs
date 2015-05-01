using nCode.Data;
using nCode.Metadata;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace nCode.CMS.Models
{
    /// <summary>
    /// Represents a CMS Content Part.
    /// </summary>
    [Table("CMS_ContentPart")]
    [RenameAttribute("CMS_ContentParts")]
    public class ContentPart : IMetadataContext
    {
        public ContentPart()
        {
            Properties = new Collection<ContentPartProperty>();
        }
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        [Key]
        public Guid ID { get; set; }

        /*
        /// <summary>
        /// Gets or sets the Content Page ID.
        /// </summary>
        public Guid ContentPageID { get; set; }

        /// <summary>
        /// Gets or sets the ID of the Parent Content Part.
        /// </summary>
        public Guid? ParentContentPartID { get; set; }

        /// <summary>
        /// Gets or sets the Display Index.
        /// </summary>
        public int DisplayIndex { get; set; }
        */

        /// <summary>
        /// Gets or sets the Content Type ID for this Content Part.
        /// </summary>
        public Guid ContentTypeID { get; set; }

        /// <summary>
        /// Gets the Content Type for this Content Part.
        /// </summary>
        [NotMapped]
        public ContentType ContentType
        {
            get
            {
                return CmsSettings.ContentTypes[ContentTypeID];
            }
        }

        /* 
        /// <summary>
        /// Gets or sets the Content Part.
        /// </summary>
        [ForeignKey("ContentPageID")]
        public virtual ContentPageEntity ContentPage { get; set; }
        */

        /*
        /// <summary>
        /// Gets or sets the Content Part.
        /// </summary>
        [ForeignKey("ParentContentPartID")]
        public virtual ContentPart ParentContentPart { get; set; }
        */

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public virtual ICollection<ContentPartProperty> Properties { get; set; }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public T GetProperty<T>(string key, T defaultValue)
        {
            var p = Properties.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (p != null)
            {
                return JsonConvert.DeserializeObject<T>(p.Value);
            }

            return defaultValue;
        }

        /// <summary>
        /// Gets the property as raw JSON.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetPropertyJson(string key)
        {
            return Properties.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).SingleOrDefault();
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string key, T value)
        {
            var p = Properties.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

            if (p == null)
            {
                p = new ContentPartProperty();
                p.ID = Guid.NewGuid();
                p.ContentPart = this;
                p.Key = key;
                Properties.Add(p);
            }

            p.Value = JsonConvert.SerializeObject(value);
        }
    }
}
