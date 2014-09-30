using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace nCode.Geographics.GsApi
{
    [DataContract]
    public class GsResponse
    {
        [DataMember(Name = "vejnavn")]
        public GsStreet Street { get; set; }

        [DataMember(Name = "husnr")]
        public string HouseNo { get; set; }

        [DataMember(Name = "supplerendebynavn")]
        public string PlaceName { get; set; }

        [DataMember(Name = "postnummer")]
        public GsPostalCode City { get; set; }

        [DataMember(Name = "kommune")]
        public GsMunicipality Municipality { get; set; }

        [DataMember(Name = "region")]
        public GsRegion Region { get; set; }

        [DataMember(Name = "wgs84koordinat")]
        public GsWgs84Coordinate Wgs84Coordinate { get; set; }
    }

    [DataContract]
    public class GsStreet
    {
        [DataMember(Name = "kode")]
        public string Code { get; set; }
        [DataMember(Name = "navn")]
        public string Name { get; set; }
    }


    [DataContract]
    public class GsPostalCode
    {
        [DataMember(Name = "nr")]
        public string PostalCode { get; set; }
        [DataMember(Name = "navn")]
        public string Name { get; set; }
    }


    [DataContract]
    public class GsMunicipality
    {
        [DataMember(Name = "kode")]
        public string Code { get; set; }
        [DataMember(Name = "navn")]
        public string Name { get; set; }
    }

    [DataContract]
    public class GsRegion
    {
        [DataMember(Name = "nr")]
        public string No { get; set; }
        [DataMember(Name = "navn")]
        public string Name { get; set; }
    }

    [DataContract]
    public class GsWgs84Coordinate
    {
        [DataMember(Name = "bredde")]
        public double Latitude { get; set; }
        [DataMember(Name = "længde")]
        public double Longitude { get; set; }
    }

}
