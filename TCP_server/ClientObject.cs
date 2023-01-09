using System.Net.Sockets;
using System.Text;

namespace TCP_server;

public class ClientObject
{
    protected internal string Id { get; } = Guid.NewGuid().ToString();
    protected internal NetworkStream Stream { get; private set; }

    string _userName;
    TcpClient _client;
    ServerObject _server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        _client = tcpClient;
        _server = serverObject;
        serverObject.AddConnection(this);
    }

    public void Process()
    {
        try
        {
            Stream = _client.GetStream();
            var message = GetMessage();
            _userName = message;

            message = _userName + " joined to system.";
            Console.WriteLine(message);
            while (true)
            {
                try
                {
                    message = GetMessage();
                    var result = ServerObject.SearchIndex.RequestIndex(Convert.ToString(message));
                    foreach (var answer in result.Answer)
                    {
                        var data = Encoding.Unicode.GetBytes(answer);
                        Stream.Write(data, 0, data.Length);
                    }
                    
                }
                catch
                {
                    message = String.Format($"{_userName}: leaved system");
                    Console.WriteLine(message);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            _server.RemoveConnection(Id);
            Close();
        }
    }

    string GetMessage()
    {
        var data = new byte[64];
        var builder = new StringBuilder();
        do
        {
            var bytes = Stream.Read(data, 0, data.Length);
            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
        } while (Stream.DataAvailable);

        return builder.ToString();
    }

    protected internal void Close()
    {
        Stream.Close();
        _client.Close();
    }
}