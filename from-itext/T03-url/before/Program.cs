// pdfHTML has no URL input: iText's sample downloads the page and converts the bytes as a stream
// (C07E04_CreateFromURL). The base URI lets pdfHTML resolve the page's relative links, images and CSS.
// https://github.com/itext/itext-publications-samples-dotnet/blob/develop/itext/itext.publications/itext.publications.htmlsamples/itext/samples/htmlsamples/chapter07/C07E04_CreateFromURL.cs
// https://kb.itextpdf.com/itext/chapter-1-hello-html-to-pdf (base URI for stream input)
using iText.Html2pdf;

var output = args.Length > 0 ? args[0] : "output.pdf";
var url = "https://example.com/";
using var http = new HttpClient();
var page = await http.GetByteArrayAsync(url);
using var stream = new FileStream(output, FileMode.Create);
HtmlConverter.ConvertToPdf(new MemoryStream(page), stream, new ConverterProperties().SetBaseUri(url));
