using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MsgApp.Models
{
    public class Messages
    {
        [Key]
        public Guid MessageId { get; set; }
        public Guid? ParentMessageId { get; set; }

        //[ForeignKey(nameof(Groups))]
        public int? GroupId { get; set; }
        public virtual Group Groups { get; set; }

        //public virtual ChatUsers ChatUsers {  get; set; }

        //[ForeignKey("SenderUser")]
        public Guid SenderId { get; set; }

        //[ForeignKey("RecieverUser")]
        public Guid ReceiverId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 2)]
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        //public virtual ChatUsers RecieverUser { get; set; }
        //public virtual ChatUsers SenderUser { get; set; }
    }
}