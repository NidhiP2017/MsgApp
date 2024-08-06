using Microsoft.AspNetCore.Mvc;
using MsgApp.DTO;
using MsgApp.Models;

namespace MsgApp.Interfaces
{
    public interface IMessageService
    {
        Task<ActionResult<MessagesDTO>> SendMessageAsync(SendMessageRequestDTO msgDTO);
        //Task<List<MessageGroupDTO>> getAllMyMessages();
        Task<List<GroupMessagesResponseDTO>> getAllMyMessages();
        Messages GetMessage(Guid msgId);
        Task<IActionResult> EditMessageAsync(Guid msgId, string content);
        Task<IActionResult> DeleteMessageAsync(Guid msgId);
        Task<List<MessagesDTO>> SearchMsgs(string msg);
        Task<IActionResult> RetriveConversationHistoryAsync(Guid userId, DateTime? before, int count, string sort);
        Task<IActionResult> ReplyToMsg(int? groupId, Guid messageId, ThreadMessageDTO messageRequest);
    }
}
