using System.Net;
using Yarp.Forwarder;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

// Adding YARP's HTTP Forwarder
builder.Services.AddHttpForwarder();

var app = builder.Build();

// Setting up an HTTP Message Invoker for use in the HTTP Forwarder
var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
{
    UseProxy = false,
    AllowAutoRedirect = false,
    AutomaticDecompression = DecompressionMethods.None,
    UseCookies = false
});

// Getting the forwarder from the IoC container
var forwarder = app.Services.GetRequiredService<IHttpForwarder>();

// Creating the necessary objects for our YARP configuration
var simpleTransformer = new SimpleTransformer();
var defaultTransformer = HttpTransformer.Default;
var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

app.UseRouting();

// Mapping three different endpoints to showcase the handling of YARP
app.UseEndpoints(endpoints =>
{
    // The landing endpoint, simply showing a bit of text for the user
    endpoints.Map("/", () => "Please enter a valid route");

    // Everything that has /google in the query string will redirect to the google.com domain
    // Using a custom transformer object
    endpoints.Map("/google/{**catch-all}", async httpContext =>
    {
        var error = await forwarder.SendAsync(httpContext, "http://www.google.com/", httpClient, requestConfig, simpleTransformer);
        if (error == ForwarderError.None)
            return;

        var errorFeature = httpContext.GetForwarderErrorFeature();
        var exception = errorFeature?.Exception;
    });

    // All other requests will be mapped to our API endpoint, which may or may not be able to handle the request
    // Using the default HTTP transformer rules bundled with YARP
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
