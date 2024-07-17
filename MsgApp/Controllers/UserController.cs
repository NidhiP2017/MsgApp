using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MsgApp.DTO;
using MsgApp.Interfaces;
using MsgApp.Models;
using System.Security.Claims;

namespace MsgApp.Controllers
{
    [ApiController]
    [Route("api/")]
    public class UserController : ControllerBase
    {
        public readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly MsgAppDbContext _appDbContext;
        public readonly UserManager<ChatUsers> _userManager;
        public readonly SignInManager<ChatUsers> _signInManager;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor, MsgAppDbContext appDbContext, UserManager<ChatUsers> userManager, SignInManager<ChatUsers> signInManager)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _appDbContext = appDbContext;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserDTO registerUser)
        {
            if (ModelState.IsValid)
            {
                var isUniqueEmail = await _userManager.FindByEmailAsync(registerUser.Email);
                if (isUniqueEmail == null)
                {
                    var result = await _userService.RegisterUser(registerUser);
                    return Ok(result);
                }
                else
                {
                    return Ok("This email is already registered, please enter a new email! ");
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                var errorMessage = string.Join("; ", errors);
                return null;
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginUser(LoginUserDTO userDTO)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(userDTO.Email);
                if (user != null)
                {
                    var result = await _userService.LoginUser(userDTO);
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Could not find user");
                }
            }
            else
                return BadRequest("Empty User");
        }

        [Authorize]
        [HttpPut]
        [Route("UpdateStatus")]
        public async Task<IActionResult> UpdateStatus(string status)
        {
            string currentUserId =  _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var u = await _userService.UpdateStatus(currentUserId, status);
            return Ok(u);
        }

        [Authorize]
        [HttpPost]
        [Route("UploadPhoto")]
        public async Task<IActionResult> Upload([FromForm] List<IFormFile> files)
        {
            string currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currentUserId != null)
            {
                var profile = await _userService.UploadPhoto(currentUserId, files);
                return Ok(profile);
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
