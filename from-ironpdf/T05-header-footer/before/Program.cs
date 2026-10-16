// Text header and footer with page numbers, set on the renderer.
// Docs: https://ironpdf.com/how-to/headers-and-footers/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

const string html = """
    <html>
    <body>
      <h1>Section one</h1>
      <div style="page-break-after: always"></div>
      <h1>Section two</h1>
      <div style="page-break-after: always"></div>
      <h1>Section three</h1>
    </body>
    </html>
    """;

// Instantiate renderer
ChromePdfRenderer renderer = new ChromePdfRenderer();

// Create header and add to rendering options
renderer.RenderingOptions.TextHeader = new TextHeaderFooter
{
    CenterText = "Quarterly report",
};

// Create footer with the {page} and {total-pages} placeholders
renderer.RenderingOptions.TextFooter = new TextHeaderFooter
{
    CenterText = "Page {page} of {total-pages}",
};

// Render PDF with header and footer
PdfDocument pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs(output);
