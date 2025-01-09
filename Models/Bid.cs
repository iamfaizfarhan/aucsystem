using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aucsystem.Models;

public class Bid
{
        [Key]
        public int BidId { get; set; }

        [Required]
        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public Property ? Property { get; set; }

        [Required]
        [ForeignKey("ApplicationUser")]
        public string ? BuyerId { get; set; }
        public ApplicationUser ? Buyer { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime BidTime { get; set; }

}