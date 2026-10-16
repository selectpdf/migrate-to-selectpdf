// Haukcode.WkHtmlToPdfDotNet README (basic converter, document, convert): https://github.com/HakanL/WkHtmlToPdf-DotNet
using WkHtmlToPdfDotNet;

var output = args.Length > 0 ? args[0] : "output.pdf";

// BasicConverter is the README's converter for single-threaded applications.
var converter = new BasicConverter(new PdfTools());

var doc = new HtmlToPdfDocument
{
    GlobalSettings = { PaperSize = PaperKind.A4 },
    Objects =
    {
        new ObjectSettings
        {
            HtmlContent = "<h1>Hello world</h1>",
            WebSettings = { DefaultEncoding = "utf-8" },
        },
    },
};

byte[] pdf = converter.Convert(doc);
File.WriteAllBytes(output, pdf);
