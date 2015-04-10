using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Catalog.Delivery.Pdk.Models
{
    public class ServicePointInformationResponse
    {
        [JsonProperty("servicePointInformationResponse")]
        public ServicePointInformation ServicePointInformation { get; set; }
    }

    public class ServicePointInformation
    {
        [JsonProperty("customerSupportPhoneNo")]
        public string CustomerSupportPhoneNo { get; set; }

        [JsonProperty("servicePoints")]
        public ServicePoint[] ServicePoints { get; set; }
    }

    public class ServicePoint
    {
        [JsonProperty("servicePointId")]
        public string ServicePointId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("routeDistance")]
        public int RouteDistance { get; set; }

        [JsonProperty("visitingAddress")]
        public ServicePointAddress VisitingAddress { get; set; }

        [JsonProperty("deliveryAddress")]
        public ServicePointAddress DeliveryAddress { get; set; }

        [JsonProperty("coordinate")]
        public ServicePointCoordinate Coordinate { get; set; }

        [JsonProperty("openingHours")]
        public ServicePointOpeningHour[] OpeningHours { get; set; }
    }

    public class ServicePointAddress
    {
        [JsonProperty("streetName")]
        public string StreetName { get; set; }

        [JsonProperty("streetNumber")]
        public string StreetNumber { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
    }

    public class ServicePointCoordinate
    {
        [JsonProperty("srId")]
        public string SourceId { get; set; }

        [JsonProperty("northing")]
        public float NorthingCoordinate { get; set; }

        [JsonProperty("easting")]
        public float EastingCoordinate { get; set; }
    }

    public class ServicePointOpeningHour
    {
        [JsonProperty("day")]
        public string DayCode { get; set; }

        [JsonProperty("from1")]
        public string From { get; set; }
        
        [JsonProperty("to1")]
        public string To { get; set; }        
    }
}
