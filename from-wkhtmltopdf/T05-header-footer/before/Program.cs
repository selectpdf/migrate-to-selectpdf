// Haukcode.WkHtmlToPdfDotNet README (HeaderSettings): https://github.com/HakanL/WkHtmlToPdf-DotNet
// [page] and [topage] are wkhtmltopdf's header/footer variables ("Footers And Headers"): https://wkhtmltopdf.org/usage/wkhtmltopdf.txt
using WkHtmlToPdfDotNet;

var output = args.Length > 0 ? args[0] : "output.pdf";

const string html = """
    <!DOCTYPE html>
    <html>
    <head><meta charset="utf-8"></head>
    <body>
      <h1>Section 1</h1>
      <h1 style="page-break-before: always">Section 2</h1>
      <h1 style="page-break-before: always">Section 3</h1>
    </body>
    </html>
    """;

var converter = new BasicConverter(new PdfTools());

var doc = new HtmlToPdfDocument
{
    GlobalSettings =
    {
        PaperSize = PaperKind.A4,
        Margins = new MarginSettings { Top = 20, Bottom = 20, Unit = Unit.Millimeters },
    },
    Objects =
    {
        new ObjectSettings
        {
            HtmlContent = html,
            WebSettings = { DefaultEncoding = "utf-8" },
            HeaderSettings = { FontSize = 9, Center = "Quarterly report", Line = true, Spacing = 2.812 },
            FooterSettings = { FontSize = 9, Center = "Page [page] of [topage]" },
        },
    },
};

File.WriteAllBytes(output, converter.Convert(doc));
