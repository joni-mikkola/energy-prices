using System.Text;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net;

namespace EnergyPricesBL
{
    public class SlackClient
    {
        private readonly Uri _uri;
        private readonly Encoding _encoding = new UTF8Encoding();

        public SlackClient(string urlWithAccessToken)
        {
            _uri = new Uri(urlWithAccessToken);
        }

        public void PostMessage(string text)
        {
            Payload payload = new Payload()
            {
                Text = text
            };

            PostMessage(payload);
        }

        public void PostError(string text, Exception ex, string username = null, string channel = null, string color = "#D00000")
        {
            Payload payload = new Payload()
            {
                Channel = channel,
                Username = username,
                Text = text,
                Color = color,
                Attachments = new Payload.Attach[] {  new Payload.Attach()
               {
                   Title = "Exception: "+ex.Message,
                   Short = false,
                   Value = ex.ToString()
               } }
            };
            PostMessage(payload);
        }

        public void PostMessage(Payload payload)
        {
            string payloadJson = JsonConvert.SerializeObject(payload);

            using (WebClient client = new WebClient())
            {
                NameValueCollection data = new NameValueCollection();
                data["payload"] = payloadJson;

                var response = client.UploadValues(_uri, "POST", data);
                string responseText = _encoding.GetString(response);
            }
        }
    }

    public class Payload
    {
        [JsonProperty("channel")]
        public string Channel { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("pretext")]
        public string Text { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("fields")]
        public Attach[] Attachments { get; set; }

        public class Attach
        {
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("value")]
            public string Value { get; set; }
            [JsonProperty("short")]
            public bool Short { get; set; }
        }
    }
}
