using System.Collections.Concurrent;

namespace DAMA.Infrastructure.Services
{
    public class ConnectionTracker : IConnectionTracker
    {
        private readonly ConcurrentDictionary<string, List<string>> _connections = new();

        public Task AddConnection(string userId, string connectionId)
        {
            _connections.AddOrUpdate(userId,
                [connectionId],
                (_, existing) =>
                {
                    existing.Add(connectionId);
                    return existing;
                });
            return Task.CompletedTask;
        }

        public Task RemoveConnection(string userId, string connectionId)
        {
            if (_connections.TryGetValue(userId, out var connections))
            {
                connections.Remove(connectionId);
                if (connections.Count == 0)
                {
                    _connections.TryRemove(userId, out _);
                }
            }
            return Task.CompletedTask;
        }

        public Task<List<string>> GetConnections(string userId)
        {
            return Task.FromResult(_connections.TryGetValue(userId, out var connections)
                ? connections
                : []);
        }
    }
}