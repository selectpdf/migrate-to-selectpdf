// Extract the text of a PDF file.
// Docs: https://selectpdf.com/pdf-library/html/PdfToTextConverter.htm
// Sample: https://selectpdf.com/demo-universal/PdfToTextConverter
using SelectPdf.Universal;

// instantiate a pdf to text converter object and load the PDF file
PdfToText pdfToText = new PdfToText();
pdfToText.Load("invoice.pdf");

// extract the text
string text = pdfToText.GetText();
pdfToText.Close();

Console.WriteLine(text);
