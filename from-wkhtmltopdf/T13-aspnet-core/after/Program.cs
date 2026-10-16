// ASP.NET Core minimal API: GET /report returns the T2 document as application/pdf.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertHtmlCodeToPdf
using SelectPdf.Universal;

const string html = """
    <!DOCTYPE html>
    <html>
    <head><meta charset="utf-8"><title>Quarterly report</title></head>
    <body>
      <h1>Quarterly report</h1>
      <table border="1" cellpadding="6" style="border-collapse: collapse">
        <tr><th>Product</th><th>Units</th></tr>
        <tr><td>Alpha</td><td>120</td></tr>
        <tr><td>Beta</td><td>340</td></tr>
        <tr><td>Gamma</td><td>560</td></tr>
      </table>
    </body>
    </html>
    """;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/report", () =>
{
    // instantiate a html to pdf converter object for this request, as in SelectPdf's sample
    HtmlToPdf converter = new HtmlToPdf();

    // A4 with wkhtmltopdf's default 10 mm margins (28 points)
    converter.Options.PdfPageSize = PdfPageSize.A4;
    converter.Options.MarginLeft = 28;
    converter.Options.MarginRight = 28;
    converter.Options.MarginTop = 28;
    converter.Options.MarginBottom = 28;

    PdfDocument doc = converter.ConvertHtmlString(html);

    // save the pdf document to memory, close it and return it
    byte[] pdf = doc.Save();
    doc.Close();
    return Results.File(pdf, "application/pdf", "report.pdf");
});

app.Run();
