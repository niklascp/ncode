namespace nCode.CRM
{
    using System;
    using System.Linq;
    using System.IO;
    using System.Xml.Serialization;

    using nCode;
    using nCode.Metadata;

    partial class CrmModel
    {
        /// <summary>
        /// Initialized the Crm Model.
        /// </summary>
        public CrmModel()
            : base(SqlUtilities.ConnectionString)
        {

        }
    }

    partial class Customer : IMetadataContext
    {
        /// <summary>
        /// Gets a Customer specific property (given by the Customer ID) of type T.
        /// </summary>
        public static T GetProperty<T>(CrmModel model, Guid id, string key, T defaultValue)
        {
            var property = (from p in model.CustomerProperties
                            where p.CustomerID == id && p.Key == key
                            select p.Value).SingleOrDefault();

            if (property == null)
                return defaultValue;

            /* Copy the string data to a Memory Stream. */
            using (MemoryStream ms = new MemoryStream())
            {
                StreamWriter sw = new StreamWriter(ms);
                sw.Write(property);
                sw.Flush();

                /* Reset the Memory Stream. */
                ms.Position = 0;

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                return (T)xmlSerializer.Deserialize(ms);
            }
        }

        /// <summary>
        /// Sets a Customer specific property (given by the Customer ID) of type T.
        /// </summary>
        public static void SetProperty<T>(CrmModel model, Guid id, string key, T value)
        {
            /* Serialize the data and copy to string. */
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(ms, value);

                /* Reset the Memory Stream. */
                ms.Position = 0;

                using (StreamReader sr = new StreamReader(ms))
                {
                    var property = (from p in model.CustomerProperties
                                    where p.CustomerID == id && p.Key == key
                                    select p).SingleOrDefault();

                    /* Property does not exists. */
                    if (property == null) {
                        property = new CustomerProperty();
                        property.ID = Guid.NewGuid();
                        property.CustomerID = id;
                        property.Key = key;
                        model.CustomerProperties.InsertOnSubmit(property);
                    }

                    property.Value = sr.ReadToEnd();

                }
            }
        }

        /// <summary>
        /// Gets a Customer specific property of type T.
        /// </summary>
        public T GetProperty<T>(CrmModel model, string key, T defaultValue)
        {
            return GetProperty<T>(model, ID, key, defaultValue);
        }

        /// <summary>
        /// Sets a Customer specific property of type T.
        /// </summary>
        public void SetProperty<T>(CrmModel model, string key, T value)
        {
            SetProperty<T>(model, ID, key, value);
        }

        /// <summary>
        /// Gets a Customer specific property of type T.
        /// </summary>
        public T GetProperty<T>(string key, T defaultValue)
        {
            using (var model = new CrmModel())
            {
                return GetProperty<T>(model, key, defaultValue);
            }
        }

        /// <summary>
        /// Sets a Customer specific property of type T.
        /// </summary>
        public void SetProperty<T>(string key, T value)
        {
            using (var model = new CrmModel())
            {
                SetProperty<T>(model, key, value);
                model.SubmitChanges();
            }
        }
    }
}
