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
var transformer = new SimpleTransformer();
var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.Map("/", () => "Please enter a valid route");
    endpoints.Map("/google/{**catch-all}", async httpContext =>
    {
        var error = await forwarder.SendAsync(httpContext, "http://www.google.com/",
            httpClient, requestConfig, transformer);

        if (error != ForwarderError.None)
        {
            var errorFeature = httpContext.GetForwarderErrorFeature();
            var exception = errorFeature?.Exception;
        }
    });
});

app.Run();
