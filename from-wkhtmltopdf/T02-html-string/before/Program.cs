// Haukcode.WkHtmlToPdfDotNet README (HtmlContent): https://github.com/HakanL/WkHtmlToPdf-DotNet
using WkHtmlToPdfDotNet;

var output = args.Length > 0 ? args[0] : "output.pdf";

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

var converter = new BasicConverter(new PdfTools());

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

File.WriteAllBytes(output, converter.Convert(doc));
