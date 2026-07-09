using MediatR;
using RealtimeChat.Application.Common.DTO;

namespace RealtimeChat.Application.Users.Queries;

/// <summary>
/// Returns a list of users currently connected to the chat.
/// The "online" state is determined by the infrastructure layer (e.g., SignalR connection tracker).
/// </summary>
public record GetOnlineUsersQuery : IRequest<List<UserDto>>;
