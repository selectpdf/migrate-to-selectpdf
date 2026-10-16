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
await page.SetContentAsync("""
    <h1>Section one</h1>
    <h1 style="break-before: page">Section two</h1>
    <h1 style="break-before: page">Section three</h1>
    """);
// The templates are drawn inside the top and bottom margins; the elements with the classes
// pageNumber and totalPages receive the page numbers.
await page.PdfAsync(output, new PdfOptions
{
    DisplayHeaderFooter = true,
    HeaderTemplate = "<div style=\"font-size:10px; width:100%; text-align:center\">Quarterly report</div>",
    FooterTemplate = "<div style=\"font-size:10px; width:100%; text-align:center\">Page <span class=\"pageNumber\"></span> of <span class=\"totalPages\"></span></div>",
    MarginOptions = new MarginOptions { Top = "25mm", Bottom = "25mm" },
});
