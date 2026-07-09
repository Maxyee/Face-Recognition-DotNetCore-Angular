using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Face_Recognition_Demo.Models;

namespace Face_Recognition_Demo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<User> userManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized();

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound(new { message = "User not found" });

                var roles = await _userManager.GetRolesAsync(user);

                return Ok(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FirstName,
                    user.LastName,
                    user.CreatedAt,
                    user.IsActive,
                    Roles = roles
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.UserName,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.CreatedAt,
                        u.IsActive
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "An error occurred" });
            }
        }

        [HttpGet("admin-only")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok(new { message = "This is an admin-only endpoint" });
        }

        [HttpGet("user-only")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult UserEndpoint()
        {
            return Ok(new { message = "This endpoint is accessible by users and admins" });
        }
    }
}