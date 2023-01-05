using System.Net;
using System.Net.Sockets;

namespace TCP_server;

public class ServerObject
{
    public static InvertedIndex SearchIndex = new InvertedIndex();
    TcpListener _tcpListener = new TcpListener(IPAddress.Any, 8888);
    List<ClientObject> _clients = new();

    protected internal void AddConnection(ClientObject clientObject)
    {
        _clients.Add(clientObject);
    }

    protected internal void RemoveConnection(string id)
    {
        ClientObject client = _clients.FirstOrDefault(c => c.Id == id);
        if (client != null)
            _clients.Remove(client);
        client?.Close();
    }

    protected internal void Listen()
    {
        try
        {
            _tcpListener.Start();
            SearchIndex.BuildIndex("");
            Console.WriteLine("Server is started. Waiting for connections...");

            while (true)
            {
                TcpClient tcpClient = _tcpListener.AcceptTcpClient();

                ClientObject clientObject = new ClientObject(tcpClient, this);
                Thread clientThread = new Thread(clientObject.Process);
                clientThread.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            Disconnect();
        }
    }

    protected internal void Disconnect()
    {
        foreach (var client in _clients)
        {
            client.Close();
        }

        _tcpListener.Stop();
        Environment.Exit(0);
    }
}