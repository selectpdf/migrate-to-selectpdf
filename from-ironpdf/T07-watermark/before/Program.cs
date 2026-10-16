// Watermark every page of an existing PDF.
// Docs: https://ironpdf.com/how-to/custom-watermark/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

PdfDocument pdf = PdfDocument.FromFile("invoice.pdf");

// Apply an HTML watermark to every page, rotated 45 degrees, at 50% opacity
pdf.ApplyWatermark("<h1 style='color:red'>CONFIDENTIAL</h1>", rotation: 45, opacity: 50);

pdf.SaveAs(output);
