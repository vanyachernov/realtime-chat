using MediatR;
using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Application.Common.DTO;
using RealtimeChat.Application.Messages.Queries;

namespace RealtimeChat.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public MessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Returns message history for the specified room in chronological order (oldest first).
    /// Each element is a <see cref="MessageDto"/> with Id, SenderId, RoomId, Content, and SentAt (UTC).
    /// If the <paramref name="roomId"/> does not match any existing room, an empty list is returned —
    /// no 404 is raised, since an empty room and a non-existent room are semantically equivalent here.
    /// </summary>
    /// <param name="roomId">Unique identifier of the chat room.</param>
    /// <param name="take">Number of most recent messages to retrieve (1–200, default 50).</param>
    /// <returns>A list of <see cref="MessageDto"/> ordered from oldest to newest.</returns>
    [HttpGet("history")]
    public async Task<ActionResult<List<MessageDto>>> GetHistory(
        [FromQuery] Guid roomId,
        [FromQuery] int take = 50)
    {
        var messages = await _mediator.Send(new GetMessageHistoryQuery
        {
            RoomId = roomId,
            Take = take
        });

        return Ok(messages);
    }
}
