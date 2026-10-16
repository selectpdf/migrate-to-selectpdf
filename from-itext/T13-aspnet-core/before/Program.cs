// ASP.NET Core minimal API returning the T2 document, converted with pdfHTML into a MemoryStream.
// https://kb.itextpdf.com/itext/chapter-1-hello-html-to-pdf
using iText.Html2pdf;

var app = WebApplication.CreateBuilder(args).Build();

app.MapGet("/report", () =>
{
    var html = """
        <html><body>
          <h1>Quarterly report</h1>
          <table border="1">
            <tr><th>Product</th><th>Units</th></tr>
            <tr><td>Alpha</td><td>120</td></tr>
            <tr><td>Beta</td><td>80</td></tr>
            <tr><td>Gamma</td><td>45</td></tr>
          </table>
        </body></html>
        """;
    var pdf = new MemoryStream();
    HtmlConverter.ConvertToPdf(html, pdf);   // closes the stream; ToArray still works
    return Results.File(pdf.ToArray(), "application/pdf", "report.pdf");
});

app.Run();
