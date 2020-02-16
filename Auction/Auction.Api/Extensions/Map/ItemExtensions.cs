using Auction.Api.Data.Entities;
using Auction.Api.Resources.Items;
using Auction.Api.Resources.Auctions;

namespace Auction.Api.Extensions.Map
{
    public static class ItemExtensions
    {
        public static Item MapAsNewEntity(this CreateItemResource model, Auction.Api.Data.Entities.Auction auction)
        {
            return new Item
            {
                Name = model.Name,
                Auction = auction,
                MinPrice = model.MinPrice
            };
        }

        public static ItemResource MapAsResource(this Item model)
        {
            return new ItemResource
            {
                Name = model.Name,
                Id = model.Id,
                MinPrice = model.MinPrice
            };
        }

        public static void UpdateWith(this Item item, UpdateItemResource model)
        {
            item.Name = model.Name;
            item.MinPrice = model.MinPrice;
        }

    }
}
