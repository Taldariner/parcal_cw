using System.Collections.Concurrent;
using System.Diagnostics;

namespace TCP_server;

public class InvertedIndexMessage
{
    public string Request;
    public int Count;
    public List<string> Answer;

    public InvertedIndexMessage(string request)
    {
        Request = request;
        Count = 0;
        Answer = new List<string>();
    }
}

public class InvertedIndex
{
    ConcurrentDictionary<string, ConcurrentDictionary<string, int>> invertedIndex = new();
    Mutex mutex = new();
    public int threadCount = 4;

    public void BuildIndex(string directory, string[] separators)
    {
        string pathToFolder = directory;
        string[] separatingStrings = separators;

        var subfolders = new List<string> {@"\test\neg", @"\test\pos", @"\train\neg", @"\train\pos", @"\train\unsup"};
        var allFiles = subfolders.Select(file => Directory.EnumerateFiles(pathToFolder + file)).SelectMany(x => x)
            .ToList();
        invertedIndex =
            new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>(Environment.ProcessorCount * 2,
                allFiles.Count);

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
            threads[i].Join();
    }

    void ThreadBuild(List<string> allFiles, int startIndex, int endIndex, string directory, string[] separators)
    {
        for (var i = startIndex; i < endIndex; i++)
        {
            //mutex.WaitOne();
            var file = allFiles[i];
            var content = File.ReadAllText(file).ToLower().Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .ToList();
            AddToIndex(content, file.Replace(directory, ""));
            //mutex.ReleaseMutex();
        }
    }

    void AddToIndex(List<string> words, string document)
    {
        foreach (var word in words)
        {
            if (!invertedIndex.ContainsKey(word))
            {
                invertedIndex.TryAdd(word, new ConcurrentDictionary<string, int>());
                invertedIndex[word].TryAdd(document, 1);
            }
            else if (!invertedIndex[word].ContainsKey(document))
                invertedIndex[word].TryAdd(document, 1);
            else
                invertedIndex[word][document]++;
        }
    }

    public InvertedIndexMessage RequestIndex(string word)
    {
        var timer = new Stopwatch();

        timer.Start();
        InvertedIndexMessage requestResult = new InvertedIndexMessage(word);
        requestResult.Answer.Add("Result for world \"" + word + "\"" + ":\n");
        if (invertedIndex.ContainsKey(word))
        {
            foreach (var result in invertedIndex[word])
            {
                requestResult.Answer.Add(result.Key + " \t- " + result.Value + "\n");
                requestResult.Count += result.Value;
            }
        }

        timer.Stop();

        requestResult.Answer.Add("Totally founded: " + requestResult.Count + " results in " +
                                 timer.ElapsedMilliseconds + " milliseconds.\n");
        return requestResult;
    }
}