// Merge two PDF files.
// Docs: https://ironpdf.com/how-to/merge-or-split-pdfs/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

var merged = PdfDocument.Merge(PdfDocument.FromFile("invoice.pdf"), PdfDocument.FromFile("accessible.pdf"));
merged.SaveAs(output);
