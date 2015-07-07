using System;
using System.Web.Http;

using nCode.Catalog.UI;
using nCode.Metadata;

namespace nCode.Catalog.Controllers
{
    /// <summary>
    /// Api Controller from Catalog Settings Administration.
    /// </summary>
    [RoutePrefix("admin/catalog/settings")]
    public class CatalogSettingsAdministrationController : ApiController
    {
        private const string defaultItemListSettingsKey = "nCode.Catalog.DefaultItemListSettings";
        private const string itemListSettingsKey = "nCode.Catalog.ItemListSettings";

        /// <summary>
        /// Gets the item list settings for the given path.
        /// </summary>
        [Route("itemlistsettings")]
        public IHttpActionResult GetListSettings([FromUri]string path)
        {
            if (string.IsNullOrEmpty(path))
                return Ok(Settings.GetProperty<ItemListSettings>(defaultItemListSettingsKey, null));

            var parts = path.Split(new char[] { ':' }, 2);

            if (parts.Length < 2)
                return BadRequest(string.Format("Invalid path: '{0}.'", path));

            var prefix = parts[0].ToUpper();

            /* Category or Brand prefix. */
            if (prefix == "C" || prefix == "B")
            {
                var id = (!string.IsNullOrEmpty(parts[1])) ? new Guid(parts[1]) : (Guid?)null;

                if (id == Guid.Empty)
                    id = null;

                if (id == null)
                    return Ok(Settings.GetProperty<ItemListSettings>(defaultItemListSettingsKey + "(" + prefix + ")", null));

                IMetadataContext metadataContext = null;

                if (prefix == "C")
                    metadataContext = CategoryNavigationItem.GetFromID(id.Value);
                else if (prefix == "B")
                    metadataContext = BrandNavigationItem.GetFromID(id.Value);

                if (metadataContext != null)
                    return Ok(metadataContext.GetProperty<ItemListSettings>(itemListSettingsKey, null));
                else
                    return NotFound();
            }

            return BadRequest(string.Format("Invalid path: Unknown prefix: '{0}'.", parts[0]));
        }

        /// <Updates the item list settings for the given path.
        /// </summary>
        [Route("itemlistsettings")]
        public IHttpActionResult PostListSettings([FromUri]string path, [FromBody]ItemListSettings listSettings)
        {
            if (string.IsNullOrEmpty(path))
            {
                Settings.SetProperty<ItemListSettings>(defaultItemListSettingsKey, listSettings);
                return Ok();
            }

            var parts = path.Split(new char[] { ':' }, 2);

            if (parts.Length < 2)
                return BadRequest(string.Format("Invalid path: '{0}.'", path));

            var prefix = parts[0].ToUpper();

            /* Category or Brand prefix. */
            if (prefix == "C" || prefix == "B")
            {
                var id = (!string.IsNullOrEmpty(parts[1])) ? new Guid(parts[1]) : (Guid?)null;

                if (id == Guid.Empty)
                    id = null;


                if (id == null)
                {
                    Settings.SetProperty<ItemListSettings>(defaultItemListSettingsKey + "(" + prefix + ")", listSettings);
                    return Ok();
                }

                IMetadataContext metadataContext = null;

                if (prefix == "C")
                    metadataContext = CategoryNavigationItem.GetFromID(id.Value);
                else if (prefix == "B")
                    metadataContext = BrandNavigationItem.GetFromID(id.Value);

                if (metadataContext != null)
                {
                    metadataContext.SetProperty<ItemListSettings>(itemListSettingsKey, listSettings);
                    return Ok();
                }
                else
                    return NotFound();
            }

            return BadRequest(string.Format("Invalid path: Unknown prefix: '{0}'.", parts[0]));
        }
    }
}
