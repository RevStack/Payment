

namespace RevStack.Payment.Model
{
    public interface ICreditCard : RevStack.Pattern.IEntity<string>
    {
        string CardNumber { get; set; }
        string ExpirationMonth { get; set; }
        string ExpirationYear { get; set; }
        string CVV { get; set; }
    }
}
