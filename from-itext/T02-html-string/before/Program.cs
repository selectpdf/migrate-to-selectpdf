// pdfHTML add-on: HtmlConverter.ConvertToPdf(string, Stream).
// https://kb.itextpdf.com/itext/chapter-1-hello-html-to-pdf
using iText.Html2pdf;

var output = args.Length > 0 ? args[0] : "output.pdf";
var html = """
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
using var stream = new FileStream(output, FileMode.Create);
HtmlConverter.ConvertToPdf(html, stream);
