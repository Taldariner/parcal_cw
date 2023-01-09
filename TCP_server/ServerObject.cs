using System.Diagnostics;
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
            string[] separatingStrings = {".", ",", "<br", " ", ":", ";", "/>", "<br/>", "\"", "?", "!"};
            var folder = @"D:\Учеба\7 семестр\Паралельні обчислення\data\";

            _tcpListener.Start();

            var timer = new Stopwatch();
            timer.Start();
            for (int i = 0; i < 10; i++)
            {
                SearchIndex.BuildIndex(folder, separatingStrings);
            }
            timer.Stop();

            Console.WriteLine("Inverted index built in " + timer.ElapsedMilliseconds/10 + " milliseconds using " +
                              SearchIndex.threadCount.ToString() +
                              " threads. Server is started. Waiting for connections...");

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