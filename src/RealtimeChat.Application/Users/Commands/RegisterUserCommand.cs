using MediatR;
using RealtimeChat.Application.Common.DTO;

namespace RealtimeChat.Application.Users.Commands;

/// <summary>
/// Registers a new user. Username must be unique across the system;
/// duplicate usernames will cause a persistence-level error.
/// </summary>
public record RegisterUserCommand : IRequest<UserDto>
{
    /// <summary>
    /// Unique display name. Must be between 3 and 100 characters.
    /// </summary>
    public required string Username { get; init; }
}
