namespace EnergyPricesBL
{
    public class NasdaqDownloader
    {
        public NasdaqDownloader()
        {

        }
        public static async Task<string> Download(DateTime date)
        {
            var nasdaqUrl = "http://www.nasdaqomx.com/webproxy/DataFeedProxyIRC1.aspx";
            HttpClient client = new HttpClient();
            var dict = new Dictionary<string, string>();
            dict.Add("xmlquery", $"<post>\r\n<param name=\"Exchange\" value=\"NMF\"/>\r\n<param name=\"SubSystem\" value=\"History\"/>\r\n<param name=\"Action\" value=\"GetMarket\"/>\r\n<param name=\"Market\" value=\"GITS:NC:ENO\"/>\r\n<param name=\"inst__an\" value=\"id,nm,upc,tp,ed,fnm\"/>\r\n<param name=\"inst__e\" value=\"9\"/>\r\n<param name=\"hi__a\" value=\"31,5,6,29,1,2,8,60,19,57,58,9,30,20\"/>\r\n<param name=\"fromDate\" value=\"{date.ToString("yyyy-MM-dd")}\"/>\r\n<param name=\"toDate\" value=\"{date.ToString("yyyy-MM-dd")}\"/>\r\n<param name=\"empdata\" value=\"false\"/>\r\n<param name=\"ext_xslt\" value=\"nordpool-v2/inst_hi_tables_1_new.xsl\"/>\r\n<param name=\"XPath\" value=\"//inst[ph/hi/@rv!='' or ph/hi/@tv!='']\"/>\r\n<param name=\"ext_xslt_options\" value=\",excel,\"/>\r\n<param name=\"ext_xslt_notlabel\" value=\"fnm\"/>\r\n<param name=\"ext_xslt_lang\" value=\"en\"/>\r\n<param name=\"ext_xslt_tableId\" value=\"historyTable\"/>\r\n<param name=\"ext_xslt_tableClass\" value=\"tablesorter\"/>\r\n<param name=\"ext_xslt_market\" value=\"GITS:NC:ENO\"/>\r\n<param name=\"ext_contenttype\" value=\"application/ms-excel\"/>\r\n<param name=\"ext_contenttypefilename\" value=\"history_{date.ToString("yyyy-MM-dd")}.xls\"/>\r\n<param name=\"app\" value=\"www.nasdaqomx.com//commodities/market-prices/history\"/>\r\n</post>");

            using (HttpContent formContent = new FormUrlEncodedContent(dict))
            {
                using (HttpResponseMessage response = await client.PostAsync(nasdaqUrl, formContent).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }
        }
    }
}
