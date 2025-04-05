using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectEnvironmentPlacer.Objects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Interface;

namespace ObjectEnvironmentPlacer.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/environments2d")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IEnvironment2DRepository _environmentRepository;
        private readonly IPlayerEnvironmentRepository _playerEnvironmentRepository;
        private readonly ILogger<EnvironmentController> _logger;

        public EnvironmentController(
            IEnvironment2DRepository environmentRepository,
            IPlayerEnvironmentRepository playerEnvironmentRepository,
            ILogger<EnvironmentController> logger)
        {
            _environmentRepository = environmentRepository;
            _playerEnvironmentRepository = playerEnvironmentRepository;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Environment2D>> Create([FromBody] Environment2D request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Name))
            {
                _logger.LogWarning("Environment creation failed: Name is empty.");
                return BadRequest(new { Message = "Environment name cannot be empty." });
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("User ID missing in token.");
                return Unauthorized(new { Message = "Invalid or missing token." });
            }

            var createdEnvironment = await _environmentRepository.InsertAsync(request.Name, request.Description);
            await _playerEnvironmentRepository.AddPlayerToEnvironment(userId, createdEnvironment.ID);

            _logger.LogInformation($"Environment {createdEnvironment.ID} created and linked to user {userId}.");

            return Ok(createdEnvironment);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Environment2D>>> GetAll()
        {
            var environments = await _environmentRepository.GetAllAsync();
            return Ok(environments);
        }

        [HttpGet("getwithobjects/{id:guid}")]
        public async Task<ActionResult<Environment2D>> GetEnvironmentWithObjects(Guid id)
        {
            var environment = await _environmentRepository.GetByIdWithObjectsAsync(id);

            if (environment == null)
            {
                _logger.LogWarning($"Environment {id} not found.");
                return NotFound(new { Message = $"Environment with ID {id} not found." });
            }

            return Ok(environment);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var environment = await _environmentRepository.GetByIdAsync(id);

            if (environment == null)
            {
                _logger.LogWarning($"Environment {id} not found for deletion.");
                return NotFound(new { Message = $"Environment with ID {id} not found." });
            }

            await _environmentRepository.DeleteAsync(id);

            _logger.LogInformation($"Environment {id} deleted.");
            return NoContent();
        }

        [HttpPost("addplayer")]
        public async Task<IActionResult> AddPlayerToEnvironment([FromQuery] string playerId, [FromQuery] Guid environmentId)
        {
            await _playerEnvironmentRepository.AddPlayerToEnvironment(playerId, environmentId);

            _logger.LogInformation($"Player {playerId} added to environment {environmentId}.");
            return Ok(new { Message = "Player added to environment." });
        }

        [HttpDelete("removeplayer")]
        public async Task<IActionResult> RemovePlayerFromEnvironment([FromQuery] string playerId, [FromQuery] Guid environmentId)
        {
            await _playerEnvironmentRepository.RemovePlayerFromEnvironment(playerId, environmentId);

            _logger.LogInformation($"Player {playerId} removed from environment {environmentId}.");
            return Ok(new { Message = "Player removed from environment." });
        }
    }
}
