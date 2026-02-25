using System.Text;

using BasicWebServer.Server.Responses;

namespace BasicWebServer.Server.HTTP;

public class Request
{
    private Request()
    {
        this.Headers = new HeaderCollection();
        this.Body = string.Empty;
        this.FormData = new Dictionary<string, string>();
    }

    public Method Method { get; private set; }

    public string Path { get; private set; } = "/";

    public HeaderCollection Headers { get; private set; }

    public string Body { get; private set; }

    public Dictionary<string, string> FormData { get; private set; }

    public static Request Parse(string requestText)
    {
        // requestText format:
        // First line: METHOD PATH HTTP/1.1
        // Then headers: Name: Value
        // Empty line
        // Body (optional)
        var request = new Request();

        string[] allLines = requestText.Split("\r\n");

        if (allLines.Length == 0 || string.IsNullOrWhiteSpace(allLines[0]))
        {
            return request;
        }

        // Request line
        string[] requestLineParts = allLines[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (requestLineParts.Length < 2)
        {
            return request;
        }

        request.Method = Enum.TryParse<Method>(requestLineParts[0], out var method)
            ? method
            : Method.GET;

        request.Path = requestLineParts[1];

        // Headers + Body separation
        int emptyLineIndex = Array.IndexOf(allLines, string.Empty);
        if (emptyLineIndex == -1)
        {
            emptyLineIndex = allLines.Length;
        }

        // Parse headers
        for (int i = 1; i < emptyLineIndex; i++)
        {
            string line = allLines[i];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            int colonIndex = line.IndexOf(':');
            if (colonIndex == -1)
            {
                continue;
            }

            string name = line.Substring(0, colonIndex).Trim();
            string value = line.Substring(colonIndex + 1).Trim();

            request.Headers.Add(name, value);
        }

        // Body (everything after the empty line)
        if (emptyLineIndex < allLines.Length - 1)
        {
            string body = string.Join("\r\n", allLines.Skip(emptyLineIndex + 1));
            request.Body = body;
        }

        request.FormData = ParseForm(request.Headers, request.Body);

        return request;
    }

    private static Dictionary<string, string> ParseForm(HeaderCollection headers, string body)
    {
        if (!headers.Contains(Header.ContentType))
        {
            return new Dictionary<string, string>();
        }

        string contentType = headers[Header.ContentType].Value;

        if (!string.Equals(contentType, ContentType.Form, StringComparison.OrdinalIgnoreCase))
        {
            return new Dictionary<string, string>();
        }

        return ParseFormData(body);
    }

    private static Dictionary<string, string> ParseFormData(string body)
    {
        var result = new Dictionary<string, string>();

        if (string.IsNullOrEmpty(body))
        {
            return result;
        }

        string decoded = Uri.UnescapeDataString(body.Replace("+", " "));

        string[] pairs = decoded.Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            string[] kv = pair.Split('=', 2); // split into max 2 parts

            string key = kv[0];
            string value = kv.Length > 1 ? kv[1] : string.Empty;

            if (string.IsNullOrWhiteSpace(key))
            {
                continue;
            }

            result[key] = value;
        }

        return result;
    }
}
