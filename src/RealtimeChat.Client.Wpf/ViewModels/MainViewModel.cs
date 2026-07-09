using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using RealtimeChat.Client.Wpf.Services;

namespace RealtimeChat.Client.Wpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SignalRChatService _chatService;
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;
    private string _currentMessage = string.Empty;
    private string _username = string.Empty;
    private string _connectionStatus = "Disconnected";

    public ObservableCollection<MessageViewModel> Messages { get; } = new();

    public string CurrentMessage
    {
        get => _currentMessage;
        set => SetField(ref _currentMessage, value);
    }

    public string Username
    {
        get => _username;
        set => SetField(ref _username, value);
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set => SetField(ref _connectionStatus, value);
    }

    public ICommand SendCommand { get; }

    public MainViewModel(SignalRChatService chatService, string username, string apiBaseUrl)
    {
        _chatService = chatService;
        _apiBaseUrl = apiBaseUrl.TrimEnd('/');
        _httpClient = new HttpClient();
        Username = username;

        SendCommand = new RelayCommand(
            _ => _ = SendMessageAsync(),
            _ => !string.IsNullOrWhiteSpace(CurrentMessage));

        _chatService.MessageReceived += OnMessageReceived;
        _chatService.UserJoined += OnUserJoined;
        _chatService.Reconnecting += error =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => ConnectionStatus = "Reconnecting...");
        };
        _chatService.Reconnected += connectionId =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => ConnectionStatus = "Connected");
        };
        _chatService.Closed += error =>
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() => ConnectionStatus = "Disconnected");
        };
    }

    public async Task InitializeAsync()
    {
        try
        {
            ConnectionStatus = "Connecting...";
            await _chatService.ConnectAsync();
            await _chatService.JoinRoomAsync(Username);
            ConnectionStatus = "Loading history...";
            await LoadHistoryAsync();
            ConnectionStatus = "Connected";
        }
        catch (Exception ex)
        {
            ConnectionStatus = "Connection Failed";
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(new MessageViewModel("System", $"Could not connect to the chat server. Check your network connection. (Error: {ex.Message})", DateTime.UtcNow));
            });
        }
    }

    private async Task LoadHistoryAsync()
    {
        try
        {
            var roomId = await GetRoomIdAsync();
            if (roomId is null) return;

            var url = $"{_apiBaseUrl}/api/messages/history?roomId={roomId}&take=50";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var messages = JsonSerializer.Deserialize<List<HistoryMessageDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (messages is null) return;

            // Resolve sender usernames from senderId → username mapping.
            // For simplicity, use a cached lookup per unique senderId.
            var usernameLookup = new Dictionary<Guid, string>();

            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                foreach (var m in messages)
                {
                    if (!usernameLookup.TryGetValue(m.SenderId, out var senderName))
                    {
                        senderName = m.SenderId.ToString()[..8];
                        usernameLookup[m.SenderId] = senderName;
                    }

                    Messages.Add(new MessageViewModel(senderName, m.Content, m.SentAt));
                }
            });
        }
        catch (Exception ex)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(new MessageViewModel("System", $"Could not load message history. Check your network connection. (Error: {ex.Message})", DateTime.UtcNow));
            });
        }
    }

    private async Task<Guid?> GetRoomIdAsync()
    {
        try
        {
            var url = $"{_apiBaseUrl}/api/rooms";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var rooms = JsonSerializer.Deserialize<List<RoomDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return rooms?.FirstOrDefault()?.Id;
        }
        catch
        {
            return null;
        }
    }

    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentMessage))
            return;

        var content = CurrentMessage;
        CurrentMessage = string.Empty;

        try
        {
            await _chatService.SendMessageAsync(Username, content);
        }
        catch (Exception ex)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                Messages.Add(new MessageViewModel("System", $"Send failed: {ex.Message}", DateTime.UtcNow));
            });
        }
    }

    private void OnMessageReceived(string username, string content, DateTime sentAt)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Messages.Add(new MessageViewModel(username, content, sentAt));
        });
    }

    private void OnUserJoined(string username)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Messages.Add(new MessageViewModel("System", $"{username} joined the chat", DateTime.UtcNow));
        });
    }

    private record HistoryMessageDto(Guid Id, Guid SenderId, Guid RoomId, string Content, DateTime SentAt);
    private record RoomDto(Guid Id, string Name);
}
