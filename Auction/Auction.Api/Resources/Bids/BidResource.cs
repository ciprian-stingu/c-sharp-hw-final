namespace Auction.Api.Resources.Bids
{
    public class BidResource
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public bool Winner { get; set; }
    }
}
