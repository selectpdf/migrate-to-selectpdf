// Save an existing PDF as PDF/A-2b.
// Docs: https://ironpdf.com/how-to/pdfa/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

// Load an existing PDF
PdfDocument pdf = PdfDocument.FromFile("invoice.pdf");

// Save as PDF/A-2b compliant document
pdf.SaveAsPdfA(output, PdfAVersions.PdfA2b);
