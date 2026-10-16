// Hello world: render an HTML string to a one-page PDF.
// Docs: https://ironpdf.com/how-to/html-string-to-pdf/
// Licence key: https://ironpdf.com/get-started/license-keys/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

// Instantiate Renderer
var renderer = new ChromePdfRenderer();

// Create a PDF from a HTML string
var pdf = renderer.RenderHtmlAsPdf("<h1>Hello world</h1>");

// Export to a file
pdf.SaveAs(output);
