using System;
using System.Web.Http;

using nCode.Catalog.UI;

namespace nCode.Catalog.Controllers
{
    /// <summary>
    /// Api Controller from Catalog Settings Administration.
    /// </summary>
    [RoutePrefix("admin/catalog/settings")]
    public class CatalogSettingsAdministrationController : ApiController
    {
        private const string itemListSettingsKeyPrefix = "nCode.Catalog.ItemListSettings";

        /// <summary>
        /// Gets the item list settings for the given path.
        /// </summary>
        [Route("listsettings")]
        public IHttpActionResult GetListSettings([FromUri]string path)
        {
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

                if (id != null)
                    return Ok(Settings.GetProperty<ItemListSettings>(itemListSettingsKeyPrefix + "(" + path + ")", null));
                else
                    return Ok(Settings.GetProperty<ItemListSettings>(itemListSettingsKeyPrefix + "(" + parts[0] + ":default)", null));
            }

            return BadRequest(string.Format("Invalid path: Unknown prefix: '{0}'.", parts[0]));
        }

        /// <Updates the item list settings for the given path.
        /// </summary>
        [Route("listsettings")]
        public IHttpActionResult PostListSettings([FromUri]string path, [FromBody]ItemListSettings listSettings)
        {
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

                if (id != null)
                    Settings.SetProperty<ItemListSettings>(itemListSettingsKeyPrefix + "(" + path + ")", listSettings);
                else
                    Settings.SetProperty<ItemListSettings>(itemListSettingsKeyPrefix + "(C:default)", listSettings);

                return Ok();
            }

            return BadRequest(string.Format("Invalid path: Unknown prefix: '{0}'.", parts[0]));
        }
    }
}
