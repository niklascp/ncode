using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace nCode.Catalog.Controllers
{
    [RoutePrefix("admin/catalog/package")]
    public class PackageController : ApiController
    {
        [Route]
        public HttpResponseMessage Get()
        {
            var draft = Settings.GetProperty<PackageDraftItem[]>("nCode.Catalog.PackageDraft", null) ?? new PackageDraftItem[] { };

            return Request.CreateResponse(HttpStatusCode.OK, draft);
        }

        [Route]
        public HttpResponseMessage PostSet([FromBody]string[] orderNoSet)
        {
            var draft = Settings.GetProperty<PackageDraftItem[]>("nCode.Catalog.PackageDraft", null) ?? new PackageDraftItem[] { };
            var workingDraft = new List<PackageDraftItem>(draft);

            foreach (var orderNo in orderNoSet)
            {
                var existing = workingDraft.FirstOrDefault(x => x.OrderNo == orderNo);

                if (existing == null)
                {
                    workingDraft.Add(new PackageDraftItem
                    {
                        OrderNo = orderNo,
                    });
                }
            }

            Settings.SetProperty<PackageDraftItem[]>("nCode.Catalog.PackageDraft", workingDraft.ToArray());

            return Request.CreateResponse(HttpStatusCode.Created);
        }

        [Route("{orderNo}")]
        public HttpResponseMessage Post(string orderNo, [FromBody]PackageDraftItem item)
        {
            var draft = Settings.GetProperty<PackageDraftItem[]>("nCode.Catalog.PackageDraft", null) ?? new PackageDraftItem[] { };

            var existing = draft.FirstOrDefault(x => x.OrderNo == orderNo);

            if (existing == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            existing.PackageCount = item.PackageCount;
            existing.PackageProductCode = item.PackageProductCode;
            existing.PackageProductServices = item.PackageProductServices ?? new string[] { };

            Settings.SetProperty<PackageDraftItem[]>("nCode.Catalog.PackageDraft", draft.ToArray());

            return Request.CreateResponse(HttpStatusCode.Found);
        }

    }

    public class PackageDraftItem
    {
        public PackageDraftItem()
        {
            PackageProductCode = "P19DK";
            PackageProductServices = new string[] { };
            PackageCount = 1;
        }

        [JsonProperty("orderNo")]
        public string OrderNo { get; set; }

        [JsonProperty("packageProductCode")]
        public string PackageProductCode { get; set; }

        [JsonProperty("packageProductServices")]
        public string[] PackageProductServices { get; set; }

        [JsonProperty("packageCount")]
        public int PackageCount { get; set; }
    }
}