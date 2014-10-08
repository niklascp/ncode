using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Compilation;
using System.Web.UI;

namespace nCode.Catalog.Payment.Invoice
{
    public class InvoicePaymentProvider : PaymentProvider
    {
        public override IPaymentTypeSetupControl GetPaymentTypeSetupControl()
        {
            return (IPaymentTypeSetupControl)BuildManager.CreateInstanceFromVirtualPath("~/Admin/Catalog/Payment/Invoice/PaymentTypeSetupControl.ascx", typeof(Control));
        }

        public override IPaymentEditControl GetPaymentEditControl()
        {
            return null;
        }

        public override string GetIntructionString(CatalogModel model, Order order, PaymentType paymentType)
        {
            var prepayment = paymentType.GetProperty<bool>("Prepayment", false);
            if (order.Status == OrderStatus.Order && prepayment)
                return base.GetIntructionString(model, order, paymentType);
            else if (order.Status == OrderStatus.Invoice && order.PaymentStatus != PaymentStatus.Completed)
                return base.GetIntructionString(model, order, paymentType);
            
            return null;
        }

        public override DateTime GetDueDate(CatalogModel model, Order order, PaymentType paymentType)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            if (order.InvoiceDate == null)
                throw new ArgumentException("Cannot calculate DueDate for an order without InvoiceDate.");

            var invoiceData = order.InvoiceDate.Value;
            var dueDate = invoiceData;

            var numberOfCurrentMonths = paymentType.GetProperty<int>("NumberOfCurrentMonths", 0);
            var numberOfDays = paymentType.GetProperty<int>("NumberOfDays", 14);

            if (numberOfCurrentMonths > 0)
                dueDate = new DateTime(dueDate.Year, dueDate.Month, 1).AddMonths(numberOfCurrentMonths);

            dueDate = dueDate.AddDays(numberOfDays);

            if (dueDate.DayOfWeek == DayOfWeek.Saturday)
                dueDate = dueDate.AddDays(-1) >= invoiceData ? dueDate.AddDays(-1) : dueDate.AddDays(2); /* Correct to Friday, or monday if friday is before invoice date */
            if (dueDate.DayOfWeek == DayOfWeek.Sunday)
                dueDate = dueDate.AddDays(-2) >= invoiceData ? dueDate.AddDays(-2) : dueDate.AddDays(1); /* Correct to Friday, or monday if friday is before invoice date */

            return dueDate;
        }
    }
}
