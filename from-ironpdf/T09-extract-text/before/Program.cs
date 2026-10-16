// Extract the text of a PDF file.
// Docs: https://ironpdf.com/how-to/extract-text-and-images/
using IronPdf;

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

PdfDocument pdf = PdfDocument.FromFile("invoice.pdf");

// Extract text
string text = pdf.ExtractAllText();

Console.WriteLine(text);
