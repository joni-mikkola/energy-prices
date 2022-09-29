namespace EnergyPricesDB.Models
{
    public class PriceModel
    {
        public DateTime date { get; set; }
        public string id { get; set; }
        public decimal last { get; set; }

        public PriceModel Clone()
        {
            var p = new PriceModel();
            p.date = date;
            p.id = id;
            p.last = last;
            return p;
        }

    }
}
