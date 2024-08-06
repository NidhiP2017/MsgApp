using System.ComponentModel.DataAnnotations;

namespace MsgApp.DTO
{
    public class SendMessageRequestDTO
    {
        [Required]
        public Guid SenderId { get; set; }

        [Required]
        public Guid ReceiverId { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
