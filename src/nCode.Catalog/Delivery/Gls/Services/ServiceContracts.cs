using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;

namespace nCode.Catalog.Delivery.Gls.Services
{
    [ServiceContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public interface IParcelShopService
    {
        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetOneParcelShop", ReplyAction = "*")]
        PakkeshopData GetOneParcelShop(string ParcelShopNumber);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetOneParcelShop", ReplyAction = "*")]
        Task<PakkeshopData> GetOneParcelShopAsync(string ParcelShopNumber);
        
        /*
        // CODEGEN: Generating message contract since element name ParcelShopNumber from namespace http://gls.dk/webservices/ is not marked nillable
        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetParcelShopInfo", ReplyAction = "*")]
        GetParcelShopInfoResponse GetParcelShopInfo(GetParcelShopInfoRequest request);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetParcelShopInfo", ReplyAction = "*")]
        Task<GetParcelShopInfoResponse> GetParcelShopInfoAsync(GetParcelShopInfoRequest request);

        // CODEGEN: Generating message contract since element name GetAllParcelShopsResult from namespace http://gls.dk/webservices/ is not marked nillable
        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetAllParcelShops", ReplyAction = "*")]
        GetAllParcelShopsResponse GetAllParcelShops(GetAllParcelShopsRequest request);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetAllParcelShops", ReplyAction = "*")]
        Task<GetAllParcelShopsResponse> GetAllParcelShopsAsync(GetAllParcelShopsRequest request);

        // CODEGEN: Generating message contract since element name GetAllParcelShops_DK_SEResult from namespace http://gls.dk/webservices/ is not marked nillable
        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetAllParcelShops_DK_SE", ReplyAction = "*")]
        GetAllParcelShops_DK_SEResponse GetAllParcelShops_DK_SE(GetAllParcelShops_DK_SERequest request);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetAllParcelShops_DK_SE", ReplyAction = "*")]
        Task<GetAllParcelShops_DK_SEResponse> GetAllParcelShops_DK_SEAsync(GetAllParcelShops_DK_SERequest request);

        */ 

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetParcelShopsInZipcode", ReplyAction = "*")]
        PakkeshopData[] GetParcelShopsInZipcode(string zipcode);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetParcelShopsInZipcode", ReplyAction = "*")]
        Task<PakkeshopData[]> GetParcelShopsInZipcodeAsync(string zipcode);

        /*
        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetNearstParcelShops", ReplyAction = "*")]
        GetNearstParcelShopsResponse GetNearstParcelShops(GetNearstParcelShopsRequest request);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetNearstParcelShops", ReplyAction = "*")]
        Task<GetNearstParcelShopsResponse> GetNearstParcelShopsAsync(GetNearstParcelShopsRequest request);
        */

        [OperationContractAttribute(Action = "http://gls.dk/webservices/SearchNearestParcelShops", ReplyAction = "*")]
        ParcelShopSearchResult SearchNearestParcelShops(string street, string zipcode, int Amount);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/SearchNearestParcelShops", ReplyAction = "*")]
        Task<ParcelShopSearchResult> SearchNearestParcelShopsAsync(string street, string zipcode, int Amount);
        /*
        // CODEGEN: Generating message contract since element name street from namespace http://gls.dk/webservices/ is not marked nillable
        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetParcelShopDropPoint", ReplyAction = "*")]
        GetParcelShopDropPointResponse GetParcelShopDropPoint(GetParcelShopDropPointRequest request);

        [OperationContractAttribute(Action = "http://gls.dk/webservices/GetParcelShopDropPoint", ReplyAction = "*")]
        Task<GetParcelShopDropPointResponse> GetParcelShopDropPointAsync(GetParcelShopDropPointRequest request);
        */
    }
    
    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetParcelShopInfoRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string ParcelShopNumber;

        public GetParcelShopInfoRequestBody()
        {
        }

        public GetParcelShopInfoRequestBody(string ParcelShopNumber)
        {
            this.ParcelShopNumber = ParcelShopNumber;
        }
    }
 
    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetParcelShopInfoResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public PakkeshopData GetParcelShopInfoResult;

        public GetParcelShopInfoResponseBody()
        {
        }

        public GetParcelShopInfoResponseBody(PakkeshopData GetParcelShopInfoResult)
        {
            this.GetParcelShopInfoResult = GetParcelShopInfoResult;
        }
    }

    [DataContractAttribute()]
    public partial class GetAllParcelShopsRequestBody
    {

        public GetAllParcelShopsRequestBody()
        {
        }
    }

    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetAllParcelShopsResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public PakkeshopData[] GetAllParcelShopsResult;

        public GetAllParcelShopsResponseBody()
        {
        }

        public GetAllParcelShopsResponseBody(PakkeshopData[] GetAllParcelShopsResult)
        {
            this.GetAllParcelShopsResult = GetAllParcelShopsResult;
        }
    }

    [DataContractAttribute()]
    public partial class GetAllParcelShops_DK_SERequestBody
    {
        public GetAllParcelShops_DK_SERequestBody()
        {
        }
    }
    
    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetAllParcelShops_DK_SEResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public PakkeshopData[] GetAllParcelShops_DK_SEResult;

        public GetAllParcelShops_DK_SEResponseBody()
        {
        }

        public GetAllParcelShops_DK_SEResponseBody(PakkeshopData[] GetAllParcelShops_DK_SEResult)
        {
            this.GetAllParcelShops_DK_SEResult = GetAllParcelShops_DK_SEResult;
        }
    }
    
    
    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetNearstParcelShopsRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string street;

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
        public string zipcode;

        [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
        public int Amount;

        public GetNearstParcelShopsRequestBody()
        {
        }

        public GetNearstParcelShopsRequestBody(string street, string zipcode, int Amount)
        {
            this.street = street;
            this.zipcode = zipcode;
            this.Amount = Amount;
        }
    }
       
    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetNearstParcelShopsResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ParcelShopSearchResult GetNearstParcelShopsResult;

        public GetNearstParcelShopsResponseBody()
        {
        }

        public GetNearstParcelShopsResponseBody(ParcelShopSearchResult GetNearstParcelShopsResult)
        {
            this.GetNearstParcelShopsResult = GetNearstParcelShopsResult;
        }
    }
    
    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetParcelShopDropPointRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string street;

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
        public string zipcode;

        [System.Runtime.Serialization.DataMemberAttribute(Order = 2)]
        public int Amount;

        public GetParcelShopDropPointRequestBody()
        {
        }

        public GetParcelShopDropPointRequestBody(string street, string zipcode, int Amount)
        {
            this.street = street;
            this.zipcode = zipcode;
            this.Amount = Amount;
        }
    }

    [DataContractAttribute(Namespace = "http://gls.dk/webservices/")]
    public partial class GetParcelShopDropPointResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public ParcelShopSearchResult GetParcelShopDropPointResult;

        public GetParcelShopDropPointResponseBody()
        {
        }

        public GetParcelShopDropPointResponseBody(ParcelShopSearchResult GetParcelShopDropPointResult)
        {
            this.GetParcelShopDropPointResult = GetParcelShopDropPointResult;
        }
    }

    public partial class ParcelShopServiceClient : ClientBase<IParcelShopService>, IParcelShopService
    {

        public ParcelShopServiceClient()
            : base(new BasicHttpBinding() { MaxReceivedMessageSize = 16777216 }, new EndpointAddress("http://www.gls.dk/webservices_v2/wsPakkeshop.asmx"))
        {
        }

        public ParcelShopServiceClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public ParcelShopServiceClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ParcelShopServiceClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public ParcelShopServiceClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }
               
        public PakkeshopData GetOneParcelShop(string ParcelShopNumber)
        {
            return Channel.GetOneParcelShop(ParcelShopNumber);
        }

        public Task<PakkeshopData> GetOneParcelShopAsync(string ParcelShopNumber)
        {
            return Channel.GetOneParcelShopAsync(ParcelShopNumber);
        }

        /*

        public PakkeshopData GetParcelShopInfo(string ParcelShopNumber)
        {
            GetParcelShopInfoRequest inValue = new GetParcelShopInfoRequest();
            inValue.Body = new GetParcelShopInfoRequestBody();
            inValue.Body.ParcelShopNumber = ParcelShopNumber;
            GetParcelShopInfoResponse retVal = ((wsPakkeshopSoap)(this)).GetParcelShopInfo(inValue);
            return retVal.Body.GetParcelShopInfoResult;
        }       
        
        GetAllParcelShopsResponse wsPakkeshopSoap.GetAllParcelShops(GetAllParcelShopsRequest request)
        {
            return base.Channel.GetAllParcelShops(request);
        }

        public PakkeshopData[] GetAllParcelShops()
        {
            GetAllParcelShopsRequest inValue = new GetAllParcelShopsRequest();
            inValue.Body = new GetAllParcelShopsRequestBody();
            GetAllParcelShopsResponse retVal = ((wsPakkeshopSoap)(this)).GetAllParcelShops(inValue);
            return retVal.Body.GetAllParcelShopsResult;
        }

        GetAllParcelShops_DK_SEResponse wsPakkeshopSoap.GetAllParcelShops_DK_SE(GetAllParcelShops_DK_SERequest request)
        {
            return base.Channel.GetAllParcelShops_DK_SE(request);
        }

        public PakkeshopData[] GetAllParcelShops_DK_SE()
        {
            GetAllParcelShops_DK_SERequest inValue = new GetAllParcelShops_DK_SERequest();
            inValue.Body = new GetAllParcelShops_DK_SERequestBody();
            GetAllParcelShops_DK_SEResponse retVal = ((wsPakkeshopSoap)(this)).GetAllParcelShops_DK_SE(inValue);
            return retVal.Body.GetAllParcelShops_DK_SEResult;
        }
       
        */

        public PakkeshopData[] GetParcelShopsInZipcode(string zipcode)
        {
            return Channel.GetParcelShopsInZipcode(zipcode);
        }

        public Task<PakkeshopData[]> GetParcelShopsInZipcodeAsync(string zipcode)
        {
            return Channel.GetParcelShopsInZipcodeAsync(zipcode);
        }
 
        /*

        
        GetNearstParcelShopsResponse wsPakkeshopSoap.GetNearstParcelShops(GetNearstParcelShopsRequest request)
        {
            return base.Channel.GetNearstParcelShops(request);
        }

        public ParcelShopSearchResult GetNearstParcelShops(string street, string zipcode, int Amount)
        {
            GetNearstParcelShopsRequest inValue = new GetNearstParcelShopsRequest();
            inValue.Body = new GetNearstParcelShopsRequestBody();
            inValue.Body.street = street;
            inValue.Body.zipcode = zipcode;
            inValue.Body.Amount = Amount;
            GetNearstParcelShopsResponse retVal = ((wsPakkeshopSoap)(this)).GetNearstParcelShops(inValue);
            return retVal.Body.GetNearstParcelShopsResult;
        }

        
        Task<GetNearstParcelShopsResponse> wsPakkeshopSoap.GetNearstParcelShopsAsync(GetNearstParcelShopsRequest request)
        {
            return base.Channel.GetNearstParcelShopsAsync(request);
        }

        public Task<GetNearstParcelShopsResponse> GetNearstParcelShopsAsync(string street, string zipcode, int Amount)
        {
            GetNearstParcelShopsRequest inValue = new GetNearstParcelShopsRequest();
            inValue.Body = new GetNearstParcelShopsRequestBody();
            inValue.Body.street = street;
            inValue.Body.zipcode = zipcode;
            inValue.Body.Amount = Amount;
            return ((wsPakkeshopSoap)(this)).GetNearstParcelShopsAsync(inValue);
        }
        */

        public ParcelShopSearchResult SearchNearestParcelShops(string street, string zipcode, int Amount)
        {
            return base.Channel.SearchNearestParcelShops(street, zipcode, Amount);
        }

        public Task<ParcelShopSearchResult> SearchNearestParcelShopsAsync(string street, string zipcode, int Amount)
        {
            return base.Channel.SearchNearestParcelShopsAsync(street, zipcode, Amount); ;
        }

        /*
        
        GetParcelShopDropPointResponse wsPakkeshopSoap.GetParcelShopDropPoint(GetParcelShopDropPointRequest request)
        {
            return base.Channel.GetParcelShopDropPoint(request);
        }

        public ParcelShopSearchResult GetParcelShopDropPoint(string street, string zipcode, int Amount)
        {
            GetParcelShopDropPointRequest inValue = new GetParcelShopDropPointRequest();
            inValue.Body = new GetParcelShopDropPointRequestBody();
            inValue.Body.street = street;
            inValue.Body.zipcode = zipcode;
            inValue.Body.Amount = Amount;
            GetParcelShopDropPointResponse retVal = ((wsPakkeshopSoap)(this)).GetParcelShopDropPoint(inValue);
            return retVal.Body.GetParcelShopDropPointResult;
        }

        
        Task<GetParcelShopDropPointResponse> wsPakkeshopSoap.GetParcelShopDropPointAsync(GetParcelShopDropPointRequest request)
        {
            return base.Channel.GetParcelShopDropPointAsync(request);
        }

        public Task<GetParcelShopDropPointResponse> GetParcelShopDropPointAsync(string street, string zipcode, int Amount)
        {
            GetParcelShopDropPointRequest inValue = new GetParcelShopDropPointRequest();
            inValue.Body = new GetParcelShopDropPointRequestBody();
            inValue.Body.street = street;
            inValue.Body.zipcode = zipcode;
            inValue.Body.Amount = Amount;
            return ((wsPakkeshopSoap)(this)).GetParcelShopDropPointAsync(inValue);
        }
         */


    }
}