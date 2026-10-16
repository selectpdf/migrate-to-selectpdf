// Page size, orientation and margins: A4 landscape with 20 mm margins.
// Docs: https://selectpdf.com/pdf-library/html/PdfPageProperties.htm
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

PdfDocument doc = converter.ConvertHtmlString("<h1>Hello world</h1>");
doc.Save(output);
doc.Close();
