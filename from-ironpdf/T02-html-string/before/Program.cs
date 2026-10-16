// HTML string to PDF.
// Docs: https://ironpdf.com/how-to/html-string-to-pdf/
// This sample is also the Docker sample (../../docker/Dockerfile): https://ironpdf.com/get-started/ironpdf-docker/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

// The two settings IronPDF's Docker guide gives with its apt-get package list: the Dockerfile installs the
// dependencies, so the automatic configuration is off, and GPU acceleration is disabled.
Installation.LinuxAndDockerDependenciesAutoConfig = false;
Installation.ChromeGpuMode = IronPdf.Engines.Chrome.ChromeGpuModes.Disabled;

const string html = """
    <html>
    <body>
      <h1>Quarterly report</h1>
      <table border="1" cellpadding="6">
        <tr><th>Product</th><th>Units</th></tr>
        <tr><td>Alpha</td><td>120</td></tr>
        <tr><td>Beta</td><td>85</td></tr>
        <tr><td>Gamma</td><td>42</td></tr>
      </table>
    </body>
    </html>
    """;

// Instantiate Renderer
var renderer = new ChromePdfRenderer();

// Create a PDF from a HTML string
var pdf = renderer.RenderHtmlAsPdf(html);

// Export to a file
pdf.SaveAs(output);
