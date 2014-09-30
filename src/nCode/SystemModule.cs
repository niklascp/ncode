using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.UI;

using nCode.Configuration;
using nCode.Data;
using nCode.Metadata;
using nCode.Security;

namespace nCode
{
    public class SystemModule : Module
    {
        public override decimal Version
        {
            get
            {
                return 2.41m;
            }
        }

        public override void Upgrade()
        {
            /*
             *  Database Schema -- When migrating completely to EF Migrations it is safe to remove this.
             */

            var schema = SchemaControl.Schemas.SingleOrDefault(x => string.Equals(x.Name, "System", StringComparison.InvariantCultureIgnoreCase));

            /* Install System schema, if not already installed. */
            if (schema == null)
            {
                schema = new Schema("System", "~/Admin/System/Schema.xml");
                SchemaControl.Install(schema);
            }
            /* Otherwise, simple update it. */
            else
            {
                SchemaControl.Update(schema);
            }

            if (0m < InstalledVersion && InstalledVersion < 2.2m)
            {
                /* For some reason, the unique key constraints created by the old Xml-Schema contains a space. Drop it, since a new (without the space) will exist. */
                SqlUtilities.ExecuteStatement("ALTER TABLE [System_MetadataDescriptors] DROP CONSTRAINT [IX_System_MetadataDescriptors_ObjectTypeID_ Name]");
                SqlUtilities.ExecuteStatement("ALTER TABLE [System_MetadataProperties] DROP CONSTRAINT [IX_System_MetadataProperties_ObjectTypeID_ ObjectID_ Key]");
            }

            /*
             * EF Migrations Upgrade.
             */

            /* Apply EF Migrations. */
            var configuration = new SystemDbConfiguration();
            configuration.UpdateWithLog();

            /* Ensure system roles. */
            foreach (string role in SystemRoles.Roles)
            {
                if (!Roles.GetAllRoles().Any(x => string.Equals(x, role, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Roles.CreateRole(role);
                }
            }
        }

        public override void ApplicationStart(HttpApplication app)
        {            
            var routeHandler = new CultureRouteHandler();

            /* Add Route Mappings */
            RouteTable.Routes.Add(
                "System.Page(SpecificCulture)",
                    new Route("{Culture}/{*Path}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary() { { "Culture", "[a-z]{2}-[a-z]{2,3}" }, { "Path", new CultureRouteContraint() } },
                        routeHandler)
                );

            RouteTable.Routes.Add(
                "System.Page(DefaultCulture)",
                    new Route("{*Path}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary() { { "Path", new CultureRouteContraint() } },
                        routeHandler)
                );

            ContentRewriteControl.AddHandler(new ObfuscateEmailRewriteHandler());

            /* Register MetadataTypes Mappings */
            EditControlBindings.Bindings.Add(typeof(string), "~/Admin/System/MetadataEditControls/TextBox.ascx");
        }
    }
}
