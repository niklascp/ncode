// This code is from the Microsoft WCF sample that can be found here:
// http://msdn.microsoft.com/en-us/library/ms751486(VS.85).aspx


using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.ServiceModel.Channels;

namespace nCode.ServiceModel
{
	/// <summary>
	/// Converts the string representation of a MessageVersion into the corresponding MessageVersion.
	/// </summary>
	class MessageVersionConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
				return true;

			return base.CanConvertFrom(context, sourceType);
		}


		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;

			return base.CanConvertTo(context, destinationType);
		}


		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
		{
			if (value is string)
			{
				string messageVersionAsString = (string)value;
				MessageVersion messageVersion = null;
				switch (messageVersionAsString)
				{
					case ConfigurationStrings.Soap11WSAddressing10:
						messageVersion = MessageVersion.Soap11WSAddressing10;
						break;
					case ConfigurationStrings.Soap12WSAddressing10:
						messageVersion = MessageVersion.Soap12WSAddressing10;
						break;
					case ConfigurationStrings.Soap11WSAddressingAugust2004:
						messageVersion = MessageVersion.Soap11WSAddressingAugust2004;
						break;
					case ConfigurationStrings.Soap12WSAddressingAugust2004:
						messageVersion = MessageVersion.Soap12WSAddressingAugust2004;
						break;
					case ConfigurationStrings.Soap11:
						messageVersion = MessageVersion.Soap11;
						break;
					case ConfigurationStrings.Soap12:
						messageVersion = MessageVersion.Soap12;
						break;
					case ConfigurationStrings.None:
						messageVersion = MessageVersion.None;
						break;
					case ConfigurationStrings.Default:
						messageVersion = MessageVersion.Default;
						break;
					default:
						throw new ArgumentOutOfRangeException("value");
				}

				return messageVersion;
			}

			return base.ConvertFrom(context, culture, value);
		}


		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (typeof(string) == destinationType && value is MessageVersion)
			{
				string messageVersionAsString = null;
				MessageVersion messageVersion = (MessageVersion)value;

				if (messageVersion == MessageVersion.Default)
				{
					messageVersionAsString = ConfigurationStrings.Default;
				}
				else if (messageVersion == MessageVersion.Soap11WSAddressing10)
				{
					messageVersionAsString = ConfigurationStrings.Soap11WSAddressing10;
				}
				else if (messageVersion == MessageVersion.Soap12WSAddressing10)
				{
					messageVersionAsString = ConfigurationStrings.Soap12WSAddressing10;
				}
				else if (messageVersion == MessageVersion.Soap11WSAddressingAugust2004)
				{
					messageVersionAsString = ConfigurationStrings.Soap11WSAddressingAugust2004;
				}
				else if (messageVersion == MessageVersion.Soap12WSAddressingAugust2004)
				{
					messageVersionAsString = ConfigurationStrings.Soap12WSAddressingAugust2004;
				}
				else if (messageVersion == MessageVersion.Soap11)
				{
					messageVersionAsString = ConfigurationStrings.Soap11;
				}
				else if (messageVersion == MessageVersion.Soap12)
				{
					messageVersionAsString = ConfigurationStrings.Soap12;
				}
				else if (messageVersion == MessageVersion.None)
				{
					messageVersionAsString = ConfigurationStrings.None;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value");
				}
				return messageVersionAsString;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
	}
}
