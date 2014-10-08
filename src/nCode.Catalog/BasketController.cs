using nCode.Catalog.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace nCode.Catalog
{
    /// <summary>
    /// Controls access to baskets.
    /// </summary>
    public static class BasketController
    {
        private const string salesChannelKey = "nCode.Catalog.SalesChannel";
        private const string basketsApplicationItem = "nCode.Catalog.Baskets";
        private const string basketSessionItem = "nCode.Catalog.Basket({0})";

        private static HttpContext GetHttpContext()
        {
            HttpContext context = HttpContext.Current;

            if (context == null)
                throw new InvalidOperationException("Must have a current HttpContext.");

            if (context.Session == null)
                throw new InvalidOperationException("Must have a current Session.");
            return context;
        }

        /// <summary>
        /// Sets the Sales Channel for this Http Request.
        /// </summary>
        /// <param name="code">The code.</param>
        public static void SetSalesChannel(string code)
        {
            using (var db = new CatalogDbContext())
            {
                var context = GetHttpContext();

                context.Items[salesChannelKey] = db.SalesChannels.Where(x => x.Code == code).Single();
            }
        }

        /// <summary>
        /// Gets the Sales Channel, that this Request is related to.
        /// </summary>
        /// <value>
        /// The sales channel.
        /// </value>
        public static SalesChannel SalesChannel
        {
            get
            {
                var context = GetHttpContext();

                if (context.Items[salesChannelKey] == null)
                    SetSalesChannel(null);

                return (SalesChannel)context.Items[salesChannelKey];
            }
        }

        /// <summary>
        /// Gets or sets the current basket of the user.
        /// </summary>
        public static Basket CurrentBasket
        {
            get
            {
                var context = GetHttpContext();
                var key = string.Format(basketSessionItem, SalesChannel.Code);

                var basket = context.Session[key] as Basket;

                if (basket == null)
                {
                    basket = new Basket(SalesChannel.Code);
                    context.Session[key] = basket;
                }

                return basket;
            }
            set
            {
                var context = GetHttpContext();
                var key = string.Format(basketSessionItem, SalesChannel.Code);

                context.Session[key] = value;
            }
        }        
    }
}
