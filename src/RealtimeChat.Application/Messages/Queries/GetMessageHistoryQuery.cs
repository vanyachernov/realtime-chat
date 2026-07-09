using MediatR;
using RealtimeChat.Application.Common.DTO;

namespace RealtimeChat.Application.Messages.Queries;

/// <summary>
/// Retrieves the most recent messages for a given room, returned in chronological order (oldest first).
/// </summary>
public record GetMessageHistoryQuery : IRequest<List<MessageDto>>
{
    public required Guid RoomId { get; init; }

    /// <summary>
    /// Maximum number of most recent messages to return. Must be between 1 and 200.
    /// </summary>
    public required int Take { get; init; }
}
