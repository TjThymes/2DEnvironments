    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using ObjectEnvironmentPlacer.Objects;
    using ObjectEnvironmentPlacer.Repositories;
    using ObjectEnvironmentPlacer.Interface;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using System.IdentityModel.Tokens.Jwt; // ✅ Important for JwtRegisteredClaimNames


namespace ObjectEnvironmentPlacer.Controllers
{
        [ApiController]
        [Route("api/objects")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public class ObjectController : ControllerBase
        {
            private readonly IObjectRepository _objectRepository;
            private readonly ILogger<ObjectController> _logger;

            public ObjectController(IObjectRepository objectRepository, ILogger<ObjectController> logger)
            {
                _objectRepository = objectRepository;
                _logger = logger;
            }

            [HttpGet(Name = "GetObjects")]
            public async Task<ActionResult<IEnumerable<GameObject2D>>> GetAll()
            {
                _logger.LogInformation("Fetching all objects.");
                var objects = await _objectRepository.GetAllAsync();
                return Ok(objects);
            }

            [HttpGet("{id:guid}", Name = "GetObjectById")]
            public async Task<ActionResult<GameObject2D>> GetById(Guid id)
            {
                _logger.LogInformation($"Fetching object with ID: {id}");
                var obj = await _objectRepository.GetByIdAsync(id);

                if (obj == null)
                {
                    _logger.LogWarning($"Object with ID {id} not found.");
                    return NotFound($"Object with ID {id} not found.");
                }

                return Ok(obj);
            }

            [HttpPost(Name = "CreateObject")]
            public async Task<ActionResult<GameObject2D>> Create([FromBody] GameObject2D gameObject)
            {
                if (gameObject == null || gameObject.EnvironmentID == Guid.Empty)
                {
                    _logger.LogWarning("Invalid object data received for creation.");
                    return BadRequest("Invalid object data.");
                }

                if (gameObject.ID != Guid.Empty)
                {
                    _logger.LogWarning("Attempt to create an object with a pre-existing ID.");
                    return BadRequest("New objects should not have an existing ID.");
                }

                gameObject.ID = Guid.NewGuid();
                await _objectRepository.InsertAsync(gameObject);
                _logger.LogInformation($"Object created with ID: {gameObject.ID}");
                return CreatedAtRoute("GetObjectById", new { id = gameObject.ID }, gameObject);
            }


            [HttpPut("{id:guid}", Name = "UpdateObjectById")]
            public async Task<IActionResult> Update(Guid id, [FromBody] GameObject2D updatedObject)
            {
                if (updatedObject == null || id != updatedObject.ID)
                {
                    _logger.LogWarning("Mismatched object ID.");
                    return BadRequest("ID in the URL does not match the object ID.");
                }

                var existingObject = await _objectRepository.GetByIdAsync(id);
                if (existingObject == null)
                {
                    _logger.LogWarning($"Object with ID {id} not found.");
                    return NotFound(new { message = $"Object with ID {id} not found." });
                }

                await _objectRepository.UpdateAsync(updatedObject);
                _logger.LogInformation($"Object with ID {id} updated successfully.");
                return Ok(new { message = "Object updated successfully." });
            }


            [HttpDelete("{id:guid}", Name = "DeleteObjectById")]
            public async Task<IActionResult> Delete(Guid id)
            {
                var existingObject = await _objectRepository.GetByIdAsync(id);
                if (existingObject == null)
                {
                    _logger.LogWarning($"Object with ID {id} not found.");
                    return NotFound($"Object with ID {id} not found.");
                }

                await _objectRepository.DeleteAsync(id);
                _logger.LogInformation($"Object with ID {id} deleted successfully.");
                return NoContent();
            }

            [HttpPost("SaveObject/{environmentId}")]
            public async Task<ActionResult<GameObject2D>> SaveObjectToEnvironment(Guid environmentId, [FromBody] GameObject2D gameObject)
            {
                try
                {
                    if (gameObject == null)
                    {
                        _logger.LogWarning("❌ Invalid object data.");
                        return BadRequest("Invalid object data.");
                    }

                    if (environmentId == Guid.Empty)
                    {
                        _logger.LogWarning("❌ Environment ID is missing.");
                        return BadRequest("Environment ID is required.");
                    }

                if (gameObject.ID == Guid.Empty)
                {
                    gameObject.ID = Guid.NewGuid(); // Alleen als Unity geen ID gaf
                }

                // ✅ Assign EnvironmentID and Generate New ID
                gameObject.EnvironmentID = environmentId;

                    await _objectRepository.SaveObjectToEnvironmentAsync(gameObject);
                    _logger.LogInformation($"✅ Object {gameObject.ID} saved to Environment {environmentId}");

                    return Ok(gameObject);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"❌ Error saving object: {ex.Message}");
                    return StatusCode(500, "Internal Server Error");
                }
            }

        }
    }
