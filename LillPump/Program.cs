using System.Net;
using System.Net.Sockets;
using SampleMessageContract;

const int port = 8080;

var server = new TcpListener(IPAddress.Any, port);

Console.WriteLine("Server started on port {0}", port);
Console.WriteLine("Waiting for a connection...");
//accept a single client
server.Start();
var client = await server.AcceptTcpClientAsync();
Console.WriteLine("Client connected From {0}", client.Client.RemoteEndPoint);

var sharedStream = new SharedStream(client.GetStream());
var helloWorldMessagePost = new HelloWorldMessagePost(sharedStream);

helloWorldMessagePost.Recevied += (data) =>
{
    Console.WriteLine($"Received message {data.Version}");
};
helloWorldMessagePost.EnableReader = true;

var run = true;



helloWorldMessagePost.Write(new HandshakeMessageData(1));



while (run)
{
}


client.Close();
server.Stop();