# From IronPDF to SelectPdf

Before-and-after projects for moving a .NET application from [IronPDF](https://ironpdf.com/) (`IronPdf`
2026.10.3) to [SelectPdf](https://selectpdf.com/pdf-library/) (`SelectPdf.Universal` 26.5.0).

## Who it is for

Projects that use IronPDF's `ChromePdfRenderer` to turn HTML or URLs into PDF and its `PdfDocument` to merge,
stamp, fill, sign, extract from or rasterize existing PDFs. Both libraries convert HTML with a Chromium-based
engine, so most of the work is renaming types and converting units; the
[task folders](#task-by-task) show each change on a working project.

Staying with IronPDF is the better choice when you depend on one of the features listed under
[what does not carry over](#what-does-not-carry-over) (Markdown, RTF and image converters, text replacement,
printing, barcode stamps, PDF to HTML), when you deploy to macOS on Intel (SelectPdf supports macOS on Apple
Silicon only), or when IronPDF's measured results suit your workload better: compare both libraries' rows in the
[2026-10-15 benchmark](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md),
for example [throughput on Windows](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md#throughput).

## Price and licensing

Both sell perpetual licences with one year of updates. Each situation is priced with the cheapest published licence
that covers it.

| | IronPDF | SelectPdf |
|---|---|---|
| 1 developer, internal application | Lite, USD 999 (1 developer, 1 location, 1 project) | Single Developer, USD 499 (1 developer, 1 deployment machine) |
| 1 developer, SaaS or software sold to customers | Plus (3 developers) + OEM Redistribution add-on, USD 3,498 (1,499 + 1,999); the add-on is not offered with Lite | Single Developer OEM, USD 799 (unlimited deployment machines) |
| 5 developers, internal | Professional (up to 10 developers), USD 2,399 (USD 2,999 from 2026-11-01) | 5-Developers (up to 10 deployment machines), USD 799 |
| Unlimited developers, SaaS or redistributed | Unlimited + OEM Redistribution add-on, USD 6,798 (4,799 + 1,999; USD 7,998 from 2026-11-01) | Enterprise OEM, USD 1,599 |
| Renewal after the first year (optional) | Support and updates, yearly: USD 599 (Lite), 899 (Plus), 1,799 (Professional), 3,599 (Unlimited); auto-renewal can be cancelled | Maintenance plan, 1, 3 or 5 years |
| Free use | 30-day trial key | 30-day trial with a "Demo Version" banner; free Community Edition for HTML to PDF, at most 5 pages per document |
| Source | [ironpdf.com/licensing](https://ironpdf.com/licensing/), read 2026-10-22 | [selectpdf.com/pricing](https://selectpdf.com/pricing/), read 2026-10-16 |

Iron Software announces that its Professional and Unlimited prices rise to USD 2,999 and USD 5,999 on
2026-11-01. Check both pages for the current terms before buying.

## Packages and setup

| | IronPDF (before) | SelectPdf (after) |
|---|---|---|
| HTML engine version | Chromium 109.0.5414.120 (CEF 109.1.18, in `IronPdf.Native.Chrome.*` 2026.10.6; Chromium 109 stable since 2023-01-10) | Chromium 154.0.8037.58 (CEF 154; Chromium 154 stable since 2026-09-22), in the engine package |
| NuGet, Windows | `IronPdf` | `SelectPdf.Universal` + `SelectPdf.Universal.Native.win-x64`, or `SelectPdf.Universal.Windows` alone |
| NuGet, Linux and Docker | `IronPdf.Linux` | `SelectPdf.Universal` + `SelectPdf.Universal.Native.linux-x64`, or `SelectPdf.Universal.Linux` alone |
| Namespace | `IronPdf` (plus `IronPdf.Rendering`, `IronPdf.Signing`, `IronPdf.Imaging`) | `SelectPdf.Universal` (plus `SelectPdf.Universal.Drawing` for `RectangleF`, `SizeF`) |
| Licence key | `License.LicenseKey = "..."` before the first call; without a key, saving throws | `GlobalProperties.LicenseKey = "..."` before the first conversion; without a key the library runs as a trial and adds a "Demo Version" banner to each page |

Sources: [IronPDF licence keys](https://ironpdf.com/get-started/license-keys/),
[IronPDF Docker guide](https://ironpdf.com/get-started/ironpdf-docker/),
[IronPDF library page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/ironpdf#gotchas) (the
behaviour without a key), [SelectPdf installation](https://selectpdf.com/pdf-library/html/Installation.htm),
[SelectPdf licensing](https://selectpdf.com/pdf-library/html/License.htm),
[`GlobalProperties.LicenseKey`](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_GlobalProperties_LicenseKey.htm).

Use one SelectPdf engine package per build: the `after/` projects pick the Native package by runtime identifier
(win-x64 by default, linux-x64 when published with `-r linux-x64`), as in SelectPdf's "Publishing for Several
Platforms" setup ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)). The projects here
read no SelectPdf key, so their output carries the trial banner; the `before/` projects read
`IRONPDF_LICENSE_KEY`.

## Mapping

| IronPDF | SelectPdf | Notes |
|---|---|---|
| `new ChromePdfRenderer()` | `new HtmlToPdf()` | |
| `renderer.RenderHtmlAsPdf(html)` | `converter.ConvertHtmlString(html)` | `ConvertHtmlString(html, baseUrl)` resolves relative links, images and CSS |
| `renderer.RenderUrlAsPdf(url)` | `converter.ConvertUrl(url)` | |
| `RenderHtmlAsPdfAsync`, `RenderUrlAsPdfAsync` | `ConvertHtmlStringAsync`, `ConvertUrlAsync` | [Async demo](https://selectpdf.com/demo-universal/AsyncConversion) |
| `renderer.RenderingOptions` | `converter.Options` ([`HtmlToPdfOptions`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm)) | |
| `RenderingOptions.PaperSize = PdfPaperSize.A4` | `Options.PdfPageSize = PdfPageSize.A4` | A4 is the default in both |
| `RenderingOptions.PaperOrientation = PdfPaperOrientation.Landscape` | `Options.PdfPageOrientation = PdfPageOrientation.Landscape` | |
| `RenderingOptions.MarginTop` (and the others), **millimetres** | `Options.MarginTop` (and the others), **points**, integers | 1 mm = 72 / 25.4 pt; 20 mm = 57 |
| `RenderingOptions.CssMediaType` | `Options.CssMediaType` | Screen is the default in both |
| `RenderingOptions.EnableJavaScript` | `Options.JavaScriptEnabled` | |
| `RenderingOptions.WaitFor.RenderDelay(ms)` | `Options.ConversionDelay` | SelectPdf's delay is in **seconds** |
| `RenderingOptions.Timeout` (seconds) | `Options.NavigationTimeout` (seconds) | 60 is the default in both |
| `RenderingOptions.TextHeader` / `TextFooter` = `new TextHeaderFooter { CenterText = ... }` | `converter.Header` / `converter.Footer` + `PdfTextSection` with `HorizontalAlign` | Turn them on with `Options.DisplayHeader` / `DisplayFooter`; the height is set in points |
| `RenderingOptions.HtmlHeader` / `HtmlFooter` = `new HtmlHeaderFooter { HtmlFragment = ... }` | `converter.Header.Add(new PdfHtmlSection(html, baseUrl))` | [`PdfHtmlSection`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfHtmlSection.htm) |
| `{page}`, `{total-pages}` | `{page_number}`, `{total_pages}` | See [what does not carry over](#what-does-not-carry-over) for the other IronPDF placeholders |
| `RenderingOptions.FirstPageNumber` | `converter.Header.FirstPageNumber`, `converter.Footer.FirstPageNumber` | |
| `PdfDocument` (IronPDF) | `PdfDocument` (SelectPdf) | Same name, different type: close with `doc.Close()` |
| `PdfDocument.FromFile(path)` | `new PdfDocument(path)` | |
| `pdf.SaveAs(path)` / `pdf.BinaryData` | `doc.Save(path)` / `byte[] bytes = doc.Save()` | |
| `PdfDocument.Merge(a, b)` | `var doc = new PdfDocument(); doc.Append(a); doc.Append(b);` | |
| `pdf.ApplyWatermark(html, rotation, opacity)` | `doc.Templates.Add(rect)` + `PdfTextElement` (`Direction`, `Transparency`, `ForeColor`) | A template as large as the page repeats on every page; `Direction` turns counter-clockwise, IronPDF's `rotation` clockwise |
| `pdf.Form.FindFormField(name).Value = ...` | `PdfFormManager.Load(path)`, then `form.Fields[name]` cast to `PdfFormFieldTextBox` (`.Text`), `PdfFormFieldComboBox` (`.SelectedValue`), `PdfFormFieldCheckBox` (`.Checked`) | Forms are filled through [`PdfFormManager`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfFormManager.htm), not `PdfDocument` |
| `pdf.ExtractAllText()` | `new PdfToText()`, `Load(path)`, `GetText()` | `GetText(page)` for one page; [`PdfToText`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfToText.htm) |
| `pdf.RasterizeToImageFiles(pattern, pageIndexes, ImageType.Png, DPI: 150)` | `new PdfRasterizer()`, `Load(path)`, `StartPageNumber` / `EndPageNumber`, `Resolution = 150`, `ImagesFormat = PdfImageFormat.Png`, `ConvertToImages()` | IronPDF page indexes start at **0**, SelectPdf page numbers at **1** |
| `new PdfSignature(pfx, password)` + `pdf.Sign(signature)` | `PdfDigitalCertificatesStore.GetCertificates(pfx, password)` + `new PdfDigitalSignatureElement(rect, certificate)` added to a page | [`PdfDigitalSignatureElement`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfDigitalSignatureElement.htm); the rectangle is in points |
| `pdf.SaveAsPdfA(path, PdfAVersions.PdfA2b)` | `doc.ConvertToPdfA(PdfStandard.PdfA2B)` + `doc.Save(path)` | For new documents: `Options.PdfStandard` on the converter, or `new PdfDocument(PdfStandard)` |
| `Results.File(pdf.BinaryData, "application/pdf")` | `Results.File(doc.Save(), "application/pdf")` | |
| `Installation.LinuxAndDockerDependenciesAutoConfig`, `Installation.ChromeGpuMode` | none | SelectPdf documents no OS packages or GPU settings for Linux ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)) |

API sources: the IronPDF how-to pages linked from each `before/Program.cs`, the IronPDF XML documentation shipped
in the `IronPdf.Slim` package, the [SelectPdf reference](https://selectpdf.com/pdf-library/) and the SelectPdf
pages linked from each `after/Program.cs`.

## Task by task

Every pair passes the same `expect.json` checks (`python tools/check_guides.py from-ironpdf`).

| Task | Before | After | What changed |
|---|---|---|---|
| T01 Hello world | [before](T01-hello-world/before/Program.cs) | [after](T01-hello-world/after/Program.cs) | `ChromePdfRenderer.RenderHtmlAsPdf` becomes `HtmlToPdf.ConvertHtmlString`, `SaveAs` becomes `Save` plus `Close`, and the licence key line goes. |
| T02 HTML string | [before](T02-html-string/before/Program.cs) | [after](T02-html-string/after/Program.cs) | The same HTML. The IronPDF sample also sets the two `Installation` properties from IronPDF's Docker guide; the SelectPdf project has nothing in their place. |
| T03 URL | [before](T03-url/before/Program.cs) | [after](T03-url/after/Program.cs) | `RenderUrlAsPdf` becomes `ConvertUrl`. |
| T04 Page setup | [before](T04-page-setup/before/Program.cs) | [after](T04-page-setup/after/Program.cs) | `PaperSize` and `PaperOrientation` become `PdfPageSize` and `PdfPageOrientation`; the 20 mm margins become 57 points. |
| T05 Header and footer | [before](T05-header-footer/before/Program.cs) | [after](T05-header-footer/after/Program.cs) | Each `TextHeaderFooter` becomes a centred `PdfTextSection` in `converter.Header` / `converter.Footer`, which are switched on and given a height; `{page}` and `{total-pages}` become `{page_number}` and `{total_pages}`. |
| T06 Merge | [before](T06-merge/before/Program.cs) | [after](T06-merge/after/Program.cs) | `PdfDocument.Merge` becomes a new `PdfDocument` with `Append` for each source; all three documents are closed. |
| T07 Watermark | [before](T07-watermark/before/Program.cs) | [after](T07-watermark/after/Program.cs) | `ApplyWatermark` with an HTML `<h1>` becomes a page-sized template holding a `PdfTextElement`: red, bold serif, 24 pt, 50 % opacity, centred and turned 45 degrees clockwise (`Direction = 315`). |
| T08 Fill a form | [before](T08-fill-form/before/Program.cs) | [after](T08-fill-form/after/Program.cs) | `pdf.Form.FindFormField(...).Value` becomes `PdfFormManager` with typed fields: the check box takes `Checked = true` instead of its "on" value `Yes`. |
| T09 Extract text | [before](T09-extract-text/before/Program.cs) | [after](T09-extract-text/after/Program.cs) | `ExtractAllText` becomes `PdfToText.Load` + `GetText`. |
| T10 PDF to image | [before](T10-pdf-to-image/before/Program.cs) | [after](T10-pdf-to-image/after/Program.cs) | `RasterizeToImageFiles` with page index 0 becomes `PdfRasterizer` with page number 1, `Resolution = 150`; the images come back as byte arrays and the project writes `page-1.png` itself. |
| T11 Digital signature | [before](T11-digital-signature/before/Program.cs) | [after](T11-digital-signature/after/Program.cs) | The same self-signed `.pfx` file. `PdfSignature` + `Sign` becomes a `PdfDigitalSignatureElement` added to page 1 at a 200 × 50 pt rectangle. |
| T12 PDF/A | [before](T12-pdfa/before/Program.cs) | [after](T12-pdfa/after/Program.cs) | `SaveAsPdfA(path, PdfAVersions.PdfA2b)` becomes `ConvertToPdfA(PdfStandard.PdfA2B)` + `Save`; both outputs pass veraPDF's PDF/A-2b check. |
| T13 ASP.NET Core | [before](T13-aspnet-core/before/Program.cs) | [after](T13-aspnet-core/after/Program.cs) | `pdf.BinaryData` becomes `doc.Save()`, which returns the bytes; the endpoint is otherwise the same. |

## What does not carry over

- **Converters SelectPdf does not have.** IronPDF renders Markdown
  ([Markdown to PDF](https://ironpdf.com/how-to/md-to-pdf/)), RTF ([RTF to PDF](https://ironpdf.com/how-to/rtf-to-pdf/)),
  zipped HTML ([HTML ZIP file to PDF](https://ironpdf.com/how-to/html-zip-file-to-pdf/)) and images
  ([image to PDF](https://ironpdf.com/how-to/image-to-pdf/)) directly. SelectPdf has no such methods: convert
  Markdown or RTF to HTML first, or place images on a page with
  [`PdfImageElement`](https://selectpdf.com/demo-universal/PdfImageElement).
- **Editing features without an equivalent.** IronPDF's text find-and-replace
  ([guide](https://ironpdf.com/how-to/find-replace-text/)), printing ([guide](https://ironpdf.com/how-to/print-pdf/)),
  barcode stamps (`BarcodeStamper`, [stamping guide](https://ironpdf.com/how-to/stamp-text-image/)),
  linearization ([guide](https://ironpdf.com/how-to/linearize-pdf/)) and PDF to HTML export
  ([guide](https://ironpdf.com/how-to/pdf-to-html/)) have no counterpart in the
  [SelectPdf reference](https://selectpdf.com/pdf-library/). The nearest SelectPdf feature to text replacement is
  redaction with an overlay text
  ([`PdfRedactionManager`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfRedactionManager.htm)),
  which deletes the matched text and writes a short text, at most 12 pt, centred in the filled area.
- **Header and footer placeholders.** IronPDF's text headers also accept `{url}`, `{date}`, `{time}`,
  `{html-title}` and `{pdf-title}` ([headers and footers](https://ironpdf.com/how-to/headers-and-footers/));
  SelectPdf's documented placeholders are `{page_number}` and `{total_pages}`
  ([headers and footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm)). Put the date,
  title or URL into the header text from your own code.
- **Header layout.** `TextHeaderFooter` has `LeftText`, `CenterText` and `RightText` on one line, plus
  `DrawDividerLine`; in SelectPdf each piece of text is its own `PdfTextSection` with a position and
  `HorizontalAlign`, and there is no divider-line option: draw one with CSS in a `PdfHtmlSection` if you need it
  ([`PdfTextSection`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfTextSection.htm),
  [`PdfHtmlSection`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfHtmlSection.htm)).
- **Headers on an existing PDF.** IronPDF stamps headers and footers on a loaded document with `AddTextHeaders` /
  `AddHtmlHeaders`. In SelectPdf, `doc.Header` and `doc.Footer` work only for new documents or new pages; on a
  loaded document the header and footer go into templates added with `doc.Templates.Add`
  ([demo](https://selectpdf.com/demo-universal/ExistingPdfHeadersAndFooters)).
- **Watermark input.** `ApplyWatermark` takes an HTML fragment and positions it for you (rotation clockwise,
  opacity 0 to 100). SelectPdf takes an element at coordinates in points, so the centring is computed in the
  project ([T07 after](T07-watermark/after/Program.cs)); `Direction` turns counter-clockwise. HTML content on a
  page is drawn with [`PdfHtmlElement`](https://selectpdf.com/demo-universal/PdfHtmlElement).
- **Units and indexes.** Margins move from millimetres (doubles) to points (integers); render delays move from
  milliseconds to seconds; rasterizer pages move from 0-based indexes to 1-based numbers. Set margins explicitly
  rather than relying on either library's defaults
  ([IronPDF custom margins](https://ironpdf.com/how-to/custom-margins/),
  [SelectPdf `MarginLeft`](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_MarginLeft.htm),
  [SelectPdf `ConversionDelay`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm)).
- **Form check boxes.** IronPDF sets a check box by its "on" value (`"Yes"`); SelectPdf uses
  `PdfFormFieldCheckBox.Checked` ([form filling demo](https://selectpdf.com/demo-universal/PdfFormFilling)).
- **Signature placement.** IronPDF's `Sign` needs no page or rectangle; SelectPdf's signature is an element added
  to a page at a rectangle ([digital signatures demo](https://selectpdf.com/demo-universal/DigitalSignatures)).
- **Output is not byte-identical.** Both engines are Chromium-based, but versions, default fonts and page layout
  settings differ (SelectPdf lays the page out at `WebPageWidth`, 1024 px by default, and scales it to the page
  width: [`HtmlToPdfOptions`](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm)).
  Compare a few real documents side by side before switching.
- **Platforms.** SelectPdf runs on Windows (x64, x86), Linux (x64, ARM64, glibc 2.30 or later) and macOS 13+ on
  Apple Silicon; on an Intel Mac, HTML to PDF, PDF to text and PDF to image are unavailable
  ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm), [SelectPdf.Universal.Linux on NuGet](https://www.nuget.org/packages/SelectPdf.Universal.Linux/26.5.0)). IronPDF lists Windows, macOS and
  Linux ([NuGet](https://www.nuget.org/packages/IronPdf/2026.10.3)).
- **Commercial-only features.** SelectPdf's free Community Edition (`SelectPdf.HtmlToPdf.Universal`) is HTML to PDF
  only, at most 5 pages per document. Every other task in this guide (merge, stamps, forms, text extraction,
  rasterizing, signatures, PDF/A) needs the commercial `SelectPdf.Universal` licence, as do Word to PDF,
  redaction, PDF/UA and ZUGFeRD ([Community Edition](https://selectpdf.com/community-edition/),
  [pricing](https://selectpdf.com/pricing/)).

## Benchmark

<!-- benchmark-pair: generated, do not edit -->

IronPDF and SelectPdf on every axis of the [2026-10-15 benchmark run](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md), generated from its raw data (same machine, Windows and Linux; methodology, per-page numbers and every other library are on that page). Lower is better for times and memory, higher for throughput, checks and fidelity.

|  | IronPDF | SelectPdf |
|---|---|---|
| HTML to PDF, geometric mean of 18 page medians, ms (Windows) | 186 | 212 |
| Pages converted faster than the other library (Windows) | 14 of 18 | 4 of 18 |
| Cold start, ms (Windows) | 657 | 714 |
| Throughput, conversions/min, 1 / 4 / 8 at once, sync (Windows) | 480 / 1,485 / 1,735 | 353 / 948 / 1,078 |
| Throughput, conversions/min, 1 / 4 / 8 at once, async (Windows) | 447 / 1,456 / 1,705 | 323 / 926 / 1,158 |
| Memory, one page / 8 at once, MB (Windows) | 244 / 788 | 205 / 467 |
| Memory over 1,000 conversions, MB (Windows) | 226 → 630, 0 failures | 140 → 156, 0 failures |
| Page that never loads: gives up after, s (Windows) | 66 | 60 |
| Parallel output equals sequential (Windows) | 32 / 32 | 32 / 32 |
| HTML to PDF, geometric mean of 18 page medians, ms (Linux) | 169 | 134 |
| Pages converted faster than the other library (Linux) | 4 of 18 | 14 of 18 |
| Cold start, ms (Linux) | 1,090 | 745 |
| Throughput, conversions/min, 1 / 4 / 8 at once, sync (Linux) | 453 / 1,332 / 1,520 | 619 / 1,929 / 2,597 |
| Throughput, conversions/min, 1 / 4 / 8 at once, async (Linux) | 468 / 1,231 / 1,704 | 603 / 1,872 / 2,639 |
| Memory, one page / 8 at once, MB (Linux) | 730 / 1,322 | 462 / 727 |
| Memory over 1,000 conversions, MB (Linux) | 451 → 1,076, 0 failures | 369 → 389, 0 failures |
| Page that never loads: gives up after, s (Linux) | 66 | 60 |
| Parallel output equals sequential (Linux) | 31 / 32 | 32 / 32 |
| Content checks passed (Windows) | 41 / 48 | 46 / 48 |
| Content checks passed (Linux) | 41 / 48 | 46 / 48 |
| Fidelity vs Chrome, mean SSIM (Windows) | 0.928 | 0.984 |
| PDF/A, PDF/UA, PDF/X: levels produced / valid (of 17) | 14 / 5 | 17 / 17 |
| Tagged structure checks | 42 / 112 | 112 / 112 |
| Text extraction, lowest F1 | 0.96 | 0.97 |
| Search | not supported | supported |
| PDF to image | supported | supported |
| Word to PDF | supported | supported |
| Redaction | supported | supported |
| Merge | supported | supported |
| Split | supported | supported |
| Watermark | supported | supported |
| Digital signature | supported | supported |
| Form filling | supported | supported |
| ZUGFeRD / Factur-X | supported | supported |
| Docker, minimal image (Linux) | runs as root only, 206 MB | runs, 270 MB |
| Docker, vendor-documented image (Linux) | runs, 239 MB | not needed (the minimal image runs) |
| Publish size win-x64 / linux-x64, MB | 321.3 / 314.6 | 477.0 / 449.6 |

## Deployment

- **Windows.** Both libraries ship their engine as NuGet packages; nothing else is installed. SelectPdf needs one
  Native package per build, chosen by runtime identifier
  ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)).
- **Linux and Docker.** IronPDF's Docker guide installs a list of Chromium dependencies with `apt-get` and sets
  `LinuxAndDockerDependenciesAutoConfig = false` (GPU acceleration is off by default)
  ([guide](https://ironpdf.com/get-started/ironpdf-docker/)). SelectPdf's Docker guide uses the official
  Debian and Ubuntu based .NET images with no `apt-get` step, publishing with `-r linux-x64`; base images have no
  fonts, and `SelectPdf.Universal.Fonts` adds Noto fonts for scripts beyond Latin, Greek and Cyrillic
  ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)). Compare the two
  Dockerfiles: [IronPDF](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/ironpdf/docker),
  [SelectPdf](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/docker), and the measured
  images in the [benchmark's deployment section](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md#deployment).
- **Alpine.** Neither library runs on Alpine directly: IronPDF points to its separate `IronPdfEngine` container
  ([Docker guide](https://ironpdf.com/get-started/ironpdf-docker/)), SelectPdf to its remote engine container
  ([Remote Engine](https://selectpdf.com/pdf-library/html/Remote-Engine.htm)).
- **Windows containers.** SelectPdf needs a Windows Server Core image with the core Windows fonts; Nano Server is
  not supported ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)).
