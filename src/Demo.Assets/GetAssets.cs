using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Demo.Assets;

public static class GetAssets
{
    [FunctionName(nameof(GetAssets))]
    public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "assets")] HttpRequest req) 
        => new OkObjectResult(new Asset(1, "https://picsum.photos/500/200", DateTime.UtcNow));
}

public record Asset(int Id, string BlobUri, DateTime CreatedAt);
