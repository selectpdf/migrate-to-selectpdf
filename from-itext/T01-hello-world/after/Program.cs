// Hello world: a document built from code (iText: Document + Paragraph).
// Sample: https://selectpdf.com/demo-universal/PdfTextElement
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// create a new pdf document and add a page (A4 portrait, like iText's default page)
PdfDocument doc = new PdfDocument();
PdfPage page = doc.AddPage();

// a standard font, as iText's Paragraph uses Helvetica 12 by default
PdfFont font = doc.Fonts.Add(PdfStandardFont.Helvetica);
font.Size = 12;

// add a text element at the top left of the page's client area
page.Add(new PdfTextElement(0, 0, "Hello world", font));

// save and close the pdf document
doc.Save(output);
doc.Close();
