using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Auction.Api.Extensions.Map;
using Auction.Api.Resources.Bids;
using Auction.Api.Data;
using Auction.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;

namespace Auction.Api.Controllers
{
    [Route("api/auctions/{auctionId}/bid")]
    [ApiController]
    [Authorize]
    public class BidsController : ControllerBase
    {
        private readonly ApiDbContext context;
        private readonly ISimpleLogger dumbLogger;
        private IMemoryCache cache;

        public BidsController(ApiDbContext context, ISimpleLogger dumbLogger, IMemoryCache cache)
        {
            this.context = context;
            this.dumbLogger = dumbLogger;
            this.cache = cache;
        }

        [HttpGet("")]
        public async Task<IEnumerable<BidResource>> Get(int auctionId, CancellationToken cancellationToken)
        {
            this.dumbLogger.LogInfo($"[GET] BidsController({auctionId}, all)");
            var cacheKey = "AllBidsAuction" + auctionId;

            if (this.cache.TryGetValue(cacheKey, out IEnumerable<BidResource> bids))
            {
                this.dumbLogger.LogInfo($"[GET] BidsController({auctionId}, all) return from cache...");
                return bids;
            }
            var list = await this.context.Bids
                .Include(h => h.Auction)
                .Where(h => h.Auction.Id == auctionId)
                .ToListAsync(cancellationToken);

            bids = list.Select(r => r.MapAsResource());
            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
            this.cache.Set(cacheKey, bids, cacheOptions);
            this.dumbLogger.LogInfo($"[GET] BidsController({auctionId}, all) save to cache...");
            return bids;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BidResource>> Get(int auctionId, int id, CancellationToken cancellationToken)
        {
            this.dumbLogger.LogInfo($"[GET] BidsController({auctionId}, {id})");

            if (auctionId <= 0 || id <= 0)
            {
                throw new ArgumentException("Negative id exception");
            }

            var entity = await this.context.Auctions.FindAsync(auctionId);
            if (entity == null)
            {
                this.dumbLogger.LogInfo($"Auction {auctionId} not found.");
                return new BadRequestObjectResult(new Exception($"Auction {auctionId} not found."));
            }

            var bid = await this.context.Bids.FindAsync(id);
            if (bid == null)
            {
                this.dumbLogger.LogInfo($"Bid {id} not found.");
                return this.NotFound();
            }

            if (bid.Auction == null || bid.Auction.Id != auctionId)
            {
                this.dumbLogger.LogInfo($"Bid {id} not found for auction {auctionId}.");
                return this.NotFound();
            }


            return bid.MapAsResource();
        }

        [HttpPost]
        public async Task<ActionResult<BidResource>> Post(int auctionId, CreateBidResource model, CancellationToken cancellationToken)
        {
            var auction = await this.context.Auctions.FindAsync(auctionId);

            if (auction == null)
            {
                this.dumbLogger.LogInfo($"Auction {auctionId} not found.");
                return new BadRequestObjectResult(new Exception($"Auction {auctionId} not found."));
            }

            this.cache.Remove("AllBidsAuction" + auctionId);

            var entity = model.MapAsNewEntity(auction);
            this.context.Bids.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return this.CreatedAtAction("Get", new { auctionId, id = entity.Id }, entity.MapAsResource());
        }

        //no delete / update because all bids are final!
    }
}