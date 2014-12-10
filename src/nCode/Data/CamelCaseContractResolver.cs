using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace nCode.Data
{
    class CamelCaseContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var res = base.CreateProperty(member, memberSerialization);

            var attrs = member.GetCustomAttributes(typeof(JsonPropertyAttribute), true);

            if (attrs.Any())
            {
                var attr = (attrs[0] as JsonPropertyAttribute);
                if (res.PropertyName != null && attr.PropertyName != null)
                    res.PropertyName = attr.PropertyName;
            }

            return res;
        }
    }
}
