using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Models
{
    public class HostMapping
    {
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("frontpagePath")]
        public string FrontpagePath { get; set; }

        [JsonProperty("masterPageFile")]
        public string MasterPageFile { get; set; }

        [JsonProperty("defaultCulture")]
        public string DefaultCulture { get; set; }
    }
}
