// iText Core: page size and orientation with PageSize.A4.Rotate(), margins with Document.SetMargins (in points).
// https://kb.itextpdf.com/itext/chapter-1-introducing-basic-building-blocks
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

var output = args.Length > 0 ? args[0] : "output.pdf";
float margin = 20 / 25.4f * 72;   // 20 mm in points
using var pdf = new PdfDocument(new PdfWriter(output));
using var document = new Document(pdf, PageSize.A4.Rotate());
document.SetMargins(margin, margin, margin, margin);
document.Add(new Paragraph("Hello world"));
