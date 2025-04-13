using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ObjectEnvironmentPlacer.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HealthCheckController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("Check-SqlConnectionString")]
        public async Task<IActionResult> CheckSqlConnection()
        {
            string connectionString = _configuration.GetConnectionString("SqlConnectionString");

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    return Ok("✅ SQL connection successful.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ SQL connection failed: {ex.Message}");
            }
        }
    }
}
