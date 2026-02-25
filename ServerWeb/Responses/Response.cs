using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class Response
{
    public Response()
    {
        this.StatusCode = StatusCode.OK;
        this.Headers = new HeaderCollection();
        this.Body = string.Empty;
    }

    public StatusCode StatusCode { get; protected set; }

    public HeaderCollection Headers { get; }

    public string Body { get; set; }

    public Action<Request, Response>? PreRenderAction { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}");

        foreach (var header in this.Headers)
        {
            sb.AppendLine(header.ToString());
        }

        sb.AppendLine();

        if (!string.IsNullOrEmpty(this.Body))
        {
            sb.Append(this.Body);
        }

        return sb.ToString();
    }
}
