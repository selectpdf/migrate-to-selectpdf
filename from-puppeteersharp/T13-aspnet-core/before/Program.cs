// Generate PDF files: https://github.com/hardkoded/puppeteer-sharp#generate-pdf-files
using PuppeteerSharp;

// Downloads Chrome for Testing next to the application (a no-op once it is downloaded).
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();

// One browser for the application; a new page per request.
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });

var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/report", async () =>
{
    await using var page = await browser.NewPageAsync();
    await page.SetContentAsync("""
        <h1>Quarterly report</h1>
        <table border="1">
          <tr><th>Product</th><th>Units</th></tr>
          <tr><td>Alpha</td><td>120</td></tr>
          <tr><td>Beta</td><td>85</td></tr>
          <tr><td>Gamma</td><td>42</td></tr>
        </table>
        """);
    var pdf = await page.PdfDataAsync();
    return Results.File(pdf, "application/pdf", "report.pdf");
});

await app.RunAsync();
