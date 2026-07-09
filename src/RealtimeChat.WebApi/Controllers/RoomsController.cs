using Microsoft.AspNetCore.Mvc;
using RealtimeChat.Domain.Repositories;

namespace RealtimeChat.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IChatRoomRepository _chatRoomRepository;

    public RoomsController(IChatRoomRepository chatRoomRepository)
    {
        _chatRoomRepository = chatRoomRepository;
    }

    /// <summary>
    /// Returns all available chat rooms. Used by the client to resolve room IDs
    /// for subsequent calls to the message history endpoint.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var rooms = await _chatRoomRepository.GetAllAsync();
        return Ok(rooms.Select(r => new { r.Id, r.Name }));
    }
}
