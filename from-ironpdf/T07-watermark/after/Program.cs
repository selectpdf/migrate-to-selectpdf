// Stamp a text on every page of an existing PDF with a template.
// Samples: https://selectpdf.com/demo-universal/PdfStamps
//          https://selectpdf.com/demo-universal/PdfWatermarks
using SelectPdf.Universal;
using SelectPdf.Universal.Drawing;

var output = args.Length > 0 ? args[0] : "output.pdf";

// load the existing pdf document (IronPDF: PdfDocument.FromFile)
PdfDocument doc = new PdfDocument("invoice.pdf");

// a bold serif font the size of the <h1> in the IronPDF watermark (2em of 16 px = 24 pt)
PdfFont font = doc.Fonts.Add(PdfStandardFont.TimesBold);
font.Size = 24;

// a template as large as the page repeats on all pdf pages automatically
RectangleF page = doc.Pages[0].ClientRectangle;
PdfTemplate template = doc.Templates.Add(page);

// centre the text box (start point = its top left corner) on the page, turned 45 degrees clockwise like IronPDF's rotation: 45 (SelectPdf's Direction
// turns counter-clockwise around the text's start point, so it is 315 here)
SizeF size = template.MeasureString("CONFIDENTIAL", font);   // the element gets this width, so the text stays on one line
float a = (size.Width + size.Height) / 2 / MathF.Sqrt(2), b = (size.Width - size.Height) / 2 / MathF.Sqrt(2);
PdfTextElement text = new PdfTextElement(page.Width / 2 - b, page.Height / 2 - a,
    size.Width + 1, "CONFIDENTIAL", font);
text.Direction = 315;
text.ForeColor = new PdfColor(255, 0, 0);   // color:red
text.Transparency = 50;                     // IronPDF opacity: 50 (0 = transparent, 100 = opaque)
template.Add(text);

// save and close the pdf document
doc.Save(output);
doc.Close();
