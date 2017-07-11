

namespace RevStack.Payment.Model
{
    public interface IShipping : RevStack.Pattern.IEntity<string>
    {
        string FirstName { get; set; }
        string LastName { get; set; }
        string Address { get; set; }
        string City { get; set; }
        string StateOrProvince { get; set; }
        string Zipcode { get; set; }
        string Country { get; set; }
        string Phone { get; set; }
        string Email { get; set; }
    }
}
