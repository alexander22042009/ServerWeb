using System.Net;
using BasicWebServer.Server.HTTP;
using BasicWebServer.Server.Responses;
using BasicWebServer.Server.Routing;
using ServerWeb;

namespace BasicWebServer.Demo;

internal static class Program
{
    private const string ContentFileName = "content.txt";
    private const string ValidUsername = "user";
    private const string ValidPassword = "user123";

    private static readonly string[] DownloadUrls =
    {
        "https://judge.softuni.org/",
        "https://softuni.org/"
    };

    private const string FormHtml = """
        <form action='/HTML' method='POST'>
            <label>Name: <input type='text' name='Name' /></label><br/>
            <label>Age: <input type='number' name='Age' /></label><br/>
            <button type='submit'>Submit</button>
        </form>
        """;

    private const string DownloadFormHtml = """
        <form action='/Content' method='POST'>
            <button type='submit'>Download Sites Content</button>
        </form>
        """;

    private const string LoginFormHtml = """
        <form action='/Login' method='POST'>
            <label>Username: <input type='text' name='Username' /></label><br/>
            <label>Password: <input type='password' name='Password' /></label><br/>
            <button type='submit'>Login</button>
        </form>
        """;

    static async Task Main(string[] args)
    {
        var server = new HttpServer(ConfigureRoutes);
        await server.StartAsync();
    }

    private static void ConfigureRoutes(IRoutingTable routing)
    {
        routing.MapGet("/", new HtmlResponse("<h1>Welcome!</h1><p>Basic Web Server.</p>"))
            .MapGet("/Text", new TextResponse("Hello from text route."))
            .MapGet("/HTML", new HtmlResponse(FormHtml))
            .MapPost("/HTML", new HtmlResponse("", AddFormDataAction))
            .MapGet("/Redirect", new RedirectResponse("https://softuni.org/"))
            .MapGet("/Content", new HtmlResponse(DownloadFormHtml))
            .MapPost("/Content", CreateContentResponse())
            .MapGet("/Cookies", new HtmlResponse("", AddCookiesAction))
            .MapGet("/Session", new TextResponse("", DisplaySessionInfoAction))
            .MapGet("/Login", new HtmlResponse(LoginFormHtml))
            .MapPost("/Login", new HtmlResponse("", LoginAction))
            .MapGet("/Logout", new HtmlResponse("", LogoutAction))
            .MapGet("/UserProfile", new HtmlResponse("", GetUserDataAction));
    }

    private static Response CreateContentResponse()
    {
        var response = new TextFileResponse(ContentFileName);
        response.PrepareResponseAsync = async _ =>
            await DownloadSitesAsTextFile(ContentFileName, DownloadUrls);
        return response;
    }

    private static void AddFormDataAction(Request request, Response response)
    {
        response.Body = "";
        foreach (var (key, value) in request.FormData)
            response.Body += $"{key}: {value}\r\n";
        if (string.IsNullOrEmpty(response.Body))
            response.Body = "No form data submitted.";
    }

    private static void AddCookiesAction(Request request, Response response)
    {
        bool hasCustomCookies = request.Cookies.Count > 0 &&
            (request.Cookies.Count > 1 || !request.Cookies.Contains(Session.SessionCookieName));

        if (!hasCustomCookies)
        {
            response.Cookies.Add("My-Cookie", "My-Value");
            response.Cookies.Add("My-Cookie-2", "My-Value-2");
            response.Body = "<p>Cookies set! Refresh the page.</p>";
        }
        else
        {
            response.Body = "<p>Cookies received:</p><ul>";
            foreach (var cookie in request.Cookies)
                response.Body += $"<li><b>{cookie.Name}</b>: {cookie.Value}</li>";
            response.Body += "</ul>";
        }
    }

    private static void DisplaySessionInfoAction(Request request, Response response)
    {
        string dateStored = request.Session[Session.CurrentDateKey];
        response.Body = string.IsNullOrEmpty(dateStored)
            ? "Current date stored!"
            : $"Session created at: {dateStored}";
    }

    private static void LoginAction(Request request, Response response)
    {
        request.Session.Clear();
        string username = request.FormData.GetValueOrDefault("Username", "");
        string password = request.FormData.GetValueOrDefault("Password", "");

        if (username == ValidUsername && password == ValidPassword)
        {
            request.Session[Session.UserKey] = username;
            response.Body = "<p>Login successful!</p><a href='/UserProfile'>Profile</a>";
        }
        else
        {
            response.Body = LoginFormHtml + "<p style='color:red'>Invalid credentials.</p>";
        }
    }

    private static void LogoutAction(Request request, Response response)
    {
        request.Session.Remove(Session.UserKey);
        response.Body = "<p>Logged out.</p><a href='/Login'>Login</a>";
    }

    private static void GetUserDataAction(Request request, Response response)
    {
        if (request.Session.ContainsKey(Session.UserKey))
        {
            string user = request.Session[Session.UserKey];
            response.Body = $"<h2>Welcome, {user}!</h2><a href='/Logout'>Logout</a>";
        }
        else
        {
            response.Body = "<p>Please <a href='/Login'>login</a> first.</p>";
        }
    }

    private static async Task<string> DownloadWebSiteContent(string url)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        string content = await client.GetStringAsync(url);
        return content.Length > 2000 ? content[..2000] : content;
    }

    private static async Task DownloadSitesAsTextFile(string fileName, string[] urls)
    {
        var tasks = urls.Select(DownloadWebSiteContent).ToArray();
        string[] results = await Task.WhenAll(tasks);
        string combined = string.Join(Environment.NewLine + new string('-', 80) + Environment.NewLine, results);
        await File.WriteAllTextAsync(fileName, combined);
    }
}
