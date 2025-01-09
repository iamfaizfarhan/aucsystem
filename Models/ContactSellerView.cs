using Aucsystem.Models;

public class ContactSellerViewModel
{
    public string ? FullName { get; set; }
    public string ? Email { get; set; }
    public string ? PhoneNumber { get; set; }
    public string ? ProfilePicture { get; set; }
    public int TotalProperties { get; set; }
    public bool IsVerifiedSeller => TotalProperties >= 3;
    public List<Property> ? ListedProperties { get; set; }
    public string ? SellerId { get; internal set; }
}