namespace Auction.Api.Resources.Bids
{
    public class CreateBidResource
    {
        public decimal Value { get; set; }

        public bool Winner { get; set; }
    }
}
