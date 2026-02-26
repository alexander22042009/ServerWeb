using System.Text;
using BasicWebServer.Server.HTTP;

namespace BasicWebServer.Server.Responses;

public class TextFileResponse : Response
{
    public string FileName { get; }

    public Func<Request, Task>? PrepareResponseAsync { get; set; }

    public TextFileResponse(string fileName)
    {
        this.FileName = fileName;
        this.Headers.Add(Header.ContentType, ContentType.PlainText);
    }

    public override string ToString()
    {
        if (string.IsNullOrEmpty(this.Body) && File.Exists(this.FileName))
        {
            this.Body = File.ReadAllText(this.FileName);
            int length = Encoding.UTF8.GetByteCount(this.Body);
            this.Headers.Add(Header.ContentLength, length.ToString());
            string escapedName = "\"" + this.FileName.Replace("\\", "/").Split('/').Last().Replace("\"", "\\\"") + "\"";
            this.Headers.Add(Header.ContentDisposition, $"attachment; filename={escapedName}");
        }

        return base.ToString();
    }
}
