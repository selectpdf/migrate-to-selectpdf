// iText Core: PdfTextExtractor.GetTextFromPage, page by page (default strategy: LocationTextExtractionStrategy).
// https://api.itextpdf.com/iText/dotnet/9.8.0/classi_text_1_1_kernel_1_1_pdf_1_1_canvas_1_1_parser_1_1_pdf_text_extractor.html
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

using var pdf = new PdfDocument(new PdfReader("invoice.pdf"));
for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
    Console.WriteLine(PdfTextExtractor.GetTextFromPage(pdf.GetPage(i)));
