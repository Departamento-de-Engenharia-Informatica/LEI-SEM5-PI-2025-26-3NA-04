using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace APDL.Tests.E2E
{
    public class HealthCheckE2ETests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthCheckE2ETests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthCheck_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/health");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("healthy", content);
        }
    }
}

