using System.Net;
using System.Net.Sockets;
using System.Text;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;
using BasicWebServer.Server.Responses;

namespace ServerWeb;

public class HttpServer
{
    private readonly IPAddress ipAddress;
    private readonly int port;

    private readonly TcpListener listener;
    private readonly IRoutingTable routingTable;

    public HttpServer(int port, Action<IRoutingTable> routesConfig)
        : this(IPAddress.Loopback, port, routesConfig)
    {
    }

    public HttpServer(IPAddress ipAddress, int port, Action<IRoutingTable> routesConfig)
    {
        this.ipAddress = ipAddress;
        this.port = port;

        this.listener = new TcpListener(this.ipAddress, this.port);

        this.routingTable = new RoutingTable();
        routesConfig(this.routingTable);
    }
    public HttpServer(Action<IRoutingTable> routesConfig)
        : this(IPAddress.Loopback, 8080, routesConfig)
    {
    }

    public void Start()
    {
        this.listener.Start();
        Console.WriteLine($"Server started at http://{this.ipAddress}:{this.port}");

        while (true)
        {
            using TcpClient client = this.listener.AcceptTcpClient();
            using NetworkStream networkStream = client.GetStream();

            string requestText = ReadRequest(networkStream);

            if (string.IsNullOrWhiteSpace(requestText))
            {
                continue;
            }

            Request request = Request.Parse(requestText);

            Response response = this.routingTable.MatchRequest(request);

            response.PreRenderAction?.Invoke(request, response);

            WriteResponse(networkStream, response);
        }
    }

    private static string ReadRequest(NetworkStream networkStream)
    {
        byte[] buffer = new byte[8192];
        int bytesRead = networkStream.Read(buffer, 0, buffer.Length);

        if (bytesRead <= 0)
        {
            return string.Empty;
        }

        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    private static void WriteResponse(NetworkStream networkStream, Response response)
    {
        string responseText = response.ToString();
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);

        networkStream.Write(responseBytes, 0, responseBytes.Length);
    }
}
