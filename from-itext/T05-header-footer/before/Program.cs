// pdfHTML: headers and footers are CSS @page margin boxes; counter(page) and counter(pages) give "Page N of M".
// https://kb.itextpdf.com/itext/chapter-2-defining-styles-with-css
using iText.Html2pdf;

var output = args.Length > 0 ? args[0] : "output.pdf";
var html = """
    <html><head><style>
      @page {
        @top-center { content: "Quarterly report"; }
        @bottom-center { content: "Page " counter(page) " of " counter(pages); }
      }
      h1 { page-break-after: always; }
      h1:last-child { page-break-after: auto; }
    </style></head><body>
      <h1>Section one</h1>
      <h1>Section two</h1>
      <h1>Section three</h1>
    </body></html>
    """;
using var stream = new FileStream(output, FileMode.Create);
HtmlConverter.ConvertToPdf(html, stream);
