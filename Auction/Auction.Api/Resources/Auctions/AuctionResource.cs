using System;

namespace Auction.Api.Resources.Auctions
{
    public class AuctionResource
    {
        public int Id { get; set; }

        //[Required]
        public string Name { get; set; }

        public DateTime EndDate { get; set; }
    }
}
