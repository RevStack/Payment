

namespace RevStack.Payment.Model
{
    public interface ICharge : RevStack.Pattern.IEntity<string>
    {
        ICustomer Customer { get; set; }
        IShipping Shipping { get; set; }
        ICreditCard CreditCard { get; set; }
        decimal Amount { get; set; }
    }
}
