// Haukcode.WkHtmlToPdfDotNet README (GlobalSettings: Orientation, PaperSize, Margins): https://github.com/HakanL/WkHtmlToPdf-DotNet
// The settings map to wkhtmltopdf's --orientation, --page-size and --margin-* options: https://wkhtmltopdf.org/usage/wkhtmltopdf.txt
using WkHtmlToPdfDotNet;

var output = args.Length > 0 ? args[0] : "output.pdf";

var converter = new BasicConverter(new PdfTools());

var doc = new HtmlToPdfDocument
{
    GlobalSettings =
    {
        PaperSize = PaperKind.A4,
        Orientation = Orientation.Landscape,
        Margins = new MarginSettings(20, 20, 20, 20) { Unit = Unit.Millimeters },
    },
    Objects =
    {
        new ObjectSettings
        {
            HtmlContent = "<h1>Hello world</h1>",
            WebSettings = { DefaultEncoding = "utf-8" },
        },
    },
};

File.WriteAllBytes(output, converter.Convert(doc));
