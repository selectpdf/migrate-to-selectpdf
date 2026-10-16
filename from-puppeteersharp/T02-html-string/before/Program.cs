// Generate PDF files: https://github.com/hardkoded/puppeteer-sharp#generate-pdf-files
using PuppeteerSharp;

var output = args.Length > 0 ? args[0] : "output.pdf";

// Downloads Chrome for Testing next to the application (a no-op once it is downloaded).
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
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
await page.PdfAsync(output);
