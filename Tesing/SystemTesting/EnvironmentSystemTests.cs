using Xunit;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using FluentAssertions;
using System.Net.Http.Headers;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace ObjectEnvironmentPlacer.Tests.SystemTests
{
    public class EnvironmentSystemTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public EnvironmentSystemTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            // Assume token will be handled here manually if needed (optional)
        }

        [Fact]
        public async Task GetAllEnvironments_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/environments2d");

            response.EnsureSuccessStatusCode();
        }
    }
}
