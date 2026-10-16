// URL to PDF.
// Docs: https://ironpdf.com/how-to/url-to-pdf/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

// Instantiate Renderer
var renderer = new ChromePdfRenderer();

// Create a PDF from a URL
var pdf = renderer.RenderUrlAsPdf("https://example.com/");

// Export to a file
pdf.SaveAs(output);
