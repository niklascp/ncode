using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Xml;

using nCode;
using nCode.Configuration;
using nCode.Data;
using nCode.Search;

using nCode.CMS.Model;

namespace nCode.CMS
{
    /// <summary>
    /// Represent CMS module.
    /// </summary>
    public sealed class CmsModule : Module
    {
        public override decimal Version
        {
            get
            {
                return 1.22m;
            }
        }

        private ContentType InitializeContentType(string moduleName, string root, XmlNode nodeContentType)
        {
            string type = null;
            NameValueCollection config = new NameValueCollection();

            // Parse legacy-format
            foreach (XmlAttribute attr in nodeContentType.Attributes)
            {
                if (attr.Name.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                    type = attr.Value;
                else
                    config.Add(attr.Name, attr.Value);
            }

            // Parse
            foreach (XmlNode node in nodeContentType.ChildNodes)
            {
                if (node.LocalName.Equals("type", StringComparison.InvariantCultureIgnoreCase))
                    type = node.InnerText;
                else
                    config.Add(node.LocalName, node.InnerText);
            }

            Type contentTypeType;

            if (type != null)
                contentTypeType = Type.GetType(type);
            else
                contentTypeType = typeof(GenericContentType);

            if (contentTypeType != null)
            {
                ContentType contentType = Activator.CreateInstance(contentTypeType) as ContentType;
                    
                if (contentType != null)
                {
                    contentType.ModuleName = moduleName;
                    contentType.Initialize(config, root);
                    return contentType;
                }
                else
                {
                    throw new ApplicationException(string.Format("Failed to load Content Type '{0}'.", type));
                }
            }
            else
            {
                throw new ApplicationException(string.Format("Failed to load Content Type '{0}'.", type));
            }

            return null;
        }

        private void AddContentType(string moduleName, string root, FileInfo configFile)
        {
            try
            {
                using (var fs = configFile.Open(FileMode.Open))
                {
                    XmlDocument docSettings = new XmlDocument();
                    docSettings.Load(fs);

                    // Load Content Types
                    var contentTypeNode = docSettings.SelectSingleNode("contentType");
                    ContentType contentType = InitializeContentType(moduleName, root, contentTypeNode);

                    if (contentType != null)
                    {
                        CmsSettings.ContentTypes.Add(contentType);
                    }
                }
            }
            catch (ApplicationException ex)
            {
                throw new ApplicationException(string.Format("Error loading ContentTypes for Module '{0}' from file '{1}'. See inner exception for details.", moduleName, configFile), ex);
            }
        }


        public override void Upgrade()
        {
            /* Legacy, update schema if already installed */
            var schema = SchemaControl.Schemas.SingleOrDefault(x => string.Equals(x.Name, "CMS", StringComparison.InvariantCultureIgnoreCase));

            if (schema != null)
            {
                SchemaControl.Update(schema);
            }
            else
            {
                schema = new Schema("CMS", "~/Admin/CMS/Schema.xml");
                SchemaControl.Install(schema);
            }

            /* Apply EF Migrations. */
            var configuration = new CmsDbConfiguration();
            configuration.UpdateWithLog();

            /*
            SchemaDefinition sd = new SchemaDefinition();
            sd.LoadFromDbContext(typeof(CmsDbContext));
            sd.UpdateDatabase();
            */

            /* Install */
            if (InstalledVersion == 0m)
            {
                CmsSettings.UseLegacyPathSchema = false;
                CmsSettings.UsePermalinks = true;
                CmsSettings.OmitExtension = true;
            }
        }

        public override void RegisterRoutes(RouteCollection routes)
        {
            var routeHandler = new ContentPageRouteHandler();
            var constraint = new ContentPageRouteContraint();

            if (CmsSettings.UseLegacyPathSchema || CmsSettings.AcceptLegacyPathSchema)
            {
                routes.Add(
                    "CMS.ContentPage(Legacy)",
                        new Route("cms/{*StaticPath}",
                            new RouteValueDictionary(),
                            new RouteValueDictionary() { { "StaticPath", constraint } },
                            routeHandler
                        )
                    );
            }

            if (!CmsSettings.UseLegacyPathSchema)
            {
                routes.Add(
                    "CMS.ContentPage",
                        new Route("{*StaticPath}",
                            new RouteValueDictionary(),
                            new RouteValueDictionary() { { "StaticPath", constraint } },
                            routeHandler)
                    );
            }
        }

        public void AddContentTypes(HttpApplication app, Module module, string searchPath)
        {
            var directoryInfo = new DirectoryInfo(app.Server.MapPath(searchPath));

            if (directoryInfo.Exists)
            {
                var contentTypeDirectories = directoryInfo.GetDirectories();
                foreach (var contentTypeDirectory in contentTypeDirectories)
                {
                    /* Test for Content Type Definition File, i.e. ContentType.config. */
                    var contentTypeConfigFile = new FileInfo(Path.Combine(contentTypeDirectory.FullName, "ContentType.config"));
                    var rootVirtualPath = VirtualPathUtility.AppendTrailingSlash(searchPath) + contentTypeDirectory.Name;

                    if (contentTypeConfigFile.Exists)
                    {
                        AddContentType(module.Name, rootVirtualPath, contentTypeConfigFile);
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class it allows application wide startup logic for the module.
        /// </summary>
        /// <param name="app"></param>
        public override void ApplicationStart(HttpApplication app)
        {        
            /* Load Content Types */
            foreach (var m in Settings.Modules)
            {
                /* Add "Custom" Content Types. */
                AddContentTypes(app, m, "~/" + m.Name + "/ContentControls");
                /* Add "Standard" Content Types. */
                AddContentTypes(app, m, "~/Admin/" + m.Name + "/ContentControls");
            }

            RegisterRoutes(RouteTable.Routes);

            /* Add Content Rewrite Handlers. */
            ContentRewriteControl.AddHandler(new ContentPageRewriteHandler());

            if (SearchHandler.IsInitialized)
            {
                SearchHandler.AddSource(new ContentPageSearchSource());
            }
        }
    }
}
