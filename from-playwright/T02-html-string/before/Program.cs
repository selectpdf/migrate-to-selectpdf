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
    <h1>Quarterly report</h1>
    <table border="1">
      <tr><th>Product</th><th>Units</th></tr>
      <tr><td>Alpha</td><td>120</td></tr>
      <tr><td>Beta</td><td>85</td></tr>
      <tr><td>Gamma</td><td>42</td></tr>
    </table>
    """);
await page.PdfAsync(new() { Path = output });
