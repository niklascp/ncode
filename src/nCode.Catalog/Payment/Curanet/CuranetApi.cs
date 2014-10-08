using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Serialization;

using nCode;
using nCode.ServiceModel;

namespace nCode.Catalog.Payment.Curanet
{

    public class CuranetApi : ClientBase<ICuranetApi>, ICuranetApi
    {
        private static Binding GetBinding() {            
            BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ProxyCredentialType = HttpProxyCredentialType.Basic;
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
            binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;

            CustomBinding cbinding = new CustomBinding(binding);

            /* Encoding work around - BasicHttpBinding only supports utf8, but Curanet is
             * stocked in the '90 and loves ISO-8859-1. Replace MessageEncodingBindingElement. */
            for (int i = 0; i < cbinding.Elements.Count; i++)
            {
                if (cbinding.Elements[i] is MessageEncodingBindingElement)
                {
                    StmEncoderBindingElement stmEncoderBindingElement = new StmEncoderBindingElement();
                    stmEncoderBindingElement.MediaType = "text/xml";
                    stmEncoderBindingElement.Encoding = "ISO-8859-1";
                    cbinding.Elements[i] = stmEncoderBindingElement;
                    break;
                }
            }

            return cbinding;
        }

        private static EndpointAddress GetEndpoint(CuranetPaymentProvider provider) {
            EndpointAddress endpoint = new EndpointAddress(provider.ApiUrl);
            return endpoint;
        }

        public CuranetApi(CuranetPaymentProvider provider)
            : base(GetBinding(), GetEndpoint(provider))
        {
            ClientCredentials.UserName.UserName = provider.Username;
            ClientCredentials.UserName.Password = provider.Password;
        }

        public int CaptureTransaction(int transactnum)
        {
            return Channel.CaptureTransaction(transactnum);
        }

        public int CancelTransaction(int transactnum)
        {
            return Channel.CancelTransaction(transactnum);
        }

        public int ReleaseSubscription(int transactnum)
        {
            return Channel.ReleaseSubscription(transactnum);
        }

        public int ChangeAmount(int transactnum, int amount)
        {
            return Channel.ChangeAmount(transactnum, amount);
        }

        public TransactionStatus CheckTransaction(int transactnum, string type, string orderid, string bankMerchantID, string boNo)
        {
            return Channel.CheckTransaction(transactnum, type, orderid, bankMerchantID, boNo);
        }

        public int RenewTransaction(int transactnum)
        {
            throw new NotImplementedException();
        }
    }

    /// <remarks/>
    [Serializable()]
    [SoapType("returnArray", Namespace = "urn:pgwapi")]
    public class TransactionStatus
    {
        public int returncode { get; set; }
        public string transactnum { get; set; }
        public string currency { get; set; }
        public int amount { get; set; }
        public string cardtype { get; set; }
        public string orderidprefix { get; set; }
        public string orderid { get; set; }
    }
}
