using System.Collections.Concurrent;
using RealtimeChat.Application.Users.Queries;

namespace RealtimeChat.Infrastructure.RealTime;

public class InMemoryOnlineUserTracker : IOnlineUserTracker
{
    private readonly ConcurrentDictionary<string, Guid> _connections = new();

    public void AddConnection(string connectionId, Guid userId)
    {
        _connections[connectionId] = userId;
    }

    public void RemoveConnection(string connectionId)
    {
        _connections.TryRemove(connectionId, out _);
    }

    public Task<IReadOnlyList<Guid>> GetOnlineUserIdsAsync(CancellationToken cancellationToken = default)
    {
        var activeUserIds = _connections.Values.Distinct().ToList();
        return Task.FromResult<IReadOnlyList<Guid>>(activeUserIds);
    }
}
