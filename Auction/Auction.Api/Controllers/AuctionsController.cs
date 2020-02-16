namespace Auction.Api.Controllers
{
    using System;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Resources.Auctions;
    using Services;
    using System.Linq;
    using Auction.Api.Extensions.Map;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.Extensions.Caching.Distributed;
    using Newtonsoft.Json;
    using System.Threading;

    [Route("api/auctions")]
    [ApiController]
    [Authorize]
    public class AuctionsController : ControllerBase
    {
        private readonly ApiDbContext context;
        private readonly ISimpleLogger dumbLogger;
        private IDistributedCache cache;

        public AuctionsController(ApiDbContext context, ISimpleLogger dumbLogger, IDistributedCache cache)
        {
            this.context = context;
            this.dumbLogger = dumbLogger;
            this.cache = cache;
        }

        [HttpGet]
        public async Task<IEnumerable<AuctionResource>> GetHotels(CancellationToken cancellationToken)
        {
            this.dumbLogger.LogInfo($"[GET] AuctionController(all)");
            var auctions = await this.context.Auctions.ToListAsync(cancellationToken);
            var cacheKey = "AllHotels";
            var cachedHotes = await this.cache.GetStringAsync(cacheKey, cancellationToken);
            if(!string.IsNullOrEmpty(cachedHotes))
            {
                this.dumbLogger.LogInfo($"[GET] AuctionController(all) return from cache...");
                return JsonConvert.DeserializeObject<IEnumerable<AuctionResource>>(cachedHotes);
            }
            var result = auctions.Select(r => r.MapAsResource());
            await this.cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(result), cancellationToken);
            this.dumbLogger.LogInfo($"[GET] AuctionController(all) save to cache...");
            return result;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionResource>> Get(int id, CancellationToken cancellationToken)
        {
            this.dumbLogger.LogInfo($"[GET] AuctionController({id})");
            if (id <= 0)
            {
                throw new ArgumentException("Negative id exception");
            }

            var entity = await this.context.Auctions.FindAsync(id);
            if (entity == null)
            {
                return this.NotFound();
            }

            return entity.MapAsResource();
        }

        [HttpPost]
        public async Task<ActionResult<AuctionResource>> Post(CreateAuctionResource model, CancellationToken cancellationToken)
        {
            await this.cache.RemoveAsync("AllHotels", cancellationToken);

            var entity = model.MapAsNewEntity();
            this.context.Auctions.Add(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return this.CreatedAtAction("Get", new {id = entity.Id }, entity.MapAsResource());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AuctionResource>> Delete(int id, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Negative id exception");
            }

            await this.cache.RemoveAsync("AllHotels", cancellationToken);

            var entity = await this.context.Auctions.FindAsync(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            this.context.Auctions.Remove(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return entity.MapAsResource();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, UpdateAuctionResource model, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Negative id exception");
            }

            await this.cache.RemoveAsync("AllHotels", cancellationToken);

            var entity = await this.context.Auctions.FindAsync(id);

            if (entity == null)
            {
                return this.NotFound();
            }

            entity.UpdateWith(model);
            this.context.Auctions.Update(entity);
            await this.context.SaveChangesAsync(cancellationToken);

            return this.NoContent();
        }
    }
}
