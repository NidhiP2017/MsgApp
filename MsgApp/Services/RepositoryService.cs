using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MsgApp.Interfaces;
using System.Security.Claims;

namespace MsgApp.Services
{
    public class RepositoryService: IRepositoryService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public RepositoryService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> GetCurrentLoggedInUser()
        {
            string currentUserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (currentUserId == null)
                return null;
            else
                return currentUserId;
        }
    }
}
