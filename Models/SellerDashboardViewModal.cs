using Aucsystem.Models;

public class SellerDashboardViewModel
{
    public List<Property> Properties { get; set; } = new List<Property>();
    public List<Auction> Auctions { get; set; } = new List<Auction>();
    public List<BidViewModel> Bids { get; set; } = new List<BidViewModel>();
}
