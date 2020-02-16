namespace Auction.Api.Extensions.Map
{
    using Data.Entities;
    using Resources.Auctions;

    public static class AuctionExtensions
    {
        public static Auction MapAsNewEntity(this CreateAuctionResource model)
        {
            return new Auction
            {
                Name = model.Name,
                EndDate = model.EndDate
            };
        }

        public static AuctionResource MapAsResource(this Auction model)
        {
            return new AuctionResource
            {
                Name = model.Name,
                EndDate = model.EndDate,
                Id = model.Id
            };
        }

        public static void UpdateWith(this Auction auction, UpdateAuctionResource model)
        {
            auction.EndDate = model.EndDate;
            auction.Name = model.Name;
        }
    }
}
