

namespace RevStack.Payment.Model
{
    public interface IVoid : RevStack.Pattern.IEntity<string>
    {
        decimal Amount { get; set; }
        //string TransactionId { get; set; }
    }
}
