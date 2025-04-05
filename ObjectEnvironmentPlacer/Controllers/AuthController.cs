using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ObjectEnvironmentPlacer.Objects;
using ObjectEnvironmentPlacer.Other;
using System.Threading.Tasks;
using ObjectEnvironmentPlacer.Interface;

namespace ObjectEnvironmentPlacer.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenGenerator _tokenGenerator; // ✅ Add JwtTokenGenerator

        public AuthController(
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            JwtTokenGenerator tokenGenerator) // ✅ Inject JwtTokenGenerator
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                return BadRequest(new { Message = "Username, Email, and Password are required." });
            }

            var existingUser = await _userManager.FindByNameAsync(model.Name);
            if (existingUser != null)
            {
                return BadRequest(new { Message = "Username is already taken." });
            }

            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { Message = "Email is already registered." });
            }

            var user = new ApplicationUser
            {
                UserName = model.Name,
                Email = model.Email,
                Name = model.Name
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully" });
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("check-username")]
        public async Task<IActionResult> CheckUsernameAvailability([FromQuery] string username)
        {
            var existingUser = await _userManager.FindByNameAsync(username);
            bool isTaken = existingUser != null;
            return Ok(new { available = !isTaken });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if (user == null)
                {
                    return Unauthorized(new { Message = "Ongeldige gebruikersnaam of wachtwoord." });
                }

                var isValid = await _userManager.CheckPasswordAsync(user, model.Password);
                if (!isValid)
                {
                    return Unauthorized(new { Message = "Ongeldige gebruikersnaam of wachtwoord." });
                }

                var token = _tokenGenerator.GenerateToken(user);
                return Ok(new { accessToken = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Interne serverfout", Error = ex.Message });
            }
        }

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] RegisterModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                user.Name = model.Name;
            }

            if (!string.IsNullOrWhiteSpace(model.Email) && user.Email != model.Email)
            {
                var emailInUse = await _userManager.FindByEmailAsync(model.Email);
                if (emailInUse != null)
                {
                    return BadRequest(new { Message = "Email already in use" });
                }

                user.Email = model.Email;
                user.NormalizedEmail = model.Email.ToUpper();
                user.UserName = model.Email;
                user.NormalizedUserName = model.Email.ToUpper();
            }

            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordChangeResult = await _userManager.ResetPasswordAsync(user, token, model.Password);

                if (!passwordChangeResult.Succeeded)
                {
                    return BadRequest(new { Message = "Failed to update password", Errors = passwordChangeResult.Errors });
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { Message = "Failed to update user", Errors = updateResult.Errors });
            }

            return Ok(new { Message = "User updated successfully" });
        }

        [HttpPost("DeleteUser")]
        public async Task<IActionResult> DeleteUser([FromBody] RegisterModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                return BadRequest(new { Message = "Failed to delete user", Errors = deleteResult.Errors });
            }

            return Ok(new { Message = "User deleted successfully" });
        }
    }
}