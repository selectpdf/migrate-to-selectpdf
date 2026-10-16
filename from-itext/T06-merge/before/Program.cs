// iText Core: PdfMerger copies page ranges of source documents into one document.
// https://kb.itextpdf.com/itext/chapter-6-reusing-existing-pdf-documents
using iText.Kernel.Pdf;
using iText.Kernel.Utils;

var output = args.Length > 0 ? args[0] : "output.pdf";
using var pdf = new PdfDocument(new PdfWriter(output));
var merger = new PdfMerger(pdf);
foreach (var file in new[] { "invoice.pdf", "accessible.pdf" })
{
    using var source = new PdfDocument(new PdfReader(file));
    merger.Merge(source, 1, source.GetNumberOfPages());
}
