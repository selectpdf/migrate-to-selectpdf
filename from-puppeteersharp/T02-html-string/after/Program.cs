// HTML string to PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertHtmlCodeToPdf
// Print media: https://selectpdf.com/pdf-library/html/MediaTypes.htm
// Layout width: https://selectpdf.com/pdf-library/html/ResizingContentDuringConversion.htm
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

const string html = """
    <h1>Quarterly report</h1>
    <table border="1">
      <tr><th>Product</th><th>Units</th></tr>
      <tr><td>Alpha</td><td>120</td></tr>
      <tr><td>Beta</td><td>85</td></tr>
      <tr><td>Gamma</td><td>42</td></tr>
    </table>
    """;

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// The settings page.pdf (PdfAsync) prints with by default: Letter paper, no margins (also SelectPdf's
// default), print CSS media, and the page laid out at the paper width (8.5 in at 96 px per inch = 816 px).
// SelectPdf's own defaults are A4, screen media and a 1024 px layout width scaled to the page.
converter.Options.PdfPageSize = PdfPageSize.Letter;
converter.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
converter.Options.WebPageWidth = 816;
// PuppeteerSharp 25 produces tagged PDFs by default (PdfOptions.Tagged = true); SelectPdf's default is false.
converter.Options.Tagged = true;

// create a new pdf document converting the html string
PdfDocument doc = converter.ConvertHtmlString(html);

// save and close the pdf document
doc.Save(output);
doc.Close();
