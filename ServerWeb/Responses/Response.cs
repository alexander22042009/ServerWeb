using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class Response
{
    public Response(StatusCode statusCode = StatusCode.OK)
    {
        this.StatusCode = statusCode;
        this.Headers = new HeaderCollection();
        this.Body = string.Empty;
        this.Cookies = new CookieCollection();
    }

    public StatusCode StatusCode { get; protected set; }

    public HeaderCollection Headers { get; }

    public string Body { get; set; }

    public CookieCollection Cookies { get; }

    public Action<Request, Response>? PreRenderAction { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

        foreach (var header in this.Headers)
            sb.AppendLine(header.ToString());

        foreach (var cookie in this.Cookies)
            sb.AppendLine($"{Header.SetCookie}: {cookie}");

        sb.AppendLine();

        if (!string.IsNullOrEmpty(this.Body))
            sb.Append(this.Body);

        return sb.ToString();
    }
}
