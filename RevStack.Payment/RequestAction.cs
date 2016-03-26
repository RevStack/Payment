
namespace RevStack.Payment
{
    public enum RequestAction
    {
        Authorize,
        Settle,
        Sale,
        Refund,
        Void,
        GetTransactions,
        GetTransactionDetails,
        CreateSubscription,
        UpdateSubscription,
        CancelSubscription,
        GetSubscriptionStatus
    }
}