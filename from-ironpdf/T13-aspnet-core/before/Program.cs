// ASP.NET Core minimal API: GET /report returns the T2 document as application/pdf.
// Docs: https://ironpdf.com/how-to/html-string-to-pdf/
using IronPdf;

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

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/report", () =>
{
    var renderer = new ChromePdfRenderer();
    using var pdf = renderer.RenderHtmlAsPdf(html);
    return Results.File(pdf.BinaryData, "application/pdf", "report.pdf");
});

app.Run();
