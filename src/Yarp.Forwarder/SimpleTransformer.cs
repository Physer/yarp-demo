using Yarp.ReverseProxy.Forwarder;

namespace Yarp.Forwarder;

public class SimpleTransformer : HttpTransformer
{
    public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix)
    {
        await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix);
        proxyRequest.RequestUri = RequestUtilities.MakeDestinationAddress(destinationPrefix, PathString.Empty, QueryString.Empty);
        proxyRequest.Headers.Host = null;
    }
}
