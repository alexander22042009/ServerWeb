using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class RedirectResponse : Response
{
    public RedirectResponse(string location)
    {
        this.StatusCode = StatusCode.Found;
        this.Headers.Add(Header.Location, location);
    }
}
