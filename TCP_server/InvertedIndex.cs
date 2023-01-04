namespace TCP_server;

public class InvertedIndexMessage
{
    public string Request;
    public List<string> Answer;

    public InvertedIndexMessage(string request)
    {
        Request = request;
        Answer = new List<string>();
    }
    
}

public class InvertedIndex
{

}