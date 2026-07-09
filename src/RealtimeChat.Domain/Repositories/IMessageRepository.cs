using RealtimeChat.Domain.Entities;

namespace RealtimeChat.Domain.Repositories;

public interface IMessageRepository
{
    Task AddAsync(Message message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves message history for the specified room.
    /// </summary>
    /// <param name="take">The maximum number of messages to return.</param>
    /// <returns>A list of messages sorted by <see cref="Message.SentAt"/> in descending order (newest first). Returns an empty collection if no messages are found.</returns>
    Task<IEnumerable<Message>> GetHistoryAsync(Guid roomId, int take, CancellationToken cancellationToken = default);
}
