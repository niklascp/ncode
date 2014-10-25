using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;

using Dapper;

namespace nCode.Catalog.Controllers
{
    [RoutePrefix("admin/catalog/statistics")]
    public class StatisticsController : ApiController
    {
        [Route("topbrands")]
        public dynamic GetTopBrandsBySale(DateTime fromDate, DateTime toDate)
        {
            using (var conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                var sql = @"
with 
/* Calculates sales in the default currency. */
sales AS (
    select
        brandName = b.[Name],
        vatIncluded = o.[VatIncluded],
        vatGroupCode = oi.[VatGroupCode],
        currencyCode = dc.[Code],
        price = sum(case
            when oc.[Code] <> dc.[Code] then dc.[Rate] / oc.[Rate] 
            else 1
        end * oi.[Qty] * oi.[UnitPrice])
    from
        Catalog_Brand b 
        join Catalog_Item i on i.[Brand] = b.[ID]
        join Catalog_OrderItem oi on oi.[ItemNo] = i.[ItemNo] 
        join Catalog_Order o on o.[OrderNo] = oi.[OrderNo]    
        join Catalog_Currency oc ON oc.[Code] = o.[CurrencyCode]
        join Catalog_Currency dc ON dc.[IsDefault] = 1
    where
        o.[Status] = 1 AND o.[InvoiceDate] between @fromDate and @toDate
    group by
        b.[Name],
        o.[VatIncluded],
        oi.[VatGroupCode],
        dc.[Code]
)
/* Calculates sale and add Vat */
select top 10
    brandName,
    currencyCode,
    amount = sum(case
            when s.[VatIncluded] = 0 and vg.[Rate] > 0 then 1 + vg.[Rate] 
            else 1
        end * s.[Price])
from
    sales s
    left join Catalog_VatGroup vg ON vg.[Code] = s.[VatGroupCode]
group by
    brandName,
    currencyCode
order by
    amount desc;";

                return conn.Query(sql, new { fromDate  = fromDate, toDate = toDate });
            }

        }
    }
}
