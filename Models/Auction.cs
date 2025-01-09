using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Aucsystem.Models
{
    public class Auction
    {
        public int AuctionId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public List<Property> Properties { get; set; } = new List<Property>();
    }
}
