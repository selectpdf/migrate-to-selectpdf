// Page size, orientation and margins: A4 landscape with 20 mm margins, on a page built from code
// (iText: new Document(pdf, PageSize.A4.Rotate()) + SetMargins).
// Docs: https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfDocument.htm
// Sample: https://selectpdf.com/demo-universal/PdfTextElement
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";
float margin = 20 / 25.4f * 72;   // 20 mm in points, as in iText

// create a new pdf document and add an A4 landscape page with the margins
PdfDocument doc = new PdfDocument();
PdfPage page = doc.AddPage(PdfCustomPageSize.A4, new PdfMargins(margin), PdfPageOrientation.Landscape);

PdfFont font = doc.Fonts.Add(PdfStandardFont.Helvetica);
font.Size = 12;

// (0, 0) is the top left corner of the area inside the margins
page.Add(new PdfTextElement(0, 0, "Hello world", font));

doc.Save(output);
doc.Close();
