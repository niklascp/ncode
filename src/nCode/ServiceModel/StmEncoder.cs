// This code is from the Microsoft WCF sample that can be found here:
// http://msdn.microsoft.com/en-us/library/ms751486(VS.85).aspx

using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;


namespace nCode.ServiceModel
{
	class StmEncoder : MessageEncoder
	{
		private readonly StmEncoderFactory _factory;
		private readonly XmlWriterSettings _writerSettings;
		private readonly string _contentType;

		public StmEncoder(StmEncoderFactory factory)
		{
			_factory = factory;
			_writerSettings = new XmlWriterSettings {Encoding = Encoding.GetEncoding(factory.CharSet)};
			_contentType = string.Format("{0}; charset={1}", _factory.MediaType, _writerSettings.Encoding.HeaderName);
		}


		public override string ContentType { get { return _contentType; } }


		public override string MediaType { get { return _factory.MediaType; } }


		public override MessageVersion MessageVersion { get { return _factory.MessageVersion; } }


		public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
		{
			byte[] msgContents = new byte[buffer.Count];
			Array.Copy(buffer.Array, buffer.Offset, msgContents, 0, msgContents.Length);
			bufferManager.ReturnBuffer(buffer.Array);

			MemoryStream stream = new MemoryStream(msgContents);
			return ReadMessage(stream, int.MaxValue);
		}


		public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
		{
			XmlReader reader = XmlReader.Create(stream);
			MessageVersion msgVersion = (this.MessageVersion ?? MessageVersion.None);
			return Message.CreateMessage(reader, maxSizeOfHeaders, msgVersion);
		}


		public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
		{
			byte[] messageBytes;
			int messageLength;
			using (MemoryStream stream = new MemoryStream())
			{
				using (XmlWriter writer = XmlWriter.Create(stream, _writerSettings))
				{
					if (writer != null)
					{
						message.WriteMessage(writer);
						writer.Close();						
					}
				}

				messageBytes = stream.GetBuffer();
				messageLength = (int) stream.Position;					
				stream.Close();
			}

			int totalLength = messageLength + messageOffset;
			byte[] totalBytes = bufferManager.TakeBuffer(totalLength);
			Array.Copy(messageBytes, 0, totalBytes, messageOffset, messageLength);

			ArraySegment<byte> byteArray = new ArraySegment<byte>(totalBytes, messageOffset, messageLength);
			return byteArray;
		}


		public override void WriteMessage(Message message, Stream stream)
		{
			XmlWriter writer = XmlWriter.Create(stream, _writerSettings);
			if (writer != null)
			{
				message.WriteMessage(writer);
				writer.Close();				
			}
		}
	}
}
