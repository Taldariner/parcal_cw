using TCP_server;

ServerObject server = new ServerObject();
try
{
    Thread listenThread = new Thread(server.Listen);
    listenThread.Start();
}
catch (Exception e)
{
    server.Disconnect();
    Console.WriteLine(e);
}