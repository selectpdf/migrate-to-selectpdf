// URL to PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertUrlToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertUrlToPdf
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// create a new pdf document converting an url
PdfDocument doc = converter.ConvertUrl("https://example.com/");

// save and close the pdf document
doc.Save(output);
doc.Close();
