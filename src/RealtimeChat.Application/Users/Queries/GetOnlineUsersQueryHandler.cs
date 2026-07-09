using MediatR;
using RealtimeChat.Application.Common.DTO;

namespace RealtimeChat.Application.Users.Queries;

/// <summary>
/// Abstraction for tracking which users are currently online.
/// The concrete implementation lives in Infrastructure (e.g., in-memory dictionary backed by SignalR events).
/// </summary>
public interface IOnlineUserTracker
{
    void AddConnection(string connectionId, Guid userId);
    void RemoveConnection(string connectionId);
    Task<IReadOnlyList<Guid>> GetOnlineUserIdsAsync(CancellationToken cancellationToken = default);
}

public class GetOnlineUsersQueryHandler : IRequestHandler<GetOnlineUsersQuery, List<UserDto>>
{
    private readonly IOnlineUserTracker _onlineTracker;
    private readonly Domain.Repositories.IUserRepository _userRepository;

    public GetOnlineUsersQueryHandler(
        IOnlineUserTracker onlineTracker,
        Domain.Repositories.IUserRepository userRepository)
    {
        _onlineTracker = onlineTracker;
        _userRepository = userRepository;
    }

    public async Task<List<UserDto>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        var onlineIds = await _onlineTracker.GetOnlineUserIdsAsync(cancellationToken);
        var result = new List<UserDto>();

        foreach (var id in onlineIds)
        {
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            if (user is not null)
            {
                result.Add(new UserDto(user.Id, user.Username, user.CreatedAt));
            }
        }

        return result;
    }
}
