# From wkhtmltopdf (Haukcode.WkHtmlToPdfDotNet, DinkToPdf) to SelectPdf

Runnable projects for moving a .NET application from a wkhtmltopdf wrapper to
[SelectPdf.Universal](https://www.nuget.org/packages/SelectPdf.Universal/26.5.0) 26.5.0. The `before/` projects use
[Haukcode.WkHtmlToPdfDotNet](https://www.nuget.org/packages/Haukcode.WkHtmlToPdfDotNet/1.5.95) 1.5.95; DinkToPdf
has the same settings classes and is covered in [DinkToPdf](#dinktopdf).

> Maintained by SelectPdf, the vendor of the library this guide migrates to. SelectPdf also publishes the
> compatibility packages used in the `compat/` folders. The neutral comparison of both libraries is in
> [csharp-pdf/pdf-libraries-compared](https://github.com/csharp-pdf/pdf-libraries-compared)
> ([wkhtmltopdf](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf),
> [SelectPdf](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf)). SelectPdf's own articles
> on the subject: [wkhtmltopdf alternative](https://selectpdf.com/comparisons/wkhtmltopdf-alternative/),
> [DinkToPdf alternative](https://selectpdf.com/comparisons/dinktopdf-alternative/).

## Who it is for

.NET projects that convert HTML or URLs to PDF through `libwkhtmltox` and want a maintained engine: the wkhtmltopdf
repository is [archived](https://github.com/wkhtmltopdf/wkhtmltopdf), its last release is 0.12.6 (2020-06-10), and
its Qt WebKit engine was, in the project's words, "deprecated in 2015 and removed in 2016"
([status](https://wkhtmltopdf.org/status.html)).

Staying is reasonable when your documents are simple, render correctly today, come only from HTML you control, and
your deployment already works: the wrapper is free (LGPL), and the conversions you have keep working as long as
the native library loads on your servers. Note the project's own warning before relying on that: "Do not use
wkhtmltopdf with any untrusted HTML" ([status](https://wkhtmltopdf.org/status.html)).

## Price and licensing

| | Haukcode.WkHtmlToPdfDotNet / wkhtmltopdf | SelectPdf |
|---|---|---|
| Licence | LGPL-3.0-or-later (wrapper), LGPL-3.0 (wkhtmltopdf) ([nuget.org](https://www.nuget.org/packages/Haukcode.WkHtmlToPdfDotNet/1.5.95), [wkhtmltopdf](https://github.com/wkhtmltopdf/wkhtmltopdf)) | Commercial, perpetual ([Pricing](https://selectpdf.com/pricing/), [EULA](https://selectpdf.com/eula/)) |
| Price | Free | From USD 499 (Single Developer: 1 developer, 1 deployment machine, 1 year of maintenance); one key covers SelectPdf.Universal and the classic library ([Pricing](https://selectpdf.com/pricing/)) |
| Free use | Open source | Community Edition (`SelectPdf.HtmlToPdf.Universal`): "Free for any use, including production", up to 5 PDF pages per document, HTML to PDF ([Community Edition](https://selectpdf.com/community-edition/)) |
| Trial | Not applicable | Fully functional, with a "Demo Version" banner on each page until a key is set ([Licensing](https://selectpdf.com/pdf-library/html/License.htm)) |
| Compatibility packages | Not applicable | `SelectPdf.Compat.WkHtmlToPdfDotNet` and `SelectPdf.Compat.DinkToPdf`: MIT licensed, built on the Community Edition, free for commercial use up to 5 pages per document. The `.Licensed` variants are the same code on SelectPdf.Universal, with no page limit, and need a SelectPdf licence; without a key the output carries a demo watermark ([README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf#packages), [.Licensed README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet.Licensed/README.md)) |

All licensing pages read on 2026-10-16.

## Two routes

| | Route 1: compatibility package (`compat/`) | Route 2: the SelectPdf API (`after/`) |
|---|---|---|
| Package | [SelectPdf.Compat.WkHtmlToPdfDotNet](https://www.nuget.org/packages/SelectPdf.Compat.WkHtmlToPdfDotNet/1.0.0) 1.0.0 (or [SelectPdf.Compat.DinkToPdf](https://www.nuget.org/packages/SelectPdf.Compat.DinkToPdf/1.0.0)) plus one engine package | `SelectPdf.Universal` plus one engine package |
| Your code | Keeps the wrapper's namespace and types (`IConverter`, `SynchronizedConverter`, `HtmlToPdfDocument`, `GlobalSettings`, `ObjectSettings`) | Rewritten against `HtmlToPdf` and `HtmlToPdfOptions` |
| Layout | Margins, header and footer placement and wkhtmltopdf's content scale are translated by the package | You set margins, header bands and `WebPageWidth` yourself (see [Mapping](#mapping)) |
| Rendering | Chromium, through SelectPdf | Chromium |
| Licence | Free package on the free Community Edition, up to 5 pages per document; `.Licensed` variant on SelectPdf.Universal | Commercial licence, or the free Community Edition for HTML to PDF up to 5 pages |
| Beyond HTML to PDF | No | Merge, watermark, forms, text extraction, signatures, PDF/A (see [What you can add](#what-you-can-add)) |

Choose the compatibility package to move off `libwkhtmltox` first and keep the converter code, settings and tests
you have. Choose the SelectPdf API when you also want the features the wrapper API has no settings for, or when
you are already rewriting the PDF code. The two can follow each other: `SelectPdfCompat.Configure` passes the
underlying SelectPdf converter to your code before each conversion, for options the wrapper API lacks
([package README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#other-selectpdf-options)).

What the compatibility packages do, from their
[README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf#readme): they keep the API of DinkToPdf and
Haukcode.WkHtmlToPdfDotNet, with the wrapper's own namespaces and types, and render with Chromium through SelectPdf
instead of Qt WebKit; CSS sizes are scaled the way wkhtmltopdf's smart shrinking scaled them, headers and footers
are placed where wkhtmltopdf put them, and `[page]` / `[topage]` work in header and footer text. Settings with no
SelectPdf equivalent are ignored and reported through the `Warning` event. In this guide, each `compat/` project
has the `before/` project's `Program.cs` with three comment lines added; its `Sample.csproj` references the
compatibility package and an engine package instead of Haukcode.WkHtmlToPdfDotNet.

## Packages and setup

**Before** ([Sample.csproj](T01-hello-world/before/Sample.csproj)):

```xml
<PackageReference Include="Haukcode.WkHtmlToPdfDotNet" Version="1.5.95" />
```

Namespaces `WkHtmlToPdfDotNet` and `WkHtmlToPdfDotNet.Contracts`. No licence key. The package ships `libwkhtmltox`
for win-x64, win-x86, linux-x64, linux-x86, linux-arm64 and osx-x64
([comparison repository](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf#facts)).
It renders with Qt WebKit (`libwkhtmltox` 0.12.6); SelectPdf renders with Chromium 154.0.8037.58 (CEF 154; Chromium
154 stable since 2026-09-22), shipped in its engine package.

**Route 1, compatibility package** ([Sample.csproj](T01-hello-world/compat/Sample.csproj)), with the engine
package picked by platform as in the
[package README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#migrate):

```xml
<PropertyGroup>
  <SelectPdfRid>$(RuntimeIdentifier)</SelectPdfRid>
  <SelectPdfRid Condition="'$(SelectPdfRid)' == '' And
      $([MSBuild]::IsOSPlatform('Windows'))">win-x64</SelectPdfRid>
  <SelectPdfRid Condition="'$(SelectPdfRid)' == ''">linux-x64</SelectPdfRid>
</PropertyGroup>
<ItemGroup>
  <PackageReference Include="SelectPdf.Compat.WkHtmlToPdfDotNet" Version="1.0.0" />
  <PackageReference Include="SelectPdf.Universal.Native.$(SelectPdfRid)" Version="26.5.0" />
</ItemGroup>
```

Namespaces unchanged (`WkHtmlToPdfDotNet`). The free package depends on
[SelectPdf.HtmlToPdf.Universal](https://www.nuget.org/packages/SelectPdf.HtmlToPdf.Universal/26.5.0) 26.5.0 (the
Community Edition) and needs no licence key. The engine packages are `SelectPdf.Universal.Native.<rid>` for
`win-x64`, `win-x86`, `linux-x64`, `linux-arm64` and `osx-arm64`; reference one per build, because they all install
into the same folder ([README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf#quick-start)). Delete the
`libwkhtmltox` files and any code that loaded them.

**Route 2, SelectPdf API** ([Sample.csproj](T01-hello-world/after/Sample.csproj)):

```xml
<PackageReference Include="SelectPdf.Universal" Version="26.5.0" />
<PackageReference Include="SelectPdf.Universal.Native.win-x64" Version="26.5.0"
                  Condition="'$(RuntimeIdentifier)' == 'win-x64' Or '$(RuntimeIdentifier)' == ''" />
<PackageReference Include="SelectPdf.Universal.Native.linux-x64" Version="26.5.0"
                  Condition="'$(RuntimeIdentifier)' == 'linux-x64'" />
```

Namespace `SelectPdf.Universal`. A single per-platform package (`SelectPdf.Universal.Windows`, `.Linux`,
`.Linux.Arm64`, `.MacOS`) can replace the two references
([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)). Without a licence key the library runs
as a trial and adds a "Demo Version" banner to each page; the key is set with `GlobalProperties.LicenseKey`
([Licensing](https://selectpdf.com/pdf-library/html/License.htm)). The projects here set no key. For HTML to PDF of
up to 5 pages, the free Community Edition (`SelectPdf.HtmlToPdf.Universal`) uses the same `SelectPdf.Universal`
namespace ([README, About SelectPdf](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf#about-selectpdf)).
For .NET Framework projects SelectPdf.Universal also runs on .NET Framework 4.6.1+ through .NET Standard 2.0; the
classic Windows-only `Select.Pdf` library is the other option there.

## Mapping

Haukcode.WkHtmlToPdfDotNet settings to `HtmlToPdf` / `HtmlToPdfOptions` (`converter.Options`). SelectPdf property
names are from the [HtmlToPdfOptions reference](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm);
wkhtmltopdf options and defaults from the [wkhtmltopdf manual](https://wkhtmltopdf.org/usage/wkhtmltopdf.txt).
The compatibility package's own mapping is in its
[README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#how-settings-map).

| Haukcode.WkHtmlToPdfDotNet | SelectPdf.Universal | Notes |
|---|---|---|
| `new BasicConverter(new PdfTools())` / `SynchronizedConverter`, `converter.Convert(doc)` → `byte[]` | `new HtmlToPdf()`, `ConvertHtmlString(html)` / `ConvertUrl(url)` → `PdfDocument`; `doc.Save()` → `byte[]`, `doc.Save(path)`, `doc.Close()` | |
| `ObjectSettings.HtmlContent` | `ConvertHtmlString(html)` (an overload takes a base URL for relative links) | [ConvertHtmlCodeToPdf](https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm) |
| `ObjectSettings.Page` (URL or file) | `ConvertUrl(url)` | [ConvertUrlToPdf](https://selectpdf.com/pdf-library/html/ConvertUrlToPdf.htm) |
| Several `Objects` | One conversion per object, then `PdfDocument.Append` | as in the [merge sample](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T06-merge) |
| `GlobalSettings.PaperSize = PaperKind.A4` | `PdfPageSize = PdfPageSize.A4` | Both default to A4 ([PdfPageSize](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_PdfPageSize.htm), manual) |
| `PaperSize = new PechkinPaperSize(w, h)` | `PdfPageSize = PdfPageSize.Custom`, `PdfPageCustomSize` | |
| `GlobalSettings.Orientation` | `PdfPageOrientation` | |
| `GlobalSettings.Margins` (`MarginSettings`, millimetres unless `Unit` says otherwise) | `MarginTop`, `MarginBottom`, `MarginLeft`, `MarginRight`: integers in **points** (1 mm = 2.83 pt; 10 mm = 28, 20 mm = 57) | `MarginSettings()` sets `Unit = Millimeters` ([source](https://github.com/HakanL/WkHtmlToPdf-DotNet/blob/master/src/WkHtmlToPdf-DotNet/Settings/MarginSettings.cs)); points per [MarginLeft](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_MarginLeft.htm). **Defaults differ:** wkhtmltopdf 10 mm, SelectPdf 0 (see below) |
| `HeaderSettings` / `FooterSettings` `Left`, `Center`, `Right` | `Options.DisplayHeader = true`, `converter.Header.Add(new PdfTextSection(x, y, text, font))` with `HorizontalAlign`; same for `Footer` | [Headers and footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm) |
| `[page]`, `[topage]` | `{page_number}`, `{total_pages}` in a `PdfTextSection` | The only two documented placeholders |
| `FontName`, `FontSize` (points) | the section's `PdfFont`, e.g. `PdfFont.CreateStandardFont(PdfStandardFont.Helvetica, 9)` | |
| `HeaderSettings.Spacing` (millimetres) | header band `Height` (points) and the section's `y` | |
| `HeaderSettings.Line` | no line section type; [T05](T05-header-footer/after/Program.cs) draws the rule with a small `PdfHtmlSection` | |
| `HeaderSettings.HtmlUrl` | `new PdfHtmlSection(url)` | |
| `GlobalSettings.PageOffset` | `Header.FirstPageNumber` / `Footer.FirstPageNumber` | |
| `WebSettings.EnableIntelligentShrinking` (smart shrinking, on by default), `LoadSettings.ZoomFactor` | `WebPageWidth` (pixels, default 1024) | See [Content scale](#content-scale) |
| `GlobalSettings.ViewportSize` | `WebPageWidth`, `WebPageHeight` | |
| `WebSettings.PrintMediaType` | `CssMediaType = HtmlToPdfCssMediaType.Print` | Default is `Screen` in both |
| `WebSettings.EnableJavascript` | `JavaScriptEnabled` | |
| `WebSettings.Background` | `DrawBackground` | |
| `WebSettings.UserStyleSheet` (a URL or path) | `CustomCSS` (the CSS text) | |
| `LoadSettings.JSDelay` (milliseconds, default 200) | `ConversionDelay` (**seconds**, default 0) | Defaults: manual (`--javascript-delay`); SelectPdf value read in our run |
| `LoadSettings.Username`, `Password` | `Authentication.Username`, `Authentication.Password` | |
| `LoadSettings.Proxy` | `ProxyOptions` (`HostName`, `PortNumber`, `Type`, `Username`, `Password`) | |
| `LoadSettings.CustomHeaders`, `Cookies`, `Post` | `HttpHeaders`, `HttpCookies`, `HttpPostParameters` | |
| `LoadSettings.BlockLocalFileAccess` | `DenyLocalFileAccess` (default false) | |
| `GlobalSettings.ColorMode = ColorMode.Grayscale` | `ColorSpace = PdfColorSpace.GrayScale` | |
| `GlobalSettings.DocumentTitle` | `PdfDocumentInformation.Title` | |
| `GlobalSettings.Outline`, `OutlineDepth` | `PdfBookmarkOptions.CssSelectors` (e.g. `h1, h2`) | |
| `GlobalSettings.ImageQuality` | `JpegCompressionLevel` | |
| `GlobalSettings.ImageDPI` | `ImagesMaxResolution` (pixels per inch) | |
| `GlobalSettings.UseCompression` | `PdfCompressionLevel` | |
| `ObjectSettings.ProduceForms` | `CreateFormFields` | |
| `ObjectSettings.UseExternalLinks`, `UseLocalLinks` | `ExternalLinksEnabled`, `InternalLinksEnabled` | |
| `TableOfContentsSettings` | `TableOfContents.Mode = PdfTableOfContentsMode.GeneratePages` | [PdfTableOfContentsMode](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfTableOfContentsMode.htm) |
| `converter.Error` event, empty array on failure | `HtmlToPdfException` thrown | See [What does not carry over](#what-does-not-carry-over) |

### Margins and header bands

wkhtmltopdf draws the header and footer inside the top and bottom margins. SelectPdf reserves the header band in
addition to the margin: "Page coordinates start inside the page margins and below the band the document header
reserves" ([Headers and footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm)). To keep the
content where it was, make margin + band height equal the wkhtmltopdf margin: [T05](T05-header-footer/after/Program.cs)
uses `MarginTop = 28` and `Header.Height = 29` for wkhtmltopdf's 20 mm (57 pt) top margin. SelectPdf's margins
default to 0 (read from `HtmlToPdfOptions` in our run); wkhtmltopdf's default to 10 mm, which the `after/` projects
set explicitly as 28 points.

### Content scale

wkhtmltopdf with smart shrinking (its default) draws content smaller than its CSS size: the compatibility package's
source records that "CSS 100mm prints 79.8mm"
([ConversionPlan.cs](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/Shared/Internal/ConversionPlan.cs)).
SelectPdf lays the page out in a browser window `WebPageWidth` pixels wide (default 1024) and scales the result to
fill the PDF page between the margins
([WebPageWidth](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WebPageWidth.htm)).
The two scales differ, so after moving to the SelectPdf API, text and boxes come out at a different size unless you
set `WebPageWidth`. Measured with a 100 mm wide CSS box on A4 portrait with 10 mm margins, in our runs:

| | Box width in the PDF |
|---|---|
| wkhtmltopdf 0.12.6 (Haukcode.WkHtmlToPdfDotNet), smart shrinking on | 226.3 pt (79.8 mm) |
| SelectPdf.Compat.WkHtmlToPdfDotNet | 226.5 pt |
| SelectPdf.Universal, `WebPageWidth` 1024 (default) | 199.1 pt (70.2 mm) |
| SelectPdf.Universal, `WebPageWidth = 900` | 226.5 pt |

To match smart shrinking, set `WebPageWidth` to the content width in points × 96 / 72 / 0.798: 900 for A4 portrait
with 10 mm margins, 1216 for A4 landscape with 20 mm margins. With `EnableIntelligentShrinking = false`
wkhtmltopdf keeps CSS sizes; the equivalent is the content width at 96 pixels per inch (719 for A4 portrait with
10 mm margins), which is what the WebPageWidth reference recommends "to print at 100%". The `after/` projects keep
SelectPdf's default width, so their text is smaller than in `before/`; the `compat/` projects match `before/`.
With smart shrinking, wkhtmltopdf shrinks content wider than the page further (same source); in SelectPdf,
content wider than `WebPageWidth` is cut at the right edge unless `FitWideContent = true`
([WebPageWidth](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WebPageWidth.htm)).

## Task by task

Each task folder has `before/` (Haukcode.WkHtmlToPdfDotNet, from the
[comparison sample](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf/samples)),
`after/` (SelectPdf.Universal, from SelectPdf's
[comparison sample](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples)) and
`compat/` (the `before/` code on SelectPdf.Compat.WkHtmlToPdfDotNet). All three pass the same `expect.json`.

| Task | before | after | compat |
|---|---|---|---|
| T01 Hello world | [before](T01-hello-world/before/) | [after](T01-hello-world/after/) | [compat](T01-hello-world/compat/) |
| T02 HTML string | [before](T02-html-string/before/) | [after](T02-html-string/after/) | [compat](T02-html-string/compat/) |
| T03 URL | [before](T03-url/before/) | [after](T03-url/after/) | [compat](T03-url/compat/) |
| T04 Page setup | [before](T04-page-setup/before/) | [after](T04-page-setup/after/) | [compat](T04-page-setup/compat/) |
| T05 Header and footer | [before](T05-header-footer/before/) | [after](T05-header-footer/after/) | [compat](T05-header-footer/compat/) |
| T13 ASP.NET Core | [before](T13-aspnet-core/before/) | [after](T13-aspnet-core/after/) | [compat](T13-aspnet-core/compat/) |

- **T01 Hello world.** `HtmlToPdfDocument` with one `ObjectSettings.HtmlContent` becomes `ConvertHtmlString`, and
  the `byte[]` written with `File.WriteAllBytes` becomes `doc.Save(path)`. The `after/` project sets A4 and the
  10 mm margins wkhtmltopdf uses by default.
- **T02 HTML string.** The same HTML in both. `WebSettings.DefaultEncoding` has no `HtmlToPdfOptions` counterpart;
  the HTML declares its charset with `<meta charset="utf-8">`.
- **T03 URL.** `ObjectSettings.Page` becomes `ConvertUrl`.
- **T04 Page setup.** `Orientation.Landscape` and `MarginSettings(20, 20, 20, 20) { Unit = Unit.Millimeters }`
  become `PdfPageOrientation.Landscape` and margins of 57 points.
- **T05 Header and footer.** The text header and footer become `PdfTextSection` objects in `converter.Header` and
  `converter.Footer`, `Page [page] of [topage]` becomes `Page {page_number} of {total_pages}`, and the 20 mm margins
  are split into a 28 pt margin plus a 29 pt band (see [Margins and header bands](#margins-and-header-bands)).
  `HeaderSettings.Line` is drawn by a one-line `PdfHtmlSection`.
- **T13 ASP.NET Core.** The `before/` project registers a `SynchronizedConverter` singleton, as the wrapper requires;
  the `after/` project creates an `HtmlToPdf` per request, as SelectPdf's sample does, and returns `doc.Save()`.
  The `compat/` project keeps the singleton registration.

## What you can add

wkhtmltopdf takes web pages as input and writes one PDF; it cannot open an existing PDF
([comparison repository](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf#supported-tasks)),
and neither can the compatibility packages, which keep its API. With the SelectPdf API these tasks are new rather
than migrated; the verified samples are in the comparison repository:

| Task | SelectPdf sample |
|---|---|
| T06 Merge PDFs | [T06-merge](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T06-merge) |
| T07 Watermark / stamp | [T07-watermark](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T07-watermark) |
| T08 Fill a PDF form | [T08-fill-form](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T08-fill-form) |
| T09 Extract text | [T09-extract-text](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T09-extract-text) |
| T10 PDF to image | [T10-pdf-to-image](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T10-pdf-to-image) |
| T11 Digital signature | [T11-digital-signature](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T11-digital-signature) |
| T12 PDF/A output | [T12-pdfa](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T12-pdfa) |
| T14 Linux / Docker | [docker](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/docker) (see [Deployment](#deployment)) |

These need SelectPdf.Universal; the Community Edition does not extract text, convert PDF pages to images or edit
existing PDFs ([Community Edition](https://selectpdf.com/community-edition/)).

## What does not carry over

- **Default margins.** wkhtmltopdf uses 10 mm; SelectPdf's margins are 0 unless set, and are integers in points.
  Code that never set margins gets content to the page edge after moving to the SelectPdf API.
- **Content scale.** Smart shrinking has no switch in SelectPdf; set `WebPageWidth` as in
  [Content scale](#content-scale), or the output comes out at a different size.
- **Header and footer variables.** Only `{page_number}` and `{total_pages}` are documented, in `PdfTextSection`
  text ([Headers and footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm)). wkhtmltopdf's
  `[date]`, `[isodate]`, `[time]`, `[title]`, `[doctitle]` and `[webpage]` have no placeholder: put the value into
  the text yourself. `[frompage]`, `[section]`, `[subsection]`, `[sitepage]` and `[sitepages]` have no equivalent.
  The compatibility package substitutes the value itself for `[date]`, `[title]`, `[webpage]` and similar
  ([README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#how-settings-map)).
- **HTML header and footer pages.** wkhtmltopdf calls the `HtmlUrl` page with `?page=&topage=` query parameters.
  A `PdfHtmlSection` gets no page parameters, and the placeholders are documented only for text sections, so use a
  `PdfTextSection` for page numbers. The compatibility package behaves the same way
  ([README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#what-behaves-differently)).
- **Header text size on Windows.** In our Windows run, wkhtmltopdf drew `FontSize = 9` header text at 11.2 pt and
  its body text larger than on Linux, where the same projects gave 9 pt; SelectPdf and the compatibility package
  gave 9 pt on both. Compare Linux output if your layout depends on it.
- **Settings with no equivalent.** `Copies`, `Collate`, `DPI`, `MinimumFontSize`, `LoadImages`,
  `StopSlowScript`, `DebugJavascript`, `LoadErrorHandling` and `DumpOutline` have no `HtmlToPdfOptions`
  counterpart. The compatibility package ignores `Copies`, `DPI` and `MinimumFontSize` with a `Warning`
  ([README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#how-settings-map)).
- **Table of contents and cover pages.** With the SelectPdf API, `TableOfContents.Mode = GeneratePages` draws a
  table of contents on pages of its own from the headings it matches; a cover is a separate conversion appended in
  front. The compatibility package puts `CoverSettings` first, but skips `TableOfContentsSettings` with a `Warning`
  and no TOC page.
- **Several objects.** wkhtmltopdf joins the objects of one `HtmlToPdfDocument` into one PDF. With the SelectPdf
  API, convert each and `Append` them. The free compatibility package joins HTML objects into one document, but a
  document with several objects where one is a URL fails, because it needs PDF merging; the `.Licensed` package
  converts and merges them. Only the first object's header, footer and web settings apply to the whole document
  ([README, Limits](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#limits)).
- **Failures.** The wrapper returns an empty array and raises `Error`
  ([comparison repository](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf#gotchas)).
  SelectPdf throws: in our run, `ConvertUrl` on an unresolvable host threw `HtmlToPdfException` ("Domain name cannot
  be resolved"). The compatibility package keeps the empty array and the `Error` event unless
  `SelectPdfCompat.ThrowOnError = true`.
- **Waiting for JavaScript.** wkhtmltopdf waits 200 ms after load by default; SelectPdf's `ConversionDelay` is 0
  and in whole seconds. Pages that draw charts after load may need `ConversionDelay = 1` or `WindowStatus`.
  `NavigationTimeout` (60 seconds in our run) bounds the page load.
- **Concurrency.** `SynchronizedConverter` runs every conversion on one thread
  ([README](https://github.com/HakanL/WkHtmlToPdf-DotNet#synchronized-converter)). SelectPdf converts several at
  once, up to `HtmlToPdfOptions.MaximumConcurrentConversions` (default 8,
  [reference](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_MaximumConcurrentConversions.htm));
  the compatibility package's `SynchronizedConverter` no longer queues conversions on one thread.
- **Rendering.** Chromium lays pages out like a current browser: layouts that worked around WebKit's missing flexbox
  and grid support render as written, page breaks follow CSS `break-before`, `break-after` and `break-inside`, and
  text metrics differ slightly ([README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#what-behaves-differently)).
  Compare a sample of your real documents before switching.
- **Untrusted HTML.** Local file access is allowed by default in SelectPdf as well (`DenyLocalFileAccess` defaults
  to false, [reference](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfOptions.htm)); set it
  to true when converting HTML you do not control.
- **Page limit on the free routes.** The free compatibility package and the Community Edition stop at 5 pages per
  document; the free compatibility package throws `InvalidOperationException` with the page count unless
  `SelectPdfCompat.FailOnPageLimit = false`, which keeps the first 5 pages and raises a `Warning`
  ([README, Limits](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#limits)).

## Benchmark

<!-- benchmark-pair: generated, do not edit -->

wkhtmltopdf and SelectPdf on every axis of the [2026-10-15 benchmark run](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md), generated from its raw data (same machine, Windows and Linux; methodology, per-page numbers and every other library are on that page). Lower is better for times and memory, higher for throughput, checks and fidelity.

|  | wkhtmltopdf | SelectPdf |
|---|---|---|
| HTML to PDF, geometric mean of 18 page medians, ms (Windows) | 421 | 212 |
| Pages converted faster than the other library (Windows) | 3 of 18 | 15 of 18 |
| Cold start, ms (Windows) | 356 | 714 |
| Throughput, conversions/min, 1 / 4 / 8 at once, sync (Windows) | 209 / 199 / 201 | 353 / 948 / 1,078 |
| Throughput, conversions/min, 1 / 4 / 8 at once, async (Windows) | 204 / 194 / 198 | 323 / 926 / 1,158 |
| Memory, one page / 8 at once, MB (Windows) | 94 / 122 | 205 / 467 |
| Memory over 1,000 conversions, MB (Windows) | 84 → 529, 0 failures | 140 → 156, 0 failures |
| Page that never loads: gives up after, s (Windows) | 315 (never returned) | 60 |
| Parallel output equals sequential (Windows) | 32 / 32 | 32 / 32 |
| HTML to PDF, geometric mean of 18 page medians, ms (Linux) | 324 | 134 |
| Pages converted faster than the other library (Linux) | 3 of 18 | 15 of 18 |
| Cold start, ms (Linux) | 321 | 745 |
| Throughput, conversions/min, 1 / 4 / 8 at once, sync (Linux) | 255 / 249 / 252 | 619 / 1,929 / 2,597 |
| Throughput, conversions/min, 1 / 4 / 8 at once, async (Linux) | 252 / 255 / 248 | 603 / 1,872 / 2,639 |
| Memory, one page / 8 at once, MB (Linux) | 188 / 194 | 462 / 727 |
| Memory over 1,000 conversions, MB (Linux) | 195 → 609, 0 failures | 369 → 389, 0 failures |
| Page that never loads: gives up after, s (Linux) | 315 (never returned) | 60 |
| Parallel output equals sequential (Linux) | 32 / 32 | 32 / 32 |
| Content checks passed (Windows) | 30 / 42 | 46 / 48 |
| Content checks passed (Linux) | 27 / 42 | 46 / 48 |
| Fidelity vs Chrome, mean SSIM (Windows) | 0.809 | 0.984 |
| PDF/A, PDF/UA, PDF/X: levels produced / valid (of 17) | 0 / 0 | 17 / 17 |
| Tagged structure checks | – | 112 / 112 |
| Text extraction, lowest F1 | not supported | 0.97 |
| Search | not supported | supported |
| PDF to image | not supported | supported |
| Word to PDF | not supported | supported |
| Redaction | not supported | supported |
| Merge | not supported | supported |
| Split | not supported | supported |
| Watermark | not supported | supported |
| Digital signature | not supported | supported |
| Form filling | not supported | supported |
| ZUGFeRD / Factur-X | not supported | supported |
| Docker, minimal image (Linux) | does not run, 124 MB | runs, 270 MB |
| Docker, vendor-documented image (Linux) | image does not build | not needed (the minimal image runs) |
| Publish size win-x64 / linux-x64, MB | 122.2 / 126.3 | 477.0 / 449.6 |

## Deployment

**wkhtmltopdf.** The wrapper's NuGet package carries `libwkhtmltox` per platform, but on Linux the library needs
system libraries: on a bare `mcr.microsoft.com/dotnet/runtime:8.0` image the package throws
`NotSupportedException` ("missing native dependencies (libjpeg62, etc)"). The wrapper README's Docker recipe
installs `libgdiplus`, fontconfig, X11 libraries and a wkhtmltox 0.12.5 package built for Debian 9 with
`libssl1.1`; on the Debian 12 based .NET 8 image it stops with "Dependency is not satisfiable: libssl1.1", so the
comparison repository has no working wkhtmltopdf Docker sample
([wkhtmltopdf T14](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf#supported-tasks),
[Dockerfile used](https://github.com/csharp-pdf/pdf-benchmarks/blob/main/deployment/wkhtmltopdf/documented.Dockerfile),
[deployment results](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md#deployment)).
The `before/` projects of this guide ran on Ubuntu 24.04 with `libjpeg62` installed.

**SelectPdf.** The [Docker sample](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/docker)
is a multi-stage build on the official .NET images, published with `-r linux-x64`, with no `apt-get` step: the
engine package bundles the Chromium libraries
([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)). The same holds for the
compatibility packages, whose README shows the same kind of Dockerfile and says to remove the `apt-get` lines
added for wkhtmltopdf ([README, Docker](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.WkHtmlToPdfDotNet/README.md#docker)).
Differences to plan for:

- Linux needs glibc 2.30 or later (Debian 11+, Ubuntu 20.04+, RHEL 9+); Alpine (musl) images convert only through
  the separate remote engine container ([SelectPdf.Universal.Linux on NuGet](https://www.nuget.org/packages/SelectPdf.Universal.Linux/26.5.0),
  [Remote Engine](https://selectpdf.com/pdf-library/html/Remote-Engine.htm)).
- Base images have no fonts; the built-in fonts cover Latin, Greek and Cyrillic, and `SelectPdf.Universal.Fonts` adds others
  ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)).
- Platforms: Windows x64/x86, Linux x64/ARM64, macOS on Apple Silicon. wkhtmltopdf's package also has linux-x86 and
  osx-x64 binaries, which SelectPdf has no engine for
  ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)).
- One engine package per build: a project developed on Windows and deployed to Linux picks the package by runtime
  identifier and is published per platform, as in the `Sample.csproj` files here.

The `before/`, `after/` and `compat/` projects of T01 to T05 also built and passed their checks on Ubuntu 24.04
(WSL), with `after/` built for `linux-x64`.

## DinkToPdf

[DinkToPdf](https://www.nuget.org/packages/DinkToPdf/1.0.8) (last release 1.0.8, 2017-04-18) has the same settings
classes as Haukcode.WkHtmlToPdfDotNet, which is a fork of it, under the `DinkToPdf` namespace; it does not bundle
the native library ([comparison repository](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/wkhtmltopdf#gotchas)).
The [Mapping](#mapping) table applies to it too, except for the members only the WkHtmlToPdfDotNet API has
(`CoverSettings`, `TableOfContentsSettings`, `LoadSettings.Post`), and the header URL property is spelled `HtmUrl`
([compatibility packages README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf#building-and-testing),
[DinkToPdf package README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.DinkToPdf/README.md#how-settings-map)).

For Route 1 use [SelectPdf.Compat.DinkToPdf](https://www.nuget.org/packages/SelectPdf.Compat.DinkToPdf/1.0.0)
(or [SelectPdf.Compat.DinkToPdf.Licensed](https://www.nuget.org/packages/SelectPdf.Compat.DinkToPdf.Licensed/1.0.0)),
which keeps the `DinkToPdf` namespace and types. Its
[README](https://github.com/selectpdf/selectpdf-compat-wkhtmltopdf/blob/main/src/SelectPdf.Compat.DinkToPdf/README.md)
adds one step to those above: delete the `libwkhtmltox.*` files from the project and the `CustomAssemblyLoadContext`
code that loaded them. SelectPdf's article for this case:
[DinkToPdf alternative](https://selectpdf.com/comparisons/dinktopdf-alternative/).
