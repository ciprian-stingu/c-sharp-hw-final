using System.ComponentModel.DataAnnotations;

namespace Auction.Api.Data.Entities
{
    public class Item
    {
        public int Id { get; set; }

        //[Required]
        public string Name { get; set; }

        public decimal MinPrice { get; set; }

        public Auction Auction { get; set; }
    }
}
