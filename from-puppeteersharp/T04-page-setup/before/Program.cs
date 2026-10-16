// Generate PDF files: https://github.com/hardkoded/puppeteer-sharp#generate-pdf-files
// PdfOptions: https://www.puppeteersharp.com/api/PuppeteerSharp.PdfOptions.html
using PuppeteerSharp;
using PuppeteerSharp.Media;

var output = args.Length > 0 ? args[0] : "output.pdf";

// Downloads Chrome for Testing next to the application (a no-op once it is downloaded).
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.SetContentAsync("<h1>Hello world</h1>");
await page.PdfAsync(output, new PdfOptions
{
    Format = PaperFormat.A4,
    Landscape = true,
    MarginOptions = new MarginOptions { Top = "20mm", Right = "20mm", Bottom = "20mm", Left = "20mm" },
});
