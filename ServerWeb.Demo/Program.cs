using System.Net;
using BasicWebServer.Server;

namespace BasicWebServer.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ip = IPAddress.Parse("127.0.0.1");
            int port = 8085;

            var server = new HttpServer(ip, port);
            server.Start();
        }
    }
}
