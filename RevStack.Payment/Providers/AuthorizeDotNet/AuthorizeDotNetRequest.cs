using System;
using System.Collections.Generic;

namespace RevStack.Payment.Providers.AuthorizeDotNet
{
    public class AuthorizeDotNetRequest : GatewayRequest
    {
        #region private members & constructors

        //private const string ChargeTestUrl = "https://test.authorize.net/gateway/transact.dll";
        private const string ChargeTestUrl = "https://secure.authorize.net/gateway/transact.dll";
        private const string ChargeLiveUrl = "https://secure.authorize.net/gateway/transact.dll";
        private const string SoapTestUrl = "https://apitest.authorize.net/soap/v1/Service.asmx";
        private const string SoapLiveUrl = "https://api.authorize.net/soap/v1/Service.asmx";
        private bool _testMode;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeDotNetRequest"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public AuthorizeDotNetRequest(string username, string password)
            : this(username, password, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeDotNetRequest"/> class.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="isTestMode">if set to <c>true</c> [is test mode].</param>
        public AuthorizeDotNetRequest(string username, string password, bool isTestMode)
        {
            TestMode = isTestMode;

            Post = new Dictionary<string, string>();
            Queue(AuthorizeDotNetApi.ApiLogin, username);
            Queue(AuthorizeDotNetApi.TransactionKey, password);
            if (isTestMode)
                Queue(AuthorizeDotNetApi.IsTestRequest, isTestMode.ToString().ToUpper());
            // default settings
            Queue(AuthorizeDotNetApi.DelimitData, "TRUE");
            Queue(AuthorizeDotNetApi.DelimitCharacter, "|");
            Queue(AuthorizeDotNetApi.RelayResponse, "TRUE");
            Queue(AuthorizeDotNetApi.EmailCustomer, "FALSE");
            Queue(AuthorizeDotNetApi.Method, "CC");
            Queue(AuthorizeDotNetApi.Country, "US");
            Queue(AuthorizeDotNetApi.ShipCountry, "US");
            Queue(AuthorizeDotNetApi.DuplicateWindowTime, "120");
        }

        /// <summary>
        /// Sets the API action.
        /// </summary>
        /// <param name="action">The action.</param>
        private void SetApiAction(RequestAction action)
        {
            var apiValue = "AUTH_CAPTURE";

            ApiAction = action;
            switch (action)
            {
                case RequestAction.Sale:
                    apiValue = "AUTH_CAPTURE";
                    PostUrl = TestMode ? ChargeTestUrl : ChargeLiveUrl;
                    break;
                case RequestAction.Authorize:
                    apiValue = "AUTH_ONLY";
                    PostUrl = TestMode ? ChargeTestUrl : ChargeLiveUrl;
                    break;
                case RequestAction.Settle:
                    apiValue = "PRIOR_AUTH_CAPTURE";
                    PostUrl = TestMode ? ChargeTestUrl : ChargeLiveUrl;
                    break;
                case RequestAction.Refund:
                    apiValue = "CREDIT";
                    PostUrl = TestMode ? ChargeTestUrl : ChargeLiveUrl;
                    break;
                case RequestAction.Void:
                    apiValue = "VOID";
                    PostUrl = TestMode ? ChargeTestUrl : ChargeLiveUrl;
                    break;
                case RequestAction.GetTransactions:
                    apiValue = "GET_TRANS";
                    PostUrl = TestMode ? SoapTestUrl : SoapLiveUrl;
                    break;
                case RequestAction.GetTransactionDetails:
                    apiValue = "GET_TRANS_DETAILS";
                    PostUrl = TestMode ? SoapTestUrl : SoapLiveUrl;
                    break;
                case RequestAction.CreateSubscription:
                    apiValue = "CREATE_SUBSCRIPTION";
                    PostUrl = TestMode ? SoapTestUrl : SoapLiveUrl;
                    break;
                case RequestAction.UpdateSubscription:
                    apiValue = "UPDATE_SUBSCRIPTION";
                    PostUrl = TestMode ? SoapTestUrl : SoapLiveUrl;
                    break;
                case RequestAction.CancelSubscription:
                    apiValue = "CANCEL_SUBSCRIPTION";
                    PostUrl = TestMode ? SoapTestUrl : SoapLiveUrl;
                    break;
                case RequestAction.GetSubscriptionStatus:
                    apiValue = "GET_SUBSCRIPTION_STATUS";
                    PostUrl = TestMode ? SoapTestUrl : SoapLiveUrl;
                    break;
            }
            Queue(AuthorizeDotNetApi.TransactionType, apiValue);
        }

        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether [test mode].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [test mode]; otherwise, <c>false</c>.
        /// </value>
        public bool TestMode
        {
            get { return _testMode; }
            set
            {
                _testMode = value;
            }
        }

        /// <summary>
        /// Gets or sets the post URL.
        /// </summary>
        /// <value>
        /// The post URL.
        /// </value>
        public override string PostUrl { get; set; }

        /// <summary>
        /// Gets or sets the API action.
        /// </summary>
        /// <value>
        /// The API action.
        /// </value>
        public override RequestAction ApiAction { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        public override void Validate()
        {
            //make sure we have all the fields we need
            //starting with the login/key pair
            AssertValidation(AuthorizeDotNetApi.ApiLogin, AuthorizeDotNetApi.TransactionKey);

            //each call has its own requirements... check each
            switch (ApiAction)
            {
                case RequestAction.Sale:
                case RequestAction.Authorize:
                    AssertValidation(AuthorizeDotNetApi.CreditCardNumber, AuthorizeDotNetApi.CreditCardExpiration,
                                     AuthorizeDotNetApi.Amount);
                    break;
                case RequestAction.Settle:
                    AssertValidation(AuthorizeDotNetApi.TransactionId);//, AuthorizeDotNetApi.AuthorizationCode);
                    break;
                case RequestAction.Refund:
                    AssertValidation(AuthorizeDotNetApi.TransactionId, AuthorizeDotNetApi.Amount,
                                     AuthorizeDotNetApi.CreditCardNumber);
                    break;
                case RequestAction.Void:
                    AssertValidation(AuthorizeDotNetApi.TransactionId);
                    break;
            }
        }

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
        public override GatewayRequest AddCustomer(string id, string first, string last, string address, string city, string state,
                                                   string zip, string phone, string email, string country)
        {
            Queue(AuthorizeDotNetApi.FirstName, first);
            Queue(AuthorizeDotNetApi.LastName, last);
            Queue(AuthorizeDotNetApi.Address, address);
            Queue(AuthorizeDotNetApi.City, city);
            Queue(AuthorizeDotNetApi.State, state);
            Queue(AuthorizeDotNetApi.Zip, zip);
            Queue(AuthorizeDotNetApi.CustomerId, id);
            return this;
        }

        /// <summary>
        /// Adds the shipping.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <param name="address">The address.</param>
        /// <param name="state">The state.</param>
        /// <param name="zip">The zip.</param>
        /// <returns></returns>
        public override GatewayRequest AddShipping(string first, string last, string address, string city, string state, string zip, string phone, string email, string country)
        {
            Queue(AuthorizeDotNetApi.ShipFirstName, first);
            Queue(AuthorizeDotNetApi.ShipLastName, last);
            Queue(AuthorizeDotNetApi.ShipAddress, address);
            Queue(AuthorizeDotNetApi.ShipCity, city);
            Queue(AuthorizeDotNetApi.ShipState, state);
            Queue(AuthorizeDotNetApi.ShipZip, zip);
            return this;
        }

        /// <summary>
        /// Adds the merchant value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public override GatewayRequest AddMerchantValue(string key, string value)
        {
            Queue(key, value);
            return this;
        }

        /// <summary>
        /// Adds the invoice.
        /// </summary>
        /// <param name="invoiceNumber">The invoice number.</param>
        /// <returns></returns>
        public override GatewayRequest AddInvoice(string invoiceNumber)
        {
            Queue(AuthorizeDotNetApi.InvoiceNumber, invoiceNumber);
            return this;
        }

        /// <summary>
        /// Adds the currency.
        /// </summary>
        /// <param name="currency">The currency.</param>
        /// <returns></returns>
        public override GatewayRequest AddCurrency(string currency)
        {
            if (string.IsNullOrEmpty(currency))
                currency = "USD";
            //Queue(AuthorizeDotNetApi.Currency, currency);
            return this;
        }
        #endregion

        #region Requests

        /// <summary>
        /// Authorizes a charge for the specified amount on the given credit card.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonthAndYear">The expiration month and year.</param>
        /// <param name="cvv">The CVV.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public override GatewayRequest Authorize(string cardNumber, string expirationMonthAndYear, string cvv,
                                                 decimal amount)
        {
            SetApiAction(RequestAction.Authorize);
            Queue(AuthorizeDotNetApi.CreditCardNumber, cardNumber);
            Queue(AuthorizeDotNetApi.CreditCardExpiration, expirationMonthAndYear);
            Queue(AuthorizeDotNetApi.CreditCardCode, cvv);
            Queue(AuthorizeDotNetApi.Amount, amount.ToString());
            return this;
        }

        /// <summary>
        /// Sales the specified card number.
        /// </summary>
        /// <param name="cardNumber">The card number.</param>
        /// <param name="expirationMonthAndYear">The expiration month and year.</param>
        /// <param name="cvv">The CVV.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public override GatewayRequest Sale(string cardNumber, string expirationMonthAndYear, string cvv, decimal amount)
        {
            SetApiAction(RequestAction.Sale);
            Queue(AuthorizeDotNetApi.CreditCardNumber, cardNumber);
            Queue(AuthorizeDotNetApi.CreditCardExpiration, expirationMonthAndYear);
            Queue(AuthorizeDotNetApi.CreditCardCode, cvv);
            Queue(AuthorizeDotNetApi.Amount, amount.ToString());
            return this;
        }

        /// <summary>
        /// Settles the transaction that matches the specified transaction id and amount.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public override GatewayRequest Settle(string transactionId, decimal amount)
        {
            SetApiAction(RequestAction.Settle);
            Queue(AuthorizeDotNetApi.TransactionId, transactionId);
            if (amount > 0) Queue(AuthorizeDotNetApi.Amount, amount.ToString());
            return this;
        }

        /// <summary>
        /// Voids the transaction that matches the specified transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <returns></returns>
        public override GatewayRequest Void(string transactionId)
        {
            SetApiAction(RequestAction.Void);
            Queue(AuthorizeDotNetApi.TransactionId, transactionId);
            return this;
        }

        /// <summary>
        /// Refunds the transaction that matches the specified transaction id.
        /// </summary>
        /// <param name="transactionId">The transaction id.</param>
        /// <param name="amount">The amount.</param>
        /// <param name="cardNumber">The card number.</param>
        /// <returns></returns>
        public override GatewayRequest Refund(string transactionId, decimal amount, string cardNumber)
        {
            SetApiAction(RequestAction.Refund);
            Queue(AuthorizeDotNetApi.TransactionId, transactionId);
            Queue(AuthorizeDotNetApi.CreditCardNumber, cardNumber);
            Queue(AuthorizeDotNetApi.Amount, amount.ToString());
            return this;
        }

        public override GatewayRequest GetTransactions(string batchId)
        {
            SetApiAction(RequestAction.GetTransactions);
            Queue(AuthorizeDotNetApi.BatchId, batchId);
            return this;
        }

        public override GatewayRequest GetTransactionDetails(string transactionId)
        {
            SetApiAction(RequestAction.GetTransactionDetails);
            Queue(AuthorizeDotNetApi.TransactionId, transactionId);
            return this;
        }

        /// <summary>
        /// Starts recurring billing charges for the subscriber.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <returns></returns>
        public override GatewayRequest Subscribe(string name, string description, decimal amount, string cardNumber, string expirationMonthAndYear, string cvv, 
                                                 short billingCycles, 
                                                 BillingInterval billingInterval,
                                                 short totalOccurrences,
                                                 DateTime startsOn, 
                                                 short trialOccurences, 
                                                 decimal trialAmount)
        {
            SetApiAction(RequestAction.CreateSubscription);
            Queue(AuthorizeDotNetApi.Amount, amount.ToString());
            Queue(AuthorizeDotNetApi.BillingCycles, billingCycles.ToString());
            Queue(AuthorizeDotNetApi.BillingInterval, Enum.GetName(typeof(BillingInterval), billingInterval));
            Queue(AuthorizeDotNetApi.CreditCardCode, cvv);
            Queue(AuthorizeDotNetApi.CreditCardExpiration, expirationMonthAndYear);
            Queue(AuthorizeDotNetApi.CreditCardNumber, cardNumber);
            //Queue(AuthorizeDotNetApi.Email, request.CustomerEmail);
            Queue(AuthorizeDotNetApi.TotalOccurences, totalOccurrences.ToString());
            Queue(AuthorizeDotNetApi.Description, description);
            Queue(AuthorizeDotNetApi.StartsOn, startsOn.ToString());
            Queue(AuthorizeDotNetApi.SubscriptionName, name);
            Queue(AuthorizeDotNetApi.TrialAmount, trialAmount.ToString());
            Queue(AuthorizeDotNetApi.TrialBillingCycles, trialOccurences.ToString());
            return this;
        }

        /// <summary>
        /// Updates recurring billing charges for the subscriber.
        /// </summary>
        /// <param name="request">The subscription request.</param>
        /// <returns></returns>
        public override GatewayRequest UpdateSubscription(string id, string name, string description, decimal amount, string cardNumber, string expirationMonthAndYear, string cvv, 
                                                          short trialBillingCycles,
                                                          decimal trialAmount)
        {
            SetApiAction(RequestAction.UpdateSubscription);
            Queue(AuthorizeDotNetApi.Amount, amount.ToString());
            //Queue(AuthorizeDotNetApi.BillingCycles, billingCycles.ToString());
            //Queue(AuthorizeDotNetApi.BillingInterval, billingInterval.ToString());
            Queue(AuthorizeDotNetApi.CreditCardCode, cvv);
            Queue(AuthorizeDotNetApi.CreditCardExpiration, expirationMonthAndYear);
            Queue(AuthorizeDotNetApi.CreditCardNumber, cardNumber);
            //Queue(AuthorizeDotNetApi.Email, request.CustomerEmail);
            //Queue(AuthorizeDotNetApi.TotalOccurences, totalOccurrences.ToString());
            Queue(AuthorizeDotNetApi.Description, description);
            //Queue(AuthorizeDotNetApi.StartsOn, startsOn.ToString());
            Queue(AuthorizeDotNetApi.SubscriptionID, id);
            Queue(AuthorizeDotNetApi.SubscriptionName, name);
            Queue(AuthorizeDotNetApi.TrialAmount, trialAmount.ToString());
            Queue(AuthorizeDotNetApi.TrialBillingCycles, trialBillingCycles.ToString());
            return this;
        }

        /// <summary>
        /// Stops the recurring billing charges that matches the specified subscription id.
        /// </summary>
        /// <param name="id">The subscription id.</param>
        /// <returns></returns>
        public override GatewayRequest CancelSubscription(string id) 
        {
            SetApiAction(RequestAction.CancelSubscription);
            Queue(AuthorizeDotNetApi.SubscriptionID, id);
            return this;
        }

        #endregion
    }
}