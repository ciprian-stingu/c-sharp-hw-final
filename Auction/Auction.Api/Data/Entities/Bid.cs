namespace Auction.Api.Data.Entities
{
    public class Bid
    {
        public int Id { get; set; }

        public decimal Value { get; set; }

        public bool Winner { get; set; }

        public Auction Auction { get; set; }
    }
}
