using System.Collections.Generic;
using Aucsystem.Models;
public class PropertyPageViewModel
{
    public Property ? Property { get; set; }
    public ApplicationUser ? Seller { get; set; }
    public int TotalProperties { get; set; }
    public ICollection<BidViewModel> ? Bids { get; set; }
}