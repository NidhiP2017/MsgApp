using Microsoft.AspNetCore.Mvc;
using MsgApp.DTO;
using MsgApp.Models;

namespace MsgApp.Interfaces
{
    public interface IGroupService
    {
        Task<GroupResponseDTO> CreateGroup(GroupCreateRequestDTO request);
        Task<Group> GetGroupByIdAsync(int grpId);
        Task<List<string>> GetGroupMemberIdsAsync(int groupId);
        Task<bool> IsUserMemberOfGroup(string userId, int groupId);
        Task<List<Group>> GetGroups();
        Task<GroupResponseDTO> EditGroupName(int grpId, string editGrpName);
        Task<IActionResult> EditGroupMembers(int grpId, UpdateGroupMembersDTO request);
        Task<Group> GetGroupWithMembers(int groupId);
        Task<IActionResult> SendMessage(int groupId, GroupMessageRequestDTO messageRequest);
        Task<IActionResult> SendReplyInThread(int groupId, Guid messageId, ThreadMessageDTO messageRequest);
    }
}
