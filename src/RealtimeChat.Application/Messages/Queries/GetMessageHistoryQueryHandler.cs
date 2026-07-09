using MediatR;
using RealtimeChat.Application.Common.DTO;
using RealtimeChat.Domain.Repositories;

namespace RealtimeChat.Application.Messages.Queries;

public class GetMessageHistoryQueryHandler : IRequestHandler<GetMessageHistoryQuery, List<MessageDto>>
{
    private readonly IMessageRepository _messageRepository;

    public GetMessageHistoryQueryHandler(IMessageRepository messageRepository)
    {
        _messageRepository = messageRepository;
    }

    public async Task<List<MessageDto>> Handle(GetMessageHistoryQuery request, CancellationToken cancellationToken)
    {
        var messages = await _messageRepository.GetHistoryAsync(request.RoomId, request.Take, cancellationToken);

        return messages
            .Select(m => new MessageDto(m.Id, m.SenderId, m.RoomId, m.Content, m.SentAt))
            .ToList();
    }
}
