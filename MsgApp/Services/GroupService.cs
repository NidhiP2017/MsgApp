using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MsgApp.DTO;
using MsgApp.Interfaces;
using MsgApp.Models;
using System;

namespace MsgApp.Services
{
    public class GroupService : IGroupService
    {
        private readonly IRepositoryService _repositoryService;
        private readonly MsgAppDbContext _appDbContext;
        private readonly IMessageService _msgService;
        public GroupService(IRepositoryService repository, MsgAppDbContext appDbContext, IMessageService msgService)
        {
            _repositoryService = repository; 
            _appDbContext = appDbContext;
            _msgService = msgService;
        }
        public async Task<GroupResponseDTO> CreateGroup(GroupCreateRequestDTO request)
        {
            if (string.IsNullOrEmpty(request.GroupName))
            {
                throw new ArgumentException("Group name is required.");
            }
            var currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            ChatUsers currentUser = await _appDbContext.ChatUsers.FirstOrDefaultAsync(u => u.Id == currentUserId);
            if (currentUser == null)
            {
                throw new Exception("Unable to retrieve currentuser");
            }
            var group = new Models.Group
            {
                GroupName = request.GroupName
            };
            await _appDbContext.Groups.AddAsync(group);
            await _appDbContext.SaveChangesAsync();

            var groupMember = new GroupMember
            {
                UserId = currentUser.Id,
                GroupId = group.GroupId,
                JoinTime = DateTime.UtcNow,
                IncludePreviousChat = true
            };
            await _appDbContext.GroupMembers.AddAsync(groupMember);
            await _appDbContext.SaveChangesAsync();

            var response = new GroupResponseDTO
            {
                GroupId = group.GroupId,
                GroupName = group.GroupName
            };
            return response;
        }

        public async Task<List<Group>> GetGroups()
        {
            var currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            if (currentUserId == null)
            {
                throw new Exception("Login is mandatory");
            }

            var userGroups = await _appDbContext.GroupMembers
                .Include(gm => gm.Group.GroupMembers)
                .Where(gm => gm.UserId == currentUserId)
                .Select(gm => gm.Group)
                .ToListAsync();

            return userGroups;

        }
        public async Task<GroupResponseDTO> EditGroupName(int grpId, string editGrpName)
        {
            var group = await _appDbContext.Groups.FindAsync(grpId);
            if (string.IsNullOrEmpty(editGrpName))
            {
                throw new ArgumentException("New Group name is required.");
            }
            var currentUserId = await _repositoryService.GetCurrentLoggedInUser();

            if (currentUserId == null)
            {
                throw new Exception("Unable to retrieve currentuser");
            }
            var IsPartOfGrp = _appDbContext.GroupMembers.FirstOrDefault(u => u.UserId == currentUserId && u.GroupId == grpId);
            if (IsPartOfGrp == null)
            {
                throw new Exception("You are not a part of group so you cannot edit its name");
            }
            else
            {
                group.GroupName = editGrpName;
                await _appDbContext.SaveChangesAsync();
                var response = new GroupResponseDTO
                {
                    GroupId = grpId,
                    GroupName = editGrpName,
                };
                return response;
            }
        }
        public async Task<Group> GetGroupWithMembers(int grpId)
        {
            // Retrieve the group with its associated GroupMembers
            var groupMemb = await _appDbContext.Groups
                        .Include(g => g.GroupMembers)
                        .ThenInclude(gm => gm.User)
                .FirstOrDefaultAsync(g => g.GroupId == grpId);

            return groupMemb;
        }
        public async Task<IActionResult> EditGroupMembers(int grpId, UpdateGroupMembersDTO request)
        {
            var currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            if (currentUserId == null)
            {
                return new NotFoundObjectResult("User not found");
            }

            var IsPartOfGrp = _appDbContext.GroupMembers.FirstOrDefault(u => u.UserId == currentUserId && u.GroupId == grpId);
            if (IsPartOfGrp == null)
            {
                return new UnauthorizedObjectResult("You are not authorized to edit for this group because you are not a part of this group");
            }
            var existingGrpMembers = await GetGroupWithMembers(grpId);
            if (existingGrpMembers == null)
            {
                return new NotFoundObjectResult("Group not found.");
            }
            if (request.MembersToAdd != null && request.MembersToAdd[0] != "string")
            {
                foreach (var memberId in request.MembersToAdd)
                {
                    var presentMembers = existingGrpMembers.GroupMembers.FirstOrDefault(gm => gm.UserId == memberId);
                    if (presentMembers == null)
                    {
                        var timestampNow = DateTime.Now;
                        bool include = request.IncludePreviousChat;
                        var newMember = new GroupMember
                        {
                            UserId = memberId,
                            GroupId = grpId,
                            JoinTime = timestampNow,  // Set the timestamp
                            IncludePreviousChat = include
                        };
                        existingGrpMembers.GroupMembers.Add(newMember);
                        await _appDbContext.SaveChangesAsync();
                    }
                    else
                    {
                        return new OkObjectResult("Member already exist in the group");
                    }

                }
            }
            if (request.MembersToRemove != null && request.MembersToRemove[0] != "string")
            {
                foreach (var memberId in request.MembersToRemove)
                {
                    var currentGrpMembers = await GetGroupWithMembers(grpId);
                    if (currentGrpMembers != null)
                    {
                        var memberToRemove = currentGrpMembers.GroupMembers.FirstOrDefault(gm => gm.UserId == memberId);
                        if (memberToRemove != null)
                        {
                            existingGrpMembers.GroupMembers.Remove(memberToRemove);
                            await _appDbContext.SaveChangesAsync();

                        }
                    }
                    else
                    {
                        return new UnauthorizedObjectResult("You cannot modify members in this group");
                    }
                }
            }
            var result = await GetGroupWithMembers(grpId);
            var response = new AddMemberResDTO
            {
                GroupId = result.GroupId,
                GroupName = result.GroupName,
                GroupMembers = result.GroupMembers
                        .Where(gm => gm.User != null)
                        .Select(gm => gm.User.UserName)
                        .ToList()
            };
            return new OkObjectResult(response);
        }
        public async Task<bool> IsUserMemberOfGroup(string userId, int groupId)
        {
            var isMember = await _appDbContext.GroupMembers
           .AnyAsync(gm => gm.UserId == userId && gm.GroupId == groupId);

            return isMember;
        }
        public async Task<Group> GetGroupByIdAsync(int grpId)
        {
            return await _appDbContext.Groups
                .FirstOrDefaultAsync(g => g.GroupId == grpId);
        }
        public async Task<List<string>> GetGroupMemberIdsAsync(int groupId)
        {
            return await _appDbContext.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => gm.UserId)
                .ToListAsync();
        }
        public async Task<IActionResult> SendMessage(int groupId, GroupMessageRequestDTO messageRequest)
        {
            var currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            var isMemberOfGroup = await IsUserMemberOfGroup(currentUserId, groupId);

            if (!isMemberOfGroup)
            {
                return new UnauthorizedObjectResult("You are not a member of this group.");
            }
            if (string.IsNullOrEmpty(messageRequest.content))
            {
                throw new ArgumentException("Message is required.");
            }
            var group = await GetGroupByIdAsync(groupId);
            if (group == null)
            {
                return new NotFoundObjectResult("Group not found.");
            }

            var memberIds = await GetGroupMemberIdsAsync(groupId);

            foreach (var memberId in memberIds)
            {
                if (currentUserId != memberId)
                {
                    var message = new Messages
                    {
                        Content = messageRequest.content,
                        SenderId = Guid.Parse(currentUserId),
                        ReceiverId = Guid.Parse(memberId),
                        GroupId = groupId,
                        Timestamp = DateTime.Now,
                    };
                    //message.ReceiverId = memberId;
                    //var AddedMessage = _appContext.Messages.Add(message).Entity;
                    await _appDbContext.Messages.AddAsync(message);
                    await _appDbContext.SaveChangesAsync();
                }
            }
            var response = new
            {
                senderId = currentUserId,
                groupId = groupId,
                content = messageRequest.content,
                timestamp = DateTime.Now
            };
            return new OkObjectResult(response);
        }
        public async Task<IActionResult> SendReplyInThread(int groupId, Guid messageId, ThreadMessageDTO messageRequest)
        {
            var currentUserId = await _repositoryService.GetCurrentLoggedInUser();
            var isMemberOfGroup = await IsUserMemberOfGroup(currentUserId, groupId);
            if (!isMemberOfGroup)
            {
                return new UnauthorizedObjectResult("You are not a member of this group so you cannot reply here in thread.");
            }
            return await _msgService.ReplyToMsg(groupId, messageId, messageRequest);
        }
    }
}
