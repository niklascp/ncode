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

namespace nCode
{
    /// <summary>
    /// Global Settings Class
    /// </summary>
    public static class Settings
    {
        private const string cacheKey = "nCode.Settings";

        private static object cacheLock = new object();

        static Settings()
        {
            InitializeModules();
        }

        /*
         * Site settings
         */

        public static bool IsSetupComplete
        {
            get { return GetProperty("nCode.System.Setup", false); }
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
            get { return GetProperty<string>("nCode.System.SenderName", string.Empty); }
            set { SetProperty<string>("nCode.System.SenderName", value); }
        }

        /// <summary>
        /// Gets or sets the email-address on the sender of emails.
        /// </summary>
        public static string SenderAddress
        {
            get { return GetProperty<string>("nCode.System.SenderAddress", string.Empty); }
            set { SetProperty<string>("nCode.System.SenderAddress", value); }
        }

        /// <summary>
        /// Get a values whether to Obfuscate Email in content.
        /// </summary>
        public static bool ObfuscateEmail
        {
            get { return GetProperty<bool>("nCode.System.ObfuscateEmail", true); }
            set { SetProperty<bool>("nCode.System.ObfuscateEmail", value); }
        }


        /*
         * Misc settings
         */

        /// <summary>
        /// Gets or sets Google Analytics Account that sould be used for tracking.
        /// </summary>
        public static string GoogleAnalyticsAccount
        {
            get { return GetProperty<string>("nCode.System.GoogleAnalyticsAccount", null); }
            set { SetProperty<string>("nCode.System.GoogleAnalyticsAccount", value); }
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
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    try
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SELECT [Value] FROM [System_Properties] WHERE [Key] = @Key", conn);
                        cmd.Parameters.AddWithValue("@Key", key);

                        object o = cmd.ExecuteScalar();

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
                            /* Copy the string data to a Memory Stream. */
                            using (MemoryStream ms = new MemoryStream())
                            {
                                StreamWriter sw = new StreamWriter(ms);
                                sw.Write((string)o);
                                sw.Flush();

                                /* Reset the Memory Stream. */
                                ms.Position = 0;

                                /* Deserialize */
                                T v;
                                try
                                {
                                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                                    v = (T)xmlSerializer.Deserialize(ms);
                                }
                                catch (InvalidOperationException)
                                {
                                    return defaultValue;
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
                    catch (SqlException)
                    {
                        return defaultValue;
                    }
                }
            }
        }

        /// <summary>
        /// Sets a property of type T with the given key. Returns the default value if the property does no exists.
        /// </summary>
        public static void SetProperty<T>(string key, T value)
        {
            /* Update Cache */
            if (PropertyCache.ContainsKey(key))
            {
                PropertyCache[key] = value;
            }

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                /* Serialize the data and copy to string. */
                using (MemoryStream ms = new MemoryStream())
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(ms, value);

                    /* Reset the Memory Stream. */
                    ms.Position = 0;

                    StreamReader sr = new StreamReader(ms);
                    string serializedValue = sr.ReadToEnd();

                    /* Try to update */
                    SqlCommand updateCommand = new SqlCommand("UPDATE [System_Properties] SET [Modified] = @Modified, [Value] = @Value WHERE [Key] = @Key", conn);
                    updateCommand.Parameters.AddWithValue("@Modified", DateTime.Now);
                    updateCommand.Parameters.AddWithValue("@Key", key);                    
                    updateCommand.Parameters.AddWithValue("@Value", serializedValue);

                    int affectedRows = updateCommand.ExecuteNonQuery();

                    /* No rows updated, insert instead */
                    if (affectedRows == 0)
                    {
                        SqlCommand insertCommand = new SqlCommand("INSERT INTO [System_Properties] ([ID], [Created], [Modified], [Key], [Value]) VALUES (@ID, @Created, @Modified, @Key, @Value)", conn);
                        insertCommand.Parameters.AddWithValue("@ID", Guid.NewGuid());
                        insertCommand.Parameters.AddWithValue("@Created", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@Modified", DateTime.Now);
                        insertCommand.Parameters.AddWithValue("@Key", key);
                        insertCommand.Parameters.AddWithValue("@Value", serializedValue);
                        insertCommand.ExecuteNonQuery();
                    }
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
