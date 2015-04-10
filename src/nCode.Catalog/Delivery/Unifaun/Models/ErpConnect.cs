using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace nCode.Catalog.Delivery.Unifaun.Models
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "pacsoft", Namespace = "", IsNullable = false)]
    public partial class PacsoftDocument
    {
        public PacsoftDocument()
        {
            Shipments = new List<PacsoftShipment>();
            Receivers = new List<PacsoftReceiver>();
        }
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("shipment")]
        public List<PacsoftShipment> Shipments { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("receiver")]
        public List<PacsoftReceiver> Receivers { get; set; }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    public partial class PacsoftShipment
    {
        public PacsoftShipment()
        {
            Values = new List<PacsoftNameValue>();
        }

        /// <remarks/>
        [XmlElement("val")]
        public List<PacsoftNameValue> Values { get; set; }

        /// <remarks/>
        [XmlElement("service")]
        public PacsoftShipmentService Service { get; set; }

        /// <remarks/>
        [XmlElement("container")]
        public PacsoftShipmentContainer Container { get; set; }

        /// <remarks/>
        [XmlAttribute("orderno")]
        public string OrderNo { get; set; }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    public partial class PacsoftNameValue
    {
        [XmlAttribute("n")]
        public string Name { get; set; }

        /// <remarks/>
        [XmlTextAttribute()]
        public string Value { get; set; }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    public partial class PacsoftShipmentService
    {
        public PacsoftShipmentService()
        {
            Addons = new List<PacsoftShipmentServiceAddon>();
        }

        /// <remarks/>
        [XmlElement("addon")]
        public List<PacsoftShipmentServiceAddon> Addons { get; set; }

        /// <remarks/>
        [XmlAttribute("srvid")]
        public string ServiceId { get; set; }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    public partial class PacsoftShipmentServiceAddon
    {
        /// <remarks/>
        [XmlAttribute("adnid")]
        public string AddonId { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class PacsoftShipmentContainer
    {
        public PacsoftShipmentContainer()
        {
            Values = new List<PacsoftNameValue>();
        }

        [XmlElement("val")]
        public List<PacsoftNameValue> Values { get; set;}

        /// <remarks/>
        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    /// <remarks/>
    [XmlType(AnonymousType = true)]
    public partial class PacsoftReceiver
    {
        public PacsoftReceiver()
        {
            Values = new List<PacsoftNameValue>();
        }

        [XmlElement("val")]
        public List<PacsoftNameValue> Values { get; set; }

        /// <remarks/>
        [XmlAttribute("rcvid")]
        public string ReceiverId { get; set; }
    }
}
