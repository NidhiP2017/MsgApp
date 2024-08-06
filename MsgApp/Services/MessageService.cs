using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MsgApp.DTO;
using MsgApp.Interfaces;
using MsgApp.Migrations;
using MsgApp.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace MsgApp.Services
{
    public class MessageService : IMessageService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly MsgAppDbContext _appDbContext;
        private readonly IMapper _imapper;
        public MessageService(IRepositoryService repository, MsgAppDbContext appDbContext, IMapper mapper)
        {
            _repositoryService = repository;
            _appDbContext = appDbContext;
            _imapper = mapper;
        }
        public async Task<ActionResult<MessagesDTO>> SendMessageAsync(SendMessageRequestDTO msgDTO)
        {
            string currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            ChatUsers ReceiverUser = await _appDbContext.ChatUsers.FirstOrDefaultAsync(u => u.Id == msgDTO.ReceiverId.ToString());
            string checkReceiverId = ReceiverUser.Id;
            string checkSenderId = msgDTO.SenderId.ToString();

            if (checkSenderId == null)
            {
                return new BadRequestObjectResult("Unauthorized");
            }
            if (msgDTO == null || string.IsNullOrWhiteSpace(msgDTO.Content))
            {
                return new OkObjectResult("Message content is required");
            }
            if (checkReceiverId == null || checkReceiverId == null)
            {
                return new OkObjectResult("Receiver Not Found");
            }
            if (currentUserId.Equals(checkReceiverId))
            {
                return new OkObjectResult("Can not send a message to yourself.");
            }

            if (msgDTO != null)
            {
                var newMsg = new Messages()
                {
                    SenderId = Guid.Parse(currentUserId),
                    ReceiverId = Guid.Parse(checkReceiverId),
                    Content = msgDTO.Content,
                    Timestamp = DateTime.Now,
                };
                await _appDbContext.Messages.AddAsync(newMsg);
                await _appDbContext.SaveChangesAsync();

                /*var response = new MessagesDTO()
                {
                    MessageId = newMsg.MessageId,
                    SenderId = msgDTO.SenderId,
                    ReceiverId = msgDTO.ReceiverId,
                    Content = msgDTO.Content,
                    Timestamp = newMsg.Timestamp
                };*/
                return new OkObjectResult(newMsg);
            }
            else
            {
                return new OkObjectResult("Invalid Content");
            }
        }

        public async Task<List<GroupMessagesResponseDTO>> getAllMyMessages()
        {
            string Id = await _repositoryService.GetCurrentLoggedInUser();
            Guid currentUserId = Guid.Parse(Id);

            var groups = await _appDbContext.GroupMembers
            .Where(gm => gm.UserId == Id)
            .Select(gm => gm.GroupId)
            .Distinct()
            .ToListAsync();

            var msgs = await _appDbContext.Messages
                .Where(a => a.GroupId != null)
                .ToListAsync();

            var uesrs = await _appDbContext.Users
                .ToListAsync();

            msgs = msgs.Where(a => groups.Contains(a.GroupId.Value)).ToList();

            List<GroupMessagesResponseDTO> userMsgs = new List<GroupMessagesResponseDTO>();

            var results = msgs.GroupBy(
                            p => p.GroupId).ToList();
            var m = new List<MsgDetailDTO>();
            var groupMsg = new GroupMessagesResponseDTO();

            foreach (var res in results)
            {
                groupMsg = new GroupMessagesResponseDTO();
                m = new List<MsgDetailDTO>();

                groupMsg.GroupId = res.Key.Value;

                foreach (var msg in res)
                {
                    m.Add(new MsgDetailDTO()
                    {
                        MessageId = msg.MessageId,
                        SenderId = msg.SenderId,
                        ReceiverId = msg.ReceiverId,
                        SenderUser = uesrs.First(a => a.Id.ToString() == msg.SenderId.ToString()).UserName,
                        ReceiverUser = uesrs.First(a => a.Id.ToString() == msg.ReceiverId.ToString()).UserName,
                    });
                }
                groupMsg.msgsDetailDTO = m;

                userMsgs.Add(groupMsg);
            }
            return userMsgs;

        }
        public async Task<IActionResult> EditMessageAsync(Guid msgId, string content)
        {
            if (content != null)
            {
                string currentUserId = await _repositoryService.GetCurrentLoggedInUser();
                Guid cid = Guid.Parse(currentUserId);
                var msg = await _appDbContext.Messages.FindAsync(msgId);
                if (msg == null || (msg.SenderId != cid && msg.ReceiverId != cid))
                {
                    return new OkObjectResult("Invalid access or empty message");
                }
                else
                {
                    msg.Content = content;
                    await _appDbContext.SaveChangesAsync();
                    return new OkObjectResult(msg);
                }
            }
            else
            {
                return new OkObjectResult("Invalid Content");
            }
        }
        public async Task<IActionResult> DeleteMessageAsync(Guid deleteMsgId)
        {
            string currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            if (deleteMsgId != null)
            {
                var msg = await _appDbContext.Messages.FindAsync(deleteMsgId);
                if (msg.SenderId != Guid.Parse(currentUserId))
                {
                    return new OkObjectResult("You cannot delete others msg");
                }
                if (msg != null)
                {
                    _appDbContext.Remove(msg);
                    await _appDbContext.SaveChangesAsync();
                    return new OkObjectResult("Message Deleted");
                }
                else
                {
                    return new OkObjectResult("Could Not find message to delete");
                }
            }
            return new OkObjectResult("Blank Message ID");
        }
        public async Task<IActionResult> RetriveConversationHistoryAsync(Guid userId, DateTime? before, int count, string sort)
        {
            var query = _appDbContext.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId);
            if (before.HasValue)
            {
                query = query.Where(m => m.Timestamp <= before.Value);
            }
            query = sort.ToLower() == "asc" ? query.OrderBy(m => m.Timestamp) : query.OrderByDescending(m => m.Timestamp);
            query = query.Take(count);
            var messages = await query.ToListAsync();
            return new OkObjectResult(messages);

        }

        public async Task<List<MessagesDTO>> SearchMsgs(string msg)
        {
            string currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            if (currentUserId != null)
            {
                var conversations = await _appDbContext.Messages
                                    .Where(c => (c.SenderId == Guid.Parse(currentUserId) || c.ReceiverId == Guid.Parse(currentUserId)) && c.Content.Contains(msg))
                                    .AsNoTracking() //to disconnect from database
                                    .ToListAsync();
                if (conversations.Any())
                {
                    List<MessagesDTO> matchedConversations = new List<MessagesDTO>();
                    matchedConversations = _imapper.Map<List<MessagesDTO>>(conversations);
                    return matchedConversations;
                }
                else
                {
                    return new List<MessagesDTO>();
                }
            }
            else
            {
                return null;
            }
        }
        [Authorize]
        public Messages GetMessage(Guid msgId)
        {
            var query = _appDbContext.Messages.FirstOrDefault(u => u.MessageId == msgId);
            if (query == null)
            {
                return null;
            }
            return query;
        }
        public async Task<IActionResult> ReplyToMsg(int? groupId, Guid messageId, ThreadMessageDTO messageRequest)
        {
            var parentMsg = GetMessage(messageId);
            string currentUserId = await _repositoryService.GetCurrentLoggedInUser();

            if (string.IsNullOrEmpty(messageRequest.Content))
            {
                throw new ArgumentException("Message is required.");
            }
            else
            {
                var message = new Messages
                {
                    Content = messageRequest.Content,
                    SenderId = Guid.Parse(currentUserId),
                    ReceiverId = parentMsg.ReceiverId,
                    ParentMessageId = parentMsg.MessageId,
                    GroupId = (groupId != null) ? groupId : null,
                    Timestamp = DateTime.Now,
                };
                await _appDbContext.Messages.AddAsync(message);
                await _appDbContext.SaveChangesAsync();

                var response = new
                {
                    senderId = currentUserId,
                    content = messageRequest.Content,
                    GroupId = (groupId != null) ? groupId : null,
                    timestamp = message.Timestamp
                };
                return new OkObjectResult(response);
            }
        }
    }
}
