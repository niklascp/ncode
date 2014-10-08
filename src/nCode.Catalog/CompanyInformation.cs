using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace nCode.Catalog
{
    /// <summary>
    /// Represents information about the company that legally sells through the Catalog.
    /// </summary>
    public static class CompanyInformation
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public static string Name
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.Name", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.Name", value);
            }
        }

        /// <summary>
        /// Gets or sets the address1.
        /// </summary>
        public static string Address1
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.Address1", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.Address1", value);
            }
        }

        /// <summary>
        /// Gets or sets the address2.
        /// </summary>
        public static string Address2
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.Address2", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.Address2", value);
            }
        }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public static string PostalCode
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.PostalCode", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.PostalCode", value);
            }
        }

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public static string City
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.City", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.City", value);
            }
        }

        /// <summary>
        /// Gets or sets the phone.
        /// </summary>
        public static string Phone
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.Phone", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.Phone", value);
            }
        }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public static string Email
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.Email", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.Email", value);
            }
        }


        /// <summary>
        /// Gets or sets the vat no.
        /// </summary>
        public static string VatNo
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.VatNo", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.VatNo", value);
            }
        }

        /// <summary>
        /// Gets or sets the logo URL.
        /// </summary>
        public static string LogoUrl
        {
            get
            {
                return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.LogoUrl", string.Empty);
            }
            set
            {
                Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.LogoUrl", value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the logo.
        /// </summary>
        public static int? LogoWidth
        {
            get
            {
                return Settings.GetProperty<int?>("nCode.Catalog.CompanyInformation.LogoWidth", null);
            }
            set
            {
                Settings.SetProperty<int?>("nCode.Catalog.CompanyInformation.LogoWidth", value);
            }
        }


        /// <summary>
        /// Gets an Url for the Sales and Delivery Terms for the given Culture.
        /// </summary>
        public static string GetTermsUrl(string culture)
        {
            return Settings.GetProperty<string>("nCode.Catalog.CompanyInformation.TermsUrl(" + culture + ")", string.Empty);
        }

        /// <summary>
        /// Sets the Url for the Sales and Delivery Terms for a given Culture.
        /// </summary>
        public static void SetTermsUrl(string culture, string url)
        {
            Settings.SetProperty<string>("nCode.Catalog.CompanyInformation.TermsUrl(" + culture + ")", url);
        }

        /// <summary>
        /// Returns an Xmm representation of the Company Information.
        /// </summary>
        /// <returns></returns>
        public static XElement AsXElement()
        {
            var data = new XElement("CompanyInformation",
                    new XElement("Name", Name),
                    new XElement("Address1", Address1),
                    new XElement("Address2", Address2),
                    new XElement("PostalCode", PostalCode),
                    new XElement("City", City),
                    new XElement("Phone", Phone),
                    new XElement("Email", Email),
                    new XElement("VatNo", VatNo));

            if (LogoUrl != string.Empty)
            {
                data.Add(new XElement("LogoUrl", Settings.Url.TrimEnd(new char[] { '/' }) + LogoUrl));
                if (LogoWidth != null)
                    data.Add(new XElement("LogoWidth", LogoWidth));
            }

            return data;
        }
    }
}
