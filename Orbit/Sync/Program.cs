using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Sync
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var creds = Encoding.UTF8.GetBytes(
                Environment.GetEnvironmentVariable("PC_APPLICATION_ID") + ":" +
                Environment.GetEnvironmentVariable("PC_SECRET"));

            var peopleClient = new HttpClient()
            {
                BaseAddress = new Uri("https://api.planningcenteronline.com/people/v2"),
            };
            peopleClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(creds));

            var people = await peopleClient.GetAsync("");

            var body = await people.Content.ReadAsStringAsync();
        }
    }
}
