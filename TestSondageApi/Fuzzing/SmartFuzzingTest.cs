using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TestSondageApi.Fuzzing
{
    public class SmartFuzzingTest
    {
        private HttpClient _httpClient;
        public SmartFuzzingTest()
        {
            var application = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
            });

            _httpClient = application.CreateClient();
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "ahhchfuzaeafusggnffhzvlmfarjjhjaknzfkajskxfuozgahzehafkckgmumnfj");

        }

        [Fact]
        public async Task GET_test()
        {
            var response = await _httpClient.GetAsync("/Survey");
            Assert.True(response.StatusCode == HttpStatusCode.OK);
        }

    }
}
