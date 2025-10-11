using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YoutubeChannelManager.BLL.DTOs.Account;
using YoutubeChannelManager.BLL.Interfaces;
using YoutubeChannelManager.DAL.Models;

namespace YoutubeChannelManager.PL.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<AppUser> userManager, 
            ITokenService tokenService, 
            SignInManager<AppUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                _logger.LogInformation("Registration attempt. Username: {Username}, Email: {Email}", 
                    registerDto.Username, registerDto.Email);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Registration failed: Invalid model state. Username: {Username}", 
                        registerDto.Username);

                    return BadRequest(ModelState);
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    _logger.LogInformation("User created successfully. Username: {Username}, Id: {UserId}", 
                        appUser.UserName, appUser.Id);

                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");

                    if (roleResult.Succeeded)
                    {
                        _logger.LogInformation("User role assigned. Username: {Username}, Role: User", 
                            appUser.UserName);

                        return Ok(new NewUserDto
                        {
                            Username = appUser.UserName,
                            Email = appUser.Email,
                            Token = await _tokenService.CreateTokenAsync(appUser)
                        });
                    }
                    else
                    {
                        _logger.LogError("Failed to assign role to user. Username: {Username}, Errors: {@Errors}", 
                            appUser.UserName, roleResult.Errors);

                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    _logger.LogWarning("User creation failed. Username: {Username}, Errors: {@Errors}", 
                        registerDto.Username, createdUser.Errors);

                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration. Username: {Username}", 
                    registerDto?.Username);

                return StatusCode(500, ex);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            try
            {
                _logger.LogInformation("Login attempt. Username: {Username}", loginDto.Username);

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Login failed: Invalid model state. Username: {Username}", 
                        loginDto.Username);

                    return BadRequest(ModelState);
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found. Username: {Username}", 
                        loginDto.Username);

                    return Unauthorized("Invalid username!");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login failed: Invalid password. Username: {Username}, UserId: {UserId}", 
                        loginDto.Username, user.Id);
                        
                    return Unauthorized("Username or password incorrect");
                }

                _logger.LogInformation("Login successful. Username: {Username}, UserId: {UserId}", 
                    user.UserName, user.Id);

                return Ok(new NewUserDto
                {
                    Username = user.UserName,
                    Email = user.Email,
                    Token = await _tokenService.CreateTokenAsync(user)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login. Username: {Username}", loginDto?.Username);
                throw;
            }
        }
    }
}