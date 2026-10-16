// Hello world: convert an HTML string to a one-page PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

HtmlToPdf converter = new HtmlToPdf();
PdfDocument doc = converter.ConvertHtmlString("<h1>Hello world</h1>");
doc.Save(output);
doc.Close();
