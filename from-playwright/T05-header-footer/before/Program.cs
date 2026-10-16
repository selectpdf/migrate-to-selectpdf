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
await page.SetContentAsync("""
    <h1>Section one</h1>
    <h1 style="break-before: page">Section two</h1>
    <h1 style="break-before: page">Section three</h1>
    """);
// The templates are drawn inside the top and bottom margins; the elements with the classes
// pageNumber and totalPages receive the page numbers.
await page.PdfAsync(new()
{
    Path = output,
    DisplayHeaderFooter = true,
    HeaderTemplate = "<div style=\"font-size:10px; width:100%; text-align:center\">Quarterly report</div>",
    FooterTemplate = "<div style=\"font-size:10px; width:100%; text-align:center\">Page <span class=\"pageNumber\"></span> of <span class=\"totalPages\"></span></div>",
    Margin = new() { Top = "25mm", Bottom = "25mm" },
});
