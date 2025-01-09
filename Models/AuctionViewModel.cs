public class AuctionViewModel
{
    public int AuctionId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<PropertyWithCurrentBidViewModel> Properties { get; set; } = new List<PropertyWithCurrentBidViewModel>();

}