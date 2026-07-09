using System.Windows;
using RealtimeChat.Client.Wpf.Services;
using RealtimeChat.Client.Wpf.ViewModels;

namespace RealtimeChat.Client.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
    private const string ApiBaseUrl = "http://localhost:5000";
    private const string HubUrl = "http://localhost:5000/chat";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var loginWindow = new LoginWindow();
        var result = loginWindow.ShowDialog();

        if (result != true || string.IsNullOrWhiteSpace(loginWindow.EnteredUsername))
        {
            Shutdown();
            return;
        }

        var username = loginWindow.EnteredUsername;
        var chatService = new SignalRChatService(HubUrl);
        var viewModel = new MainViewModel(chatService, username, ApiBaseUrl);

        var mainWindow = new MainWindow(viewModel);
        mainWindow.Closed += (_, _) => Shutdown();
        mainWindow.Show();
    }
}
