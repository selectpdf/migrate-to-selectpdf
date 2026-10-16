// iText Core: a document built with the layout API (Document + Paragraph).
// https://kb.itextpdf.com/itext/chapter-1-introducing-basic-building-blocks
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

var output = args.Length > 0 ? args[0] : "output.pdf";
using var pdf = new PdfDocument(new PdfWriter(output));
using var document = new Document(pdf);
document.Add(new Paragraph("Hello world"));
