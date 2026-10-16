// Haukcode.WkHtmlToPdfDotNet README (ObjectSettings.Page = URL): https://github.com/HakanL/WkHtmlToPdf-DotNet
using WkHtmlToPdfDotNet;

var output = args.Length > 0 ? args[0] : "output.pdf";

var converter = new BasicConverter(new PdfTools());

var doc = new HtmlToPdfDocument
{
    GlobalSettings = { PaperSize = PaperKind.A4 },
    Objects =
    {
        new ObjectSettings { Page = "https://example.com/" },
    },
};

File.WriteAllBytes(output, converter.Convert(doc));
