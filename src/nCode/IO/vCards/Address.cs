using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.IO.vCards
{
    /// <summary>
    /// Represents a vCard address.
    /// </summary>
    public class Address
    {
        private string _postOfficeAddress;
        private string _extendedAddress;
        private string _street;
        private string _locality;
        private string _region;
        private string _postalCode;
        private string _country;
        private AddressType addressType;

        /// <summary>
        /// Initializes a new vCard address.
        /// </summary>
        public Address()
        {

        }

        /// <summary>
        /// Gets or sets the post office address.
        /// </summary>
        public string PostOfficeAddress
        {
            get { return _postOfficeAddress; }
            set { _postOfficeAddress = value; }
        }

        /// <summary>
        /// Gets or sets the extended address.
        /// </summary>
        public string ExtendedAddress
        {
            get { return _extendedAddress; }
            set { _extendedAddress = value; }
        }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        public string Street
        {
            get { return _street; }
            set { _street = value; }
        }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        public string Locality
        {
            get { return _locality; }
            set { _locality = value; }
        }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        /// <summary>
        /// Gets or sets the type of this address.
        /// </summary>
        public AddressType AddressType
        {
            get { return addressType; }
            set { addressType = value; }
        }

        /// <summary>
        /// Convert this address to a vCard string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ADR;" + Enum.GetName(typeof(AddressType), AddressType) + ":");
            sb.Append(_postOfficeAddress + ";");
            sb.Append(_extendedAddress + ";");
            sb.Append(_street + ";");
            sb.Append(_locality + ";");
            sb.Append(_region + ";");
            sb.Append(_postalCode + ";");
            sb.Append(_country + "\r\n");

            return sb.ToString();
        }

    }
}
