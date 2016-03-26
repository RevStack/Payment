﻿using System.Collections.Generic;

namespace RevStack.Payment
{
    public interface IGatewayResponse
    {
        bool Approved { get; }
        decimal Amount { get; }
        string TransactionId { get; }
        string AuthorizationCode { get; }
        string ResponseCode { get; }
        string Message { get; }
        string FullResponse { get; }
        string AvsCode { get; }
        string AvsResponse { get; }
        string CcvCode { get; }
        string CcvResponse { get; }
        string SubscriptionId { get; set; }
        IList<string> SubscriptionResponse { get; set; }
    }
}