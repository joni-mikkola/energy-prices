using System.ComponentModel.DataAnnotations;

namespace EnergyPricesDB.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime Updated { get; set; }
        public string? Content { get; set; }
    }
}
