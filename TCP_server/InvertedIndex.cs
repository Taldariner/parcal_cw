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
    public InvertedIndex()
    {
        
    }

    public void BuildIndex(string directory)
    {
        
    }

    public InvertedIndexMessage RequestIndex(string word)
    {
        InvertedIndexMessage RequestResult = new InvertedIndexMessage(word);
        RequestResult.Answer.Add("Result for world \"" + word + "\"");
        return RequestResult;
    }
}