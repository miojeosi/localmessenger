using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    public partial class LoginWindow : Window
    {
        private LocalMessengerServer _server;
        private LocalMessengerClient _client;

        public LoginWindow()
        {
            InitializeComponent();
        }


        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string ipAddress = txtIpAddress.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.");
                return;
            }

            try
            {
                var client = new System.Net.Sockets.TcpClient();
                if (rbCreateChat.IsChecked == true)
                {
                    _server = new LocalMessengerServer();
                    _client = new LocalMessengerClient(client);
                    await _client.ConnectAsync(username);
                }
                else if (rbJoinChat.IsChecked == true)
                {
                    _client = new LocalMessengerClient(client, ipAddress);
                    await _client.ConnectAsync(username);
                }


                ChatWindow chatWindow = new ChatWindow(_server, _client, username);
                chatWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while connecting to the chat server.\n" + ex.Message);
            }
        }
    }
}