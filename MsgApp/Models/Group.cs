using IdentityServer4.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MsgApp.Models
{
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Group name is required.")]
        public string GroupName { get; set; }

        [JsonIgnore]
        public virtual ICollection<Messages> Messages { get; set; }
        public virtual ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();

    }
}
