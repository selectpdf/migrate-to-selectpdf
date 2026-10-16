// Haukcode.WkHtmlToPdfDotNet README (synchronized converter, dependency injection): https://github.com/HakanL/WkHtmlToPdf-DotNet
// The native library is not thread-safe: web servers use SynchronizedConverter, registered as a singleton,
// which runs every conversion on one dedicated thread.
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
var app = builder.Build();

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

app.MapGet("/report", (IConverter converter) =>
{
    var doc = new HtmlToPdfDocument
    {
        GlobalSettings = { PaperSize = PaperKind.A4 },
        Objects =
        {
            new ObjectSettings
            {
                HtmlContent = html,
                WebSettings = { DefaultEncoding = "utf-8" },
            },
        },
    };

    return Results.File(converter.Convert(doc), "application/pdf", "report.pdf");
});

app.Run();
