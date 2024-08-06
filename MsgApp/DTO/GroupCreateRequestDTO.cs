using System.ComponentModel.DataAnnotations;

namespace MsgApp.DTO
{
    public class GroupCreateRequestDTO
    {
        [Required(ErrorMessage = "Group name is required.")]
        public string GroupName { get; set; }
    }
}
