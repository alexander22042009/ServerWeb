namespace BasicWebServer.Server.HTTP;

public class Header
{
    public const string ContentType = "Content-Type";
    public const string ContentLength = "Content-Length";
    public const string Location = "Location";

    public Header(string name, string value)
    {
        this.Name = name;
        this.Value = value;
    }

    public string Name { get; }

    public string Value { get; set; }

    public override string ToString()
        => $"{this.Name}: {this.Value}";
}
