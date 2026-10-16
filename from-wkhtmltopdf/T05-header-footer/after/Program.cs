// Header and footer with page numbers, set on the converter.
// Docs: https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm
// Sample: https://selectpdf.com/demo-universal/HtmlToPdfHeadersAndFooters
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

const string html = """
    <!DOCTYPE html>
    <html>
    <head><meta charset="utf-8"></head>
    <body>
      <h1>Section 1</h1>
      <h1 style="page-break-before: always">Section 2</h1>
      <h1 style="page-break-before: always">Section 3</h1>
    </body>
    </html>
    """;

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// A4; left and right margins are wkhtmltopdf's default 10 mm (28 points).
// wkhtmltopdf draws the header and footer inside the 20 mm (57 point) top and bottom margins; SelectPdf
// reserves the header and footer height in addition to the margin, so margin + height = 57 points.
converter.Options.PdfPageSize = PdfPageSize.A4;
converter.Options.MarginLeft = 28;
converter.Options.MarginRight = 28;
converter.Options.MarginTop = 28;
converter.Options.MarginBottom = 28;

// header settings
converter.Options.DisplayHeader = true;
converter.Header.DisplayOnFirstPage = true;
converter.Header.DisplayOnOddPages = true;
converter.Header.DisplayOnEvenPages = true;
converter.Header.Height = 29;
PdfTextSection title = new PdfTextSection(0, 10, "Quarterly report",
    PdfFont.CreateStandardFont(PdfStandardFont.Helvetica, 9));
title.HorizontalAlign = PdfTextHorizontalAlign.Center;
converter.Header.Add(title);
// HeaderSettings.Line: a thin rule under the header text, drawn by an HTML section
converter.Header.Add(new PdfHtmlSection(0, 22,
    "<html><body style=\"margin:0\"><div style=\"border-top:0.5pt solid #000\"></div></body></html>", null));

// footer settings; wkhtmltopdf's [page] and [topage] are {page_number} and {total_pages} in a PdfTextSection
converter.Options.DisplayFooter = true;
converter.Footer.DisplayOnFirstPage = true;
converter.Footer.DisplayOnOddPages = true;
converter.Footer.DisplayOnEvenPages = true;
converter.Footer.Height = 29;
PdfTextSection text = new PdfTextSection(0, 2, "Page {page_number} of {total_pages}",
    PdfFont.CreateStandardFont(PdfStandardFont.Helvetica, 9));
text.HorizontalAlign = PdfTextHorizontalAlign.Center;
converter.Footer.Add(text);

PdfDocument doc = converter.ConvertHtmlString(html);

doc.Save(output);
doc.Close();
