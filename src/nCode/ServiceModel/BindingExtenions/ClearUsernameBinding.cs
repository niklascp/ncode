using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;


/*
 * Want more WCF tips?
 * Visit http://webservices20.blogspot.com/
 */


namespace nCode.ServiceModel.BindingExtenions
{
    public class ClearUsernameBinding : CustomBinding
    {
        private MessageVersion messageVersion = MessageVersion.None;

        public void SetMessageVersion(MessageVersion value)
        {
            this.messageVersion = value;
        }

        public override BindingElementCollection CreateBindingElements()
        {
            var res = new BindingElementCollection();
            res.Add(new TextMessageEncodingBindingElement() { MessageVersion = this.messageVersion});

            var securityElement = SecurityBindingElement.CreateUserNameOverTransportBindingElement();
            securityElement.EnableUnsecuredResponse = true;
            res.Add(securityElement);

            res.Add(new AutoSecuredHttpTransportElement());
            return res;
        }

        public override string Scheme
        {
            get
            {
                return "http";
            }
        }
    }
}
