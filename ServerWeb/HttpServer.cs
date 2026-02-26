using System.Net;
using System.Net.Sockets;
using System.Text;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Routing;
using BasicWebServer.Server.Responses;

namespace ServerWeb;

public class HttpServer
{
    private const int RequestSizeLimit = 1024 * 1024;

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

    public async Task StartAsync()
    {
        this.listener.Start();
        Console.WriteLine($"Server started at http://{this.ipAddress}:{this.port}");

        while (true)
        {
            TcpClient client = await this.listener.AcceptTcpClientAsync();
            _ = this.ProcessClientAsync(client);
        }
    }

    private async Task ProcessClientAsync(TcpClient client)
    {
        try
        {
            await using NetworkStream networkStream = client.GetStream();

            string requestText = await ReadRequestAsync(networkStream);
            if (string.IsNullOrWhiteSpace(requestText))
                return;

            Request request = Request.Parse(requestText);
            Response response = this.routingTable.MatchRequest(request);

            if (response is TextFileResponse fileResponse && fileResponse.PrepareResponseAsync != null)
                await fileResponse.PrepareResponseAsync(request);

            response.PreRenderAction?.Invoke(request, response);
            AddSession(request, response);

            await WriteResponseAsync(networkStream, response);
        }
        finally
        {
            client.Dispose();
        }
    }

    private static void AddSession(Request request, Response response)
    {
        if (!request.Session.ContainsKey(Session.CurrentDateKey))
            request.Session[Session.CurrentDateKey] = DateTime.UtcNow.ToString("O");

        response.Cookies.Add(Session.SessionCookieName, request.Session.Id);
    }

    private static async Task<string> ReadRequestAsync(NetworkStream networkStream)
    {
        var buffer = new byte[1024];
        var sb = new StringBuilder();
        int totalBytes = 0;

        do
        {
            int bytesRead = await networkStream.ReadAsync(buffer);
            if (bytesRead == 0)
                break;
            totalBytes += bytesRead;
            if (totalBytes > RequestSizeLimit)
                throw new InvalidOperationException("Request too large.");
            sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
        }
        while (networkStream.DataAvailable);

        return sb.ToString();
    }

    private static async Task WriteResponseAsync(NetworkStream networkStream, Response response)
    {
        string responseText = response.ToString();
        byte[] responseBytes = Encoding.UTF8.GetBytes(responseText);
        await networkStream.WriteAsync(responseBytes);
    }
}
