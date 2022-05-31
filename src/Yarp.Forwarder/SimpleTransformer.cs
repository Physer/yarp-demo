using Yarp.ReverseProxy.Forwarder;

namespace Yarp.Forwarder;

public class SimpleTransformer : HttpTransformer
{
    public override async ValueTask TransformRequestAsync(HttpContext httpContext, HttpRequestMessage proxyRequest, string destinationPrefix)
    {
        // Using the base call to copy over all values of the original request (including headers)
        await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix);

        // Customising the request so that our URI will actually be what we pass in
        proxyRequest.RequestUri = RequestUtilities.MakeDestinationAddress(destinationPrefix, PathString.Empty, QueryString.Empty);
        // Set the Host header to null so we get redirected to Google
        proxyRequest.Headers.Host = null;
    }
}
