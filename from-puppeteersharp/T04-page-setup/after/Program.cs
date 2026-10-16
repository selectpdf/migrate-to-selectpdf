// Page size, orientation and margins: A4 landscape with 20 mm margins.
// Docs: https://selectpdf.com/pdf-library/html/PdfPageProperties.htm
// Print media: https://selectpdf.com/pdf-library/html/MediaTypes.htm
// Layout width: https://selectpdf.com/pdf-library/html/ResizingContentDuringConversion.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// set converter options; margins are in points (20 mm = 56.7 pt)
converter.Options.PdfPageSize = PdfPageSize.A4;
converter.Options.PdfPageOrientation = PdfPageOrientation.Landscape;
converter.Options.MarginLeft = 57;
converter.Options.MarginRight = 57;
converter.Options.MarginTop = 57;
converter.Options.MarginBottom = 57;

// Print CSS media, as page.pdf (PdfAsync) uses, and the page laid out at the width between the margins, as a
// browser prints it: 297 mm - 2 x 20 mm = 257 mm, at 96 px per inch = 971 px.
converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
converter.Options.WebPageWidth = 971;
// PuppeteerSharp 25 produces tagged PDFs by default (PdfOptions.Tagged = true); SelectPdf's default is false.
converter.Options.Tagged = true;

PdfDocument doc = converter.ConvertHtmlString("<h1>Hello world</h1>");
// save and close the pdf document
doc.Save(output);
doc.Close();
