using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using YoutubeChannelManager.BLL.DTOs.User;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.PL.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UsersController> _logger;


        public UsersController(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }


        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                _logger.LogInformation("GetAllUsers request received");

                var users = _userManager.Users.Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email
                }).ToList();

                _logger.LogInformation("Retrieved {Count} users", users.Count);

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }


        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(string userId)
        {
            try
            {
                _logger.LogInformation("GetUserRoles request received. UserId: {UserId}", userId);

                var user = await _userManager.FindByIdAsync(userId);
                
                if (user == null)
                {
                    _logger.LogWarning("User not found. UserId: {UserId}", userId);
                    return NotFound("User not found.");
                }

                var roles = await _userManager.GetRolesAsync(user);

                _logger.LogInformation(
                    "Retrieved roles for user. UserId: {UserId}, Username: {Username}, Roles: {Roles}",
                    userId, user.UserName, string.Join(", ", roles)
                );

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user roles. UserId: {UserId}", userId);
                throw;
            }
        }


        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var actor = User.Identity?.Name ?? "Unknown";
            var actorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            try
            {
                _logger.LogInformation(
                    "AssignRole request received by {Actor} (ID: {ActorId}). TargetUserId: {UserId}, Role: {Role}",
                    actor, actorId, dto.UserId, dto.Role
                );

                var user = await _userManager.FindByIdAsync(dto.UserId);
                
                if (user == null)
                {
                    _logger.LogWarning(
                        "User not found for role assignment. TargetUserId: {UserId}, RequestedBy: {Actor}",
                        dto.UserId, actor
                    );
                    return NotFound("User not found.");
                }

                if (!await _roleManager.RoleExistsAsync(dto.Role))
                {
                    _logger.LogWarning(
                        "Role does not exist. Role: {Role}, TargetUser: {Username}, RequestedBy: {Actor}",
                        dto.Role, user.UserName, actor
                    );
                    return BadRequest($"Role '{dto.Role}' does not exist.");
                }

                var result = await _userManager.AddToRoleAsync(user, dto.Role);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(
                        "Failed to assign role. Actor: {Actor}, TargetUser: {Username}, Role: {Role}, Errors: {@Errors}",
                        actor, user.UserName, dto.Role, result.Errors
                    );
                    return BadRequest(result.Errors);
                }


                _logger.LogInformation(
                    "ROLE ASSIGNED: Actor: {Actor} assigned role '{Role}' to User: {Username} (ID: {UserId})",
                    actor, dto.Role, user.UserName, user.Id
                );

                return Ok($"Role '{dto.Role}' assigned to user '{user.UserName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error assigning role. Actor: {Actor}, TargetUserId: {UserId}, Role: {Role}",
                    actor, dto?.UserId, dto?.Role
                );
                throw;
            }
        }


        [HttpPut("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto dto)
        {
            var actor = User.Identity?.Name ?? "Unknown";
            
            try
            {
                _logger.LogInformation(
                    "ChangeRole request by {Actor}. TargetUserId: {UserId}, NewRole: {NewRole}",
                    actor, dto.UserId, dto.NewRole
                );

                var user = await _userManager.FindByIdAsync(dto.UserId);
                
                if (user == null)
                {
                    _logger.LogWarning(
                        "User not found for role change. UserId: {UserId}, RequestedBy: {Actor}",
                        dto.UserId, actor
                    );
                    return NotFound("User not found.");
                }

                if (!await _roleManager.RoleExistsAsync(dto.NewRole))
                {
                    _logger.LogWarning(
                        "Role does not exist. NewRole: {NewRole}, RequestedBy: {Actor}",
                        dto.NewRole, actor
                    );
                    return BadRequest($"Role '{dto.NewRole}' does not exist.");
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                var result = await _userManager.AddToRoleAsync(user, dto.NewRole);

                if (!result.Succeeded)
                {
                    _logger.LogWarning(
                        "Failed to change role. Actor: {Actor}, TargetUser: {Username}, NewRole: {NewRole}, Errors: {@Errors}",
                        actor, user.UserName, dto.NewRole, result.Errors
                    );
                    return BadRequest(result.Errors);
                }

               
                _logger.LogInformation(
                    "ROLE CHANGED: Actor: {Actor} changed User: {Username} (ID: {UserId}) from [{OldRoles}] to [{NewRole}]",
                    actor, user.UserName, user.Id, string.Join(", ", currentRoles), dto.NewRole
                );

                return Ok($"Role changed to '{dto.NewRole}' for user '{user.UserName}'.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error changing role. Actor: {Actor}, TargetUserId: {UserId}, NewRole: {NewRole}",
                    actor, dto?.UserId, dto?.NewRole
                );
                throw;
            }
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var actor = User.Identity?.Name ?? "Unknown";
            var actorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            
            try
            {
                _logger.LogWarning(
                    "DELETE USER REQUEST by {Actor} (ID: {ActorId}) for UserId: {UserId}",
                    actor, actorId, userId
                );

                var user = await _userManager.FindByIdAsync(userId);
                
                if (user == null)
                {
                    _logger.LogWarning(
                        "User not found for deletion. UserId: {UserId}, RequestedBy: {Actor}",
                        userId, actor
                    );
                    return NotFound();
                }

                var roles = await _userManager.GetRolesAsync(user);
                var userEmail = user.Email;
                var userName = user.UserName;

                var result = await _userManager.DeleteAsync(user);

                if (!result.Succeeded)
                {
                    _logger.LogError(
                        "Failed to delete user. Actor: {Actor}, TargetUser: {Username}, Errors: {@Errors}",
                        actor, userName, result.Errors
                    );
                    return BadRequest(result.Errors);
                }

                
                _logger.LogWarning(
                    "USER DELETED: Actor: {Actor} (ID: {ActorId}) deleted User: {Username} (ID: {UserId}), Email: {Email}, Roles: [{Roles}]",
                    actor, actorId, userName, userId, userEmail, string.Join(", ", roles)
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error deleting user. Actor: {Actor}, TargetUserId: {UserId}",
                    actor, userId
                );
                throw;
            }
        }


        [HttpGet("roles")]
        public IActionResult GetAllRoles()
        {
            try
            {
                _logger.LogInformation("GetAllRoles request received");

                var roles = _roleManager.Roles.Select(r => new
                {
                    r.Id,
                    r.Name
                }).ToList();

                _logger.LogInformation("Retrieved {Count} roles", roles.Count);

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all roles");
                throw;
            }
        }
    }
}