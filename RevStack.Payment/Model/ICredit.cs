

namespace RevStack.Payment.Model
{
    public interface ICredit : RevStack.Pattern.IEntity<string>
    {
        decimal Amount { get; set; }
        //string CardNumber { get; set; }
    }
}
