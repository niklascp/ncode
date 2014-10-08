using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace nCode.Catalog
{
    /// <summary>
    /// Represents a Basket Item
    /// </summary>
    public class BasketItem
    {
        private int qty;
        private Dictionary<string, object> properties;      /* Holds additional user-defined properties. */

        /// <summary>
        /// Initializes a new instance of the <see cref="BasketItem"/> class.
        /// </summary>
        public BasketItem()
        {
            ID = Guid.NewGuid();
            qty = 1;
            properties = new Dictionary<string, object>();

            AllowRemoval = true;
        }

        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        /// <value>
        /// The unique identifier.
        /// </value>
        public Guid ID { get; set; }

        public virtual int Qty
        {
            get
            {
                return qty;
            }
            set
            {
                qty = value;
                /* Update basket sums if attached to basket, otherwise
                 * it is updated when the item is added to a basket. */
                if (Basket != null)
                    Basket.UpdateSums();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to change the Qty of this item.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Qty should be Fixed; otherwise, <c>false</c>.
        /// </value>
        public virtual bool FixedQty { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to remove this item from the basket.
        /// </summary>
        public virtual bool AllowRemoval { get; set; }

        /// <summary>
        /// Gets the basket.
        /// </summary>
        public Basket Basket { get; internal set; }

        /// <summary>
        /// Gets or sets the image file.
        /// </summary>
        public virtual string ImageFile { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the unit weight.
        /// </summary>
        public virtual decimal UnitWeight { get; set; }

        /// <summary>
        /// Gets or sets the net unit price.
        /// </summary>
        public virtual decimal NetUnitPrice { get; set; }

        /// <summary>
        /// Gets or sets the unit price.
        /// </summary>
        public virtual decimal UnitPrice { get; set; }

        /// <summary>
        /// Gets the weight.
        /// </summary>
        public virtual decimal Weight
        {
            get
            {
                return Qty * UnitWeight;
            }
        }

        /// <summary>
        /// Gets the net price.
        /// </summary>
        public virtual decimal NetPrice
        {
            get
            {
                return Qty * NetUnitPrice;
            }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public virtual decimal Price
        {
            get
            {
                return Qty * UnitPrice;
            }
        }

        /// <summary>
        /// Gets or sets the Vat Group Code.
        /// </summary>
        public virtual string VatGroupCode { get; set; }

        /// <summary>
        /// Gets the Metadata Property identified by the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        public virtual T GetProperty<T>(string key, T defaultValue)
        {
            object value;

            if (properties.TryGetValue(key, out value))
                return (T)value;

            return defaultValue;
        }

        /// <summary>
        /// Sets the Metadata Property identified by the given key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void SetProperty<T>(string key, T value)
        {
            if (properties.ContainsKey(key))
                properties[key] = value;
            else
                properties.Add(key, value);
        }

        ///// <summary>
        ///// Gets a list of Properties as raw Xml Elements.
        ///// </summary>
        ///// <returns></returns>
        //public IList<XElement> GetPropertyElements()
        //{
        //    return GetPropertyElements(null);
        //}

        ///// <summary>
        ///// Gets a list of Properties as raw Xml Elements.
        ///// </summary>
        ///// <param name="transformations">The transformations.</param>
        ///// <returns></returns>
        //public IList<XElement> GetPropertyElements(IDictionary<string, Func<object, string>> transformations)
        //{
        //    List<XElement> elements = new List<XElement>();

        //    foreach (string key in properties.Keys)
        //    {
        //        object value = properties[key];
        //        var element = new XElement(key, new XAttribute("Type", value.GetType()));
        //        Func<object, string> transformation;

        //        if (transformations != null && transformations.TryGetValue(key, out transformation))
        //            element.Value = transformation(value);
        //        else if (value != null)
        //            element.Value = value.ToString();

        //        elements.Add(element);
        //    }

        //    return elements;
        //}
    }
}
