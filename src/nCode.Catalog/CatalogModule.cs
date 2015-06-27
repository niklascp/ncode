using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Routing;
using System.Web.Http;

using nCode.Configuration;
using nCode.Data;
using nCode.Search;
using nCode.Catalog.Models;
using Owin;

namespace nCode.Catalog
{
    public class CatalogModule : Module
    {
        private const string showPricesIncludingVatKey = "nCode.Catalog.Sales.ShowPricesIncludingVatKey";
        private const string pricesIncludesVatKey = "nCode.Catalog.Sales.PricesIncludesVat";

        private void RegisterRoutes(RouteCollection routes)
        {
            var categoryRouteHandler = new CategoryRouteHandler();

            routes.Add(
                "Catalog.Category(SpecificCulture)",
                    new Route("{Culture}/category/{CategoryTitle}/{CategoryNo}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary() { { "Culture", "[a-z]{2}-[a-z]{2,3}" }, { "CategoryTitle", @"[A-Za-z0-9-]+" }, { "CategoryNo", @"[0-9]+" } },
                        categoryRouteHandler)
                );
            routes.Add(
                "Catalog.Category(DefaultCulture)",
                    new Route("category/{CategoryTitle}/{CategoryNo}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary() { { "CategoryTitle", @"[A-Za-z0-9-]+" }, { "CategoryNo", @"[0-9]+" } },
                        categoryRouteHandler)
                );

            routes.Add(
                "Catalog.Item(SpecificCulture)",
                    new Route("{Culture}/item/{ItemTitle}/{*ItemNo}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary() { { "Culture", "[a-z]{2}-[a-z]{2,3}" }, { "ItemTitle", @"[A-Za-z0-9-]+" } },
                        new ItemRouteHandler())
                );
            routes.Add(
                "Catalog.Item(DefaultCulture)",
                    new Route("item/{ItemTitle}/{*ItemNo}",
                        new RouteValueDictionary(),
                        new RouteValueDictionary() { { "ItemTitle", @"[A-Za-z0-9-]+" } },
                        new ItemRouteHandler())
                );
        }

        private void GenericStartup()
        {
            RegisterRoutes(RouteTable.Routes);

            TermEvaluators.Add("ActiveteSalesFeatures", () => SalesSettings.ActivateSalesFeatures);
            TermEvaluators.Add("ActiveteSalesStatistics", () => SalesSettings.ActivateSalesStatistics);

            TermEvaluators.Add("UseDiscountCodes", () => SalesSettings.AllowDiscountCodes);

            TermEvaluators.Add("UseAdvancedStockControlLevel", () => SalesSettings.StockControlLevel == StockControlLevel.Advanced);

            ContentRewriteControl.AddHandler(new ItemRewriteHandler());

            if (SearchHandler.IsInitialized)
                SearchHandler.AddSource(new ItemSearchSource());
        }

        public override decimal Version
        {
            get
            {
                return 1.99m;
            }
        }

        public override void Upgrade()
        {
            if (0.00m < InstalledVersion && InstalledVersion < 1.93m)
            {
                SqlUtilities.DropForeignKeyIfExist("Catalog_ItemVariantPrices", "FK_Catalog_ItemVariantPrices_Catalog_PriceGroups");
                SqlUtilities.DropForeignKeyIfExist("Catalog_ItemListPrices", "FK_Catalog_ItemListPrices_Catalog_PriceGroups");
                SqlUtilities.DropForeignKeyIfExist("Catalog_ItemPrices", "FK_Catalog_ItemPrices_Catalog_PriceGroups");
                SqlUtilities.ExecuteStatement("IF OBJECT_ID('Catalog_PriceGroups') IS NOT NULL ALTER TABLE [Catalog_PriceGroups] ADD [Guid] [uniqueidentifier] NULL;");
                SqlUtilities.ExecuteStatement("IF OBJECT_ID('Catalog_PriceGroups') IS NOT NULL UPDATE [Catalog_PriceGroups] SET [Guid] = NEWID();");
                SqlUtilities.ExecuteStatement("IF OBJECT_ID('Catalog_PriceGroups') IS NOT NULL ALTER TABLE [Catalog_PriceGroups] ALTER COLUMN [Guid] [uniqueidentifier] NOT NULL;");
            }

            /* Install Catalog schema, if not already installed. */
            var schema = SchemaControl.Schemas.SingleOrDefault(x => string.Equals(x.Name, "Catalog", StringComparison.InvariantCultureIgnoreCase));

            if (schema == null)
            {
                schema = new Schema("Catalog", "~/Admin/Catalog/Schema.xml");
                SchemaControl.Install(schema);
            }
            else
            {
                SchemaControl.Update(schema);
            }

            /* Update/Install EF Schema */
            var sd = new SchemaDefinition();
            sd.LoadFromDbContext(typeof(CatalogDbContext));
            sd.UpdateDatabase();

            #region Upgrade to 1.0
            if (InstalledVersion < 1.0m)
            {
                /* Upgrade "UseSimpleStockControl" setting to StockControlLevel. */
                if (Settings.GetProperty<bool>("nCode.Catalog.Sales.UseSimpleStockControl", false))
                    SalesSettings.StockControlLevel = StockControlLevel.Simple;
            }
            #endregion

            #region Upgrade to 1.1
            if (InstalledVersion < 1.1m)
            {
                /* Localize Payment Types. */
                using (var catalogModel = new CatalogModel())
                {
                    var paymentTypes = catalogModel.PaymentTypes.ToList();

                    foreach (var paymentType in paymentTypes)
                    {
                        var localization = new PaymentTypeLocalization();
                        localization.ID = Guid.NewGuid();
                        localization.PaymentTypeID = paymentType.ID;
                        localization.Culture = null;
                        localization.Title = paymentType.DisplayName;
                        localization.Description = paymentType.Description;

                        catalogModel.PaymentTypeLocalizations.InsertOnSubmit(localization);
                    }

                    catalogModel.SubmitChanges();
                }
            }
            #endregion

            #region Upgrade to 1.5
            if (InstalledVersion < 1.5m)
            {
                /* Introduction of ListPrices table */

                /* Initially update any existing items with the precalculated list prices. */
                using (var catalogModel = new CatalogModel())
                {
                    foreach (var item in catalogModel.Items)
                    {
                        VatUtilities.UpdateListPrice(catalogModel, item);
                    }

                    catalogModel.SubmitChanges();
                }
            }
            #endregion

            #region Upgrade to 1.82
            if (InstalledVersion < 1.82m)
            {
                /* Introduction of Calculated Availibility on Variant Tree */

                /* Initially update any existing items with the precalculated list prices. */
                using (var catalogModel = new CatalogModel())
                {
                    foreach (var item in catalogModel.Items)
                    {
                        item.UpdateAvailability(catalogModel);
                    }

                    catalogModel.SubmitChanges();
                }
            }
            #endregion

            #region Upgrade to 1.90
            if (InstalledVersion < 1.90m)
            {
                /* Add default Sales Channel. */
                using (var dbContext = new CatalogDbContext())
                {
                    var salesChannel = dbContext.SalesChannels.Where(x => x.Code == null).SingleOrDefault();
                    if (salesChannel == null)
                    {
                        salesChannel = new SalesChannel()
                            {
                                Guid = Guid.NewGuid(),
                                Code = null,
                                Name = "Default Sales Channel",
                                ShowPricesIncludingVat = Settings.GetProperty<bool>(showPricesIncludingVatKey, true),
                                PriceGroupCode = null,
                                OrderConfirmTemplateName = "OrderConfirm",
                                InvoiceTemplateName = "Invoice",
                            };
                        dbContext.SalesChannels.Add(salesChannel);
                    }

                    dbContext.SaveChanges();
                }
            }
            #endregion

            #region Upgrade to 1.93
            if (InstalledVersion < 1.93m)
            {
                using (var dbContext = new CatalogDbContext())
                {
                    /* Alter default Sales Channel code. */
                    var salesChannel = dbContext.SalesChannels.Where(x => x.Code == "Default").SingleOrDefault();
                    if (salesChannel != null)
                        salesChannel.Code = null;

                    /* Add default Price Group. */
                    var priceGroup = dbContext.PriceGroups.Where(x => x.Code == null).SingleOrDefault();
                    if (priceGroup == null)
                    {
                        priceGroup = new Models.PriceGroup()
                        {
                            Code = null,
                            Name = "(Standard)",
                            PricesIncludeVat = Settings.GetProperty<bool>(pricesIncludesVatKey, true)
                        };
                        dbContext.PriceGroups.Add(priceGroup);
                    }

                    dbContext.SaveChanges();
                }
            }
            #endregion
        }

        public override void ApplicationStart(System.Web.HttpApplication app)
        {
            GenericStartup();
        }

        public override void Startup(IAppBuilder app)
        {
            GenericStartup();
        }
    }
}
