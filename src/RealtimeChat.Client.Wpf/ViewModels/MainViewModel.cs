using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RealtimeChat.Client.Wpf.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _currentMessage = string.Empty;
    private string _username = "User";

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

    public ICommand SendCommand { get; }

    public MainViewModel()
    {
        SendCommand = new RelayCommand(
            _ => SendMessage(),
            _ => !string.IsNullOrWhiteSpace(CurrentMessage));
    }

    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(CurrentMessage))
            return;

        var message = new MessageViewModel(Username, CurrentMessage, DateTime.UtcNow);
        Messages.Add(message);
        CurrentMessage = string.Empty;
    }
}
