// Hello world: convert an HTML string to a one-page PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

HtmlToPdf converter = new HtmlToPdf();

// A4 is SelectPdf's default page size, as it is wkhtmltopdf's. SelectPdf's margins default to 0;
// wkhtmltopdf's default to 10 mm, which is 28 points (margins are in points, 1/72 inch).
converter.Options.PdfPageSize = PdfPageSize.A4;
converter.Options.MarginLeft = 28;
converter.Options.MarginRight = 28;
converter.Options.MarginTop = 28;
converter.Options.MarginBottom = 28;

PdfDocument doc = converter.ConvertHtmlString("<h1>Hello world</h1>");
doc.Save(output);
doc.Close();
