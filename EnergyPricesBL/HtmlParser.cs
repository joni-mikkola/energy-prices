using System.Text.RegularExpressions;

namespace EnergyPricesBL
{
    public class HtmlParser
    {
        public static List<List<string>> ParseFile(string filename, bool history)
        {
            return ParseString(System.IO.File.ReadAllText(filename), history);
        }

        public static List<List<string>> ParseString(string content, bool history)
        {
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(content);
            string nameId = "insnm";
            string lastId = "sp";
            string bidId = "bp";
            string askId = "ap";
            if(!history)
            {
                nameId = "nm";
                lastId = "stlpr";
            }
            return doc.DocumentNode.SelectSingleNode("//table")
                        .Descendants("tbody").ToList()
                        .Select(tr => tr.Elements("tr").Select(td => $"{td.SelectSingleNode($"td[@name='{nameId}']").InnerText}|{td.SelectSingleNode($"td[@name='{bidId}']").InnerText}|{td.SelectSingleNode($"td[@name='{askId}']").InnerText}|{td.SelectSingleNode($"td[@name='{lastId}']").InnerText}").ToList())
                        .ToList();
        }
    }
}
