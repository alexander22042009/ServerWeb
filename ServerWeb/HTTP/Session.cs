namespace BasicWebServer.Server.HTTP;

public class Session
{
    public const string SessionCookieName = "SessionId";
    public const string CurrentDateKey = "CurrentDate";
    public const string UserKey = "UserId";

    private readonly Dictionary<string, string> data = new();

    public Session(string id)
    {
        this.Id = id;
    }

    public string Id { get; }

    public string this[string key]
    {
        get => this.data.TryGetValue(key, out var value) ? value : string.Empty;
        set => this.data[key] = value ?? string.Empty;
    }

    public bool ContainsKey(string key) => this.data.ContainsKey(key);

    public void Clear() => this.data.Clear();

    public void Remove(string key) => this.data.Remove(key);
}
