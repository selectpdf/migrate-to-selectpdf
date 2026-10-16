// Header and footer with page numbers, set on the converter.
// Docs: https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm
// Sample: https://selectpdf.com/demo-universal/HtmlToPdfHeadersAndFooters
// Print media: https://selectpdf.com/pdf-library/html/MediaTypes.htm
// Layout width: https://selectpdf.com/pdf-library/html/ResizingContentDuringConversion.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

const string html = """
    <h1>Section one</h1>
    <h1 style="break-before: page">Section two</h1>
    <h1 style="break-before: page">Section three</h1>
    """;

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// The settings page.PdfAsync prints with by default: Letter paper, no margins (also SelectPdf's
// default), print CSS media, and the page laid out at the paper width (8.5 in at 96 px per inch = 816 px).
// SelectPdf's own defaults are A4, screen media and a 1024 px layout width scaled to the page.
converter.Options.PdfPageSize = PdfPageSize.Letter;
converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
converter.Options.WebPageWidth = 816;

// page.PdfAsync draws the templates inside the 25 mm top and bottom margins. SelectPdf's header and
// footer have their own height, in points (25 mm = 71 pt), and the page content is placed between them.
converter.Options.DisplayHeader = true;
converter.Header.Height = 71;
// The header template becomes an HTML section, laid out at the same 816 px width as the page.
PdfHtmlSection header = new PdfHtmlSection(0, 9,
    "<div style=\"font-size:10px; width:100%; text-align:center\">Quarterly report</div>", null);
header.WebPageWidth = 816;
converter.Header.Add(header);

// Page numbers come from the {page_number} and {total_pages} placeholders of a PdfTextSection
// (10 px = 7.5 pt).
converter.Options.DisplayFooter = true;
converter.Footer.Height = 71;
PdfTextSection footer = new PdfTextSection(0, 49, "Page {page_number} of {total_pages}",
    PdfFont.CreateStandardFont(PdfStandardFont.Helvetica, 7.5f));
footer.HorizontalAlign = PdfTextHorizontalAlign.Center;
converter.Footer.Add(footer);

PdfDocument doc = converter.ConvertHtmlString(html);
// save and close the pdf document
doc.Save(output);
doc.Close();
