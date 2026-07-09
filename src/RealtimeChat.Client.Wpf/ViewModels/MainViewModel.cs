using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RealtimeChat.Client.Wpf.Services;

namespace RealtimeChat.Client.Wpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private readonly SignalRChatService _chatService;
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

    public MainViewModel(SignalRChatService chatService, string username)
    {
        _chatService = chatService;
        Username = username;

        SendCommand = new RelayCommand(
            _ => _ = SendMessageAsync(),
            _ => !string.IsNullOrWhiteSpace(CurrentMessage));

        _chatService.MessageReceived += OnMessageReceived;
        _chatService.UserJoined += OnUserJoined;
    }

    public async Task InitializeAsync()
    {
        try
        {
            ConnectionStatus = "Connecting...";
            await _chatService.ConnectAsync();
            await _chatService.JoinRoomAsync(Username);
            ConnectionStatus = "Connected";
        }
        catch (Exception ex)
        {
            ConnectionStatus = $"Error: {ex.Message}";
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

    private void OnMessageReceived(Guid id, Guid senderId, Guid roomId, string content, DateTime sentAt)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Messages.Add(new MessageViewModel(Username, content, sentAt));
        });
    }

    private void OnUserJoined(string username)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            Messages.Add(new MessageViewModel("System", $"{username} joined the chat", DateTime.UtcNow));
        });
    }
}
