using Auction.Api.Data.Entities;
using Auction.Api.Resources.Bids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Api.Extensions.Map
{
    public static class BidExtensions
    {
        public static Bid MapAsNewEntity(this CreateBidResource model, Auction.Api.Data.Entities.Auction auction)
        {
            return new Bid
            {
                Value = model.Value,
                Auction = auction,
                Winner = model.Winner
            };
        }

        public static BidResource MapAsResource(this Bid model)
        {
            return new BidResource
            {
                Value = model.Value,
                Id = model.Id,
                Winner = model.Winner
            };
        }

        public static void UpdateWith(this Bid bid, UpdateBidResource model)
        {
            bid.Winner = model.Winner;
            bid.Value = model.Value;
        }
    }
}
