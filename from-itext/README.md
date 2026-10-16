# From iText to SelectPdf

Before-and-after projects for moving a .NET application from [iText](https://itextpdf.com/) (`itext` 9.8.0 with
the pdfHTML add-on, `itext.pdfhtml` 6.3.4) to [SelectPdf](https://selectpdf.com/pdf-library/)
(`SelectPdf.Universal` 26.5.0).

## Who it is for

Projects that use pdfHTML's `HtmlConverter` to turn HTML into PDF and iText Core to merge, stamp, fill, sign and
extract text from existing PDFs, and that want HTML rendered by Chromium, including JavaScript, under a commercial
licence instead of the AGPL.

Staying with iText is the better choice when you build documents from code with iText's layout model (paragraphs,
tables and lists that flow across pages) or work at the level of content streams and PDF objects, when you use
iText under the AGPL and want to keep it that way, when you need a library with no native code (iText's packages
are managed .NET Standard 2.0 assemblies, so they run where SelectPdf's engine does not, such as Alpine Linux or
macOS on Intel), or when you want HTML converted without a browser and without running scripts. For measured
differences, compare both libraries' rows in the
[2026-10-15 benchmark](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md),
for example the [deployment section](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md#deployment).

## Packages and setup

| | iText (before) | SelectPdf (after) |
|---|---|---|
| NuGet | `itext` + `itext.bouncy-castle-adapter`; `itext.pdfhtml` for HTML | Windows: `SelectPdf.Universal` + `SelectPdf.Universal.Native.win-x64`, or `SelectPdf.Universal.Windows` alone; Linux: `SelectPdf.Universal` + `SelectPdf.Universal.Native.linux-x64`, or `SelectPdf.Universal.Linux` alone |
| Namespaces | `iText.Kernel.Pdf`, `iText.Layout`, `iText.Html2pdf`, `iText.Forms`, `iText.Signatures`, ... | `SelectPdf.Universal` (plus `SelectPdf.Universal.Drawing` for `RectangleF`, `SizeF`) |
| Licence | AGPL: no key. Commercial: a JSON licence file loaded with `LicenseKey.LoadLicenseFile` from iText's licensing library | `GlobalProperties.LicenseKey = "..."` before the first conversion; without a key the library runs as a trial and adds a "Demo Version" banner to each page |

Sources: [Installing iText Community for .NET](https://kb.itextpdf.com/itext/installing-itext-community-for-net-developers),
[Installing the iText license key](https://kb.itextpdf.com/itext/installing-the-itext-license-key-and-license-key-l),
[iText licensing](https://itextpdf.com/how-buy),
[SelectPdf installation](https://selectpdf.com/pdf-library/html/Installation.htm),
[SelectPdf licensing](https://selectpdf.com/pdf-library/html/License.htm),
[`GlobalProperties.LicenseKey`](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_GlobalProperties_LicenseKey.htm).

Use one SelectPdf engine package per build: the `after/` projects pick the Native package by runtime identifier
(win-x64 by default, linux-x64 when published with `-r linux-x64`), as in SelectPdf's "Publishing for Several
Platforms" setup ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)). The projects here
read no SelectPdf key, so their output carries the trial banner.

## Mapping

Both libraries measure PDF coordinates, page sizes and margins in **points** (1/72 inch). iText's origin is the
bottom-left corner of the page; SelectPdf places elements from the top-left corner of the page's client area
(inside the margins).

| iText | SelectPdf | Notes |
|---|---|---|
| `new PdfDocument(new PdfWriter(path))` + `new Document(pdf)` | `new PdfDocument()` + `doc.AddPage()` ... `doc.Save(path)`, `doc.Close()` | [`PdfDocument`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfDocument.htm) |
| `document.Add(new Paragraph(text))` | `page.Add(new PdfTextElement(x, y, text, font))` | [`PdfTextElement` demo](https://selectpdf.com/demo-universal/PdfTextElement) |
| `PdfFontFactory.CreateFont(StandardFonts.HELVETICA)` | `doc.Fonts.Add(PdfStandardFont.Helvetica)`, then `font.Size` | |
| `new Document(pdf, PageSize.A4.Rotate())` | `doc.AddPage(PdfCustomPageSize.A4, margins, PdfPageOrientation.Landscape)` | |
| `document.SetMargins(top, right, bottom, left)` | `new PdfMargins(left, right, top, bottom)` | Both in points; the argument order differs |
| `HtmlConverter.ConvertToPdf(html, stream)` | `new HtmlToPdf().ConvertHtmlString(html)`, then `doc.Save(stream)` | |
| `ConverterProperties.SetBaseUri(uri)` | `ConvertHtmlString(html, baseUrl)` | |
| Download the page, then `ConvertToPdf(stream, ..., SetBaseUri(url))` | `converter.ConvertUrl(url)` | |
| `ConverterProperties.SetMediaDeviceDescription(...)` | `converter.Options.CssMediaType` | Screen is SelectPdf's default |
| CSS `@page { @top-center { content: ... } }` | `converter.Header.Add(new PdfTextSection(...))` with `Options.DisplayHeader = true` | Or `PdfHtmlSection` for an HTML header |
| `counter(page)`, `counter(pages)` | `{page_number}`, `{total_pages}` in a `PdfTextSection` | [Headers and footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm) |
| Page event handler (`PdfDocumentEvent.END_PAGE`) for headers on code-built pages | `doc.Header` / `doc.Footer`, or a template from `doc.Templates.Add(rect)` | Templates repeat on every page ([templates demo](https://selectpdf.com/demo-universal/PdfTemplates)) |
| `new PdfMerger(pdf).Merge(source, 1, n)` | `doc.Append(source)` | [Merge demo](https://selectpdf.com/demo-universal/PdfMerge) |
| `new PdfCanvas(page, true)` + `Canvas.ShowTextAligned(paragraph, x, y, page, align, valign, angle)` | `doc.Templates.Add(rect)` + `PdfTextElement` with `Direction` (degrees, counter-clockwise) and `Transparency` (0 to 100) | iText's angle is in radians, its opacity 0 to 1 |
| `PdfFormCreator.GetAcroForm(pdf, false).GetField(name).SetValue(value)` | `PdfFormManager.Load(path)`, then `form.Fields[name]` cast to `PdfFormFieldTextBox` (`.Text`), `PdfFormFieldComboBox` (`.SelectedValue`), `PdfFormFieldCheckBox` (`.Checked`) | [`PdfFormManager`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfFormManager.htm) |
| `PdfTextExtractor.GetTextFromPage(page)` | `new PdfToText()`, `Load(path)`, `GetText()` or `GetText(pageNumber)` | [`PdfToText`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfToText.htm) |
| `PdfSigner.SignDetached(new PrivateKeySignature(key, ...), chain, ...)` with the key read through BouncyCastle | `PdfDigitalCertificatesStore.GetCertificates(pfx, password)` + `new PdfDigitalSignatureElement(rect, certificate)` added to a page | [`PdfDigitalSignatureElement`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfDigitalSignatureElement.htm) |
| `SignerProperties.SetReason(...)` | `signature.Reason` (also `Location`, `ContactInfo`) | |
| `ConverterProperties.SetPdfAConformance(PdfAConformance.PDF_A_2B)` + `SetDocumentOutputIntent(...)` + an embeddable-fonts `FontProvider` | `converter.Options.PdfStandard = PdfStandard.PdfA2B` | No ICC profile or font provider from the application |
| `new PdfADocument(writer, conformance, outputIntent)` | `new PdfDocument(PdfStandard.PdfA2B)` | For documents built from code |
| Existing PDF to PDF/A | `doc.ConvertToPdfA(PdfStandard.PdfA2B)` on a loaded document | [`ConvertToPdfA`](https://selectpdf.com/pdf-library/html/M_SelectPdf_Universal_PdfDocument_ConvertToPdfA.htm) |

API sources: the iText pages linked from each `before/Program.cs`, the
[iText .NET API reference](https://api.itextpdf.com/iText/dotnet/9.8.0/), the
[SelectPdf reference](https://selectpdf.com/pdf-library/) and the SelectPdf pages linked from each
`after/Program.cs`.

## Task by task

Every pair passes the same `expect.json` checks (`python tools/check_guides.py from-itext`).

| Task | Before | After | What changed |
|---|---|---|---|
| T01 Hello world | [before](T01-hello-world/before/Program.cs) | [after](T01-hello-world/after/Program.cs) | iText's `Document` + `Paragraph` becomes a SelectPdf `PdfDocument` with one page and a Helvetica 12 pt `PdfTextElement`: the document is still built from code, not from HTML. |
| T02 HTML string | [before](T02-html-string/before/Program.cs) | [after](T02-html-string/after/Program.cs) | `HtmlConverter.ConvertToPdf(html, stream)` becomes `HtmlToPdf.ConvertHtmlString(html)` + `Save`. The same HTML; Chromium lays it out, so the page does not look the same as pdfHTML's (see below). |
| T03 URL | [before](T03-url/before/Program.cs) | [after](T03-url/after/Program.cs) | pdfHTML has no URL input, so the iText project downloads the page and passes a base URI; SelectPdf's `ConvertUrl` loads the page itself. |
| T04 Page setup | [before](T04-page-setup/before/Program.cs) | [after](T04-page-setup/after/Program.cs) | `PageSize.A4.Rotate()` and `SetMargins` become one `AddPage(PdfCustomPageSize.A4, new PdfMargins(margin), PdfPageOrientation.Landscape)`; the 20 mm margin is the same number of points in both. |
| T05 Header and footer | [before](T05-header-footer/before/Program.cs) | [after](T05-header-footer/after/Program.cs) | The CSS `@page` margin boxes leave the HTML and become centred `PdfTextSection`s in the converter's header and footer; `counter(page)` and `counter(pages)` become `{page_number}` and `{total_pages}`. |
| T06 Merge | [before](T06-merge/before/Program.cs) | [after](T06-merge/after/Program.cs) | `PdfMerger.Merge` per source becomes `Append` per source on a new `PdfDocument`; all three documents are closed. |
| T07 Watermark | [before](T07-watermark/before/Program.cs) | [after](T07-watermark/after/Program.cs) | The loop that draws on each page's `PdfCanvas` becomes one page-sized template, which repeats on every page; the text is Helvetica Bold 72 pt, 30 % opacity, centred and turned 45 degrees, as in the iText project. |
| T08 Fill a form | [before](T08-fill-form/before/Program.cs) | [after](T08-fill-form/after/Program.cs) | `PdfAcroForm.GetField(...).SetValue(...)` becomes `PdfFormManager` with typed fields: the check box takes `Checked = true` instead of its "on" state name `Yes`. |
| T09 Extract text | [before](T09-extract-text/before/Program.cs) | [after](T09-extract-text/after/Program.cs) | The page loop with `PdfTextExtractor.GetTextFromPage` becomes one `PdfToText.GetText()` call. |
| T11 Digital signature | [before](T11-digital-signature/before/Program.cs) | [after](T11-digital-signature/after/Program.cs) | The same self-signed certificate, exported to a `.pfx` file instead of read through BouncyCastle. `PdfSigner.SignDetached` becomes a `PdfDigitalSignatureElement` with the reason `Approved`, added to page 1 at a 200 × 50 pt rectangle. |
| T12 PDF/A | [before](T12-pdfa/before/Program.cs) | [after](T12-pdfa/after/Program.cs) | `SetPdfAConformance`, the sRGB output intent and the font provider become `Options.PdfStandard = PdfStandard.PdfA2B`; the iText project's `sRGB.icc` file is not needed. Both outputs pass veraPDF's PDF/A-2b check. |
| T13 ASP.NET Core | [before](T13-aspnet-core/before/Program.cs) | [after](T13-aspnet-core/after/Program.cs) | Converting into a `MemoryStream` becomes `doc.Save()`, which returns the bytes. |

## What you can add

- **PDF to image (T10).** iText has no .NET API for it: pdfRender runs as a command-line tool with a licence file
  ([How do I use pdfRender in a .NET environment?](https://kb.itextpdf.com/itext/how-do-i-use-pdfrender-in-a-net-environment)).
  SelectPdf renders pages with [`PdfRasterizer`](https://selectpdf.com/pdf-library/html/PdfToImageConverter.htm);
  see the [SelectPdf T10 sample](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T10-pdf-to-image)
  in the comparison repository.
- **JavaScript in HTML.** pdfHTML does not run scripts
  ([iText KB](https://kb.itextpdf.com/itext/do-we-need-a-browser-engine-to-render-html-css-to-)); SelectPdf runs
  them by default (`Options.JavaScriptEnabled`) and can wait for them with `Options.ConversionDelay`
  ([`HtmlToPdfOptions`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm)).

## What does not carry over

- **The same HTML gives a different PDF.** pdfHTML parses HTML and CSS with its own layout engine; SelectPdf
  renders with Chromium. Fonts, line breaks and page breaks change, and scripts that pdfHTML ignored now run.
  SelectPdf lays the page out at `WebPageWidth` (1024 px by default) and scales it to the width between the
  margins; set it to the paper width in pixels at 96 per inch (793 for A4 portrait with no margins) to print at
  100 % ([`HtmlToPdfOptions`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm)).
  Re-check every template after the move.
- **pdfHTML extension points.** Custom tag workers, CSS appliers and `HtmlConverter.ConvertToElements` (HTML into
  iText layout elements you add to a `Document` yourself) have no SelectPdf counterpart; SelectPdf converts a
  whole page, or places an HTML fragment on a page with
  [`PdfHtmlElement`](https://selectpdf.com/demo-universal/PdfHtmlElement).
- **iText's layout model.** SelectPdf has text, image, HTML and shape elements
  ([graphic elements demo](https://selectpdf.com/demo-universal/GraphicElements)) but no table or list element
  like iText's `Table` and `List`
  ([Chapter 1: Introducing basic building blocks](https://kb.itextpdf.com/itext/chapter-1-introducing-basic-building-blocks));
  tables built from code move to HTML.
- **Low-level content and PDF objects.** iText's `PdfCanvas` writes content-stream operators directly
  ([Chapter 2: Adding low-level content](https://kb.itextpdf.com/itext/chapter-2-adding-low-level-content)) and
  its kernel exposes the PDF object model (dictionaries, arrays, streams). SelectPdf works with elements placed at
  coordinates; its reference has no operator-level or object-level API
  ([SelectPdf reference](https://selectpdf.com/pdf-library/)). Code that edits PDF objects directly has to be
  rewritten or kept on iText.
- **Renderers and event handlers.** Custom renderers and page event handlers
  ([Chapter 3](https://kb.itextpdf.com/itext/chapter-3-using-renderers-and-event-handlers)) have no equivalent;
  repeating content goes into `Header`, `Footer` or templates.
- **Text extraction strategies.** iText lets you plug in extraction strategies and event listeners
  ([`PdfTextExtractor`](https://api.itextpdf.com/iText/dotnet/9.8.0/classi_text_1_1_kernel_1_1_pdf_1_1_canvas_1_1_parser_1_1_pdf_text_extractor.html));
  SelectPdf's `PdfToText` returns the text of the document, of one page or of a rectangle on a page, with a
  layout option and a search method, but takes no custom listener
  ([`PdfToText`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfToText.htm)).
- **Signing options.** iText's signing module exposes the signature field name, the CMS container and external
  signature interfaces ([digital signatures hub](https://kb.itextpdf.com/itext/digital-signatures-hub)).
  SelectPdf's signature element takes a certificate, reason, location, contact information, digest algorithm,
  background image and an LTV switch, with time-stamp authorities, PKCS#11 and external signers as separate
  classes, but no field name property
  ([`PdfDigitalSignatureElement`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfDigitalSignatureElement.htm),
  [`TimeStampAuthority`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_TimeStampAuthority.htm),
  [`PdfPkcs11Signer`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfPkcs11Signer.htm),
  [`IPdfExternalSigner`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_IPdfExternalSigner.htm)).
  Check that your signing profile is covered before moving.
- **Complex scripts.** iText needs the closed-source pdfCalligraph add-on for Indic ligatures and Hebrew
  right-to-left text ([iText KB](https://kb.itextpdf.com/itext/which-languages-are-supported-in-pdfhtml)). In
  SelectPdf there is no separate add-on; HTML text is laid out by Chromium, and Linux images need fonts for
  scripts beyond Latin, Greek and Cyrillic (`SelectPdf.Universal.Fonts`,
  [Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)). Test your languages.
- **Form check boxes.** iText sets a check box by the name of its "on" appearance state (`"Yes"`); SelectPdf uses
  `PdfFormFieldCheckBox.Checked` ([form filling demo](https://selectpdf.com/demo-universal/PdfFormFilling)).
- **Platforms.** iText is managed code with no native dependencies. SelectPdf needs its native engine package and
  runs on Windows (x64, x86), Linux (x64, ARM64, glibc 2.30 or later) and macOS 13+ on Apple Silicon; on an Intel
  Mac, HTML to PDF, PDF to text and PDF to image are unavailable
  ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)).
- **Source code.** iText is open source under the AGPL
  ([iText Core](https://github.com/itext/itext-dotnet), [pdfHTML](https://github.com/itext/i7n-pdfhtml));
  SelectPdf is closed-source commercial software ([EULA](https://selectpdf.com/eula/)).
- **Commercial-only features.** SelectPdf's free Community Edition (`SelectPdf.HtmlToPdf.Universal`) is HTML to PDF
  only, at most 5 pages per document. Every other task in this guide (documents built from code, merge, stamps,
  forms, text extraction, signatures, PDF/A) needs the commercial `SelectPdf.Universal` licence, as do Word to
  PDF, redaction, PDF/UA and ZUGFeRD ([Community Edition](https://selectpdf.com/community-edition/),
  [pricing](https://selectpdf.com/pricing/)).

## Deployment

- **Windows.** iText needs only its NuGet packages. SelectPdf needs one Native package per build, chosen by
  runtime identifier ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)).
- **Linux and Docker.** Neither library documents `apt-get` packages on the official .NET images. pdfHTML's
  default font provider uses the fonts shipped with pdfHTML, not system fonts
  ([Chapter 6: Using fonts in pdfHTML](https://kb.itextpdf.com/itext/chapter-6-using-fonts-in-pdfhtml)).
  SelectPdf is published with `-r linux-x64` and bundles the Chromium libraries in its Native package; base images
  have no fonts, and `SelectPdf.Universal.Fonts` adds Noto fonts for scripts beyond Latin, Greek and Cyrillic
  ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)). Compare the two
  Dockerfiles: [iText](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/itext/docker),
  [SelectPdf](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/docker), and the measured
  images in the [benchmark's deployment section](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md#deployment).
- **Alpine.** SelectPdf needs glibc 2.30 or later, so Alpine (musl) images can only convert through its separate
  remote engine container ([Remote Engine](https://selectpdf.com/pdf-library/html/Remote-Engine.htm)).
- **Windows containers.** SelectPdf needs a Windows Server Core image with the core Windows fonts; Nano Server is
  not supported ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)).

## Licensing

| | iText | SelectPdf |
|---|---|---|
| Model | Dual licence: AGPL-3.0, or commercial | Commercial, perpetual, 1 year of maintenance; renewal optional |
| Free use | AGPL: free of charge; your application's full source must be disclosed under the AGPL, and the iText producer line must be kept in every PDF | 30-day trial with a "Demo Version" banner; free Community Edition for HTML to PDF, at most 5 pages per document |
| Commercial price | Not published: OEM licences are priced per use case, the volume licence is an annual subscription priced on the number of PDF files processed; 30-day trial | Single Developer, USD 499: 1 developer, 1 deployment machine; OEM licences from USD 799 |
| Source | [itextpdf.com/how-buy](https://itextpdf.com/how-buy), [AGPLv3 licence](https://itextpdf.com/how-buy/AGPLv3-license), read 2026-10-16 | [selectpdf.com/pricing](https://selectpdf.com/pricing/), read 2026-10-16 |

Check both pages for the current terms before buying; tiers above the entry licence are listed there.
