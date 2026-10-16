// HTML string to PDF.
// Docs: https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm
// Sample: https://selectpdf.com/demo-universal/ConvertHtmlCodeToPdf
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

const string html = """
    <html><body>
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

// create a new pdf document converting the html string
PdfDocument doc = converter.ConvertHtmlString(html);

// save and close the pdf document
doc.Save(output);
doc.Close();
