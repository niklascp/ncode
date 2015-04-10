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
using nCode.Catalog.Delivery.Pdk.Services;
using nCode.Geographics;
namespace nCode.Catalog.Delivery.Pdk
{
    [RoutePrefix("catalog/delivery/pdk")]
    public class PdkController : ApiController
    {
        [Route("SearchNearestParcelShops"), HttpGet]
        public HttpResponseMessage SearchNearestParcelShops([FromUri]string countryCode, [FromUri]string postalCode, [FromUri]string city, [FromUri]string address)
        {
            if (string.IsNullOrEmpty(countryCode))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "countryCode cannot be empty.");

            if (string.IsNullOrEmpty(postalCode))
                return Request.CreateResponse(HttpStatusCode.BadRequest, "postalCode cannot be empty.");
            
            var streetAddress = AddressUtilities.ExtractStreetAddress(address);
            
            IPdkServicePointServiceClient pdkService = new PdkServicePointServiceClient();
            
            try
            {
                var result = pdkService.GetServicePointFromAddress(
                    consumerId: "93fd8a32-1e02-4605-8abc-3896e07fff4f",
                    countryCode: countryCode,
                    postalCode: postalCode,
                    city: city,
                    streetName: streetAddress.StreetName,
                    streetNumber: streetAddress.HouseNo,
                    numberOfServicePoints: 7,
                    locale: "da"
                );

                if (result != null && result.ServicePointInformation != null && result.ServicePointInformation.ServicePoints != null && result.ServicePointInformation.ServicePoints.Any())
                {
                    var resultView = result.ServicePointInformation.ServicePoints.Select(x => new
                        {
                            id = x.ServicePointId,
                            name = x.Name,
                            address = x.VisitingAddress.StreetName + " " + x.VisitingAddress.StreetNumber,
                            //addressAdditional = null,
                            postalCode = x.VisitingAddress.PostalCode,
                            city = x.VisitingAddress.City,
                            countryCode = x.VisitingAddress.CountryCode,
                            lat = x.Coordinate.NorthingCoordinate,
                            lng = x.Coordinate.EastingCoordinate,
                        });

                    return Request.CreateResponse(HttpStatusCode.OK, resultView);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "No results found.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}