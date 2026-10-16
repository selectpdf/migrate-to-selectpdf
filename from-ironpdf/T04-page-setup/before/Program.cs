// Page size, orientation and margins: A4 landscape with 20 mm margins.
// Docs: https://ironpdf.com/how-to/custom-paper-size/
//       https://ironpdf.com/how-to/page-orientation-rotation/
//       https://ironpdf.com/how-to/custom-margins/
using IronPdf;
using IronPdf.Rendering;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

ChromePdfRenderer renderer = new ChromePdfRenderer();

// Set paper size and orientation
renderer.RenderingOptions.PaperSize = PdfPaperSize.A4;
renderer.RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape;

// Margin values are in mm
renderer.RenderingOptions.MarginTop = 20;
renderer.RenderingOptions.MarginLeft = 20;
renderer.RenderingOptions.MarginRight = 20;
renderer.RenderingOptions.MarginBottom = 20;

PdfDocument pdf = renderer.RenderHtmlAsPdf("<h1>Hello world</h1>");
pdf.SaveAs(output);
