using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class UnauthorizedResponse : Response
{
    public UnauthorizedResponse()
    {
        this.StatusCode = StatusCode.Unauthorized;
    }
}
