using Microsoft.AspNetCore.Identity;

namespace MsgApp.Models
{
    public class ChatUsers : IdentityUser
    {
        public string? UserStatus { get; set; }

        public string? ProfilePhoto { get; set;}
    }
}
