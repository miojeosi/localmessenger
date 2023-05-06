using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using System.Windows;

namespace LocalMessenger { 
public class LocalMessengerServer
{
    public TcpListener _listener;
    private readonly int _port;
    private CancellationTokenSource _cts;
    private ConcurrentDictionary<string, LocalMessengerClient> _clients;
    public event EventHandler<string> MessageReceived;
    public event UserListUpdatedEventHandler UserListUpdated;
    public delegate void UserListUpdatedEventHandler(object sender, string username);

    public LocalMessengerServer()
    {
        _port = 123;
        _cts = new CancellationTokenSource();
        _clients = new ConcurrentDictionary<string, LocalMessengerClient>();
        _listener = new TcpListener(IPAddress.Any, _port);
        _listener.Start();
        Task.Run(async () => await AcceptClientsAsync());
    }


    public async Task SendMessageToClientsAsync(string sender, string message)
    {
        if (_clients.Any())
        {
            var tasks = new List<Task>();
            foreach (var client in _clients.Values)
            {
                tasks.Add(client.SendMessageAsync(sender, message));
            }
            await Task.WhenAll(tasks);
        }
    }

    private async Task HandleClientAsync(System.Net.Sockets.TcpClient client, CancellationToken cancellationToken)
    {
        string clientName = null;

        try
        {
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            using (var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
            {
                clientName = await reader.ReadLineAsync();
                if (clientName != null)
                {
                    var customTcpClient = new LocalMessengerClient(client);
                    if (!_clients.TryAdd(clientName, customTcpClient))
                    {
                        SendMessageToClientsAsync("server", $"Пользователь {clientName} уже существует");              
                        customTcpClient.DisconnectAsync(clientName);
                        Application.Current.Shutdown();
                        return;
                    }
                    
                    await ProcessClientMessagesAsync(clientName, reader, writer);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            if (clientName != null)
            {
                // Удалить клиента из списка и уведомить всех об отключении
                _clients.TryRemove(clientName, out _);
                SendMessageToClientsAsync("server",$"{clientName} покинул чат.");
            }
        }
    }


    private async Task ProcessClientMessagesAsync(string clientName, StreamReader reader, StreamWriter writer)
    {
        string message;
        while ((message = await reader.ReadLineAsync()) != null)
        {
            SendMessageToClientsAsync(message, clientName);
        }
    }


    private async Task AcceptClientsAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                HandleClientAsync(client, _cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!\n" + ex.Message);
        }
        }
    }

    public async Task DisconnectAsync(LocalMessengerClient client)
    {
        if (client == null)
        {
            return;
        }

        string username = _clients.FirstOrDefault(x => x.Value == client).Key;
        if (string.IsNullOrEmpty(username))
        {
            client.GetStream().Dispose();
            return;
        }

        string message = $"{username} покинул чат.";
        await SendMessageToClientsAsync("server", message);

        _clients.TryRemove(username, out _);
    }

}
}