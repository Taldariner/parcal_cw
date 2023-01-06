using System.Diagnostics;

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
    Dictionary<string, Dictionary<string, int>> invertedIndex = new();
    Mutex mutex = new();
    public int threadCount = 8;
    
    public void BuildIndex(string directory, string[] separators)
    {
        string pathToFolder = directory;
        string[] separatingStrings = separators;

        var subfolders = new List<string> {@"\test\neg", @"\test\pos", @"\train\neg", @"\train\pos", @"\train\unsup"};
        var allFiles = subfolders.Select(file => Directory.EnumerateFiles(pathToFolder + file)).SelectMany(x => x)
            .ToList();

        
        var threads = new Thread[threadCount];
        for (var i = 0; i < threadCount; i++)
        {
            var threadStart = allFiles.Count / threadCount * i;
            var threadEnd = i == threadCount - 1 ? allFiles.Count : allFiles.Count / threadCount * (i + 1);

            threads[i] =
                new Thread(() => ThreadBuild(allFiles, threadStart, threadEnd, pathToFolder, separatingStrings));
            threads[i].Start();
        }

        for (var i = 0; i < threadCount; i++)
        {
            threads[i].Join();
        }
    }

    void ThreadBuild(List<string> allFiles, int startIndex, int endIndex, string directory, string[] separators)
    {
        for (var i = startIndex; i < endIndex; i++)
        {
            mutex.WaitOne();
            var file = allFiles[i];
            var content = File.ReadAllText(file).ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            AddToIndex(content, file.Replace(directory, ""));
            mutex.ReleaseMutex();
        }
    }

    void AddToIndex(List<string> words, string document)
    {
        foreach (var word in words)
        {
            if (!invertedIndex.ContainsKey(word))
                invertedIndex.Add(word, new Dictionary<string, int> {{document, 1}});
            else if (!invertedIndex[word].ContainsKey(document)) invertedIndex[word].Add(document, 1);
            else invertedIndex[word][document]++;
        }
    }

    public InvertedIndexMessage RequestIndex(string word)
    {
        var timer = new Stopwatch();
        timer.Start();
        
        InvertedIndexMessage requestResult = new InvertedIndexMessage(word);
        requestResult.Answer.Add("Result for world \"" + word + "\"" + ":\n");
        foreach (var result in invertedIndex[word])
        {
            requestResult.Answer.Add(result.Key + " \t- " + result.Value + "\n");
        }

        timer.Stop();
        
        requestResult.Answer.Add("Totally founded: " + (requestResult.Answer.Count - 1).ToString() + " results in " +
                                 timer.ElapsedMilliseconds + " milliseconds.\n");
        return requestResult;
    }

    public Dictionary<string, int> this[string text] =>
        (invertedIndex.ContainsKey(text) ? invertedIndex[text] : null)!;
}