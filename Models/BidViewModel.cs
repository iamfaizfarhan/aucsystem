public class BidViewModel
{
    public int BidId { get; set; } 
    public int AuctionId { get; set; }
    public string? BuyerName { get; set; }
    public string? BuyerEmail { get; set; } 
    public string? PropertyName { get; set; }

    public int? PropertyId { get; set; }
    public decimal PropertyPrice { get; set; }
    public decimal Amount { get; set; } 
    public DateTime BidTime { get; set; }
    public bool IsSold { get; set; }
    public string? BuyerId { get;  set; }
}
