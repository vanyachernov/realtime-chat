using MediatR;
using Microsoft.AspNetCore.SignalR;
using RealtimeChat.Application.Common.DTO;
using RealtimeChat.Application.Messages.Commands;
using RealtimeChat.Application.Users.Commands;
using RealtimeChat.Domain.Repositories;

namespace RealtimeChat.WebApi.Hubs;

/// <summary>
/// SignalR hub for real-time chat communication.
/// Clients must listen for the <c>ReceiveMessage</c> event to receive
/// incoming <see cref="MessageDto"/> payloads broadcast to all connected users.
/// </summary>
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    private readonly IUserRepository _userRepository;
    private readonly IChatRoomRepository _chatRoomRepository;

    public ChatHub(IMediator mediator, IUserRepository userRepository, IChatRoomRepository chatRoomRepository)
    {
        _mediator = mediator;
        _userRepository = userRepository;
        _chatRoomRepository = chatRoomRepository;
    }

    /// <summary>
    /// Sends a message to the "General" chat room on behalf of the specified user.
    /// Persists the message via <see cref="SendMessageCommand"/> and then broadcasts
    /// a <see cref="MessageDto"/> to all connected clients via the <c>ReceiveMessage</c> event.
    /// </summary>
    /// <param name="username">Display name of the sender. Must match an existing registered user.</param>
    /// <param name="content">Message text. Must not be empty and must not exceed 2000 characters.</param>
    public async Task SendMessage(string username, string content)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
        {
            throw new HubException($"User '{username}' not found. Register first via JoinRoom.");
        }

        var room = (await _chatRoomRepository.GetAllAsync()).FirstOrDefault();
        if (room is null)
        {
            throw new HubException("No chat room available.");
        }

        var messageDto = await _mediator.Send(new SendMessageCommand
        {
            SenderId = user.Id,
            RoomId = room.Id,
            Content = content
        });

        await Clients.All.SendAsync("ReceiveMessage", new
        {
            username,
            messageDto.Id,
            messageDto.SenderId,
            messageDto.RoomId,
            messageDto.Content,
            messageDto.SentAt
        });
    }

    /// <summary>
    /// Registers a new user (or retrieves an existing one) and adds the connection
    /// to the "General" SignalR group. Creates the "General" room if it does not exist yet.
    /// Broadcasts a <c>UserJoined</c> event with the username to all clients.
    /// </summary>
    /// <param name="username">Unique display name for the joining user (3–100 characters).</param>
    public async Task JoinRoom(string username)
    {
        var existingUser = await _userRepository.GetByUsernameAsync(username);
        if (existingUser is null)
        {
            await _mediator.Send(new RegisterUserCommand { Username = username });
        }

        var rooms = await _chatRoomRepository.GetAllAsync();
        var generalRoom = rooms.FirstOrDefault();

        if (generalRoom is null)
        {
            generalRoom = new Domain.Entities.ChatRoom
            {
                Id = Guid.NewGuid(),
                Name = "General"
            };
            await _chatRoomRepository.AddAsync(generalRoom);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, generalRoom.Name);
        await Clients.All.SendAsync("UserJoined", username);
    }
}
