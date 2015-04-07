using nCode.Catalog.Delivery.Gls.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace nCode.Catalog.Delivery.Gls
{
    [RoutePrefix("catalog/delivery/gls")]
    public class GlsController : ApiController
    {
        [Route("SearchNearestParcelShops"), HttpGet]
        public HttpResponseMessage SearchNearestParcelShops([FromUri]string address, [FromUri]string postalCode)
        {
            if (string.IsNullOrEmpty(postalCode))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "postalCode cannot be empty.");

            var glsService = new ParcelShopServiceClient();

            try
            {
                var result = glsService.SearchNearestParcelShops(address, postalCode, 7);

                if (result != null && result.parcelshops != null && result.parcelshops.Any())
                {
                    var resultView = result.parcelshops.Select(x => new
                        {
                            number = x.Number,
                            name = x.CompanyName,
                            address = x.Streetname,
                            addressAdditional = x.Streetname2,
                            postalCode = x.ZipCode,
                            city = x.CityName,
                            countryCode = x.CountryCodeISO3166A2,
                            lat = decimal.Parse(x.Latitude, CultureInfo.InvariantCulture),
                            lng = decimal.Parse(x.Longitude, CultureInfo.InvariantCulture),
                        });

                    return Request.CreateResponse(HttpStatusCode.OK, resultView);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No results found.");
                }
            }
            catch (FaultException ex)
            {
                if (ex.Message.EndsWith("Address not found"))
                    return Request.CreateResponse(HttpStatusCode.NoContent, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}