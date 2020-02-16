using System.ComponentModel.DataAnnotations;

namespace Auction.Api.Data.Entities
{
    using System;
    using System.Collections.Generic;

    public class Auction
    {
        public int Id { get; set; }

        //[Required]
        public string Name { get; set; }

        public DateTime EndDate { get; set; }

        public List<Item> Items { get; set; }
        public List<Bid> Bids { get; set; }
    }
}
