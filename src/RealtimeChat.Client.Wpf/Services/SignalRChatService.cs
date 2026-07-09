using Microsoft.AspNetCore.SignalR.Client;

namespace RealtimeChat.Client.Wpf.Services;

public class SignalRChatService : IAsyncDisposable
{
    private readonly HubConnection _connection;

    /// <summary>
    /// Fired when a message is received. Parameters: username, content, sentAt.
    /// </summary>
    public event Action<string, string, DateTime>? MessageReceived;
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
                var username = json.GetProperty("username").GetString() ?? "Unknown";
                var content = json.GetProperty("content").GetString() ?? string.Empty;
                var sentAt = json.GetProperty("sentAt").GetDateTime();
                MessageReceived?.Invoke(username, content, sentAt);
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
