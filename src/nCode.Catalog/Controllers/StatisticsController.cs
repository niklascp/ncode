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
        #region Declare SQL Statements.
  
        const string saleByInvoiceDate = @"
/* For testing purposes only: */
--declare @fromDate date = '2014-01-01';
--declare @toDate date = '2014-12-31';

with 
/* Calculates sales in the default currency. */
sales AS (
    select
        [InvoiceDate] = cast(o.[InvoiceDate] as date),
        [VatIncluded] = o.[VatIncluded],
        [VatGroupCode] = oi.[VatGroupCode],
        [OrderCount] = count(distinct o.[OrderNo]),
        [ItemCount] = sum(oi.[Qty]),
        [Price] = sum(case
            when oc.[Code] <> dc.[Code] then dc.[Rate] / oc.[Rate] 
            else 1
        end * oi.[Qty] * oi.[UnitPrice])
    from
        [Catalog_Order] o 
        join [Catalog_OrderItem] oi on oi.[OrderNo] = o.[OrderNo] 
        join [Catalog_Currency] oc ON oc.[Code] = o.[CurrencyCode]
        join [Catalog_Currency] dc ON dc.[IsDefault] = 1
    where
        o.[Status] = 1 AND o.[InvoiceDate] between @fromDate and @toDate
    group by
        o.[InvoiceDate],
        o.[VatIncluded],
        oi.[VatGroupCode]
)
/* Calculates sale and add Vat */
select 
    [InvoiceDate],
    [OrderCount] = sum([OrderCount]),
    [ItemCount] = sum([ItemCount]),
    [Amount] = sum(case
            when s.[VatIncluded] = 0 and vg.[Rate] > 0 then 1 + vg.[Rate] 
            else 1
        end * s.[Price])
from
    sales s
    left join [Catalog_VatGroup] vg ON vg.[Code] = s.[VatGroupCode]
group by
    [InvoiceDate]
order by
    [InvoiceDate];
";

        const string saleByHourStatement = @"
/* For testing purposes only: */
--declare @fromDate date = '2014-01-01';
--declare @toDate date = '2014-12-31';

with 
/* Calculates sales in the default currency. */
sales AS (
    select
        [Hour] = datepart(hour, o.[Created]),
        [VatIncluded] = o.[VatIncluded],
        [VatGroupCode] = oi.[VatGroupCode],
        [OrderCount] = count(distinct o.[OrderNo]),
        [ItemCount] = sum(oi.[Qty]),
        [Price] = sum(case
            when oc.[Code] <> dc.[Code] then dc.[Rate] / oc.[Rate] 
            else 1
        end * oi.[Qty] * oi.[UnitPrice])
    from
        [Catalog_Order] o 
        join [Catalog_OrderItem] oi on oi.[OrderNo] = o.[OrderNo] 
        join [Catalog_Currency] oc ON oc.[Code] = o.[CurrencyCode]
        join [Catalog_Currency] dc ON dc.[IsDefault] = 1
    where
        o.[Status] = 1 AND o.[InvoiceDate] between @fromDate and @toDate
    group by
        datepart(hour, o.[Created]),
        o.[VatIncluded],
        oi.[VatGroupCode]
)
/* Calculates sale and add Vat */
select 
    [Hour],
    [OrderCount] = sum([OrderCount]),
    [ItemCount] = sum([ItemCount]),
    [Amount] = sum(case
            when s.[VatIncluded] = 0 and vg.[Rate] > 0 then 1 + vg.[Rate] 
            else 1
        end * s.[Price])
from
    sales s
    left join [Catalog_VatGroup] vg ON vg.[Code] = s.[VatGroupCode]
group by
    [Hour]
order by
    [Hour];
";

        #endregion

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

        [Route("sale")]
        public dynamic GetSale(DateTime fromDate, DateTime toDate, string step = null)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                DateTime parallelFromDate;
                DateTime parallelToDate;

                conn.Open();

                Func<DateTime, DateTime> stepExp;
                string stepFmt;

                if (step != null && step.Equals("days", StringComparison.OrdinalIgnoreCase))
                {
                    stepExp = x => x.AddDays(1);
                    stepFmt = "dd/MM";

                    parallelFromDate = fromDate.AddMonths(-1);
                    parallelToDate = toDate.AddMonths(-1);
                }
                else
                {
                    stepExp = x => x.AddMonths(1);
                    stepFmt = "MMM yyyy";

                    parallelFromDate = fromDate.AddYears(-1);
                    parallelToDate = toDate.AddYears(-1);
                }

                var salePerDate = conn.Query<SalePerDate>(saleByInvoiceDate, new { fromDate = fromDate, toDate = toDate });
                var parallelSalePerDate = conn.Query<SalePerDate>(saleByInvoiceDate, new { fromDate = parallelFromDate, toDate = parallelToDate });

                var categories = new List<string>();

                var orderCount = new List<int>();
                var parallelOrderCount = new List<int>();                

                var sale = new List<decimal>();
                var parallelSale = new List<decimal>();
                
                var date = fromDate;
                var parallelDate = parallelFromDate;

                while (date <= toDate)
                {
                    /* Todo - */
                    var nextDate = stepExp(date);
                    var nextParallelDate = stepExp(parallelDate);

                    categories.Add(date.ToString(stepFmt));

                    orderCount.Add(salePerDate.Where(x => date <= x.InvoiceDate && x.InvoiceDate < nextDate).Sum(x => x.OrderCount));
                    parallelOrderCount.Add(parallelSalePerDate.Where(x => parallelDate <= x.InvoiceDate && x.InvoiceDate < nextParallelDate).Sum(x => x.OrderCount));

                    sale.Add(salePerDate.Where(x => date <= x.InvoiceDate && x.InvoiceDate < nextDate).Sum(x => x.Amount));
                    parallelSale.Add(parallelSalePerDate.Where(x => parallelDate <= x.InvoiceDate && x.InvoiceDate < nextParallelDate).Sum(x => x.Amount));

                    date = nextDate;
                    parallelDate = nextParallelDate;
                }

                var totalOrderCount = orderCount.Sum();
                var totalParallelOrderCount = parallelOrderCount.Sum();

                var totalSale = sale.Sum();
                var totalParallelSale = parallelSale.Sum();

                return new
                {
                    currencyCode = CurrencyController.DefaultCurrency.Code,
                    categories = categories,

                    orderCount = orderCount,
                    parallelOrderCount = parallelOrderCount,
                    orderCountTrend = totalOrderCount > 1.1m * totalParallelOrderCount ? 1 : totalOrderCount < 0.9m * totalParallelOrderCount ? -1 : 0,

                    sale = sale,
                    parallelSale = parallelSale,
                    saleTrend = totalSale > 1.1m * totalParallelSale ? 1 :  totalSale < 0.9m * totalParallelSale ? -1 : 0,

                    totalOrderCount,
                    totalParallelOrderCount,

                    totalSale,
                    totalParallelSale                    
                };
            }
        }

        [Route("salebyhour")]
        public dynamic GetSaleByHour(DateTime fromDate, DateTime toDate)
        {
            using (var conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                var saleByHour = conn.Query<SaleByHour>(saleByHourStatement, new { fromDate = fromDate, toDate = toDate });

                var parallelFromDate = fromDate.AddYears(-1);
                var parallelToDate = toDate.AddYears(-1);
                var parallelSaleByHour = conn.Query<SaleByHour>(saleByHourStatement, new { fromDate = parallelFromDate, toDate = parallelToDate });

                var categories = new List<string>();

                var orderCount = new List<int>();
                var parallelOrderCount = new List<int>();

                var sale = new List<decimal>();
                var parallelSale = new List<decimal>();

                for (var hour = 0; hour < 24; hour++)
                {
                    categories.Add(hour.ToString());

                    orderCount.Add(saleByHour.Where(x => hour == x.Hour).Sum(x => x.OrderCount));
                    parallelOrderCount.Add(parallelSaleByHour.Where(x => hour == x.Hour).Sum(x => x.OrderCount));

                    sale.Add(saleByHour.Where(x => hour == x.Hour).Sum(x => x.Amount));
                    parallelSale.Add(parallelSaleByHour.Where(x => hour == x.Hour).Sum(x => x.Amount));
                }

                return new
                {
                    currencyCode = CurrencyController.DefaultCurrency.Code,
                    categories = categories,

                    orderCount = orderCount,
                    parallelOrderCount = parallelOrderCount,
                 
                    sale = sale,
                    parallelSale = parallelSale,                 
                };
            }
        }

        public class SaleByHour
        {
            public int Hour { get; set; }
            public int OrderCount { get; set; }
            public int ItemCount { get; set; }
            public decimal Amount { get; set; }
        }

        public class SalePerDate
        {
            public DateTime InvoiceDate { get; set; }
            public int OrderCount { get; set; }
            public int ItemCount { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
