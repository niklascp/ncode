using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace nCode.IO.vCards
{
    /// <summary>
    /// Represents a vCard.
    /// </summary>
    public class VCard
    {
        private string _formattedName;
        private string _lastName;
        private string _firstName;
        private string _middleName;
        private string _namePrefix;
        private string _nameSuffix;
        private DateTime _birthDate = DateTime.MinValue;
        private IList<Address> _addresses = new List<Address>();
        private IList<PhoneNumber> _phoneNumber = new List<PhoneNumber>();
        private IList<string> _emailAddresses = new List<string>();
        private string _title;
        private string _role;
        private string _organization;

        /// <summary>
        /// Initializes a new eCard.
        /// </summary>
        public VCard()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        #region Public Properties

        /// <summary>
        /// Gets or sets the formatted name.
        /// </summary>
        public string FormattedName
        {
            get { return _formattedName; }
            set { _formattedName = value; }
        }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        /// <summary>
        /// Gets or sets the middle name.
        /// </summary>
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        /// <summary>
        /// Gets or sets the name prefix.
        /// </summary>
        public string NamePrefix
        {
            get { return _namePrefix; }
            set { _namePrefix = value; }
        }

        /// <summary>
        /// Gets or sets the name suffix.
        /// </summary>
        public string NameSuffix
        {
            get { return _nameSuffix; }
            set { _nameSuffix = value; }
        }

        /// <summary>
        /// Gets or sets the birth date.
        /// </summary>
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set { _birthDate = value; }
        }

        /// <summary>
        /// Gets a list of addresses.
        /// </summary>
        public IList<Address> Addresses
        {
            get { return _addresses; }
        }

        /// <summary>
        /// Gets a list of phone numbers.
        /// </summary>
        public IList<PhoneNumber> PhoneNumbers
        {
            get { return _phoneNumber; }
        }

        /// <summary>
        /// Gets a list of email addresses.
        /// </summary>
        public IList<string> EmailAddresses
        {
            get { return _emailAddresses; }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the role.
        /// </summary>
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        public string Organization
        {
            get { return _organization; }
            set { _organization = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Writes the vCard string to file.
        /// </summary>
        public void Generate(string filePath, FileMode mode)
        {
            using (FileStream fs = new FileStream(filePath, mode))
            {
                Generate(fs);
            }
        }

        /// <summary>
        /// Writes the vCard string to a sream.
        /// </summary>
        public void Generate(Stream outputStream)
        {
            StreamWriter sw = new StreamWriter(outputStream);
            sw.AutoFlush = true;
            sw.Write(this.ToString());
        }

        /// <summary>
        /// Converts this vCard to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //Start VCard
            sb.Append("BEGIN:VCARD\r\n");

            //Add Formatted name
            if (_formattedName != null)
            {
                sb.Append("FN:" + _formattedName + "\r\n");
            }

            //Add the name
            sb.Append("N:" + _lastName + ";");
            sb.Append(_firstName + ";");
            sb.Append(_middleName + ";");
            sb.Append(_namePrefix + ";");
            sb.Append(_nameSuffix + "\r\n");

            //Add a birthday
            if (_birthDate != DateTime.MinValue)
            {
                sb.Append("BDAY:" + _birthDate.ToString("yyyyMMdd") + "\r\n");
            }

            //Add Delivery Addresses
            foreach (Address da in _addresses)
            {
                sb.Append(da.ToString());
            }

            //Add phone numbers
            foreach (PhoneNumber phone in _phoneNumber)
            {
                sb.Append(phone.ToString());
            }

            //Add email address
            foreach (string email in _emailAddresses)
            {
                sb.Append("EMAIL; INTERNET:" + email + "\r\n");
            }

            //Add Title
            sb.Append("TITLE:" + _title + "\r\n");

            //Business Category
            sb.Append("ROLE:" + _role + "\r\n");

            //Organization
            sb.Append("ORG:" + _organization + "\r\n");

            sb.Append("END:VCARD\r\n");

            return sb.ToString();
        }

        #endregion
    }
}
