using IdentityServer4.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MsgApp.DTO;
using MsgApp.Interfaces;
using MsgApp.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MsgApp.Services
{
    public class UserService : IUserService
    {
        private readonly MsgAppDbContext _dbContext;
        private readonly UserManager<ChatUsers> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly SignInManager<ChatUsers> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        public UserService(MsgAppDbContext dbContext, UserManager<ChatUsers> userManager, IWebHostEnvironment webHostEnvironment, SignInManager<ChatUsers> signInManager, IConfiguration configuration, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _signInManager = signInManager;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        public async Task<IActionResult> GetUsersList(string currentUserId)
        {
            var userList = await _dbContext.Users
            .Where(u => u.Id != currentUserId)
            .Select(u => new
            {
                name = u.UserName,
                email = u.Email,

            })
            .ToListAsync();
            return new OkObjectResult(new { users = userList });
        }
        public async Task<RegisteredUserResponseDTO> RegisterUser(RegisterUserDTO registerUser)
        {
            var user = new ChatUsers
            {
                UserName = registerUser.UserName,
                Email = registerUser.Email
            };
            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (result.Succeeded)
            {
                var response = new RegisteredUserResponseDTO
                {
                    UserName = registerUser.UserName,
                    Email = registerUser.Email
                };
                return response;
            }
            else
            {
                return null;
            }
        }
        public async Task<string> LoginUser(LoginUserDTO userDTO)
        {
             var user = await _userManager.FindByEmailAsync(userDTO.Email);
             if (user != null)
             {
                 var result = await _signInManager.PasswordSignInAsync(user, userDTO.Password, false, false);
                 if (result.Succeeded)
                 {
                    var token =  _tokenService.TokenGenerate(user);
                    return token;
                 }
                 else
                     return null;
             }
             else
                return null;
        }

        public async Task<IActionResult> UpdateStatus(string Id, string status)
        {
            var asd = Id.GetType();
            if (Id != null)
            {
                var user = await _userManager.FindByIdAsync(Id);
                if (user != null)
                {
                    user.UserStatus = status;
                    await _dbContext.SaveChangesAsync();
                    return new OkObjectResult("Status updated to: " + status);
                }
                return new OkObjectResult("User not found");
            }
            return new OkObjectResult("Could not find user");
        }

        public async Task<IActionResult> UploadPhoto(string Id, List<IFormFile> files)
        {
            if (files.Count == 0)
                return new OkObjectResult("No file was uploaded");
            string directoryPath = Path.Combine(_webHostEnvironment.ContentRootPath, "ProfilePhotos");
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            foreach (var file in files)
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return new OkObjectResult("Invalid file type.");
                }
                string filePath = Path.Combine(directoryPath, file.FileName + "_" + Id);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                var user = await _dbContext.ChatUsers.FindAsync("3da7ea60-925d-45c9-b3b8-d8e08d366fd0");
                user.ProfilePhoto = Path.Combine(directoryPath, file.FileName);
                await _dbContext.SaveChangesAsync();
            }
            return new OkObjectResult("Upload Successful");
        }
        public string GetToken()
        {
            var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
                        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: signIn);

            string AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return AccessToken;
        }
    }
}
