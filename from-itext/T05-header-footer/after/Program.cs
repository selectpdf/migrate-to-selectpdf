// Header and footer with page numbers, set on the converter (iText: CSS @page margin boxes).
// Docs: https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm
// Sample: https://selectpdf.com/demo-universal/HtmlToPdfHeadersAndFooters
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// the same body as the iText sample; the @top-center and @bottom-center boxes move to the converter's
// header and footer
const string html = """
    <html><head><style>
      h1 { page-break-after: always; }
      h1:last-child { page-break-after: auto; }
    </style></head><body>
      <h1>Section one</h1>
      <h1>Section two</h1>
      <h1>Section three</h1>
    </body></html>
    """;

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// header settings (@top-center); the height is in points
converter.Options.DisplayHeader = true;
converter.Header.DisplayOnFirstPage = true;
converter.Header.DisplayOnOddPages = true;
converter.Header.DisplayOnEvenPages = true;
converter.Header.Height = 40;
PdfTextSection header = new PdfTextSection(0, 15, "Quarterly report",
    PdfFont.CreateStandardFont(PdfStandardFont.Helvetica, 10));
header.HorizontalAlign = PdfTextHorizontalAlign.Center;
converter.Header.Add(header);

// footer settings (@bottom-center); counter(page) and counter(pages) become {page_number} and {total_pages}
converter.Options.DisplayFooter = true;
converter.Footer.DisplayOnFirstPage = true;
converter.Footer.DisplayOnOddPages = true;
converter.Footer.DisplayOnEvenPages = true;
converter.Footer.Height = 40;
PdfTextSection footer = new PdfTextSection(0, 10, "Page {page_number} of {total_pages}",
    PdfFont.CreateStandardFont(PdfStandardFont.Helvetica, 10));
footer.HorizontalAlign = PdfTextHorizontalAlign.Center;
converter.Footer.Add(footer);

PdfDocument doc = converter.ConvertHtmlString(html);
doc.Save(output);
doc.Close();
