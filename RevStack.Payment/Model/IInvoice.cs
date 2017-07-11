

namespace RevStack.Payment.Model
{
    public interface IInvoice : RevStack.Pattern.IEntity<string>
    {
        string InvoiceNumber { get; set; }
    }
}
