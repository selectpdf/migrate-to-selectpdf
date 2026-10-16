// URL to PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertUrlToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertUrlToPdf
// Print media: https://selectpdf.com/pdf-library/html/MediaTypes.htm
// Layout width: https://selectpdf.com/pdf-library/html/ResizingContentDuringConversion.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// The settings page.PdfAsync prints with by default: Letter paper, no margins (also SelectPdf's
// default), print CSS media, and the page laid out at the paper width (8.5 in at 96 px per inch = 816 px).
// SelectPdf's own defaults are A4, screen media and a 1024 px layout width scaled to the page.
converter.Options.PdfPageSize = PdfPageSize.Letter;
converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
converter.Options.WebPageWidth = 816;

// create a new pdf document converting an url; the converter waits for the page to load
// (NavigationTimeout, 60 seconds by default: https://selectpdf.com/pdf-library/html/ConversionDelay.htm)
PdfDocument doc = converter.ConvertUrl("https://example.com/");

// save and close the pdf document
doc.Save(output);
doc.Close();
