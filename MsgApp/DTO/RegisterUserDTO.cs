using System.ComponentModel.DataAnnotations;

namespace MsgApp.DTO
{
    public class RegisterUserDTO
    {
        [Required]
        [MaxLength(150)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(150)]
        public string Email { get; set; }
        [Required]
        [MaxLength(150)]
        public string Password { get; set; }

    }
}
