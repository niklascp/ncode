using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using nCode.Metadata.Model;
using System.Xml.Linq;
using Newtonsoft.Json;
using nCode.Data;

namespace nCode.Metadata
{
    public static class GenericMetadataHelper
    {
        private static JsonSerializerSettings jsonSerializerSettings;

        static GenericMetadataHelper()
        {
            jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCaseContractResolver()
            };
        }

        /// <summary>
        /// Serializes a object and returns it as a string.
        /// </summary>
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value, jsonSerializerSettings);
        }

        /// <summary>
        /// Deserializes a json or xml-encoded property and returns it as type T. If serializedData is null the default value is returned.
        /// </summary>
        public static T Deserialize<T>(string serializedData, T defaultValue)
        {
            if (serializedData == null)
                return defaultValue;

            if (serializedData.StartsWith("<?xml"))
            {
                /* Copy the string data to a Memory Stream. */
                using (MemoryStream ms = new MemoryStream())
                {
                    StreamWriter sw = new StreamWriter(ms);
                    sw.Write(serializedData);
                    sw.Flush();

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

                    try
                    {
                        return (T)xmlSerializer.Deserialize(ms);
                    }
                    catch (InvalidOperationException ex)
                    {
                        Log.Warn(string.Format("Failed to deserialize XML string '{0}'.", serializedData), ex);
                        return defaultValue;
                    }
                }
            }
            else
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(serializedData, jsonSerializerSettings);
                }
                catch (JsonSerializationException ex)
                {
                    Log.Warn(string.Format("Failed to deserialize JSON string '{0}'", serializedData), ex);
                    return defaultValue;
                }
            }
        }

        /// <summary>
        /// Gets the given property from the database.
        /// </summary>
        public static T GetProperty<T>(Type objectType, Guid objectId, string key, T defaultValue)
        {
            var objectTypeAttribute = objectType.GetCustomAttributes(typeof(ObjectTypeGuidAttribute), false).SingleOrDefault() as ObjectTypeGuidAttribute;

            if (objectTypeAttribute == null)
                throw new InvalidOperationException(string.Format("Could not find an ObjectTypeGuidAttribute for the type '{0}'", objectType.FullName));

            return GetProperty<T>(objectTypeAttribute.ObjectTypeGuid, objectId, key, defaultValue);
        }

        /// <summary>
        /// Gets the given property from the database.
        /// </summary>
        public static T GetProperty<T>(Guid objectTypeId, Guid objectId, string key, T defaultValue)
        {
            using (var dbContext = new SystemDbContext())
            {
                var property = (from p in dbContext.MetadataProperties
                                where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId && p.Key == key
                                select p.Value).SingleOrDefault();

                return Deserialize<T>(property, defaultValue);
            }
        }

        /// <summary>
        /// Sets the given property in the database.
        /// </summary>
        public static void SetProperty<T>(Type objectType, Guid objectId, string key, T value)
        {
            var objectTypeAttribute = objectType.GetCustomAttributes(typeof(ObjectTypeGuidAttribute), false).SingleOrDefault() as ObjectTypeGuidAttribute;

            if (objectTypeAttribute == null)
                throw new InvalidOperationException(string.Format("Could not find an ObjectTypeGuidAttribute for the type '{0}'", objectType.FullName));

            SetProperty<T>(objectTypeAttribute.ObjectTypeGuid, objectId, key, value);
        }

        /// <summary>
        /// Sets the given property in the database.
        /// </summary>
        public static void SetProperty<T>(Guid objectTypeId, Guid objectId, string key, T value)
        {
            using (var dbContext = new SystemDbContext())
            {
                var property = (from p in dbContext.MetadataProperties
                                where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId && p.Key == key
                                select p).SingleOrDefault();

                /* Property does not exists. */
                if (property == null)
                {
                    property = new MetadataPropertyEntity();
                    property.ID = Guid.NewGuid();
                    property.ObjectTypeID = objectTypeId;
                    property.ObjectID = objectId;
                    property.Key = key;
                    dbContext.MetadataProperties.Add(property);
                }

                property.Value = JsonConvert.SerializeObject(value, jsonSerializerSettings);

                dbContext.SaveChanges();
            }
        }

        /// <summary>
        /// Gets a list of all property keys for the given type.
        /// </summary>
        public static IEnumerable<string> GetPropertyKeys(Guid objectTypeId, Guid objectId)
        {
            using (var dbContext = new SystemDbContext())
            {
                var keys = (from p in dbContext.MetadataProperties
                            where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId
                            select p.Key).ToList();

                return keys;
            }
        }

        /// <summary>
        /// Gets a list of all properties for the given type.
        /// </summary>
        public static IEnumerable<XElement> GetProperties(Guid objectTypeId, Guid objectId)
        {
            using (var dbContext = new SystemDbContext())
            {
                var properties = (from p in dbContext.MetadataProperties
                                  where p.ObjectTypeID == objectTypeId && p.ObjectID == objectId
                                  select new
                                  {
                                      p.Key,
                                      p.Value
                                  }).ToList();

                var list = new List<XElement>(properties.Count);

                foreach (var p in properties)
                {
                    var element = new XElement(p.Key);
                    element.Value = p.Value;
                    list.Add(element);
                }

                return list;
            }
        }

    }
}
