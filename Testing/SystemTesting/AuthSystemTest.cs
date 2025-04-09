using Xunit;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using FluentAssertions;
using ObjectEnvironmentPlacer; // ✅ Correct namespace import for your real Program.cs!

namespace ObjectEnvironmentPlacer.Tests.SystemTests
{
    public class AuthSystemTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public AuthSystemTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_ShouldReturnSuccess()
        {
            var registerPayload = new
            {
                Name = "SystemTestUser2",
                Email = "systemtestuser2@example.com",
                Password = "Password123!"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerPayload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            response.EnsureSuccessStatusCode(); // 2xx success
        }

        [Fact]
        public async Task Login_ShouldReturnJwtToken()
        {
            var loginPayload = new
            {
                UserName = "SystemTestUser",
                Password = "Password123!"
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginPayload), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/login", content);

            response.EnsureSuccessStatusCode(); // 2xx success

            var responseBody = await response.Content.ReadAsStringAsync();
            responseBody.Should().Contain("accessToken"); // ✅ check JWT token present
        }
    }
}
