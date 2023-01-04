using System.Net;
using System.Net.Sockets;

namespace TCP_server;

public class ServerObject
{
    TcpListener _tcpListener;
    List<ClientObject> _clients = new();
    protected internal void AddConnection(ClientObject clientObject)
    {
        _clients.Add(clientObject);
    }

    protected internal void RemoveConnection(string id)
    {
        var client = _clients.FirstOrDefault(c => c.Id == id);
        if (client != null)
            _clients.Remove(client);
    }

    protected internal void Listen()
    {
        try
        {
            _tcpListener = new TcpListener(IPAddress.Any, 8888);
            _tcpListener.Start();

            Console.WriteLine("Server is started. Waiting for connections...");
            
            while (true)
            {
                var tcpClient = _tcpListener.AcceptTcpClient();

                var clientObject = new ClientObject(tcpClient, this);
                var clientThread = new Thread(clientObject.Process);
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Disconnect();
        }
    }

    protected internal void Disconnect()
    {
        _tcpListener.Stop();

        foreach (var client in _clients)
        {
            client.Close();
        }

        Environment.Exit(0);
    }
}
