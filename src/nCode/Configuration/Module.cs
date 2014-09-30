using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Xml;
using System.Xml.Linq;

namespace nCode.Configuration
{
    /// <summary>
    /// Reresents a Administration Module.
    /// </summary>
    public abstract class Module : ProviderBase
    {
        /// <summary>
        /// Creates a Module.
        /// </summary>
        public Module()
        {
            FeatureGroups = new FeatureGroupCollection();
            TermEvaluators = new Dictionary<string, Func<bool>>();
        }

        public string Title { get; private set; }

        public virtual decimal Version { get; private set; }

        public decimal InstalledVersion 
        {
            get { return Settings.GetProperty<decimal>(string.Format("nCode.{0}.Version", Name), 0.0m); } 
            set { Settings.SetProperty<decimal>(string.Format("nCode.{0}.Version", Name), value); } 
        }
             
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            XElement configFile = null;

            if (config["file"] != null)
            {
                string file = HttpContext.Current.Server.MapPath(config["file"]);

                XDocument docModule = XDocument.Load(file);
                configFile = docModule.Root;
            }

            if (!string.IsNullOrEmpty((string)config["title"]))
                Title = (string)config["title"];
            else if (configFile != null && configFile.Attribute("title") != null)
                Title = configFile.Attribute("title").Value;
            else
                Title = Name;

            /* Only try to load version if it is not overwritten in specific module class. */
            if (Version == 0.0m)
            {
                if (configFile != null && configFile.Attribute("version") != null)
                    Version = decimal.Parse(configFile.Attribute("version").Value, CultureInfo.InvariantCulture);
                else
                    Version = 1.0m;
            }

            if (configFile != null)
                LoadFeatures(configFile.Elements("featureGroup"));
        }

        private void LoadFeatures(IEnumerable<XElement> featureGroupNodes)
        {
            if (featureGroupNodes == null)
                throw new ArgumentNullException("featureGroupNodes");

            foreach (XElement featureGroupNode in featureGroupNodes)
            {
                if (featureGroupNode.Attribute("name") == null || featureGroupNode.Attribute("name").Value == string.Empty)
                    throw new Exception("Feature Group attribute Name is missing or is empty.");

                string name = featureGroupNode.Attribute("name").Value;

                FeatureGroup featureGroup = FeatureGroups[name];

                if (featureGroup == null)
                {
                    featureGroup = new FeatureGroup(this, name);
                    FeatureGroups.Add(featureGroup);
                }

                foreach (XElement featureNode in featureGroupNode.Elements("feature"))
                {
                    if (featureNode.Attribute("name") == null || featureNode.Attribute("name").Value == string.Empty)
                        throw new Exception("Feature attribute Name is missing or is empty.");

                    Feature feature = new Feature(featureGroup, featureNode.Attribute("name").Value);
            
                    if (featureNode.Attribute("icon") != null)
                        feature.Icon = featureNode.Attribute("icon").Value;

                    if (featureNode.Attribute("largeIcon") != null)
                        feature.LargeIcon = featureNode.Attribute("largeIcon").Value;

                    if (featureNode.Attribute("administrationUrl") != null)
                        feature.AdministrationUrl = featureNode.Attribute("administrationUrl").Value;

                    if (featureNode.Attribute("administrationTarget") != null)
                        feature.AdministrationTarget = featureNode.Attribute("administrationTarget").Value;

                    if (featureNode.Attribute("visibilityTerm") != null)
                        feature.VisibilityTerm = featureNode.Attribute("visibilityTerm").Value;

                    featureGroup.Features.Add(feature);
                }
            }
        }

        public FeatureGroupCollection FeatureGroups { get; private set; }

        public Dictionary<string, Func<bool>> TermEvaluators { get; private set; }

        /* Methods */

        //public virtual void Configuration(IAppBuilder app) { }

        /// <summary>
        /// When overridden in a derived class it allows application wide startup logic for the module.
        /// </summary>
        /// <param name="app"></param>
        public virtual void ApplicationStart(HttpApplication app) { }

        /// <summary>
        /// Called on Application Start. Allows the module to register routes.
        /// </summary>
        public virtual void RegisterRoutes(RouteCollection routes) { }

        /// <summary>
        /// Called on Application Start. Allows the module to register routes.
        /// </summary>
        public virtual void Upgrade() { }
    }
}
