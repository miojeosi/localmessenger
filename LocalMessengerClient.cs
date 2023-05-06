using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;

public class LocalMessengerClient
{
    private readonly string _serverAddress;
    private readonly int _port;
    private System.Net.Sockets.TcpClient _client;
    private CancellationTokenSource _cts;
    public event EventHandler<string> MessageReceived;
    public event UserListUpdatedEventHandler UserListUpdated;
    public delegate void UserListUpdatedEventHandler(object sender, string username);
    private NetworkStream _networkStream;
    
    


    public LocalMessengerClient(TcpClient client, string serverAddress = "localhost")
    {
        _client = client;
        _port = 123;
        _serverAddress = serverAddress;
        _cts = new CancellationTokenSource();
    }


    public async Task DisconnectAsync(string username)
    {
        string message = $"{username} покинул чат.";
        await SendMessageAsync("server", message);

        _client.GetStream().Close();
        _client.Close();
        _cts.Cancel();
    }


    public async Task ConnectAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            await _client.ConnectAsync(_serverAddress, _port, cancellationToken);
            _networkStream = _client.GetStream();
            UserListUpdated?.Invoke(this, username);
            using (var writer = new StreamWriter(_networkStream, Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
            {
                await writer.WriteLineAsync(username);
            }

            _ = Task.Run(() => ListenForMessagesAsync(_cts.Token));
            _ = Task.Run(() => ListenForUsersUpdatesAsync(_cts.Token));
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while connecting to the server.", ex);
        }
    }


    public NetworkStream GetStream()
    {
        return _client.GetStream();
    }


    private async Task ListenForMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using (var stream = _client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string message;
                while (!cancellationToken.IsCancellationRequested && (message = await reader.ReadLineAsync()) != null)
                {
                    MessageReceived?.Invoke(this, message);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async Task ListenForUsersUpdatesAsync(CancellationToken cancellationToken)
    {
        try
        {
            using (var stream = _client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                string user;
                while (!cancellationToken.IsCancellationRequested && (user = await reader.ReadLineAsync()) != null)
                {
                    if (user.StartsWith("Добро пожаловать,")) {
                        UserListUpdated?.Invoke(this, user);
                    }
                    
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }


    public async Task SendMessageAsync(string username, string message)
    {
        string formattedMessage = $"{username}: {message}";
        if (_client != null && _client.Connected)
        {
            using (var writer = new StreamWriter(_client.GetStream(), Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
            {
                await writer.WriteLineAsync(formattedMessage);
            }
        }
        else {
            MessageBox.Show("Some clients are null");
        }
    }

    public async Task LoginUserAsync(string username)
    {
        string formattedMessage = $"Добро пожаловать, {username}!";
        if (_client != null && _client.Connected)
        {
            using (var writer = new StreamWriter(_client.GetStream(), Encoding.UTF8, leaveOpen: true) { AutoFlush = true })
            {
                await writer.WriteLineAsync(formattedMessage);
            }
        }
        else
        {
            MessageBox.Show("Some clients are null");
        }
    }
}
