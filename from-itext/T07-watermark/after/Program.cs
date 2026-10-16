// Stamp a text on every page of an existing PDF with a template (iText: a PdfCanvas per page + ShowTextAligned).
// Samples: https://selectpdf.com/demo-universal/PdfStamps
//          https://selectpdf.com/demo-universal/PdfWatermarks
using SelectPdf.Universal;
using SelectPdf.Universal.Drawing;

var output = args.Length > 0 ? args[0] : "output.pdf";

// load the existing pdf document (iText: new PdfDocument(new PdfReader(...), new PdfWriter(...)))
PdfDocument doc = new PdfDocument("invoice.pdf");

// Helvetica Bold, 72 pt, as in the iText sample
PdfFont font = doc.Fonts.Add(PdfStandardFont.HelveticaBold);
font.Size = 72;

// a template as large as the page repeats on all pdf pages automatically, so there is no loop over the pages
RectangleF page = doc.Pages[0].ClientRectangle;
PdfTemplate template = doc.Templates.Add(page);

// centre the text box (start point = its top left corner) on the page, rotated 45 degrees (counter-clockwise around its start point)
SizeF size = template.MeasureString("CONFIDENTIAL", font);   // the element gets this width, so the text stays on one line
float a = (size.Width + size.Height) / 2 / MathF.Sqrt(2), b = (size.Width - size.Height) / 2 / MathF.Sqrt(2);
PdfTextElement text = new PdfTextElement(page.Width / 2 - a, page.Height / 2 + b,
    size.Width + 1, "CONFIDENTIAL", font);
text.Direction = 45;
text.Transparency = 30;   // iText SetOpacity(0.3f); 0 = transparent, 100 = opaque
template.Add(text);

// save and close the pdf document
doc.Save(output);
doc.Close();
