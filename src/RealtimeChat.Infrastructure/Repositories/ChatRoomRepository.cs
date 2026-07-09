using Microsoft.EntityFrameworkCore;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Repositories;
using RealtimeChat.Infrastructure.Data;

namespace RealtimeChat.Infrastructure.Repositories;

public class ChatRoomRepository : IChatRoomRepository
{
    private readonly ChatDbContext _context;

    public ChatRoomRepository(ChatDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ChatRoom room, CancellationToken cancellationToken = default)
    {
        await _context.ChatRooms.AddAsync(room, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChatRoom?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ChatRooms.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<ChatRoom>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ChatRooms.ToListAsync(cancellationToken);
    }
}
