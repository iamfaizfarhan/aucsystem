using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aucsystem.Models {
    public class Property
    {
        public int PropertyId { get; set; }

        [Required]
        public string ? Name { get; set; }

        [Required]
        public string ? Description { get; set; }

        [Required]
        public string ? Location { get; set; }

        [Required]
        public decimal Price { get; set; } 

        public string ? ImageUrl1 { get; set; } 
        public string ? ImageUrl2 { get; set; }
        public string ? ImageUrl3 { get; set; }

        public bool IsAvailable { get; set; } 
        public DateTime ListedOn { get; set; } = DateTime.Now;

        public string ? SellerId { get; set; }
        public ApplicationUser ? Seller { get; set; } 

        public ICollection<Bid> ? Bids { get; set; }
        public bool IsSold { get; set; } 
        public int? AuctionId { get; set; }
        public Auction? Auction { get; set; } 

    }

}
