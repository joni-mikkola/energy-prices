using EnergyPricesBL;
using EnergyPricesDB;
using EnergyPricesDB.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace EnergyPricesFunctions
{
    public class Functions
    {
        private readonly KeyvaultAccessor _accessor;
        public Functions(KeyvaultAccessor accessor)
        {
            _accessor = accessor;
        }

        [FunctionName("DownloadFunction")]
        public async Task Run([TimerTrigger("0 30 9 * * *", RunOnStartup = true)] TimerInfo myTimer, ILogger log)
        {
            var fromDate = DateTime.UtcNow.AddDays(-1);
            var toDate = DateTime.UtcNow.AddDays(-1);

            var connectionString = await _accessor.GetSecret("database");
            var dbOptions = new DbContextOptionsBuilder<EnergyPricesDBContext>().UseSqlServer(connectionString).Options;

            using (var ctx = new EnergyPricesDBContext(dbOptions))
            {
                while (fromDate < toDate)
                {
                    if (fromDate.DayOfWeek != DayOfWeek.Saturday && fromDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        const int MAX_RETRIES = 3;
                        int retries = 0;
                        bool success = false;

                        var monthlyPrices = new List<PriceModel>();

                        while (retries < MAX_RETRIES && !success)
                        {
                            var content = await NasdaqDownloader.Download(fromDate);
                            Thread.Sleep(1000);
                            var table = HtmlParser.ParseString(content, history: true);

                            monthlyPrices = PriceReader.ToMonthlyPrices(table);
                            success = monthlyPrices.Count > 0;
                            retries++;
                        }

                        if (monthlyPrices.Count > 0)
                        {
                            var match = ctx.NordicElectricityPrices.SingleOrDefault(x => x.Date.Date == fromDate.Date);
                            if (match != null)
                            {
                                match.Updated = DateTime.UtcNow;
                                match.Content = JsonConvert.SerializeObject(monthlyPrices);
                            }
                            else
                            {
                                ctx.NordicElectricityPrices.Add(new ProductModel
                                {
                                    Date = fromDate,
                                    Updated = DateTime.UtcNow,
                                    Content = JsonConvert.SerializeObject(monthlyPrices)
                                });
                            }

                            ctx.SaveChanges();
                        }
                    }

                    fromDate = fromDate.AddDays(1);
                }
            }
        }
    }
}
