using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Domain.Repositories;

public interface IChatRoomRepository
{
    Task AddAsync(ChatRoom room, CancellationToken cancellationToken = default);

    /// <returns>Returns the chat room if found; otherwise, null.</returns>
    Task<ChatRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ChatRoom>> GetAllAsync(CancellationToken cancellationToken = default);
}
