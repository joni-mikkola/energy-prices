using EnergyPricesDB;
using EnergyPricesDB.Models;
using EnergyPricesWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml;

namespace EnergyPricesWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly EnergyPricesDBContext _context;
        public HomeController(IConfiguration configuration, IWebHostEnvironment hostEnvironment, EnergyPricesDBContext context)
        {
            _configuration = configuration;
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            var latest = _context.NordicElectricityPrices.OrderBy(x => x.Date).Last();

            return View(new DataModel
            {
                utcDate = latest.Date
            });
        }

        [Route("/sitemap.xml")]
        public void SitemapXml()
        {
            string host = Request.Scheme + "://" + Request.Host;

            Response.ContentType = "application/xml";

            using (var xml = XmlWriter.Create(Response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                var lastModDate = DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd");
                string url = "https://energy-prices.net/";
                string[] languages = { "de-DE", "fi-FI", "en-US" };
                foreach (var lang in languages)
                {
                    xml.WriteStartElement("url");
                    xml.WriteElementString("loc", url + lang);
                    xml.WriteElementString("lastmod", lastModDate);
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
        }
    }
}
