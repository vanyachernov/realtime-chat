using Microsoft.AspNetCore.SignalR.Client;

namespace RealtimeChat.Client.Wpf.Services;

public class SignalRChatService : IAsyncDisposable
{
    private readonly HubConnection _connection;

    public event Action<Guid, Guid, Guid, string, DateTime>? MessageReceived;
    public event Action<string>? UserJoined;

    public SignalRChatService(string hubUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<object>("ReceiveMessage", raw =>
        {
            if (raw is System.Text.Json.JsonElement json)
            {
                var id = json.GetProperty("id").GetGuid();
                var senderId = json.GetProperty("senderId").GetGuid();
                var roomId = json.GetProperty("roomId").GetGuid();
                var content = json.GetProperty("content").GetString() ?? string.Empty;
                var sentAt = json.GetProperty("sentAt").GetDateTime();
                MessageReceived?.Invoke(id, senderId, roomId, content, sentAt);
            }
        });

        _connection.On<string>("UserJoined", username =>
        {
            UserJoined?.Invoke(username);
        });
    }

    public async Task ConnectAsync()
    {
        await _connection.StartAsync();
    }

    public async Task JoinRoomAsync(string username)
    {
        await _connection.InvokeAsync("JoinRoom", username);
    }

    public async Task SendMessageAsync(string username, string content)
    {
        await _connection.InvokeAsync("SendMessage", username, content);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}
