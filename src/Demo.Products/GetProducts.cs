using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace Demo.Products;

public static class GetProducts
{
    [FunctionName(nameof(GetProducts))]
    public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "products")] HttpRequest req)
        => new OkObjectResult(new Product("Bluetooth headset", "An affordable bluetooth headset of excellent quality", 79.99m));
}

public record Product(string Name, string Description, decimal Price);
