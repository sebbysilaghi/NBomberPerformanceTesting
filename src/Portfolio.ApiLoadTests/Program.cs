using System.Net.Http.Json;
using NBomber.CSharp;

const string baseUrl = "http://127.0.0.1:5055";
var profile = args.Length > 0 ? args[0].ToLowerInvariant() : "smoke";

using var client = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(10) };

var scenario = Scenario.Create("browse_and_quote_api", async context =>
{
    var list = await Step.Run("GET_products", context, async () =>
    {
        using var response = await client.GetAsync("/api/products?q=mouse");
        var products = await response.Content.ReadFromJsonAsync<Product[]>();
        return response.IsSuccessStatusCode && products?.Length == 1 && products[0].Id == 2
            ? Response.Ok()
            : Response.Fail();
    });

    if (list.IsError) return Response.Fail();

    var quote = await Step.Run("POST_quote", context, async () =>
    {
        using var response = await client.PostAsJsonAsync("/api/quotes", new { productId = 2, quantity = 2 });
        var result = response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<QuoteResult>()
            : null;
        return response.IsSuccessStatusCode && result?.Total == 59.00m
            ? Response.Ok()
            : Response.Fail();
    });

    return quote.IsError ? Response.Fail() : Response.Ok();
})
.WithoutWarmUp()
.WithLoadSimulations(profile switch
{
    "smoke" => Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(5)),
    "steady" => Simulation.KeepConstant(copies: 2, during: TimeSpan.FromSeconds(30)),
    "burst" => Simulation.Inject(rate: 3, interval: TimeSpan.FromSeconds(1), during: TimeSpan.FromSeconds(15)),
    _ => throw new ArgumentException("Profile must be: smoke, steady, or burst")
});

NBomberRunner.RegisterScenarios(scenario).Run();

record Product(int Id, string Name, decimal Price);
record QuoteResult(int ProductId, int Quantity, decimal Total);
