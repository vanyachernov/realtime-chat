using MediatR;
using RealtimeChat.Application.Common.DTO;
using RealtimeChat.Domain.Entities;
using RealtimeChat.Domain.Repositories;

namespace RealtimeChat.Application.Messages.Commands;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public SendMessageCommandHandler(IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var message = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = request.SenderId,
            RoomId = request.RoomId,
            Content = request.Content,
            SentAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message, cancellationToken);

        var user = await _userRepository.GetByIdAsync(request.SenderId, cancellationToken);
        var senderName = user?.Username ?? "Unknown";

        return new MessageDto(message.Id, message.SenderId, senderName, message.RoomId, message.Content, message.SentAt);
    }
}
