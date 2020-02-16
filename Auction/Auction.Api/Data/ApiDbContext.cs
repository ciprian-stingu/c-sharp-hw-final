namespace Auction.Api.Data
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auction>()
                .HasMany(c => c.Items)
                .WithOne(e => e.Auction);

            modelBuilder.Entity<Auction>()
                .HasMany(c => c.Bids)
                .WithOne(e => e.Auction);
        }
    }
}
