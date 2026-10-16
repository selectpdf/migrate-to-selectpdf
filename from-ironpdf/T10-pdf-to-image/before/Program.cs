// Render page 1 of a PDF file to a PNG image at 150 DPI.
// Docs: https://ironpdf.com/how-to/rasterize-pdf-to-images/
using IronPdf;

// the asterisk is replaced by the page number: page-1.png
var pattern = args.Length > 0 ? args[0] : "page-*.png";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

PdfDocument pdf = PdfDocument.FromFile("invoice.pdf");

// Export the first page (zero-based index 0) as PNG at 150 DPI
pdf.RasterizeToImageFiles(pattern, new[] { 0 }, IronPdf.Imaging.ImageType.Png, DPI: 150);
