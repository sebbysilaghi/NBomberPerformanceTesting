var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://127.0.0.1:5055");
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

var products = new[]
{
    new Product(1, "Mechanical Keyboard", 79.99m),
    new Product(2, "Wireless Mouse", 29.50m),
    new Product(3, "USB-C Hub", 45.00m),
    new Product(4, "Monitor Stand", 35.00m)
};

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.MapGet("/api/products", (string? q) =>
{
    var matches = products.Where(p => string.IsNullOrWhiteSpace(q) ||
        p.Name.Contains(q, StringComparison.OrdinalIgnoreCase));
    return Results.Ok(matches);
});

app.MapGet("/api/products/{id:int}", (int id) =>
{
    var product = products.FirstOrDefault(p => p.Id == id);
    return product is null ? Results.NotFound() : Results.Ok(product);
});

app.MapPost("/api/quotes", (QuoteRequest quote) =>
{
    if (quote.Quantity is < 1 or > 10)
        return Results.BadRequest(new { error = "Quantity must be between 1 and 10." });

    var product = products.FirstOrDefault(p => p.Id == quote.ProductId);
    if (product is null)
        return Results.NotFound();

    return Results.Ok(new { productId = product.Id, quote.Quantity,
        total = product.Price * quote.Quantity });
});

app.Run();

record Product(int Id, string Name, decimal Price);
record QuoteRequest(int ProductId, int Quantity);
