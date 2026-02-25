using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class TextResponse : ContentResponse
{
    public TextResponse(string content, Action<Request, Response>? preRenderAction = null)
        : base(content, ContentType.PlainText, preRenderAction)
    {
    }
}
