using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;

namespace nCode.Geographics.GsApi
{
    /// <summary>
    /// API for the Danish Geographical Data Service: "Geo Servicen".
    /// </summary>
    public static class GsApi
    {
        /// <summary>
        /// Lookups Geographical Data for the given Street Address in the postal area given by the Postal Code.
        /// </summary>
        public static GsResponse LookupAddress(StreetAddress streetAddress, string postalCode)
        {
            Log.Info(string.Format("Looking Up Geographical Data for Address: {0}, Postal Code: {1}.", streetAddress, postalCode));
                    
            string requestUrl = string.Format("http://geo.oiorest.dk/adresser/{0},{1},{2}.json", HttpUtility.UrlEncode(streetAddress.StreetName), HttpUtility.UrlEncode(streetAddress.HouseNo), HttpUtility.UrlEncode(postalCode));

            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Timeout = 2000;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(GsResponse));
                    return jsonSerializer.ReadObject(response.GetResponseStream()) as GsResponse;
                }
            }
            catch (WebException ex)
            {
                Log.Info(string.Format("Failed to look up Geographical Data for Address: {0}, Postal Code: {1}.", streetAddress, postalCode), ex);
            }

            Log.Info(string.Format("Lookup for Geographical Data for Address: {0}, Postal Code: {1} did not find in any results.", streetAddress, postalCode));
            
            var matches = SearchAddress(streetAddress);

            if (matches.Any(x => x.City.PostalCode == postalCode))
                return matches.First(x => x.City.PostalCode == postalCode);

            matches = SearchAddress(streetAddress, searchOnlyStreetName: true);

            return matches.FirstOrDefault(x => x.City.PostalCode == postalCode);
        }

        /// <summary>
        /// Searching for Geographical Data for the given Street Address in all postal areas. Optoannly just searches for the street name, thus
        /// ignoring the house number.
        /// </summary>
        public static IEnumerable<GsResponse> SearchAddress(StreetAddress streetAddress, bool searchOnlyStreetName = false)
        {
            Log.Info(string.Format("Searching for Geographical Data for Address: {0}, Searching only StreetName: {1}.", streetAddress, searchOnlyStreetName));
                    
            string requestUrl;
            
            if (searchOnlyStreetName)
                requestUrl = string.Format("http://geo.oiorest.dk/adresser.json?q={0}", HttpUtility.UrlEncode(streetAddress.StreetName));
            else
                requestUrl = string.Format("http://geo.oiorest.dk/adresser.json?q={0} {1}", HttpUtility.UrlEncode(streetAddress.StreetName), HttpUtility.UrlEncode(streetAddress.HouseNo));
            try
            {
                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.Timeout = 2000;

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format(
                        "Server error (HTTP {0}: {1}).",
                        response.StatusCode,
                        response.StatusDescription));
                    DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(List<GsResponse>));
                    return jsonSerializer.ReadObject(response.GetResponseStream()) as IEnumerable<GsResponse>;
                }
            }
            catch (WebException ex)
            {
                Log.Info(string.Format("Failed to search for Geographical Data for Address: {0}, Searching only StreetName: {1}.", streetAddress, searchOnlyStreetName), ex);
                
                return null;
            }
        }
    }
}
