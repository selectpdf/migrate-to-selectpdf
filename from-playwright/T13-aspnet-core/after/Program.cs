// ASP.NET Core minimal API: GET /report returns the T2 document as application/pdf.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertHtmlCodeToPdf
// Print media: https://selectpdf.com/pdf-library/html/MediaTypes.htm
// Layout width: https://selectpdf.com/pdf-library/html/ResizingContentDuringConversion.htm
using SelectPdf.Universal;

const string html = """
    <h1>Quarterly report</h1>
    <table border="1">
      <tr><th>Product</th><th>Units</th></tr>
      <tr><td>Alpha</td><td>120</td></tr>
      <tr><td>Beta</td><td>85</td></tr>
      <tr><td>Gamma</td><td>42</td></tr>
    </table>
    """;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/report", () =>
{
    // instantiate a html to pdf converter object for this request
    HtmlToPdf converter = new HtmlToPdf();
    // The settings page.PdfAsync prints with by default: Letter paper, no margins (also SelectPdf's
    // default), print CSS media, and the page laid out at the paper width (8.5 in at 96 px per inch = 816 px).
    // SelectPdf's own defaults are A4, screen media and a 1024 px layout width scaled to the page.
    converter.Options.PdfPageSize = PdfPageSize.Letter;
    converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
    converter.Options.WebPageWidth = 816;

    PdfDocument doc = converter.ConvertHtmlString(html);

    // save the pdf document to memory, close it and return it
    byte[] pdf = doc.Save();
    doc.Close();
    return Results.File(pdf, "application/pdf", "report.pdf");
});

app.Run();
