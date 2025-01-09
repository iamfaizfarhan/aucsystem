using System.ComponentModel.DataAnnotations;

public class PlaceBidViewModel
{
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Please enter a bid amount.")]
    [Range(1, double.MaxValue, ErrorMessage = "Bid amount must be greater than zero.")]
    public decimal BidAmount { get; set; }

    public string  ? PropertyName { get; set; }
    public decimal CurrentPrice { get; set; }
}
