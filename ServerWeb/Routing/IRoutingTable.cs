using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;

namespace BasicWebServer.Server.Routing;

public interface IRoutingTable
{
    IRoutingTable Map(string url, Method method, Response response);

    IRoutingTable MapGet(string url, Response response);

    IRoutingTable MapPost(string url, Response response);

    Response MatchRequest(Request request);
}
