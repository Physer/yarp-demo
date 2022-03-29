using System.Net;
using Yarp.Forwarder;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpForwarder();
var app = builder.Build();
var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
{
    UseProxy = false,
    AllowAutoRedirect = false,
    AutomaticDecompression = DecompressionMethods.None,
    UseCookies = false
});
var forwarder = app.Services.GetRequiredService<IHttpForwarder>();
var simpleTransformer = new SimpleTransformer();
var defaultTransformer = HttpTransformer.Default;
var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    // Landing
    endpoints.Map("/", () => "Please enter a valid route");

    // Google
    endpoints.Map("/google/{**catch-all}", async httpContext =>
    {
        var error = await forwarder.SendAsync(httpContext, "http://www.google.com/", httpClient, requestConfig, simpleTransformer);
        if (error == ForwarderError.None)
            return;

        var errorFeature = httpContext.GetForwarderErrorFeature();
        var exception = errorFeature?.Exception;
    });

    // API
    endpoints.Map("/{**catch-all}", async httpContext =>
    {
        var error = await forwarder.SendAsync(httpContext, "https://localhost:7013/", httpClient, requestConfig, defaultTransformer);
        if (error == ForwarderError.None)
            return;

        var errorFeature = httpContext.GetForwarderErrorFeature();
        var exception = errorFeature?.Exception;
    });
});

app.Run();
