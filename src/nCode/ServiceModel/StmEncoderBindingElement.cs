// This class uses code described here:
// ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/fxref_system.servicemodel/html/06dc0c0b-6b6b-30b8-bedd-6e965b8cac6f.htm


using System;
using System.Xml;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;


namespace nCode.ServiceModel
{
	public class StmEncoderBindingElement : MessageEncodingBindingElement, IWsdlExportExtension
	{
		private MessageVersion _messageVersion;
		private string _mediaType;
		private string _encoding;
		private readonly XmlDictionaryReaderQuotas _readerQuotas; 

		public StmEncoderBindingElement(string encoding, string mediaType, MessageVersion msgVersion)
		{
			if (encoding == null)
				throw new ArgumentNullException("encoding");

			if (mediaType == null)
				throw new ArgumentNullException("mediaType");

			if (msgVersion == null)
				throw new ArgumentNullException("msgVersion");

			_messageVersion = msgVersion;
			_mediaType = mediaType;
			_encoding = encoding;
			_readerQuotas = new XmlDictionaryReaderQuotas();
		}


		public StmEncoderBindingElement(string encoding, string mediaType) 
			: this(encoding, mediaType, MessageVersion.Soap11WSAddressing10)
		{
		}


		public StmEncoderBindingElement(string encoding)
			: this(encoding, "text.xml")
		{
		}


		public StmEncoderBindingElement()
			: this("UTF-8")
		{
		}


		private StmEncoderBindingElement(StmEncoderBindingElement binding)
			: this(binding.Encoding, binding.MediaType, binding.MessageVersion)
		{
			_readerQuotas = new XmlDictionaryReaderQuotas();
			binding.ReaderQuotas.CopyTo(_readerQuotas);
		}


		public override MessageVersion MessageVersion
		{
			get { return _messageVersion; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");
				
				_messageVersion = value;
			}
		}


		public string MediaType
		{
			get { return _mediaType; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_mediaType = value;
			}
		}


		public string Encoding
		{
			get { return _encoding; }
			set
			{
				if (value == null)
					throw new ArgumentNullException("value");

				_encoding = value;
			}
		}


		// This encoder does not enforces any quotas for unsecure messages. The 
		// quotas are enforced for the secure portions of messages when this 
		// encoder is used in a binding that is configured with security. 
		public XmlDictionaryReaderQuotas ReaderQuotas { get { return _readerQuotas; } }


		#region IMessageEncodingBindingElement Members

		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new StmEncoderFactory(this.MediaType, this.Encoding, this.MessageVersion);
		}

		#endregion


		public override BindingElement Clone()
		{
			return new StmEncoderBindingElement(this);
		}


		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			context.BindingParameters.Add(this);
			return context.BuildInnerChannelFactory<TChannel>();
		}

		public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			return context.CanBuildInnerChannelFactory<TChannel>();
		}

		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			context.BindingParameters.Add(this);
			return context.BuildInnerChannelListener<TChannel>();
		}

		public override bool CanBuildChannelListener<TChannel>(BindingContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");

			context.BindingParameters.Add(this);
			return context.CanBuildInnerChannelListener<TChannel>();
		}

		public override T GetProperty<T>(BindingContext context)
		{
			if (typeof(T) == typeof(XmlDictionaryReaderQuotas))
				return (T)(object)_readerQuotas;

			return base.GetProperty<T>(context);
		}


		#region IWsdlExportExtension Members

		void IWsdlExportExtension.ExportContract(WsdlExporter exporter, WsdlContractConversionContext context)
		{
		}


		void IWsdlExportExtension.ExportEndpoint(WsdlExporter exporter, WsdlEndpointConversionContext context)
		{
			// The MessageEncodingBindingElement is responsible for ensuring that the WSDL has the correct
			// SOAP version. We can delegate to the WCF implementation of TextMessageEncodingBindingElement for this.
			TextMessageEncodingBindingElement mebe = new TextMessageEncodingBindingElement {MessageVersion = _messageVersion};
			((IWsdlExportExtension)mebe).ExportEndpoint(exporter, context);
		}

		#endregion IWsdlExportExtension Members
	}
}
