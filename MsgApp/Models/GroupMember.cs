using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MsgApp.Models
{
    public class GroupMember
    {
        [Key]
        public int MemberId { get; set; }
        public string UserId { get; set; }

        [JsonIgnore]
        public ChatUsers User { get; set; }
        //[ForeignKey("Gid")]
        public int GroupId { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }
        public DateTime JoinTime { get; set; }
        //public virtual ChatUsers Uid { get; set; }
        //public virtual Group Gid { get; set; }
        public bool IncludePreviousChat { get; set; }

    }
}
