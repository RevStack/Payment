

namespace RevStack.Payment.Model
{
    public interface ICapture : RevStack.Pattern.IEntity<string>
    {
        //string TransactionId { get; set; }
        decimal Amount { get; set; }
    }
}
