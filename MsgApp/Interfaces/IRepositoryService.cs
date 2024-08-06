namespace MsgApp.Interfaces
{
    public interface IRepositoryService
    {
        Task<string> GetCurrentLoggedInUser();
    }
}
