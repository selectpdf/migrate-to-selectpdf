// pdfHTML: PDF/A output with ConverterProperties.SetPdfAConformance, an sRGB output intent and a font provider that
// uses only embeddable fonts (iText's HtmlToPdfA3Convert sample; here with PDF/A-2b). iText ships no ICC profile:
// sRGB.icc next to this sample is a standard sRGB IEC61966-2.1 profile.
// https://github.com/itext/itext-publications-samples-dotnet/blob/develop/itext/itext.samples/itext/samples/sandbox/pdfhtml/HtmlToPdfA3Convert.cs
using iText.Html2pdf;
using iText.StyledXmlParser.Resolver.Font;
using iText.Kernel.Pdf;

var output = args.Length > 0 ? args[0] : "output.pdf";
var html = """
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
using var icc = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "sRGB.icc"));
var properties = new ConverterProperties()
    .SetPdfAConformance(PdfAConformance.PDF_A_2B)
    .SetDocumentOutputIntent(new PdfOutputIntent("Custom", "", "http://www.color.org", "sRGB IEC61966-2.1", icc))
    .SetFontProvider(new BasicFontProvider(false, true, false));
using var stream = new FileStream(output, FileMode.Create);
HtmlConverter.ConvertToPdf(html, stream, properties);
