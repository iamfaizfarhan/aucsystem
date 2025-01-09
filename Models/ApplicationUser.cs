using Aucsystem.Models;
using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public string ? FullName { get; set; }
    public string ? UserType { get; set; }
    public string ? ProfilePicture { get; set; }
    
    public int TotalProperties { get; set; } 
    public ICollection<Property> ? Properties { get; set; } 
    public ICollection<Bid> ? Bids { get; set; } 
}
