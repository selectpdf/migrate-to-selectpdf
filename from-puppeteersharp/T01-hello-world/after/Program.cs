// Hello world: convert an HTML string to a one-page PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
// Print media: https://selectpdf.com/pdf-library/html/MediaTypes.htm
// Layout width: https://selectpdf.com/pdf-library/html/ResizingContentDuringConversion.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

HtmlToPdf converter = new HtmlToPdf();

// The settings page.pdf (PdfAsync) prints with by default: Letter paper, no margins (also SelectPdf's
// default), print CSS media, and the page laid out at the paper width (8.5 in at 96 px per inch = 816 px).
// SelectPdf's own defaults are A4, screen media and a 1024 px layout width scaled to the page.
converter.Options.PdfPageSize = PdfPageSize.Letter;
converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
converter.Options.WebPageWidth = 816;
// PuppeteerSharp 25 produces tagged PDFs by default (PdfOptions.Tagged = true); SelectPdf's default is false.
converter.Options.Tagged = true;

PdfDocument doc = converter.ConvertHtmlString("<h1>Hello world</h1>");
doc.Save(output);
doc.Close();
