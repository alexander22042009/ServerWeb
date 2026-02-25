using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;

namespace BasicWebServer.Server.Routing;

public class RoutingTable : IRoutingTable
{
    private readonly Dictionary<Method, Dictionary<string, Response>> routes;

    public RoutingTable()
    {
        this.routes = new()
        {
            { Method.GET, new Dictionary<string, Response>() },
            { Method.POST, new Dictionary<string, Response>() }
        };
    }

    public IRoutingTable Map(string url, Method method, Response response)
    {
        return method switch
        {
            Method.GET => this.MapGet(url, response),
            Method.POST => this.MapPost(url, response),
            _ => throw new InvalidOperationException($"Method '{method}' is not supported.")
        };
    }

    public IRoutingTable MapGet(string url, Response response)
    {
        this.routes[Method.GET][url] = response;
        return this;
    }

    public IRoutingTable MapPost(string url, Response response)
    {
        this.routes[Method.POST][url] = response;
        return this;
    }

    public Response MatchRequest(Request request)
    {
        if (this.routes.TryGetValue(request.Method, out var byPath) &&
            byPath.TryGetValue(request.Path, out var response))
        {
            return response;
        }

        return new NotFoundResponse();
    }
}
