// PDF/A-2b output from an HTML to PDF conversion.
// Sample: https://selectpdf.com/demo-universal/PdfAStandards
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

const string html = """
    <html lang="en"><head><title>Quarterly report</title></head><body>
      <h1>Quarterly report</h1>
      <table border="1">
        <tr><th>Product</th><th>Units</th></tr>
        <tr><td>Alpha</td><td>120</td></tr>
        <tr><td>Beta</td><td>80</td></tr>
        <tr><td>Gamma</td><td>45</td></tr>
      </table>
    </body></html>
    """;

// instantiate a html to pdf converter object
HtmlToPdf converter = new HtmlToPdf();

// the standard the produced document conforms to (iText: ConverterProperties.SetPdfAConformance, plus an ICC
// output intent and an embeddable-fonts font provider, which SelectPdf does not need from the application)
converter.Options.PdfStandard = PdfStandard.PdfA2B;

PdfDocument doc = converter.ConvertHtmlString(html);
doc.Save(output);
doc.Close();
