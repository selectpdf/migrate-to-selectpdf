// iText Core: text stamped on every page of an existing PDF, centred and rotated 45 degrees with
// ShowTextAligned, at 30 % opacity (iText's TransparentWatermark3 sample). The PdfCanvas is created with
// wrapOldContent = true, so a transformation left behind by the page's own content cannot move the stamp.
// https://github.com/itext/itext-publications-samples-dotnet/blob/develop/itext/itext.samples/itext/samples/sandbox/stamper/TransparentWatermark3.cs
// https://api.itextpdf.com/iText/dotnet/9.8.0/classi_text_1_1_kernel_1_1_pdf_1_1_canvas_1_1_pdf_canvas.html
using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

var output = args.Length > 0 ? args[0] : "output.pdf";
using var pdf = new PdfDocument(new PdfReader("invoice.pdf"), new PdfWriter(output));
var font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
for (int i = 1; i <= pdf.GetNumberOfPages(); i++)
{
    var page = pdf.GetPage(i);
    var size = page.GetPageSize();
    using var canvas = new Canvas(new PdfCanvas(page, true), size);
    canvas.ShowTextAligned(new Paragraph("CONFIDENTIAL").SetFont(font).SetFontSize(72).SetOpacity(0.3f),
        size.GetWidth() / 2, size.GetHeight() / 2, i, TextAlignment.CENTER, VerticalAlignment.MIDDLE, (float)(Math.PI / 4));
}
