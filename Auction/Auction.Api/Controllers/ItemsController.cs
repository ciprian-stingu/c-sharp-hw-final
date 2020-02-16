using Auction.Api.Data;
using Auction.Api.Extensions.Map;
using Auction.Api.Resources.Items;
using Auction.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Auction.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.EntityFrameworkCore.Internal;
    using Microsoft.Extensions.Caching.Memory;

    [Route("api/auctions/{auctionId}/item")]
    [ApiController]
    [Authorize]
    public class ItemsController : ControllerBase
    {
        private readonly ApiDbContext context;
        private readonly ISimpleLogger dumbLogger;
        private IMemoryCache cache;

        public ItemsController(ApiDbContext context, ISimpleLogger dumbLogger, IMemoryCache cache)
        {
            this.context = context;
            this.dumbLogger = dumbLogger;
            this.cache = cache;
        }

        [HttpGet("")]
        public async Task<IEnumerable<ItemResource>> Get(int auctionId, CancellationToken cancellationToken)
        {
            this.dumbLogger.LogInfo($"[GET] ItemsController({auctionId}, all)");
            var cacheKey = "AllItemsAuction" + auctionId;

            if (this.cache.TryGetValue(cacheKey, out  IEnumerable <ItemResource> items))
            {
                this.dumbLogger.LogInfo($"[GET] ItemsController({auctionId}, all) return from cache...");
                return items;
            }
            var list = await this.context.Items
                .Include(h => h.Auction)
                .Where(h => h.Auction.Id == auctionId)
                .ToListAsync(cancellationToken);

            items = list.Select(r => r.MapAsResource());
            var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(30));
            this.cache.Set(cacheKey, items, cacheOptions);
            this.dumbLogger.LogInfo($"[GET] ItemsController({auctionId}, all) save to cache...");
            return items;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ItemResource>> Get(int auctionId, int id, CancellationToken cancellationToken)
        {
            this.dumbLogger.LogInfo($"[GET] ItemsController({auctionId}, {id})");

            if (auctionId <= 0 || id <= 0)
            {
                throw new ArgumentException("Negative id exception");
            }

            var entity = await this.context.Auctions.FindAsync(auctionId);
            if (entity == null)
            {
                this.dumbLogger.LogInfo($"Auction {auctionId} not found.");
                return new BadRequestObjectResult(new Exception($"Auction {auctionId} not found."));//NotFound();
            }

            /*if (entity.Items == null)
            {
                this.dumbLogger.LogInfo($"No items found for auction {auctionId}.");
                return this.NotFound();
            }

            var item = entity.Items.Find(i => i.Id == id);
            if (item == null)
            {
                this.dumbLogger.LogInfo($"Item {id} not found for auction {auctionId}.");
                return this.NotFound();
            }*/

            var item = await this.context.Items.FindAsync(id);
            if (item == null)
            {
                this.dumbLogger.LogInfo($"Item {id} not found.");
                return this.NotFound();
            }

            if (item.Auction == null || item.Auction.Id != auctionId)
            {
                this.dumbLogger.LogInfo($"Item {id} not found for auction {auctionId}.");
                return this.NotFound();
            }


            return item.MapAsResource();
        }

        [HttpPost]
        public async Task<ActionResult<ItemResource>> Post(int auctionId, CreateItemResource model, CancellationToken cancellationToken)
        {
            var auction = await this.context.Auctions.FindAsync(auctionId);

            if (auction == null)
            {
                this.dumbLogger.LogInfo($"Auction {auctionId} not found.");
                return new BadRequestObjectResult(new Exception($"Auction {auctionId} not found.")); //this.NotFound();
            }

            this.cache.Remove("AllItemsAuction" + auctionId);

            var entity = model.MapAsNewEntity(auction);
            this.context.Items.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return this.CreatedAtAction("Get", new { auctionId, id = entity.Id }, entity.MapAsResource());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int auctionId, int id, UpdateItemResource model, CancellationToken cancellationToken)
        {
            var auction = await this.context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                this.dumbLogger.LogInfo($"Auction {auctionId} not found.");
                return new BadRequestObjectResult(new Exception($"Auction {auctionId} not found.")); //return this.NotFound();
            }

            var item = await this.context.Items.FindAsync(id);

            if (item == null)
            {
                this.dumbLogger.LogInfo($"Item {id} not found.");
                return this.NotFound();
            }

            if (item.Auction == null || item.Auction.Id != auctionId)
            {
                this.dumbLogger.LogInfo($"Item {id} not found for auction {auctionId}.");
                return this.NotFound();
            }

            this.cache.Remove("AllItemsAuction" + auctionId);

            item.UpdateWith(model);
            this.context.Items.Update(item);
            await this.context.SaveChangesAsync(cancellationToken);

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ItemResource>> Delete(int auctionId, int id, CancellationToken cancellationToken)
        {
            var auction = await this.context.Auctions.FindAsync(auctionId);
            if (auction == null)
            {
                this.dumbLogger.LogInfo($"Auction {auctionId} not found.");
                return new BadRequestObjectResult(new Exception($"Auction {auctionId} not found.")); //return this.NotFound();
            }

            var item = await this.context.Items.FindAsync(id);

            if (item == null)
            {
                this.dumbLogger.LogInfo($"Item {id} not found.");
                return this.NotFound();
            }

            if (item.Auction == null || item.Auction.Id != auctionId)
            {
                this.dumbLogger.LogInfo($"Item {id} not found for auction {auctionId}.");
                return this.NotFound();
            }

            this.cache.Remove("AllItemsAuction" + auctionId);

            this.context.Items.Remove(item);
            await this.context.SaveChangesAsync(cancellationToken);

            return item.MapAsResource();
        }

    }
}
