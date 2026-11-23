using Microsoft.Playwright;
using NBomber.CSharp;

const string baseUrl = "http://127.0.0.1:5055";

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = true });

var scenario = Scenario.Create("browser_product_search", async context =>
{
    await using var browserContext = await browser.NewContextAsync();
    var page = await browserContext.NewPageAsync();

    var navigation = await Step.Run("load_home_page", context, async () =>
    {
        var response = await page.GotoAsync(baseUrl, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
        await page.GetByRole(AriaRole.Heading, new() { Name = "Performance Demo Shop" }).WaitForAsync();
        await page.GetByRole(AriaRole.Status).GetByText("4 products found").WaitForAsync();
        return response?.Status == 200 ? Response.Ok() : Response.Fail();
    });

    if (navigation.IsError) return Response.Fail();

    var search = await Step.Run("search_mouse", context, async () =>
    {
        await page.GetByRole(AriaRole.Searchbox, new() { Name = "Search products" }).FillAsync("mouse");
        await page.GetByRole(AriaRole.Status).GetByText("1 products found").WaitForAsync();
        var result = page.GetByText("Wireless Mouse", new() { Exact = false });
        return await result.CountAsync() == 1 ? Response.Ok() : Response.Fail();
    });

    return search.IsError ? Response.Fail() : Response.Ok();
})
.WithoutWarmUp()
.WithLoadSimulations(Simulation.KeepConstant(copies: 1, during: TimeSpan.FromSeconds(15)));

NBomberRunner.RegisterScenarios(scenario).Run();
