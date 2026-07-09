using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RealtimeChat.Domain.Repositories;
using RealtimeChat.Infrastructure.Data;
using RealtimeChat.Infrastructure.Repositories;

namespace RealtimeChat.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));

        services.AddDbContext<ChatDbContext>(options =>
            options.UseMySql(connectionString, serverVersion)
                   .UseSnakeCaseNamingConvention());

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IChatRoomRepository, ChatRoomRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        services.AddSingleton<RealtimeChat.Application.Users.Queries.IOnlineUserTracker, RealtimeChat.Infrastructure.RealTime.InMemoryOnlineUserTracker>();

        return services;
    }
}
