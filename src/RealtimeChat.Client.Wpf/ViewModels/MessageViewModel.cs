namespace RealtimeChat.Client.Wpf.ViewModels;

public class MessageViewModel : ViewModelBase
{
    public string Username { get; }
    public string Content { get; }
    public string Timestamp { get; }

    public MessageViewModel(string username, string content, DateTime sentAt)
    {
        Username = username;
        Content = content;
        Timestamp = sentAt.ToLocalTime().ToString("HH:mm");
    }
}
