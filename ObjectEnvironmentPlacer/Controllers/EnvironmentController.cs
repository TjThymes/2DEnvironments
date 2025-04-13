using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ObjectEnvironmentPlacer.Objects;
using ObjectEnvironmentPlacer.Interface;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

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
                return BadRequest(new { Message = "Environment name cannot be empty." });

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Invalid or missing token." });

            try
            {
                // ✅ Ophalen bestaande omgevingen van deze gebruiker
                var existingEnvironments = await _environmentRepository.GetByUserIdAsync(userId);

                // ✅ Maximaal 5 omgevingen
                if (existingEnvironments.Count >= 5)
                    return BadRequest(new { Message = "You cannot have more than 5 environments." });

                // ✅ Naam moet uniek zijn per gebruiker
                if (existingEnvironments.Any(env => env.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
                    return BadRequest(new { Message = "An environment with the same name already exists." });

                // ✅ Nieuwe omgeving maken
                var createdEnvironment = await _environmentRepository.InsertAsync(
                    request.Name,
                    request.Description,
                    request.Width,
                    request.Height
                );

                await _playerEnvironmentRepository.AddPlayerToEnvironment(userId, createdEnvironment.ID);

                _logger.LogInformation($"Environment {createdEnvironment.ID} created and linked to user {userId}.");

                return Ok(createdEnvironment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating environment");
                return StatusCode(500, new { Message = "An unexpected error occurred while creating the environment." });
            }
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
            return environment == null
                ? NotFound(new { Message = $"Environment with ID {id} not found." })
                : Ok(environment);
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var environment = await _environmentRepository.GetByIdAsync(id);
            if (environment == null)
                return NotFound(new { Message = $"Environment with ID {id} not found." });

            await _environmentRepository.DeleteAsync(id);
            _logger.LogInformation($"Environment {id} deleted.");
            return NoContent();
        }

        [HttpPost("addplayer")]
        public async Task<IActionResult> AddPlayerToEnvironment([FromQuery] Guid environmentId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Invalid or missing token." });

            await _playerEnvironmentRepository.AddPlayerToEnvironment(userId, environmentId);

            _logger.LogInformation($"User {userId} added to environment {environmentId}.");
            return Ok(new { Message = "Player added to environment." });
        }


        [HttpDelete("removeplayer")]
        public async Task<IActionResult> RemovePlayerFromEnvironment([FromQuery] Guid environmentId)
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
             User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Invalid or missing token." });
            await _playerEnvironmentRepository.RemovePlayerFromEnvironment(userId, environmentId);
            _logger.LogInformation($"Player {userId} removed from environment {environmentId}.");
            return Ok(new { Message = "Player removed from environment." });
        }
        [HttpGet("myenvironmentids")]
        public async Task<ActionResult<IEnumerable<object>>> GetEnvironmentsForCurrentUser()
        {
            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { Message = "Invalid or missing token." });

            var environmentIds = await _playerEnvironmentRepository.GetEnvironmentIdsByUserIdAsync(userId);

            if (environmentIds == null || environmentIds.Count == 0)
                return NotFound(new { Message = "No environments found for this user." });

            var environments = new List<object>();

            foreach (var envId in environmentIds)
            {
                var env = await _environmentRepository.GetByIdAsync(envId);
                if (env != null)
                {
                    environments.Add(new
                    {
                        id = env.ID,
                        name = env.Name,
                        description = env.Description
                    });
                }
            }

            return Ok(environments);
        }

    }
}
