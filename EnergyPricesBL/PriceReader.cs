using EnergyPricesDB.Models;
using System.Globalization;

namespace EnergyPricesBL
{
    public class PriceReader
    {
        static decimal ParseDecimal(string val)
        {
            return decimal.Parse(val, CultureInfo.InvariantCulture);
        }

        static PriceModel ParsePrice(string[] items)
        {
            var price = new PriceModel();
            price.id = items[0].Trim();

            try
            {
                price.last = ParseDecimal(items[3]) / 1000.0m;
            } catch(Exception exception)
            {
                price.last = (ParseDecimal(items[1]) + (ParseDecimal(items[2]) - ParseDecimal(items[1])) / 2.0m) / 1000.0m;
            }

            return price;
        }

        public static List<PriceModel> ToMonthlyPrices(List<List<string>> table)
        {
            string result = string.Empty;
            var quarterlyPrices = new List<PriceModel>();
            var monthlyPrices = new List<PriceModel>();

            foreach (var item in table[0])
            {
                var items = item.Split("|");
                var id = items[0].Trim();

                if (id.StartsWith("ENOAFUTBLM"))
                {
                    var price = ParsePrice(items);
                    var leftover = price.id.Replace("ENOAFUTBLM", "");
                    var month = leftover.Substring(0, 3);
                    var year = leftover.Substring(leftover.Length - 2, 2);
                    DateTime converted;
                    DateTime.TryParseExact("01 " + month + " " + year, "dd MMM yy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out converted);
                    price.date = converted;
                    monthlyPrices.Add(price);
                }
                else if (id.StartsWith("ENOFUTBLQ"))
                {
                    var price = ParsePrice(items);
                    var leftover = price.id.Replace("ENOFUTBLQ", "");
                    var year = leftover.Substring(leftover.Length - 2, 2);

                    for (var i = 0; i < 3; ++i)
                    {
                        int q = int.Parse(leftover[0].ToString());
                        var month = ((q - 1) * 3 + 1 + i).ToString("00");
                        DateTime converted;
                        DateTime.TryParseExact("01 " + month + " " + year, "dd MM yy", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out converted);
                        var clonedPrice = price.Clone();
                        clonedPrice.date = converted;
                        quarterlyPrices.Add(clonedPrice);
                    }
                }
            }

            monthlyPrices = monthlyPrices.OrderBy(x => x.date).ToList();
            quarterlyPrices = quarterlyPrices.OrderBy(x => x.date).ToList();


            foreach (var m in quarterlyPrices)
            {
                if (m.date > monthlyPrices.Last().date && m.date < monthlyPrices[0].date.AddYears(1))
                {
                    monthlyPrices.Add(m);
                }
            }

            return monthlyPrices;
        }
    }
}
