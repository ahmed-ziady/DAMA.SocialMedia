public interface IConnectionTracker
{
    Task AddConnection(string userId, string connectionId);
    Task RemoveConnection(string userId, string connectionId);
    Task<List<string>> GetConnections(string userId);
}