using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Repositories;
using RealtimeChat.Infrastructure.Data;

namespace RealtimeChat.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly ChatDbContext _context;

    public MessageRepository(ChatDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Message message, CancellationToken cancellationToken = default)
    {
        await _context.Messages.AddAsync(message, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Message>> GetHistoryAsync(Guid roomId, int take, CancellationToken cancellationToken = default)
    {
        var messages = await _context.Messages
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .Take(take)
            .ToListAsync(cancellationToken);

        messages.Reverse();
        return messages;
    }
}
