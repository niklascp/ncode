using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using nCode.Catalog.Delivery.Pdk.Models;
using Newtonsoft.Json;

namespace nCode.Catalog.Delivery.Pdk.Services
{
    public class PdkServicePointServiceClient : nCode.Catalog.Delivery.Pdk.Services.IPdkServicePointServiceClient
    {
        //private static string 

        public ServicePointInformationResponse GetServicePointFromAddress(string consumerId, string countryCode, string city, string postalCode, string streetName, string streetNumber, int numberOfServicePoints, string locale)
        {
            if (string.IsNullOrEmpty(consumerId))
                throw new ArgumentNullException("consumerId");

            if (string.IsNullOrEmpty(countryCode))
                throw new ArgumentNullException("countryCode");
            
            var builder = new UriBuilder("http://api.postnord.com/wsp/rest/BusinessLocationLocator/Logistics/ServicePointService_1.0/findNearestByAddress.json");
            builder.Port = -1;

            var query = HttpUtility.ParseQueryString(builder.Query);

            query["consumerId"] = consumerId;
            query["countryCode"] = countryCode;
            
            if (!string.IsNullOrWhiteSpace(city))
                query["city"] = city;

            query["postalCode"] = postalCode;
            query["streetName"] = streetName;
            query["streetNumber"] = streetNumber;
            query["numberOfServicePoints"] = numberOfServicePoints.ToString();
            query["locale"] = locale;

            builder.Query = query.ToString();

            string url = builder.ToString();

            using (WebClient webClient = new WebClient())
            {
                webClient.Encoding = Encoding.UTF8;
                var data = webClient.DownloadString(url);
                var servicePointInformation = JsonConvert.DeserializeObject<ServicePointInformationResponse>(data);
                return servicePointInformation;
            }
        }
    }
}
