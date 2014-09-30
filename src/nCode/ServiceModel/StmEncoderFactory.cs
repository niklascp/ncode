// This class uses code described here:
// ms-help://MS.VSCC.v90/MS.MSDNQTR.v90.en/fxref_system.servicemodel/html/a447d456-8eb9-58dd-8f5d-3430a58998c1.htm


using System.ServiceModel.Channels;


namespace nCode.ServiceModel
{
	public class StmEncoderFactory : MessageEncoderFactory
	{
		private readonly MessageEncoder _encoder;
		private readonly MessageVersion _version;
		private readonly string _mediaType;
		private readonly string _charSet;

		/// <summary>
		/// Create a new instance of the SimpleMessageEncoderFactory. The factory knows
		/// how to create SimpleMessageEncoders that can read and write to a stream for
		/// various types of message encoding
		/// </summary>
		/// <param name="mediaType"></param>
		/// <param name="charSet"></param>
		/// <param name="version"></param>
		internal StmEncoderFactory(string mediaType, string charSet, MessageVersion version)
		{
			_mediaType = mediaType;
			_charSet = charSet;
			_version = version;
			_encoder = new StmEncoder(this);
		}

		public override MessageEncoder Encoder { get { return _encoder; } }
		public override MessageVersion MessageVersion { get { return _version; } }
		public string MediaType { get { return _mediaType; } }
		public string CharSet { get { return _charSet; } }
	}
}
