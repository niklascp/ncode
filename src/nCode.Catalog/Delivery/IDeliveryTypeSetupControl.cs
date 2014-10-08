using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog.Delivery
{
    /// <summary>
    /// Interface for Delivery Type Setup Controls
    /// </summary>
    public interface IDeliveryTypeSetupControl
    {
        /// <summary>
        /// Initializes the control with the specified model.
        /// </summary>
        void Initialize(CatalogModel model);

        /// <summary>
        /// Called when the control needs to load the given Delivery Type.
        /// </summary>
        void LoadDeliveryType(CatalogModel model, DeliveryType deliveryType);

        /// <summary>
        /// Called when the control needs to save the given Delivery Type.
        /// </summary>
        void SaveDeliveryType(CatalogModel model, DeliveryType deliveryType);
    }
}
