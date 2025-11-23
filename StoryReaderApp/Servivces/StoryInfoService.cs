using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using StoryReaderApp.Models;

namespace StoryReaderApp.Services
{
    public class StoryInfoService : IStoryInfoService
    {
        private static readonly HttpClient client = new HttpClient();

        static StoryInfoService()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "StoryReaderApp/1.0 (https://example.com)");
        }

        public async Task<StoryInfo> GetInfoAsync(string title)
        {
            try
            {
                string wikiTitle = Uri.EscapeDataString(title.Replace(" ", "_"));
                string url = $"https://en.wikipedia.org/api/rest_v1/page/summary/{wikiTitle}";

                var response = await client.GetStringAsync(url);
                var json = JObject.Parse(response);

                return new StoryInfo
                {
                    Title = (string)json["title"],
                    Author = (string)json["description"],
                    Summary = (string)json["extract"],
                    SourceUrl = (string)json["content_urls"]?["desktop"]?["page"]
                };
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Error fetching online info: " + ex.Message);
                return null;
            }
        }
    }
}
