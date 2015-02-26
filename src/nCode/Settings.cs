using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;
using nCode.Configuration;
using nCode.Security;
using nCode.UI;
using System.Linq;

using Dapper;
using Newtonsoft.Json;
using nCode.Data;
using nCode.Models;
using Common.Logging;

namespace nCode
{
    /// <summary>
    /// Global Settings Class
    /// </summary>
    public static class Settings
    {
        private const string cacheKey = "nCode.Settings";
        private static JsonSerializerSettings jsonSerializerSettings;
        private static object cacheLock = new object();

        static Settings()
        {
            jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCaseContractResolver()
            };
            InitializeModules();
        }
        
        private static List<HostMapping> ConvertFromLegacyHostMapping()
        {
            var log = LogManager.GetLogger(type: typeof(Settings));
            log.Info("Converting Host Mappings from Legacy Storage (in Web.config)");

            var siteSection = SiteSection.GetSection();

            if (siteSection != null && siteSection.HostMappings != null)
            {
                return siteSection.HostMappings.Select(x => new HostMapping()
                {
                    Hostname = x.Name,
                    FrontpagePath = x.FrontpagePath,
                    MasterPageFile = x.MasterPageFile,
                    DefaultCulture = x.DefaultCulture
                }).ToList();
            }

            return null;
        }

        /*
         * Site settings
         */

        /// <summary>
        /// Gets or sets a value indicating if Setup has been completed.
        /// </summary>
        public static bool IsSetupComplete
        {
            get { return GetProperty("nCode.System.Setup", false); }
        }

        /// <summary>
        /// Gets or sets the list of Host Mappings.
        /// </summary>
        public static List<HostMapping> HostMappings
        {
            get
            {
                var hostMapping = GetProperty<List<HostMapping>>("nCode.System.HostMappings", null);

                /* Try to convert from Legacy Storage in Web.config. */
                if (hostMapping == null) {
                    hostMapping = ConvertFromLegacyHostMapping();

                    /* If still no Host Mapping create a new empty one. */
                    if (hostMapping == null)
                        hostMapping = new List<HostMapping>();

                    /* Save to Settings Storage for future request. */
                    HostMappings = hostMapping;
                }

                return hostMapping;
            }
            set { SetProperty("nCode.System.HostMappings", value); }
        }

        /// <summary>
        /// Gets or sets the fully qualified url for the application root.
        /// </summary>
        public static string Url
        {
            get { return GetProperty<string>("nCode.System.Url", string.Empty); }
            set { SetProperty<string>("nCode.System.Url", value); }
        }

        /// <summary>
        /// Gets or set whether the Canonical Domain Module should enforce urls based on the fully qualified url.
        /// </summary>
        public static bool EnforceCanonicalDomain
        {
            get { return GetProperty<bool>("nCode.System.EnforceCanonicalDomain", false); }
            set { SetProperty<bool>("nCode.System.EnforceCanonicalDomain", value); }
        }

        /// <summary>
        /// Gets or set the Site Title.
        /// </summary>
        public static string Title
        {
            get { return GetProperty<string>("nCode.System.Title", string.Empty); }
            set { SetProperty<string>("nCode.System.Title", value); }
        }

        /// <summary>
        /// Gets or sets the Site Subtitle
        /// </summary>
        public static string Subtitle
        {
            get { return GetProperty<string>("nCode.System.Subtitle", string.Empty); }
            set { SetProperty<string>("nCode.System.Subtitle", value); }
        }

        /// <summary>
        /// Gets or sets the how the title-tag in the header sohuld be rendered.
        /// </summary>
        public static TitleMode TitleMode
        {
            get { return GetProperty<TitleMode>("nCode.System.TitleMode", TitleMode.Default); }
            set { SetProperty<TitleMode>("nCode.System.TitleMode", value); }
        }


        /*
         * Email settings 
         */

        /// <summary>
        /// Gets or sets the name on the sender of emails.
        /// </summary>
        public static string SenderName
        {
            get { return GetProperty("nCode.System.SenderName", string.Empty); }
            set { SetProperty("nCode.System.SenderName", value); }
        }

        /// <summary>
        /// Gets or sets the email-address on the sender of emails.
        /// </summary>
        public static string SenderAddress
        {
            get { return GetProperty("nCode.System.SenderAddress", string.Empty); }
            set { SetProperty("nCode.System.SenderAddress", value); }
        }

        /// <summary>
        /// Get a values whether to Obfuscate Email in content.
        /// </summary>
        public static bool ObfuscateEmail
        {
            get { return GetProperty("nCode.System.ObfuscateEmail", true); }
            set { SetProperty("nCode.System.ObfuscateEmail", value); }
        }


        /*
         * Misc settings
         */

        /// <summary>
        /// Gets or sets Google Analytics Account that sould be used for tracking.
        /// </summary>
        public static string GoogleAnalyticsAccount
        {
            get { return GetProperty("nCode.System.GoogleAnalyticsAccount", (string)null); }
            set { SetProperty("nCode.System.GoogleAnalyticsAccount", value); }
        }


        /*
         * Web.config shortcuts 
         */

        /// <summary>
        /// Gets a list of loaded modules.
        /// </summary>
        public static ModuleCollection Modules { get; private set; }

        /// <summary>
        /// Gets a list of import manager configurations.
        /// </summary>
        public static ImportManagerConfigurationCollection ImportManagers
        {
            get
            {
                var adminSection = AdminInterfaceSection.GetSection();

                return adminSection.ImportManagers;
            }
        }

        // Web.config Settings Properties

        /// <summary>
        /// Gets the database connection string.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Web"].ConnectionString;
            }
        }

        /// <summary>
        /// Gets a list of supported culture names.
        /// </summary>
        public static IList<string> SupportedCultureNames
        {
            get
            {
                var list = new List<string>();
                var globalizationSection = nCode.Configuration.GlobalizationSection.GetSection();

                foreach (SupportedCulture c in globalizationSection.SupportedCultures)
                    list.Add(c.Name);

                return list.AsReadOnly();
            }
        }


        /* 
         * Generic Settings 
         */

        /// <summary>
        /// Gets the Property Cache where settings are cached.
        /// </summary>
        public static IDictionary<string, object> PropertyCache
        {
            get
            {
                object o = System.Web.HttpRuntime.Cache[cacheKey];

                /* Initialize a new cache. */
                if (o == null)
                {
                    lock (cacheLock)
                    {
                        if (o == null)
                        {
                            o = new Dictionary<string, object>();
                            HttpContext.Current.Cache.Add(cacheKey, o, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), CacheItemPriority.AboveNormal, null);
                        }
                    }
                }

                return (IDictionary<string, object>)o;
            }
        }

        /// <summary>
        /// Gets a property of type T with the given key. Returns the default value if the property does no exists.
        /// </summary>
        public static T GetProperty<T>(string key, T defaultValue)
        {
            /* Check Cache */
            if (PropertyCache.ContainsKey(key))
            {
                return (T)PropertyCache[key];
            }
            else
            {
                using (var conn = new SqlConnection(ConnectionString))
                {
                    string o;

                    try
                    {
                        conn.Open();

                        /* Get value from database. */
                        o = conn.ExecuteScalar<string>("SELECT [Value] FROM [System_Properties] WHERE [Key] = @key", new { key = key });
                    }
                    catch (SqlException ex)
                    {
                        Log.Warn(string.Format("Failed to load string value for setting with key {0}.", key), ex);

                        return defaultValue;
                    }

                    if (o == null)
                    {
                        /* Cache */
                        lock (cacheLock)
                        {
                            if (!PropertyCache.ContainsKey(key))
                                PropertyCache.Add(key, defaultValue);
                        }

                        return defaultValue;
                    }
                    else
                    {
                        /* Deserialize */
                        T v;

                        /* Legacy: data is stored as XmlSerialized C# object. */
                        if (o.StartsWith("<?xml"))
                        {
                            /* Copy the string data to a Memory Stream. */
                            using (MemoryStream ms = new MemoryStream())
                            {
                                StreamWriter sw = new StreamWriter(ms);
                                sw.Write((string)o);
                                sw.Flush();

                                /* Reset the Memory Stream. */
                                ms.Position = 0;

                                try
                                {
                                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                                    v = (T)xmlSerializer.Deserialize(ms);
                                }
                                catch (InvalidOperationException ex)
                                {
                                    v = defaultValue;
                                    Log.Warn(string.Format("Failed to deserialize XML string for setting with key {0}.", key), ex);
                                }

                                /* Convert to JSON Property */
                                SetProperty(key, v);

                                return v;
                            }
                        }
                        /* Parse as JSON string. */
                        else
                        {
                            try
                            {
                                v = JsonConvert.DeserializeObject<T>(o, jsonSerializerSettings);
                            }
                            catch (JsonSerializationException ex)
                            {
                                v = defaultValue;
                                Log.Warn(string.Format("Failed to deserialize JSON string for setting with key {0}.", key), ex);
                            }

                            /* Cache */
                            lock (cacheLock)
                            {
                                if (!PropertyCache.ContainsKey(key))
                                    PropertyCache.Add(key, v);
                            }

                            return v;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Sets a property of type T with the given key to the given value.
        /// </summary>
        public static void SetProperty<T>(string key, T value)
        {
            /* Update Cache */
            if (PropertyCache.ContainsKey(key))
            {
                /* Cache */
                lock (cacheLock)
                {
                    if (PropertyCache.ContainsKey(key))
                        PropertyCache[key] = value;
                }
            }

            var serializedValue = JsonConvert.SerializeObject(value, jsonSerializerSettings);

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                var affectedRows = conn.Execute(
                    "UPDATE [System_Properties] SET [Modified] = @modified, [Value] = @value WHERE [Key] = @key",
                    new
                    {
                        modified = DateTime.Now,
                        key = key,
                        value = serializedValue,
                    }
                );

                if (affectedRows == 0)
                {
                    conn.Execute(
                        "INSERT INTO [System_Properties] ([ID], [Created], [Modified], [Key], [Value]) VALUES (@id, @created, @modified, @key, @value)",
                        new
                        {
                            id = Guid.NewGuid(),
                            created = DateTime.Now,
                            modified = DateTime.Now,
                            key = key,
                            value = serializedValue,
                        }
                    );
                }
            }
        }

        /* Module Initialization */
        private static void InitializeModules()
        {
            //Instantiate the providers collection to store the collection with
            Modules = new ModuleCollection();

            var section = AdminInterfaceSection.GetSection();

            //Make sure that there is a custom section, and that the providers exist; if not setup, throw an error
            if (section != null)
            {
                //Instantiate the providers collection using the helper method defined in the framework
                foreach (ProviderSettings ps in section.Modules)
                    Modules.Add(ProvidersHelper.InstantiateProvider(ps, typeof(Module)));
            }
        }
    }
}
