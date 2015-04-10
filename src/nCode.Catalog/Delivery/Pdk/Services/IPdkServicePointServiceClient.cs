using System;
namespace nCode.Catalog.Delivery.Pdk.Services
{
    interface IPdkServicePointServiceClient
    {
        nCode.Catalog.Delivery.Pdk.Models.ServicePointInformationResponse GetServicePointFromAddress(string consumerId, string countryCode, string city, string postalCode, string streetName, string streetNumber, int numberOfServicePoints, string locale);
    }
}
