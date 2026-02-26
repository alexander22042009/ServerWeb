using System.Collections;
using System.Collections.Generic;

namespace BasicWebServer.Server.HTTP;

public class HeaderCollection : IEnumerable<Header>, IEnumerable
{
    private readonly List<Header> headers;

    public HeaderCollection()
    {
        this.headers = new List<Header>();
    }

    public Header this[string name]
    {
        get
        {
            Header? header = this.headers.FirstOrDefault(h => string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase));
            if (header == null)
            {
                throw new InvalidOperationException($"Header '{name}' does not exist.");
            }

            return header;
        }
        set
        {
            Header? existing = this.headers.FirstOrDefault(h => string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase));
            if (existing != null)
            {
                existing.Value = value.Value;
                return;
            }

            this.headers.Add(value);
        }
    }

    public void Add(string name, string value)
        => this[name] = new Header(name, value);

    public bool Contains(string name)
        => this.headers.Any(h => string.Equals(h.Name, name, StringComparison.OrdinalIgnoreCase));

    public int Count => this.headers.Count;

    public IEnumerator<Header> GetEnumerator()
        => this.headers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => this.GetEnumerator();
}
