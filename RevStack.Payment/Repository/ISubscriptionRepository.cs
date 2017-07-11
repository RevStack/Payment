using RevStack.Payment.Model;

namespace RevStack.Payment.Repository
{
    public interface ISubscriptionRepository
    {
        T Subscribe<T>(ISubscribe subscribe) where T : ISubscription;
        T Update<T>(IUpdateSubscription subscription) where T : ISubscription;
        T Cancel<T>(ICancelSubscription subscription) where T : ISubscription;
    }
}
