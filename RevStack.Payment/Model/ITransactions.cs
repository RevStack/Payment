
namespace RevStack.Payment.Model
{
    public interface ITransactions : RevStack.Pattern.IEntity<string>
    {
        string BatchId { get; set; }
    }
}
