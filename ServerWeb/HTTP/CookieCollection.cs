using System.Collections;
using BasicWebServer.Server.Common;

namespace BasicWebServer.Server.HTTP;

public class CookieCollection : IEnumerable<Cookie>
{
    private readonly Dictionary<string, Cookie> cookies = new(StringComparer.OrdinalIgnoreCase);

    public Cookie this[string name]
    {
        get => this.cookies[name];
        set => this.cookies[name] = value;
    }

    public void Add(string name, string value) => this.cookies[name] = new Cookie(name, value);

    public void Add(Cookie cookie)
    {
        Guard.AgainstNull(cookie, nameof(cookie));
        this.cookies[cookie.Name] = cookie;
    }

    public bool Contains(string name) => this.cookies.ContainsKey(name);

    public int Count => this.cookies.Count;

    public IEnumerator<Cookie> GetEnumerator() => this.cookies.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
