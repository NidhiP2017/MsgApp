using MsgApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MsgApp.DTO
{
    public class GroupMessagesResponseDTO
    {
        public int GroupId { get; set; }
        public List<MsgDetailDTO> msgsDetailDTO { get; set; }
    }
    public class MsgDetailDTO
    {
        [Key]
        public Guid MessageId { get; set; }
        [ForeignKey("SenderUserId")]
        public Guid SenderId { get; set; }
        public string SenderUser { get; set; }

        public Guid? ParentMessageId { get; set; }

        [ForeignKey("RecieverUserId")]
        public Guid ReceiverId { get; set; }
        public string ReceiverUser { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        //public Group Group { get; set; }
        public virtual ChatUsers SenderUserId { get; set; }
        public virtual ChatUsers RecieverUserId { get; set; }
    }
}
