using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class ContentResponse : Response
{
    public ContentResponse(
        string content,
        string contentType,
        Action<Request, Response>? preRenderAction = null)
    {
        this.Body = content;
        this.Headers.Add(Header.ContentType, contentType);
        this.PreRenderAction = preRenderAction;
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(this.Body))
        {
            int length = Encoding.UTF8.GetByteCount(this.Body);
            this.Headers.Add(Header.ContentLength, length.ToString());
        }

        return base.ToString();
    }
}
