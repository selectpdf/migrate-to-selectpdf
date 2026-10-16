// Generate PDF files: https://github.com/hardkoded/puppeteer-sharp#generate-pdf-files
using PuppeteerSharp;

var output = args.Length > 0 ? args[0] : "output.pdf";

// Downloads Chrome for Testing next to the application (a no-op once it is downloaded).
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com/");
await page.EvaluateExpressionHandleAsync("document.fonts.ready"); // wait for fonts, as in the README
await page.PdfAsync(output);
