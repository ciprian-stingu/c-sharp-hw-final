using System;

namespace Auction.Api.Resources.Auctions
{
    public class CreateAuctionResource
    {
        public string Name { get; set; }

        public DateTime EndDate { get; set; }
    }
}
