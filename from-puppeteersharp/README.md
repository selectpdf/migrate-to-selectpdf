# Migrate from PuppeteerSharp to SelectPdf

## Who it is for

.NET projects that start a browser with PuppeteerSharp only to print HTML or a URL to PDF with `page.PdfAsync`,
and want to make PDFs with `SelectPdf.Universal` 26.5.0 instead. Staying with PuppeteerSharp is the better choice
when the same browser also runs tests, scraping or other automation, when a PDF is printed after a scripted session
(clicks, form entries, waiting on application state) that SelectPdf's options cannot express, when you need Firefox
or an Intel Mac, or when you want a free MIT-licensed tool with no page limit.

## Price and licensing

| | PuppeteerSharp | SelectPdf |
|---|---|---|
| Licence | MIT ([LICENSE](https://github.com/hardkoded/puppeteer-sharp/blob/master/LICENSE)). The downloaded browser is Google's Chrome for Testing build, which the package's MIT licence does not cover ([Chrome for Testing](https://developer.chrome.com/blog/chrome-for-testing)) | Commercial, perpetual ([EULA](https://selectpdf.com/eula/)) |
| Price | Free | From USD 499 (Single Developer: 1 developer, 1 deployment machine, 1 year of maintenance); OEM licences remove the deployment-machine limit ([Pricing](https://selectpdf.com/pricing/)) |
| Free use | No limits | 30-day trial, fully functional, with a "Demo Version" banner on every page until a key is set ([Licensing](https://selectpdf.com/pdf-library/html/License.htm)). The separate free Community Edition (`SelectPdf.HtmlToPdf.Universal`) converts HTML to PDF only, up to 5 pages per document ([Community Edition](https://selectpdf.com/community-edition/)) |

Vendor pages read on 2026-10-16.

## Packages and setup

| | Before: PuppeteerSharp | After: SelectPdf |
|---|---|---|
| HTML engine version | Chrome for Testing 154.0.8037.57 for PuppeteerSharp 25.13.1 (stable since 2026-09-22) | Chromium 154.0.8037.58 (CEF 154; Chromium 154 stable since 2026-09-22), in the engine package |
| NuGet | `PuppeteerSharp` 25.13.1 ([nuget.org](https://www.nuget.org/packages/PuppeteerSharp/25.13.1)) | `SelectPdf.Universal` 26.5.0 plus one engine package, `SelectPdf.Universal.Native.<rid>` (win-x64, win-x86, linux-x64, linux-arm64, osx-arm64), or one per-platform package (`SelectPdf.Universal.Windows`, `.Linux`, `.Linux.Arm64`, `.MacOS`) ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)) |
| Browser / engine | Not in the package: `BrowserFetcher.DownloadAsync()` downloads Chrome for Testing (and Chrome Headless Shell) into the application folder at run time, about 435 MB plus 273 MB per folder on Windows x64 ([README](https://github.com/hardkoded/puppeteer-sharp#generate-pdf-files), [comparison page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/puppeteersharp#gotchas)) | The Chromium engine is in the Native package and is restored with the other NuGet packages; there is no download step ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)) |
| Linux | The Debian/Ubuntu libraries listed in Puppeteer's [troubleshooting guide](https://pptr.dev/troubleshooting#chrome-doesnt-launch-on-linux) | No OS packages; glibc 2.30 or later ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm), [SelectPdf.Universal.Linux on NuGet](https://www.nuget.org/packages/SelectPdf.Universal.Linux/26.5.0)) |
| Licence key | None | None for the trial, which adds a "Demo Version" banner to every page; a purchased key is set in code with `GlobalProperties.LicenseKey` before the first conversion, for example read from an environment variable or your configuration. Keys issued for versions before 26 are refused ([Licensing](https://selectpdf.com/pdf-library/html/License.htm), [comparison page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf#gotchas)) |
| Namespace | `PuppeteerSharp`, `PuppeteerSharp.Media` | `SelectPdf.Universal` |

The projects here reference `SelectPdf.Universal` plus the engine package chosen by runtime identifier, as in the
"Publishing for Several Platforms" section of the
[Installation](https://selectpdf.com/pdf-library/html/Installation.htm) page: a build without a runtime identifier
gets the win-x64 engine, and `dotnet publish -r linux-x64` gets the Linux one. Only one engine package may be in a
build.

## Mapping

Units: PuppeteerSharp takes margins and paper sizes as strings with units (`"20mm"`, `"1in"`, `"96px"`); SelectPdf
takes margins, header and footer heights and custom page sizes as numbers in **points** (1/72 inch), and the layout
width in **pixels** (96 per inch). Timeouts are milliseconds in PuppeteerSharp and **seconds** in SelectPdf.

### Browser, page and loading

| PuppeteerSharp | SelectPdf | Notes |
|---|---|---|
| `new BrowserFetcher().DownloadAsync()`, `Puppeteer.LaunchAsync(...)` | nothing to call | The engine comes with the Native package; `ChromiumEnginePath` points to another copy if needed ([HtmlToPdfOptions](https://selectpdf.com/pdf-library/html/Properties_T_SelectPdf_Universal_HtmlToPdfOptions.htm)). |
| `browser.NewPageAsync()`, then `page` / `browser` disposal | `new HtmlToPdf()` per conversion; `PdfDocument.Close()` after saving | [Convert Html Code To Pdf](https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm) |
| `page.SetContentAsync(html)` + `page.PdfAsync(path)` | `converter.ConvertHtmlString(html)` + `doc.Save(path)` | `ConvertHtmlString(html, baseUrl)` resolves relative links. |
| `page.GoToAsync(url)` + `page.PdfAsync(path)` | `converter.ConvertUrl(url)` + `doc.Save(path)` | [Convert Url To Pdf](https://selectpdf.com/pdf-library/html/ConvertUrlToPdf.htm) |
| `page.PdfDataAsync()` / `page.PdfStreamAsync()` | `doc.Save()` (byte array) / `doc.Save(stream)` | |
| async methods | `ConvertHtmlStringAsync`, `ConvertUrlAsync` (with a `CancellationToken` overload) | [Asynchronous Conversion](https://selectpdf.com/pdf-library/html/AsyncConversion.htm) |
| `NavigationOptions.Timeout`, `page.DefaultNavigationTimeout` (ms) | `Options.NavigationTimeout` (s, default 60); `Options.MaxConversionTime` (s, the print phase, default 60); `Options.RenderPageOnTimeout` | [Conversion Delay and Timeout](https://selectpdf.com/pdf-library/html/ConversionDelay.htm) |
| `WaitUntil = WaitUntilNavigation.Load` | the default: the converter waits for the page to load | `DOMContentLoaded`, `Networkidle0` and `Networkidle2` have no equivalent. |
| a fixed wait after loading (`Task.Delay` before `PdfAsync`) | `Options.ConversionDelay` (s) | Replaces the obsolete `MinPageLoadTime` ([ConversionDelay](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_ConversionDelay.htm)). |
| `WaitForFunctionAsync(...)`, `WaitForSelectorAsync(...)` | `Options.WindowStatus` (waits for `window.status` to equal a value) or `Options.StartupMode = HtmlToPdfStartupMode.Manual` and `selectpdf.start()` in the page | The page itself has to signal readiness ([WindowStatus](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WindowStatus.htm), [Start Conversion from Javascript](https://selectpdf.com/pdf-library/html/ConvertFromJs.htm)). |
| `EvaluateExpressionAsync(...)`, `EvaluateFunctionOnNewDocumentAsync(...)` | `Options.PostLoadingScript` (after load) / `Options.StartupScript` (before the page's scripts) | Scripts run in the page; no value is returned to .NET. |
| `AddStyleTagAsync(...)` | `Options.CustomCSS` | |
| `ClickAsync(selector)` | `Options.ClickElementsSelectors` (+ `ClickElementsDelayBefore` / `After`, ms) | Clicks only, before the conversion. |
| `TypeAsync` + `ClickAsync` on a login form | `Options.LoginOptions` (login page URL, field and button selectors, user name, password) | [LoginOptions](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_LoginOptions.htm) |
| `AuthenticateAsync(credentials)` | `Options.Authentication.Username` / `.Password` | [Web Page Authentication](https://selectpdf.com/pdf-library/html/WebPageAuthentication.htm) |
| `SetCookieAsync(...)`, `SetExtraHttpHeadersAsync(...)` | `Options.HttpCookies`, `Options.HttpHeaders` | [HTTP Cookies](https://selectpdf.com/pdf-library/html/HTTPCookies.htm), [HTTP Headers](https://selectpdf.com/pdf-library/html/HTTPHeaders.htm) |
| `SetJavaScriptEnabledAsync(false)` | `Options.JavaScriptEnabled = false` | Default true in both. |
| `SetViewportAsync(new ViewPortOptions { Width = ... })` | `Options.WebPageWidth` / `WebPageHeight` (px) | In SelectPdf the width also sets the scale (below). |
| `EmulateMediaTypeAsync(MediaType.Screen / Print)` | `Options.CssMediaType = HtmlToPdfCssMediaType.Screen / Print` | Defaults differ: page.pdf prints with print media, SelectPdf uses screen ([Media Types](https://selectpdf.com/pdf-library/html/MediaTypes.htm)). |

### PdfOptions

| PdfOptions ([reference](https://www.puppeteersharp.com/api/PuppeteerSharp.PdfOptions.html)) | SelectPdf `converter.Options` / `converter.Header` / `converter.Footer` | Notes |
|---|---|---|
| `Format = PaperFormat.Letter` (default) | `PdfPageSize = PdfPageSize.Letter` | SelectPdf's default is A4 ([PdfPageSize](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_PdfPageSize.htm)). |
| `Format = PaperFormat.A4` and others | `PdfPageSize = PdfPageSize.A4`, ... | A0 to A10, B0 to B5, Letter, Ledger and more ([Setting Pdf Page Properties](https://selectpdf.com/pdf-library/html/PdfPageProperties.htm)). |
| `Width`, `Height` | `PdfPageSize = PdfPageSize.Custom` + `PdfPageCustomSize` (points) | |
| `Landscape = true` | `PdfPageOrientation = PdfPageOrientation.Landscape` | |
| `MarginOptions` (`"20mm"`; default none) | `MarginLeft` / `MarginRight` / `MarginTop` / `MarginBottom` (integer points; default 0) | 20 mm = 57 pt ([MarginTop](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_MarginTop.htm)). |
| page laid out at the paper width | `WebPageWidth` = width between the margins in px (default 1024) | SelectPdf lays the page out at `WebPageWidth` and scales it to the space between the margins; 816 for Letter, 793 for A4 portrait with no margins ([WebPageWidth](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WebPageWidth.htm)). |
| `Scale` (0.1 to 2) | `WebPageWidth` = width between the margins in px ÷ scale | For example `Scale = 0.8` on Letter without margins: 816 ÷ 0.8 = 1020. |
| `PrintBackground` (default false) | no equivalent; SelectPdf renders backgrounds | See [What does not carry over](#what-does-not-carry-over). |
| `OmitBackground` | `DrawBackground = false` | Leaves out the page background ([DrawBackground](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_DrawBackground.htm)). |
| `PageRanges = "1-5, 8"` | `PageRanges = "1-5,8"` | |
| `DisplayHeaderFooter = true` | `DisplayHeader = true`, `DisplayFooter = true` | |
| `HeaderTemplate`, `FooterTemplate` (HTML) | `converter.Header.Add(new PdfHtmlSection(x, y, html, baseUrl))`, `converter.Footer.Add(...)`; `Header.Height` / `Footer.Height` in points (default 45) | [Headers and Footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm) |
| `<span class="pageNumber">`, `<span class="totalPages">` | `{page_number}`, `{total_pages}` in a `PdfTextSection` | `Header.FirstPageNumber` and `TotalPagesOffset` shift the numbers. |
| `date`, `title`, `url` classes | none | Put the value into the section's HTML or text from your code. |
| `PreferCSSPageSize = true` | none documented | Set the size with `PdfPageSize` / `PdfPageCustomSize`. |
| `Outline = true` | `PdfBookmarkOptions.CssSelectors = new[] { "h1", "h2" }` (+ `HierarchicalBookmarks = true`) | Bookmarks from elements you select ([Automatic Bookmarks](https://selectpdf.com/pdf-library/html/AutomaticBookmarks.htm)). |
| `Tagged` (default **true** in PuppeteerSharp 25) | `Tagged = true` (default false), with `Title` and `Language` | [PdfOptions.cs](https://github.com/hardkoded/puppeteer-sharp/blob/v25.13.1/lib/PuppeteerSharp/PdfOptions.cs), [Tagged](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_Tagged.htm) |
| `Timeout` (ms) | `MaxConversionTime` (s) | |
| `WaitForFonts` (default true) | no separate option documented | |

## Task by task

Every `after/` project sets the four settings that give it the layout `page.pdf` uses by default: Letter paper,
no margins (SelectPdf's default too), print CSS media (`CssMediaType = Print`) and the page laid out at the paper
width (`WebPageWidth = 816`). It also sets `Tagged = true`, PuppeteerSharp 25's default. The `before/` projects are
the comparison repository's verified samples, unchanged.

| Task | Before | After | What changed |
|---|---|---|---|
| T01 Hello world | [before](T01-hello-world/before/Program.cs) | [after](T01-hello-world/after/Program.cs) | `BrowserFetcher`, `LaunchAsync`, `NewPageAsync`, `SetContentAsync` and `PdfAsync` become `new HtmlToPdf()`, `ConvertHtmlString` and `Save`. The code is synchronous; the async methods are listed in the mapping. |
| T02 HTML string | [before](T02-html-string/before/Program.cs) | [after](T02-html-string/after/Program.cs) | The same HTML string goes to `ConvertHtmlString`. With the default-layout settings above, the text lands at the same positions on the page. |
| T03 URL | [before](T03-url/before/Program.cs) | [after](T03-url/after/Program.cs) | `GoToAsync` and the wait for `document.fonts.ready` become `ConvertUrl`, which waits for the page to load (`NavigationTimeout`, 60 seconds by default). |
| T04 Page setup | [before](T04-page-setup/before/Program.cs) | [after](T04-page-setup/after/Program.cs) | `Format`, `Landscape` and the `"20mm"` margins become `PdfPageSize`, `PdfPageOrientation` and margins of 57 points. `WebPageWidth = 971` (257 mm at 96 px per inch) lays the page out at the width between the margins, as the browser does. |
| T05 Header and footer | [before](T05-header-footer/before/Program.cs) | [after](T05-header-footer/after/Program.cs) | The 25 mm margins that hold the templates become a header and a footer 71 points high. The header template becomes a `PdfHtmlSection`; the footer becomes a `PdfTextSection` with `{page_number}` and `{total_pages}`, in 7.5 pt Helvetica (the template's 10 px). |
| T13 ASP.NET Core | [before](T13-aspnet-core/before/Program.cs) | [after](T13-aspnet-core/after/Program.cs) | The application-wide browser and the page per request go; each request creates an `HtmlToPdf` and returns `doc.Save()` with `Results.File`. |

## What you can add

PuppeteerSharp has no API for opening or changing PDF files: `page.PdfAsync` prints the current page
([PdfOptions](https://www.puppeteersharp.com/api/PuppeteerSharp.PdfOptions.html),
[comparison page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/puppeteersharp#supported-tasks)).
Projects that need the tasks below usually pair PuppeteerSharp with a second PDF library. SelectPdf covers them in
the same package; the verified samples are in the comparison repository.

| Task | SelectPdf sample |
|---|---|
| T06 Merge PDFs | [T06-merge](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T06-merge) |
| T07 Watermark / stamp | [T07-watermark](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T07-watermark) |
| T08 Fill a PDF form | [T08-fill-form](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T08-fill-form) |
| T09 Extract text | [T09-extract-text](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T09-extract-text) |
| T10 PDF to image | [T10-pdf-to-image](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T10-pdf-to-image) |
| T11 Digital signature | [T11-digital-signature](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T11-digital-signature) |
| T12 PDF/A (PDF/A-2b) | [T12-pdfa](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf/samples/T12-pdfa) |

These are features of the commercial `SelectPdf.Universal` package; the free Community Edition converts HTML to PDF
only ([Community Edition](https://selectpdf.com/community-edition/)).

## What does not carry over

- **Scripting the page.** PuppeteerSharp drives a live page: any sequence of clicks, typing, navigation and
  `EvaluateExpressionAsync` calls that return values to .NET, then a print. SelectPdf takes the page as options:
  scripts injected before or after load (`StartupScript`, `PostLoadingScript`), clicks on selectors
  (`ClickElementsSelectors`), one form login (`LoginOptions`) and a readiness signal from the page (`WindowStatus`,
  `selectpdf.start()`). A multi-step flow (log in, search, open a result, then print) does not map; keep the browser
  for it, or render the final page's HTML in your application and pass it to `ConvertHtmlString`.
- **Waiting conditions.** `WaitUntil` network-idle states, `WaitForSelectorAsync` and `WaitForFunctionAsync` have no
  direct equivalent. The page has to set `window.status` or call `selectpdf.start()`, or the conversion waits a fixed
  `ConversionDelay` after load.
- **Browser contexts and sessions.** Incognito contexts, cookies collected by earlier navigation and per-context
  permissions do not exist; each conversion gets the cookies and headers set in its options
  ([HTTP Cookies](https://selectpdf.com/pdf-library/html/HTTPCookies.htm)).
- **Defaults.** `page.pdf` prints with print media on Letter paper at the paper width, and drops CSS backgrounds;
  SelectPdf converts with screen media on A4 at a 1024 px layout width scaled to the page, and keeps backgrounds
  ([Puppeteer PDFOptions](https://pptr.dev/api/puppeteer.pdfoptions), [Media Types](https://selectpdf.com/pdf-library/html/MediaTypes.htm),
  [WebPageWidth](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WebPageWidth.htm)).
  Set `PdfPageSize`, `CssMediaType` and `WebPageWidth` as the `after/` projects do, or the output changes size and
  scale.
- **`PrintBackground = false`.** SelectPdf has no option that leaves out element backgrounds; `DrawBackground` is
  about the page background. Pages that relied on backgrounds being dropped can inject a rule such as
  `* { background: none !important; }` through `CustomCSS`.
- **`emulateMedia` beyond the media type.** `EmulateMediaFeaturesAsync` (for example `prefers-color-scheme`) has no
  equivalent; `CssMediaType` chooses between `Screen` and `Print` only
  ([HtmlToPdfCssMediaType](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfCssMediaType.htm)).
- **`PreferCSSPageSize`.** SelectPdf's documentation sets the page size only with `PdfPageSize` and
  `PdfPageCustomSize`; a page that declares its size in `@page` needs that size repeated in code.
- **Header and footer templates.** A template mixes HTML and the `pageNumber` / `totalPages` / `date` / `title` /
  `url` classes in one fragment. In SelectPdf, page numbers come only from the `{page_number}` and `{total_pages}`
  placeholders of a `PdfTextSection`, drawn with a PDF font rather than CSS; the HTML part goes in a separate
  `PdfHtmlSection` placed in the same header or footer, and `date`, `title` and `url` are filled in by your code.
  `PdfHtmlSection` lays its HTML out at 1024 px by default, so set its `WebPageWidth` (as T05 does) to keep the
  template's font sizes. A `PdfHtmlSection` is rendered as its own HTML document: its scripts run
  (`JavaScriptEnabled`, default true), and, as with a template, it sees only the styles in its own HTML, not those of
  the converted page ([PdfHtmlSection](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfHtmlSection.htm)).
- **Tagging and outline.** PuppeteerSharp 25 tags every PDF unless `Tagged = false`; SelectPdf tags only with
  `Tagged = true`. `Outline` builds the outline from headings; SelectPdf builds bookmarks from the CSS selectors in
  `PdfBookmarkOptions`.
- **Firefox and Intel Macs.** PuppeteerSharp can drive Firefox and runs on Intel Macs; SelectPdf renders with its
  bundled Chromium and supports macOS on Apple Silicon only
  ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)).
- **Licence.** PuppeteerSharp is free under MIT with no page or watermark limits; SelectPdf is commercial, and its
  trial adds a banner to every page (see [Licensing](#price-and-licensing)).

## Benchmark

<!-- benchmark-pair: generated, do not edit -->

PuppeteerSharp and SelectPdf on every axis of the [2026-10-15 benchmark run](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md), generated from its raw data (same machine, Windows and Linux; methodology, per-page numbers and every other library are on that page). Lower is better for times and memory, higher for throughput, checks and fidelity.

|  | PuppeteerSharp | SelectPdf |
|---|---|---|
| HTML to PDF, geometric mean of 18 page medians, ms (Windows) | 486 | 212 |
| Pages converted faster than the other library (Windows) | 0 of 18 | 18 of 18 |
| Cold start, ms (Windows) | 1,003 | 714 |
| Throughput, conversions/min, 1 / 4 / 8 at once, sync (Windows) | 125 / 297 / 325 | 353 / 948 / 1,078 |
| Throughput, conversions/min, 1 / 4 / 8 at once, async (Windows) | 127 / 309 / 346 | 323 / 926 / 1,158 |
| Memory, one page / 8 at once, MB (Windows) | 418 / 1,461 | 205 / 467 |
| Memory over 1,000 conversions, MB (Windows) | 325 → 584, 0 failures | 140 → 156, 0 failures |
| Page that never loads: gives up after, s (Windows) | 30 | 60 |
| Parallel output equals sequential (Windows) | 32 / 32 | 32 / 32 |
| HTML to PDF, geometric mean of 18 page medians, ms (Linux) | 150 | 134 |
| Pages converted faster than the other library (Linux) | 4 of 18 | 14 of 18 |
| Cold start, ms (Linux) | 528 | 745 |
| Throughput, conversions/min, 1 / 4 / 8 at once, sync (Linux) | 450 / 797 / 798 | 619 / 1,929 / 2,597 |
| Throughput, conversions/min, 1 / 4 / 8 at once, async (Linux) | 471 / 798 / 838 | 603 / 1,872 / 2,639 |
| Memory, one page / 8 at once, MB (Linux) | 498 / 789 | 462 / 727 |
| Memory over 1,000 conversions, MB (Linux) | 398 → 683, 0 failures | 369 → 389, 0 failures |
| Page that never loads: gives up after, s (Linux) | 30 | 60 |
| Parallel output equals sequential (Linux) | 32 / 32 | 32 / 32 |
| Content checks passed (Windows) | 38 / 42 | 46 / 48 |
| Content checks passed (Linux) | 38 / 42 | 46 / 48 |
| Fidelity vs Chrome, mean SSIM (Windows) | 0.949 | 0.984 |
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
| Docker, minimal image (Linux) | does not run, 79 MB | runs, 270 MB |
| Docker, vendor-documented image (Linux) | – | not needed (the minimal image runs) |
| Publish size win-x64 / linux-x64, MB | 8.4 / 8.3 | 477.0 / 449.6 |

## Deployment

| | PuppeteerSharp | SelectPdf |
|---|---|---|
| Windows | `DownloadAsync()` stores the browser in the application folder; installing it before the application runs is recommended. Since Chromium 125, PDF generation on Windows servers needs sandbox permissions on the browser folder, which PuppeteerSharp tries to set after the download ([Issues generating PDF files](https://github.com/hardkoded/puppeteer-sharp/blob/master/docfx_project/docs/IssuesGeneratingPdfFiles.md)) | `SelectPdf.Universal.Native.win-x64` or `win-x86`; Windows containers need Windows Server Core with the core fonts, not Nano Server ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)) |
| Linux | Chrome's system libraries from Puppeteer's [troubleshooting guide](https://pptr.dev/troubleshooting#chrome-doesnt-launch-on-linux); linux-arm64 downloads since 25.13.0 ([release notes](https://github.com/hardkoded/puppeteer-sharp/releases/tag/v25.13.0)) | `SelectPdf.Universal.Native.linux-x64` or `linux-arm64`, glibc 2.30 or later, no OS packages. Alpine (musl) converts only through SelectPdf's separate [Remote Engine](https://selectpdf.com/pdf-library/html/Remote-Engine.htm) container. Base images have no fonts; the built-in fonts cover Latin, Greek and Cyrillic, and `SelectPdf.Universal.Fonts` adds Noto fonts for other scripts ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)) |
| Docker | [puppeteersharp/docker/Dockerfile](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/puppeteersharp/docker/Dockerfile): `apt-get install` of the library list, a non-root user, the browser downloaded at image build time, and Chrome's sandbox handled with `--cap-add=SYS_ADMIN` or `PUPPETEER_DANGEROUS_NO_SANDBOX=true` ([Running Puppeteer in Docker](https://pptr.dev/troubleshooting#running-puppeteer-in-docker)) | [selectpdf/docker/Dockerfile](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/selectpdf/docker/Dockerfile): the official `mcr.microsoft.com/dotnet/runtime:8.0` image, `dotnet publish -r linux-x64`, no `apt-get` step and no extra `docker run` flags |
| macOS | Supported | Apple Silicon only (`osx-arm64`); on an Intel Mac, HTML to PDF is unavailable ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)) |

Benchmark results for both libraries, including Docker image runs, are on the comparison repository's
[2026-10-15 results page](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md).
