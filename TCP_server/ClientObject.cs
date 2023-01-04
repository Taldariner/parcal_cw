using System.Net.Sockets;
using System.Text;

namespace TCP_server;

public class ClientObject
{
    protected internal string Id { get; private set; }
    protected internal NetworkStream Stream { get; private set; }
    
    string _userName;
    TcpClient _client;
    ServerObject _server;

    public ClientObject(TcpClient tcpClient, ServerObject serverObject)
    {
        Id = Guid.NewGuid().ToString();
        _client = tcpClient;
        _server = serverObject;
        serverObject.AddConnection(this);
    }

    public void Process()
    {
        Test test = new Test();
        try
        {
            Stream = _client.GetStream();
            var message = GetMessage();
            _userName = message;

            message = _userName + " joined to system";
            Console.WriteLine(message);
            while (test.GetQuestionsCount() != 0)
            {
                try
                {
                    var question = test.PeekQuestion();
                    var data = Encoding.Unicode.GetBytes(question);
                    Stream.Write(data, 0, data.Length);
                    message = GetMessage();
                    test.CheckQuestion(Convert.ToInt32(message));
                }
                catch
                {
                    message = String.Format($"{_userName}: leaved system");
                    Console.WriteLine(message);
                    break;
                }
            }

            //var score = Encoding.Unicode.GetBytes($"Your score is {test.Score}");
            //Stream.Write(score, 0, score.Length);
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
        var bytes = 0;
        do
        {
            bytes = Stream.Read(data, 0, data.Length);
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