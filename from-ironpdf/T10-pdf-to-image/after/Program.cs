// Render page 1 of a PDF file to a PNG image at 150 DPI.
// Docs: https://selectpdf.com/pdf-library/html/PdfToImageConverter.htm
// Sample: https://selectpdf.com/demo-universal/PdfToImageConverter
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "page-1.png";

// instantiate a pdf rasterizer (pdf to image converter) object and load the PDF file
PdfRasterizer rasterizer = new PdfRasterizer();
rasterizer.Load("invoice.pdf");

// the first page only, at 150 DPI, as PNG
rasterizer.StartPageNumber = 1;
rasterizer.EndPageNumber = 1;
rasterizer.Resolution = 150;
rasterizer.ImagesFormat = PdfImageFormat.Png;

// convert pages to images (one encoded image per page)
byte[][] images = rasterizer.ConvertToImages();
File.WriteAllBytes(output, images[0]);
