// page.PdfAsync: https://playwright.dev/dotnet/docs/api/class-page#page-pdf
using Microsoft.Playwright;

var output = args.Length > 0 ? args[0] : "output.pdf";

// Installs the Chromium build this Playwright version uses (a no-op once it is installed):
// https://playwright.dev/dotnet/docs/browsers#install-browsers-via-api
var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
if (exitCode != 0)
    throw new Exception($"Playwright exited with code {exitCode}");

using var playwright = await Playwright.CreateAsync();
await using var browser = await playwright.Chromium.LaunchAsync();
var page = await browser.NewPageAsync();
await page.SetContentAsync("<h1>Hello world</h1>");
await page.PdfAsync(new()
{
    Path = output,
    Format = "A4",
    Landscape = true,
    Margin = new() { Top = "20mm", Right = "20mm", Bottom = "20mm", Left = "20mm" },
});
