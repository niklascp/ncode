using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Runtime.Serialization;

namespace nCode.Catalog.Payment.Curanet
{
    /// <summary>
    /// Interface for the Curanet Payment Gateway Api.
    /// </summary>
    [ServiceContract(Namespace = "urn:pgwapi")]
    public interface ICuranetApi
    {

        /// <summary>
        /// Captures the transaction.
        /// </summary>
        /// <param name="transactnum">The transactnum.</param>
        /// <returns></returns>
        [OperationContract(Action = "urn:pgwapi#captureTransaction", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        int CaptureTransaction(int transactnum);

        /// <summary>
        /// Cancels the transaction.
        /// </summary>
        /// <param name="transactnum">The transactnum.</param>
        /// <returns></returns>
        [OperationContract(Action = "urn:pgwapi#cancelTransaction", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        int CancelTransaction(int transactnum);

        [OperationContract(Action = "urn:pgwapi#releaseSubscription", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        int ReleaseSubscription(int transactnum);

        /// <summary>
        /// Changes the amount.
        /// </summary>
        /// <param name="transactnum">The transactnum.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        [OperationContract(Action = "urn:pgwapi#changeAmount", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        int ChangeAmount(int transactnum, int amount);

        /// <summary>
        /// Checks the transaction.
        /// </summary>
        /// <param name="transactnum">The transactnum.</param>
        /// <param name="type">The type.</param>
        /// <param name="orderid">The orderid.</param>
        /// <param name="bankMerchantID">The bank merchant unique identifier.</param>
        /// <param name="boNo">The bo no.</param>
        /// <returns></returns>
        [OperationContract(Action = "urn:pgwapi#checkTransaction", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        TransactionStatus CheckTransaction(int transactnum, string type, string orderid, string bankMerchantID, string boNo);

        /// <summary>
        /// Renews the transaction.
        /// </summary>
        /// <param name="transactnum">The transactnum.</param>
        /// <returns></returns>
        [OperationContract(Action = "urn:pgwapi#renewTransaction", ReplyAction = "*")]
        [XmlSerializerFormat(Style = OperationFormatStyle.Rpc, Use = OperationFormatUse.Encoded)]
        [return: MessageParameter(Name = "return")]
        int RenewTransaction(int transactnum);
    }
}