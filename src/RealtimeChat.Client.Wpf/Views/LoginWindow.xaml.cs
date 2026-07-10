using System.Windows;

namespace RealtimeChat.Client.Wpf.Views;

public partial class LoginWindow : Window
{
    public string EnteredUsername { get; private set; } = string.Empty;

    public LoginWindow()
    {
        InitializeComponent();
        UsernameBox.Focus();
    }

    private void JoinButton_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameBox.Text.Trim();
        if (username.Length < 3)
        {
            MessageBox.Show("Username must be at least 3 characters.", "Validation",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        EnteredUsername = username;
        DialogResult = true;
        Close();
    }
}
