using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class HtmlResponse : ContentResponse
{
    public HtmlResponse(string content, Action<Request, Response>? preRenderAction = null)
        : base(content, ContentType.Html, preRenderAction)
    {
    }
}
