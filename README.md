# Migrate to SelectPdf

Runnable before-and-after code for moving a .NET application from another PDF library to
[SelectPdf](https://selectpdf.com/) (`SelectPdf.Universal`), one guide per library.

> **Who maintains this:** SelectPdf, the vendor of the library these guides migrate to. Each guide also says what
> does **not** carry over, and when staying with your current library is the better choice. The neutral,
> side-by-side comparison of the same libraries (with benchmarks) is
> [csharp-pdf/pdf-libraries-compared](https://github.com/csharp-pdf/pdf-libraries-compared).

## Guides

| From | Guide |
|---|---|
| wkhtmltopdf (DinkToPdf, Haukcode.WkHtmlToPdfDotNet) | [from-wkhtmltopdf](from-wkhtmltopdf/) |
| IronPDF | [from-ironpdf](from-ironpdf/) |
| iText (with pdfHTML) | [from-itext](from-itext/) |
| PuppeteerSharp | [from-puppeteersharp](from-puppeteersharp/) |
| Playwright for .NET | [from-playwright](from-playwright/) |

Each guide has a mapping table from the library's API to SelectPdf's, one folder per task with a `before/` and an
`after/` project that produce the same document, and a section on what does not carry over. How the guides are
written: [GUIDES.md](GUIDES.md).

## Run the projects

Every project is a .NET 8 console app (the ASP.NET Core task is a minimal API). To build, run and check all of
them:

```bash
python tools/check_guides.py                    # every guide
python tools/check_guides.py from-ironpdf T05   # one task of one guide
```

The checker needs the .NET 8 SDK and Python 3 with `pypdf`, `pymupdf` and `pyhanko` (and veraPDF for PDF/A
checks: `VERAPDF` = path of the launcher, and `VERAPDF_JAVA_HOME` or `JAVA_HOME` if Java is not on the path). SelectPdf runs as a trial without a licence key and adds a
"Demo Version" banner to each page. IronPDF projects read their key from `IRONPDF_LICENSE_KEY`.

## License

Code: [MIT](LICENSE).
