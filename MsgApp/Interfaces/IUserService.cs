using Microsoft.AspNetCore.Mvc;
using MsgApp.DTO;

namespace MsgApp.Interfaces
{
    public interface IUserService
    {
        Task<RegisteredUserResponseDTO> RegisterUser(RegisterUserDTO registerUser);
        Task<string> LoginUser(LoginUserDTO userDTO);
        public string GetToken();
        Task<IActionResult> UpdateStatus(string Id, string status);
        Task<IActionResult> UploadPhoto(string Id, List<IFormFile> files);
    }
}
