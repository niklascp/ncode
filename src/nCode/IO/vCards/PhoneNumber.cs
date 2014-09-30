using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.IO.vCards
{
    /// <summary>
    /// Represents a vCard phone number.
    /// </summary>
    public class PhoneNumber
    {
        private string number;
        private PhoneNumberType type;
        
        /// <summary>
        /// Initializes a new phone number.
        /// </summary>
        public PhoneNumber()
        {
        }

        /// <summary>
        /// Initializes a new phone number.
        /// </summary>
        public PhoneNumber(string number, PhoneNumberType type)
        {
            this.number = number;
            this.type = type;
        }

        /// <summary>
        /// Gets or sets the number.
        /// </summary>
        public string Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
            }
        }

        /// <summary>
        /// Gets or sets the type of this phone number.
        /// </summary>
        public PhoneNumberType PhoneNumberType
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Converts this phone number to a vCard string.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("TEL;" + Enum.GetName(typeof(PhoneNumberType), PhoneNumberType) + ":");
            sb.Append(number + "\r\n");

            return sb.ToString();
        }
    }
}
