using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsgApp.DTO;
using MsgApp.Interfaces;
using MsgApp.Models;

namespace MsgApp.Controllers
{
    [ApiController]
    [Route("api/")]
    public class GroupController
    {
        private readonly IGroupService _groupService;
        private readonly IMessageService _messageService;
        public GroupController(IGroupService groupService, IMessageService messageService)
        {
            _groupService = groupService;   
            _messageService = messageService;
        }

        [Authorize]
        [HttpPost]
        [Route("createGroup")]
        public async Task<GroupResponseDTO> CreateGroup([FromBody] GroupCreateRequestDTO request)
        {
            return await _groupService.CreateGroup(request);
        }

        [Authorize]
        [HttpGet]
        [Route("getGroups")]
        public async Task<List<Group>> GetGroups()
        {
            return await _groupService.GetGroups();
        }
        [Authorize]
        [HttpPut]
        [Route("editGroup")]
        public async Task<GroupResponseDTO> EditGroupName(int grpId, string editGrpName)
        {
            return await _groupService.EditGroupName(grpId, editGrpName);
        }
        [Authorize]
        [HttpPut]
        [Route("addEditMembers")]
        public async Task<IActionResult> addEditMembers(int grpId, [FromBody] UpdateGroupMembersDTO request)
        {
            return await _groupService.EditGroupMembers(grpId, request);
        }
        [Authorize]
        [HttpPost("{groupId}/messages")]
        public async Task<IActionResult> SendMessage(int groupId, [FromBody] GroupMessageRequestDTO messageRequest)
        {
            return await _groupService.SendMessage(groupId, messageRequest);
        }

        [Authorize]
        [HttpPost("{groupId}/{messageId}/ReplyInThread")]
        /*reply to a particular message in thread*/
        public async Task<IActionResult> SendReplyInThread(int groupId, Guid messageId, [FromBody] ThreadMessageDTO messageRequest)
        {
            return await _groupService.SendReplyInThread(groupId, messageId, messageRequest);
        }
    }
}
