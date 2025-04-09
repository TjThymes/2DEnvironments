using Xunit;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ObjectEnvironmentPlacer.Tests.SystemTests
{
    public class ObjectSystemTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ObjectSystemTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetAllObjects_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/objects");

            response.EnsureSuccessStatusCode();
        }
    }
}
