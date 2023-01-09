using System.Net.Sockets;
using System.Text;

namespace TCP_client;

public class Client
{
    readonly int _port;
    readonly string _name;
    readonly string _server;

    TcpClient _client;
    NetworkStream _stream;

    public Client(string name, string server = "127.0.0.1", int port = 8888)
    {
        _name = name;
        _server = server;
        _port = port;

        _client = new TcpClient();
    }

    public void RunClient()
    {
        try
        {
            _client.Connect(_server, _port);
            _stream = _client.GetStream();

            Console.WriteLine("Welcome to the inverted index search system! You must write word to search in console");
            var username = Encoding.Unicode.GetBytes(_name);
            _stream.Write(username, 0, username.Length);

            var receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start();
            SendMessage();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        finally
        {
            Disconnect();
        }
    }

    void SendMessage()
    {
        while (true)
        {
            var message = Console.ReadLine();
            if (message == "!break")
            {
                break;
            }

            var data = Encoding.Unicode.GetBytes(message);
            _stream.Write(data, 0, data.Length);
        }
    }

    void ReceiveMessage()
    {
        while (true)
        {
            try
            {
                var data = new byte[64];
                var builder = new StringBuilder();
                var bytes = 0;
                do
                {
                    bytes = _stream.Read(data, 0, data.Length);
                    builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                } while (_stream.DataAvailable);

                var message = builder.ToString();
                Console.WriteLine(message);
            }
            catch
            {
                Console.WriteLine("Connection is lose!");
                //Console.ReadLine();
                Disconnect();
            }
        }
    }

    void Disconnect()
    {
        _stream.Close();
        _client.Close();
        Environment.Exit(0);
    }
}