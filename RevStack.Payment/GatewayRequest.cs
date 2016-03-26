﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;

namespace RevStack.Payment
{
    public abstract class GatewayRequest
    {
        /// <summary>
        /// Gets or sets the post URL.
        /// </summary>
        /// <value>
        /// The post URL.
        /// </value>
        public abstract string PostUrl { get; set; }

        /// <summary>
        /// Gets or sets the API action.
        /// </summary>
        /// <value>
        /// The API action.
        /// </value>
        public abstract RequestAction ApiAction { get; set; }

        #region Fluent stuff

        /// <summary>
        /// Adds the customer.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <param name="address">The address.</param>
        /// <param name="city">The city.</param>
        /// <param name="state">The state.</param>
        /// <param name="zip">The zip.</param>
        /// <returns></returns>
        public abstract GatewayRequest AddCustomer(string id, string first, string last, string address, string city,
                                                   string state,
                                                   string zip, string phone, string email, string country);

        /// <summary>
        /// Adds the shipping.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <param name="address">The address.</param>
        /// <param name="state">The state.</param>
        /// <param name="zip">The zip.</param>
        /// <returns></returns>
        public abstract GatewayRequest AddShipping(string first, string last, string address, string city, string state, string zip, string phone, string email, string country);

        /// <summary>
        /// Adds the merchant value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public abstract GatewayRequest AddMerchantValue(string key, string value);

        /// <summary>
        /// Adds the invoice.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <returns></returns>
        public abstract GatewayRequest AddInvoice(string invoiceNumber);

        /// <summary>
        /// Adds the currency.
        /// </summary>
        /// <param name="invoiceNumber">The currency.</param>
        /// <returns></returns>
        public abstract GatewayRequest AddCurrency(string currency);

        #endregion

        #region Requests

        public abstract GatewayRequest Sale(string cardNumber, string expirationMonthAndYear, string cvv, decimal amount);

        /// <summary>
        /// Settles the transaction that matches the specified transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        public GatewayRequest Settle(string transactionId)
        {
            return Settle(transactionId, 0m);
        }

        /// <summary>
        /// Settles the transaction that matches the specified transaction id and amount.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public abstract GatewayRequest Settle(string transactionId, decimal amount);

        /// <summary>
        /// Voids the transaction that matches the specified transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        public abstract GatewayRequest Void(string transactionId);

        /// <summary>
        /// Refunds the transaction that matches the specified transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <returns></returns>
        public abstract GatewayRequest Refund(string transactionId, decimal amount, string cardNumber);

        /// <summary>
        /// Authorizes a charge for the specified amount on the given credit card.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonthAndYear">The expiration month and year.</param>
        /// <param name="cvv">The CVV.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public abstract GatewayRequest Authorize(string cardNumber, string expirationMonthAndYear, string cvv,
                                                 decimal amount);

        /// <summary>
        /// Gets all transactions by type.
        /// </summary>
        /// <param name="request">Transaction Type</param>
        /// <returns></returns>
        public abstract GatewayRequest GetTransactions(string batchId);

        /// <summary>
        /// Gets transaction by id.
        /// </summary>
        /// <param name="request">Transaction Id</param>
        /// <returns></returns>
        public abstract GatewayRequest GetTransactionDetails(string transactionId);

        /// <summary>
        /// Starts recurring billing charges for the given customer.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <returns></returns>
        public abstract GatewayRequest Subscribe(string name, string description, decimal amount, string cardNumber, string expirationMonthAndYear, string cvv,
                                                 short billingCycles, 
                                                 BillingInterval billingInterval,
                                                 short totalOccurrences,
                                                 DateTime startsOn,
                                                 short trialOccurrences, 
                                                 decimal trialAmount);

        /// <summary>
        /// Updates recurring billing charges for the given customer.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <returns></returns>
        public abstract GatewayRequest UpdateSubscription(string id, string name, string description, decimal amount, string cardNumber, string expirationMonthAndYear, string cvv, 
                                                          short trialOccurrences,
                                                          decimal trialAmount);

        /// <summary>
        /// Stops the recurring billing charges that matches the specified subscription id.
        /// </summary>
        /// <param name="id">The subscription id.</param>
        /// <returns></returns>
        public abstract GatewayRequest CancelSubscription(string id);

        /// <summary>
        /// Gets the recurring billing status that matches the specified subscription id.
        /// </summary>
        /// <param name="id">The subscription id.</param>
        /// <returns></returns>
        //public abstract GatewayRequest GetSubscriptionStatus(string id);

        #endregion

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public abstract void Validate();

        /// <summary>
        /// Asserts the validation.
        /// </summary>
        /// <param name="keys">The keys.</param>
        public void AssertValidation(params string[] keys)
        {
            var sb = new StringBuilder();
            foreach (var item in keys)
            {
                if (!Post.ContainsKey(item))
                {
                    sb.AppendFormat("{0}, ", item);
                }
                else
                {
                    if (string.IsNullOrEmpty(Post[item]))
                        sb.AppendFormat("No value for '{0}', which is required. ", item);
                }
                var result = sb.ToString();
                if (result.Length > 0)
                    throw new InvalidDataException("Can't submit to Gateway - missing these input fields: " +
                                                   result.Trim().TrimEnd(','));
            }
        }

        /// <summary>
        /// Gets or sets the post.
        /// </summary>
        /// <value>
        /// The post.
        /// </value>
        public Dictionary<string, string> Post { get; set; }

        public GatewayType GatewayType { get; set; }

        /// <summary>
        /// Queues the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool Queue(string key, string value)
        {
            if (Post.ContainsKey(key))
                Post.Remove(key);

            Post.Add(key, value);
            return true;
        }

        /// <summary>
        /// Converts the Post object to a string.
        /// </summary>
        /// <returns></returns>
        public string ToPostString()
        {
            var sb = new StringBuilder();
            foreach (var key in Post.Keys)
                sb.AppendFormat("{0}={1}&", key, HttpUtility.UrlEncode(Post[key]));

            var result = sb.ToString();
            return result.TrimEnd('&');
        }
    }
}