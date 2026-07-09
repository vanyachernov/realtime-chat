using MediatR;
using RealtimeChat.Application.Common.DTO;

namespace RealtimeChat.Application.Messages.Commands;

/// <summary>
/// Sends a message to a chat room. The sender and room must already exist.
/// </summary>
public record SendMessageCommand : IRequest<MessageDto>
{
    public required Guid SenderId { get; init; }
    public required Guid RoomId { get; init; }

    /// <summary>
    /// Message text. Must not be empty and must not exceed 2000 characters.
    /// </summary>
    public required string Content { get; init; }
}
