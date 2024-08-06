using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MsgApp.DTO;
using MsgApp.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MsgApp.Controllers
{
    [ApiController]
    [Route("api/")]
    public class MessagesController : Controller
    {
        public readonly IMessageService _ims;

        public MessagesController(IMessageService ims)
        {
            _ims = ims;
        }

        [Authorize]
        [HttpPost]
        [Route("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequestDTO sendMsg)
        {
            var sendNewMsg = await _ims.SendMessageAsync(sendMsg);
            return Ok(sendNewMsg);
        }

        [Authorize]
        [HttpPost]
        [Route("GetAllMyMessages")]
        public async Task<IActionResult> GetMyAllMessages()
        {
            var messages = await _ims.getAllMyMessages();
            return Ok(messages);
        }
        [Authorize]
        [HttpPut]
        [Route("editMessages/{msgId}")]
        public async Task<IActionResult> EditMessage(Guid msgId, [Required]
        [StringLength(1000, MinimumLength = 2)]string content)
        {
            var orgMsg = _ims.GetMessage(msgId);
            if (orgMsg != null)
            {
                var updateMsg = await _ims.EditMessageAsync(msgId, content);
                return Ok(updateMsg);
            }
            else
            {
                return NotFound("Msg Not found for requested Id");
            }
        }
        [Authorize]
        [HttpDelete]
        [Route("deleteMessage/{msgId}")]
        public async Task<IActionResult> DeleteMsg(Guid msgId)
        {
            var orgMsg = _ims.GetMessage(msgId);
            if (orgMsg != null)
            {
                var deleteMsg = await _ims.DeleteMessageAsync(msgId);
                return Ok(deleteMsg);
            }
            else
            {
                return NotFound("Msg Not found for requested Id");
            }
        }
        [Authorize]
        [HttpGet]
        [Route("messagesHistory")]
        public async Task<IActionResult> RetriveConversationHistoryAsync([FromQuery] Guid userId,
            [FromQuery] DateTime? before = null,
            [FromQuery] int count = 20,
            [FromQuery] string sort = "asc")
        {

            var messages = await _ims.RetriveConversationHistoryAsync(userId, before, count, sort);
            return Ok(messages);
        }

        [Authorize]
        [HttpGet]
        [Route("conversation/search")]
        public async Task<IActionResult> SearchConversations(string query)
        {
            var msgs = await _ims.SearchMsgs(query);
            return Ok(msgs);
        }
    }
}
