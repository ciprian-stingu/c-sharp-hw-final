using System;

namespace Auction.Api.Resources.Auctions
{
    public class UpdateAuctionResource
    {
        public string Name { get; set; }
        public DateTime EndDate { get; set; }
    }
}
