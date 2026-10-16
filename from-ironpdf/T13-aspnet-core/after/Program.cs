// ASP.NET Core minimal API: GET /report returns the T2 document as application/pdf.
// Sample: https://selectpdf.com/demo-universal/ConvertHtmlCodeToPdf
using SelectPdf.Universal;

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

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/report", () =>
{
    // instantiate a html to pdf converter object and convert the html string
    HtmlToPdf converter = new HtmlToPdf();
    PdfDocument doc = converter.ConvertHtmlString(html);

    // save the pdf document to memory, close it and return it
    byte[] pdf = doc.Save();
    doc.Close();
    return Results.File(pdf, "application/pdf", "report.pdf");
});

app.Run();
