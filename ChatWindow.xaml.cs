using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LocalMessenger
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public LocalMessengerServer _server;
        public LocalMessengerClient _client;
        private string _username;

        public ChatWindow(LocalMessengerServer server, LocalMessengerClient client, string username)
        {
            InitializeComponent();
            _server = server;
            _client = client;
            _username = username;

            if (_client != null)
            {
                _client.MessageReceived += MessageReceived;
                _client.UserListUpdated += UserListUpdated;
            }
            if (_server != null)
            {

                _server.MessageReceived += MessageReceived;
                _server.UserListUpdated += UserListUpdated;
            }
        }

        private void UserListUpdated(object sender, string username)
        {
            Dispatcher.Invoke(() =>
            {
                lstUsers.Items.Add(username);
            });
        }

        private string GetTimestamp()
        {
            return DateTime.Now.ToString("dd/MM HH:mm");
        }

        private void MessageReceived(object sender, string message)
        {
            Dispatcher.Invoke(() =>
            {
                txtChat.AppendText($"[{GetTimestamp()}] {message}\n");
                txtChat.ScrollToEnd();
            });
        }

        private async void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            if (_server != null)
            {
                await _server.DisconnectAsync(_client);
            }
            else if (_client != null)
            {
                await _client.DisconnectAsync(_username);
            }

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private async void Send_Click(object sender, RoutedEventArgs e)
        {
            string message = txtMessage.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Please enter a message to send.");
                return;
            }

            if (_server != null)
            {
                await _server.SendMessageToClientsAsync(_username, message);
            }
            else if (_client != null)
            {
                await _client.SendMessageAsync(_username, message);
            }

            txtMessage.Clear();
        }
    }
}