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
            // 1. Login to get a token
            var loginPayload = new
            {
                UserName = "admin",
                Password = "Admin1*"
            };

            var loginContent = new StringContent(JsonConvert.SerializeObject(loginPayload), Encoding.UTF8, "application/json");
            var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);

            loginResponse.EnsureSuccessStatusCode();

            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();
            dynamic loginResult = JsonConvert.DeserializeObject(loginResponseBody);
            string token = loginResult.accessToken;

            // 2. Attach the token
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // 3. Now call the protected endpoint
            var response = await _client.GetAsync("/api/environments2d/myenvironmentids");

            response.EnsureSuccessStatusCode(); // ✅ should not 401 now
        }
    }
}
