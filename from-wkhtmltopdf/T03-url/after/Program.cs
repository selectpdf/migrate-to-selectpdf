// URL to PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertUrlToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertUrlToPdf
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// A4 with wkhtmltopdf's default 10 mm margins (28 points)
converter.Options.PdfPageSize = PdfPageSize.A4;
converter.Options.MarginLeft = 28;
converter.Options.MarginRight = 28;
converter.Options.MarginTop = 28;
converter.Options.MarginBottom = 28;

// create a new pdf document converting an url
PdfDocument doc = converter.ConvertUrl("https://example.com/");

// save and close the pdf document
doc.Save(output);
doc.Close();
