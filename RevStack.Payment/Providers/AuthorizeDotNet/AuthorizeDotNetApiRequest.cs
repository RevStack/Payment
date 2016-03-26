using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RevStack.Payment.net.authorize.api;

namespace RevStack.Payment.Providers.AuthorizeDotNet
{
    public class AuthorizeDotNetApiRequest : IGatewayApiRequest
    {
        public IGatewayResponse Send(GatewayRequest request)
        {
            IGatewayResponse response = null;

            if (request.ApiAction == RequestAction.Authorize ||
                request.ApiAction == RequestAction.Refund || 
                request.ApiAction == RequestAction.Sale ||
                request.ApiAction == RequestAction.Settle || 
                request.ApiAction == RequestAction.Void) 
            {
                response = SendHttpChargeRequest(request);
            }

            if (request.ApiAction == RequestAction.GetTransactions)
                response = SendGetTransactionsRequest(request);

            if (request.ApiAction == RequestAction.GetTransactionDetails)
                response = SendGetTransactionDetailsRequest(request);

            if (request.ApiAction == RequestAction.CreateSubscription)
                response = SendCreateSubscriptionRequest(request);

            if (request.ApiAction == RequestAction.CancelSubscription)
                response = SendCancelSubscriptionRequest(request);

            if (request.ApiAction == RequestAction.UpdateSubscription)
                response = SendUpdateSubscriptionRequest(request);

            return response;
        }

        private IGatewayResponse SendHttpChargeRequest(GatewayRequest request) 
        {
            //validate the inputs
            request.Validate();
            var postData = request.ToPostString();

            //override the local cert policy
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            var serviceUrl = request.PostUrl;
            var webRequest = (HttpWebRequest)WebRequest.Create(serviceUrl);
            webRequest.Method = "POST";
            webRequest.ContentLength = postData.Length;
            webRequest.ContentType = "application/x-www-form-urlencoded";

            // post data is sent as a stream
            var myWriter = new StreamWriter(webRequest.GetRequestStream());
            myWriter.Write(postData);
            myWriter.Close();

            // returned values are returned as a stream, then read into a string
            var response = (HttpWebResponse)webRequest.GetResponse();
            var rawResponseStream = response.GetResponseStream();

            var result = string.Empty;
            if (rawResponseStream != null)
                using (var responseStream = new StreamReader(rawResponseStream))
                {
                    result = responseStream.ReadToEnd();
                    responseStream.Close();
                }

            IGatewayResponse gatewayResponse = new AuthorizeDotNetResponse(result,
                                                              request.Post[AuthorizeDotNetApi.DelimitCharacter].
                                                                  ToCharArray()
                                                                  [0]);

            return gatewayResponse;
        }

        private IGatewayResponse SendUnsettledTransactionsRequest(GatewayRequest request)
        {
            var result = string.Empty;
            IGatewayResponse gatewayResponse;

            var authentication = new MerchantAuthenticationType();
            authentication.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ApiLogin];
            authentication.transactionKey = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionKey];

            using (var webService = new Service())
            {
                webService.Url = request.PostUrl;
                GetUnsettledTransactionListRequestType listType = new GetUnsettledTransactionListRequestType();
                GetUnsettledTransactionListResponseType response = webService.GetUnsettledTransactionList(authentication, listType);
                
                char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];

                for (int i = 0; i < response.messages.Length; i++)
                {
                    result = response.messages[i].text + del;
                }

                result = result.TrimEnd(del);
                gatewayResponse = new AuthorizeDotNetResponse(result, del);
            }

            return gatewayResponse;
        }

        private IGatewayResponse SendGetTransactionsRequest(GatewayRequest request)
        {
            var result = string.Empty;
            IGatewayResponse gatewayResponse;

            var authentication = new MerchantAuthenticationType();
            authentication.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ApiLogin];
            authentication.transactionKey = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionKey];

            string id = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.BatchId];

            using (var webService = new Service())
            {
                webService.Url = request.PostUrl;
                GetTransactionListRequestType listType = new GetTransactionListRequestType();
                listType.batchId = id;
                GetTransactionListResponseType response = webService.GetTransactionList(authentication, listType);
                char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];

                for (int i = 0; i < response.messages.Length; i++)
                {
                    result = response.messages[i].text + del;
                }

                result = result.TrimEnd(del);
                gatewayResponse = new AuthorizeDotNetResponse(result, del);
            }

            return gatewayResponse;
        }

        private IGatewayResponse SendGetTransactionDetailsRequest(GatewayRequest request) 
        {
            var result = string.Empty;
            IGatewayResponse gatewayResponse;

            var authentication = new MerchantAuthenticationType();
            authentication.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ApiLogin];
            authentication.transactionKey = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionKey];

            string id = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionId];

            using (var webService = new Service())
            {
                webService.Url = request.PostUrl;
                GetTransactionDetailsResponseType response = webService.GetTransactionDetails(authentication, id);
                char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];

                for (int i = 0; i < response.messages.Length; i++)
                {
                    result = response.messages[i].text + del;
                }

                result = result.TrimEnd(del);
                gatewayResponse = new AuthorizeDotNetResponse(result, del);
            }

            return gatewayResponse;
        }

        private IGatewayResponse SendCancelSubscriptionRequest(GatewayRequest request)
        {
            var result = string.Empty;
            IGatewayResponse gatewayResponse;

            var authentication = new MerchantAuthenticationType();
            authentication.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ApiLogin];
            authentication.transactionKey = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionKey];

            string id = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.SubscriptionID];
            
            using (var webService = new Service())
            {
                webService.Url = request.PostUrl;
                ARBCancelSubscriptionResponseType response = webService.ARBCancelSubscription(authentication, long.Parse(id));
                char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];
                IList<string> list = new List<string>();

                for (int i = 0; i < response.messages.Length; i++)
                {
                    result = response.messages[i].text + del;
                    list.Add(response.messages[i].text);
                }

                result = result.TrimEnd(del);
                gatewayResponse = new AuthorizeDotNetResponse(result, del);
                gatewayResponse.SubscriptionResponse = list;
            }

            return gatewayResponse;
        }

        private IGatewayResponse SendCreateSubscriptionRequest(GatewayRequest request)
        {
            var result = string.Empty;
            IGatewayResponse gatewayResponse = null;
            
            //string id = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.SubscriptionID];

            var authentication = new MerchantAuthenticationType();
            authentication.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ApiLogin];
            authentication.transactionKey = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionKey];

            //do required first
            ARBSubscriptionType subscription = new ARBSubscriptionType();
            subscription.amount = decimal.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Amount]);
            subscription.amountSpecified = true;
            subscription.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.SubscriptionName];

            PaymentType payment = new PaymentType();
            var creditCard = new CreditCardType();
            creditCard.cardCode = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CreditCardCode];
            creditCard.cardNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CreditCardNumber];
            creditCard.expirationDate = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CreditCardExpiration];
            payment.Item = creditCard;
            subscription.payment = payment;

            CustomerType customer = new CustomerType();
            customer.id = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CustomerId];
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Fax))
                customer.email = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Email];
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Fax))
                customer.faxNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Fax];           
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Phone))
                customer.phoneNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Phone];
            //customer.type = CustomerTypeEnum.individual;
            customer.typeSpecified = false;
            //customer.taxId = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.t];
            //customer.driversLicense = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.];
            subscription.customer = customer;

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Address))
            {
                NameAndAddressType customerBilling = new NameAndAddressType();
                customerBilling.address = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Address];
                customerBilling.city = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.City];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Company))
                    customerBilling.company = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Company];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Country))
                    customerBilling.country = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Country];
                customerBilling.firstName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.FirstName];
                customerBilling.lastName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.LastName];
                customerBilling.state = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.State];
                customerBilling.zip = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Zip];
                subscription.billTo = customerBilling;
            }

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.ShipAddress))
            {
                NameAndAddressType shipping = new NameAndAddressType();
                shipping.address = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipAddress];
                shipping.city = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipCity];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.ShipCompany))
                    shipping.company = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipCompany];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.ShipCountry))
                    shipping.country = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipCountry];
                shipping.firstName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipFirstName];
                shipping.lastName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipLastName];
                shipping.state = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipState];
                shipping.zip = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipZip];
                subscription.shipTo = shipping;
            }
            
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.InvoiceNumber)) 
            {
                OrderType order = new OrderType();
                order.invoiceNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.InvoiceNumber];
                subscription.order = order;
            }

           
            PaymentScheduleType paymentSchedule = new PaymentScheduleType();
            PaymentScheduleTypeInterval paymentScheduleTypeInterval = new PaymentScheduleTypeInterval();
            paymentScheduleTypeInterval.length = short.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.BillingCycles]);
            paymentScheduleTypeInterval.unit = (ARBSubscriptionUnitEnum)Enum.Parse(typeof(ARBSubscriptionUnitEnum), request.Post[AuthorizeDotNet.AuthorizeDotNetApi.BillingInterval], true);
            paymentSchedule.interval = paymentScheduleTypeInterval;
            paymentSchedule.startDate = DateTime.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.StartsOn].ToString());
            paymentSchedule.startDateSpecified = true;
            paymentSchedule.totalOccurrencesSpecified = true;
            paymentSchedule.totalOccurrences = short.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TotalOccurences].ToString());

            subscription.trialAmountSpecified = false;
            paymentSchedule.trialOccurrencesSpecified = false;

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.TrialAmount))
            {
                subscription.trialAmount = decimal.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TrialAmount]);
                subscription.trialAmountSpecified = true;
                paymentSchedule.trialOccurrences = short.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TrialBillingCycles]);
                paymentSchedule.trialOccurrencesSpecified = true;
            }

            subscription.paymentSchedule = paymentSchedule;

            using (var webService = new Service())
            {
                webService.Url = request.PostUrl;
                var response = webService.ARBCreateSubscription(authentication, subscription);

                if (response.resultCode != MessageTypeEnum.Ok)
                {
                    char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];
                    IList<string> list = new List<string>();

                    for (int i = 0; i < response.messages.Length; i++)
                    {
                        result += response.messages[i].text + del;
                        list.Add(response.messages[i].text);
                    }

                    result = result.TrimEnd(del);
                    gatewayResponse = new AuthorizeDotNetResponse(result, del);
                    gatewayResponse.SubscriptionResponse = list;
                }
                else 
                {
                    IList<string> list = new List<string>();

                    for (int i = 0; i < response.messages.Length; i++)
                        list.Add(response.messages[i].text);
                    
                    gatewayResponse = new AuthorizeDotNetResponse(response.subscriptionId.ToString());
                    gatewayResponse.SubscriptionResponse = list;
                }
            }

            return gatewayResponse;
        }

        private IGatewayResponse SendUpdateSubscriptionRequest(GatewayRequest request)
        {
            var result = string.Empty;
            IGatewayResponse gatewayResponse = null;

            long id = long.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.SubscriptionID]);

            var authentication = new MerchantAuthenticationType();
            authentication.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ApiLogin];
            authentication.transactionKey = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TransactionKey];

            //do required first
            ARBSubscriptionType subscription = new ARBSubscriptionType();
            subscription.amount = decimal.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Amount]);
            subscription.amountSpecified = true;
            subscription.name = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.SubscriptionName];

            PaymentType payment = new PaymentType();
            var creditCard = new CreditCardType();
            creditCard.cardCode = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CreditCardCode];
            creditCard.cardNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CreditCardNumber];
            creditCard.expirationDate = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CreditCardExpiration];
            payment.Item = creditCard;
            subscription.payment = payment;

            CustomerType customer = new CustomerType();
            customer.id = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.CustomerId];
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Fax))
                customer.email = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Email];
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Fax))
                customer.faxNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Fax];
            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Phone))
                customer.phoneNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Phone];
            //customer.type = CustomerTypeEnum.individual;
            customer.typeSpecified = false;
            //customer.taxId = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.t];
            //customer.driversLicense = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.];
            subscription.customer = customer;

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Address))
            {
                NameAndAddressType customerBilling = new NameAndAddressType();
                customerBilling.address = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Address];
                customerBilling.city = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.City];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Company))
                    customerBilling.company = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Company];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.Country))
                    customerBilling.country = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Country];
                customerBilling.firstName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.FirstName];
                customerBilling.lastName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.LastName];
                customerBilling.state = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.State];
                customerBilling.zip = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.Zip];
                subscription.billTo = customerBilling;
            }

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.ShipAddress))
            {
                NameAndAddressType shipping = new NameAndAddressType();
                shipping.address = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipAddress];
                shipping.city = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipCity];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.ShipCompany))
                    shipping.company = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipCompany];
                if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.ShipCountry))
                    shipping.country = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipCountry];
                shipping.firstName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipFirstName];
                shipping.lastName = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipLastName];
                shipping.state = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipState];
                shipping.zip = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.ShipZip];
                subscription.shipTo = shipping;
            }

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.InvoiceNumber))
            {
                OrderType order = new OrderType();
                order.invoiceNumber = request.Post[AuthorizeDotNet.AuthorizeDotNetApi.InvoiceNumber];
                subscription.order = order;
            }


            //PaymentScheduleType paymentSchedule = new PaymentScheduleType();
            //PaymentScheduleTypeInterval paymentScheduleTypeInterval = new PaymentScheduleTypeInterval();
            //paymentScheduleTypeInterval.length = short.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.BillingCycles]);
            //paymentScheduleTypeInterval.unit = (ARBSubscriptionUnitEnum)Enum.Parse(typeof(ARBSubscriptionUnitEnum), request.Post[AuthorizeDotNet.AuthorizeDotNetApi.BillingInterval], true);
            //paymentSchedule.interval = paymentScheduleTypeInterval;
            //paymentSchedule.startDate = DateTime.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.StartsOn].ToString());
            //paymentSchedule.startDateSpecified = true;
            //paymentSchedule.totalOccurrencesSpecified = true;
            //paymentSchedule.totalOccurrences = short.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TotalOccurences].ToString());
            //paymentSchedule.trialOccurrencesSpecified = false;

            subscription.trialAmountSpecified = false;
            

            if (request.Post.ContainsKey(AuthorizeDotNet.AuthorizeDotNetApi.TrialAmount))
            {
                subscription.trialAmount = decimal.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TrialAmount]);
                subscription.trialAmountSpecified = true;
                //paymentSchedule.trialOccurrences = short.Parse(request.Post[AuthorizeDotNet.AuthorizeDotNetApi.TrialBillingCycles]);
                //paymentSchedule.trialOccurrencesSpecified = true;
            }

            //authorize does not allow us to update intervals...
            //subscription.paymentSchedule = paymentSchedule;

            using (var webService = new Service())
            {
                webService.Url = request.PostUrl;
                var response = webService.ARBUpdateSubscription(authentication, id, subscription);

                if (response.resultCode != MessageTypeEnum.Ok)
                {
                    char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];
                    IList<string> list = new List<string>();

                    for (int i = 0; i < response.messages.Length; i++)
                    {
                        result += response.messages[i].text + del;
                        list.Add(response.messages[i].text);
                    }

                    result = result.TrimEnd(del);
                    gatewayResponse = new AuthorizeDotNetResponse(result, del);
                    gatewayResponse.SubscriptionResponse = list;
                }
                else
                {
                    IList<string> list = new List<string>();

                    for (int i = 0; i < response.messages.Length; i++)
                        list.Add(response.messages[i].text);

                    gatewayResponse = new AuthorizeDotNetResponse(id.ToString());
                    gatewayResponse.SubscriptionResponse = list;
                }

                //if (response.resultCode == MessageTypeEnum.Ok)
                //{
                //    char del = request.Post[AuthorizeDotNetApi.DelimitCharacter].ToCharArray()[0];

                //    for (int i = 0; i < response.messages.Length; i++)
                //    {
                //        result = response.messages[i].text + del;
                //    }

                //    result = result.TrimEnd(del);
                //    gatewayResponse = new AuthorizeDotNetResponse(result, del);
                //    gatewayResponse.SubscriptionId = id.ToString();
                //}
            }

            return gatewayResponse;
        }
    }
}
