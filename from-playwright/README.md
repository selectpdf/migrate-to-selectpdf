# Migrate from Playwright for .NET to SelectPdf

## Who it is for

.NET projects that launch Chromium with Playwright only to print HTML or a URL to PDF with `page.PdfAsync`, and
want to make PDFs with `SelectPdf.Universal` 26.5.0 instead. Staying with Playwright is the better choice when
Playwright already runs your end-to-end tests or other automation, when a PDF is printed after a scripted session
(clicks, form entries, waiting on application state) that SelectPdf's options cannot express, when you need an
Intel Mac, or when you want a free MIT/Apache-2.0 tool with no page limit.

## Packages and setup

| | Before: Playwright | After: SelectPdf |
|---|---|---|
| NuGet | `Microsoft.Playwright` 1.63.0 ([nuget.org](https://www.nuget.org/packages/Microsoft.Playwright/1.63.0)) | `SelectPdf.Universal` 26.5.0 plus one engine package, `SelectPdf.Universal.Native.<rid>` (win-x64, win-x86, linux-x64, linux-arm64, osx-arm64), or one per-platform package (`SelectPdf.Universal.Windows`, `.Linux`, `.Linux.Arm64`, `.MacOS`) ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)) |
| Browser / engine | Not in the package: `playwright.ps1 install chromium` (or `Microsoft.Playwright.Program.Main(["install", "chromium"])` from code) downloads Chromium and the Chromium headless shell to `%USERPROFILE%\AppData\Local\ms-playwright`, `~/.cache/ms-playwright` or `PLAYWRIGHT_BROWSERS_PATH`; each Playwright version needs its own browser build, so the install runs again after an upgrade ([Browsers](https://playwright.dev/dotnet/docs/browsers#managing-browser-binaries)) | The Chromium engine is in the Native package and is restored with the other NuGet packages; there is no install step ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)) |
| Linux | System packages installed with `install-deps` or `install --with-deps` ([Install system dependencies](https://playwright.dev/dotnet/docs/browsers#install-system-dependencies)) | No OS packages; glibc 2.30 or later ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)) |
| Licence key | None | None for the trial, which adds a "Demo Version" banner to every page; a purchased key is set in code with `GlobalProperties.LicenseKey` before the first conversion, for example read from an environment variable or your configuration. Keys issued for versions before 26 are refused ([Licensing](https://selectpdf.com/pdf-library/html/License.htm), [comparison page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/selectpdf#gotchas)) |
| Namespace | `Microsoft.Playwright` | `SelectPdf.Universal` |

The projects here reference `SelectPdf.Universal` plus the engine package chosen by runtime identifier, as in the
"Publishing for Several Platforms" section of the
[Installation](https://selectpdf.com/pdf-library/html/Installation.htm) page: a build without a runtime identifier
gets the win-x64 engine, and `dotnet publish -r linux-x64` gets the Linux one. Only one engine package may be in a
build.

## Mapping

Units: Playwright takes margins and paper sizes as strings with units (`"20mm"`, `"1in"`, `"96px"`; unlabeled
values are pixels); SelectPdf takes margins, header and footer heights and custom page sizes as numbers in
**points** (1/72 inch), and the layout width in **pixels** (96 per inch). Timeouts are milliseconds in Playwright and
**seconds** in SelectPdf.

### Browser, page and loading

| Playwright ([Page](https://playwright.dev/dotnet/docs/api/class-page)) | SelectPdf | Notes |
|---|---|---|
| `playwright install chromium`, `Playwright.CreateAsync()`, `playwright.Chromium.LaunchAsync()` | nothing to call | The engine comes with the Native package; `ChromiumEnginePath` points to another copy if needed ([HtmlToPdfOptions](https://selectpdf.com/pdf-library/html/Properties_T_SelectPdf_Universal_HtmlToPdfOptions.htm)). |
| `browser.NewPageAsync()`, `page.CloseAsync()` | `new HtmlToPdf()` per conversion; `PdfDocument.Close()` after saving | [Convert Html Code To Pdf](https://selectpdf.com/pdf-library/html/ConvertHtmlCodeToPdf.htm) |
| `page.SetContentAsync(html)` + `page.PdfAsync(new() { Path = path })` | `converter.ConvertHtmlString(html)` + `doc.Save(path)` | `ConvertHtmlString(html, baseUrl)` resolves relative links. |
| `page.GotoAsync(url)` + `page.PdfAsync(...)` | `converter.ConvertUrl(url)` + `doc.Save(path)` | [Convert Url To Pdf](https://selectpdf.com/pdf-library/html/ConvertUrlToPdf.htm) |
| `page.PdfAsync()` returning `byte[]` | `doc.Save()` (byte array) / `doc.Save(stream)` | |
| async methods | `ConvertHtmlStringAsync`, `ConvertUrlAsync` (with a `CancellationToken` overload) | [Asynchronous Conversion](https://selectpdf.com/pdf-library/html/AsyncConversion.htm) |
| `Timeout` (ms), `page.SetDefaultNavigationTimeout(...)` | `Options.NavigationTimeout` (s, default 60); `Options.MaxConversionTime` (s, the print phase, default 60); `Options.RenderPageOnTimeout` | [Conversion Delay and Timeout](https://selectpdf.com/pdf-library/html/ConversionDelay.htm) |
| `WaitUntil = WaitUntilState.Load` (default) | the default: the converter waits for the page to load | `DOMContentLoaded`, `NetworkIdle` and `Commit` have no equivalent. |
| a fixed wait after loading (`page.WaitForTimeoutAsync`, `Task.Delay`) | `Options.ConversionDelay` (s) | Replaces the obsolete `MinPageLoadTime` ([ConversionDelay](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_ConversionDelay.htm)). |
| `WaitForFunctionAsync(...)`, `WaitForSelectorAsync(...)`, `Locator.WaitForAsync()` | `Options.WindowStatus` (waits for `window.status` to equal a value) or `Options.StartupMode = HtmlToPdfStartupMode.Manual` and `selectpdf.start()` in the page | The page itself has to signal readiness ([WindowStatus](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WindowStatus.htm), [Start Conversion from Javascript](https://selectpdf.com/pdf-library/html/ConvertFromJs.htm)). |
| `EvaluateAsync(...)`, `AddInitScriptAsync(...)` | `Options.PostLoadingScript` (after load) / `Options.StartupScript` (before the page's scripts) | Scripts run in the page; no value is returned to .NET. |
| `AddStyleTagAsync(...)` | `Options.CustomCSS` | |
| `ClickAsync(selector)` | `Options.ClickElementsSelectors` (+ `ClickElementsDelayBefore` / `After`, ms) | Clicks only, before the conversion. |
| `FillAsync` + `ClickAsync` on a login form | `Options.LoginOptions` (login page URL, field and button selectors, user name, password) | [LoginOptions](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_LoginOptions.htm) |
| context option `HttpCredentials` | `Options.Authentication.Username` / `.Password` | [Web Page Authentication](https://selectpdf.com/pdf-library/html/WebPageAuthentication.htm) |
| `context.AddCookiesAsync(...)`, `SetExtraHTTPHeadersAsync(...)` | `Options.HttpCookies`, `Options.HttpHeaders` | [HTTP Cookies](https://selectpdf.com/pdf-library/html/HTTPCookies.htm), [HTTP Headers](https://selectpdf.com/pdf-library/html/HTTPHeaders.htm) |
| context option `JavaScriptEnabled = false` | `Options.JavaScriptEnabled = false` | Default true in both. |
| `SetViewportSizeAsync(width, height)` | `Options.WebPageWidth` / `WebPageHeight` (px) | In SelectPdf the width also sets the scale (below). |
| `EmulateMediaAsync(new() { Media = Media.Screen / Media.Print })` | `Options.CssMediaType = HtmlToPdfCssMediaType.Screen / Print` | Defaults differ: page.PdfAsync prints with print media, SelectPdf uses screen ([Media Types](https://selectpdf.com/pdf-library/html/MediaTypes.htm)). |

### page.PdfAsync options

| `PagePdfOptions` ([reference](https://playwright.dev/dotnet/docs/api/class-page#page-pdf)) | SelectPdf `converter.Options` / `converter.Header` / `converter.Footer` | Notes |
|---|---|---|
| `Format = "Letter"` (default) | `PdfPageSize = PdfPageSize.Letter` | SelectPdf's default is A4 ([PdfPageSize](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_PdfPageSize.htm)). |
| `Format = "A4"` and others | `PdfPageSize = PdfPageSize.A4`, ... | A0 to A10, B0 to B5, Letter, Ledger and more ([Setting Pdf Page Properties](https://selectpdf.com/pdf-library/html/PdfPageProperties.htm)). |
| `Width`, `Height` | `PdfPageSize = PdfPageSize.Custom` + `PdfPageCustomSize` (points) | |
| `Landscape = true` | `PdfPageOrientation = PdfPageOrientation.Landscape` | |
| `Margin` (`"20mm"`; default 0) | `MarginLeft` / `MarginRight` / `MarginTop` / `MarginBottom` (integer points; default 0) | 20 mm = 57 pt ([MarginTop](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_MarginTop.htm)). |
| page laid out at the paper width | `WebPageWidth` = width between the margins in px (default 1024) | SelectPdf lays the page out at `WebPageWidth` and scales it to the space between the margins; 816 for Letter, 793 for A4 portrait with no margins ([WebPageWidth](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WebPageWidth.htm)). |
| `Scale` (0.1 to 2) | `WebPageWidth` = width between the margins in px ÷ scale | For example `Scale = 0.8f` on Letter without margins: 816 ÷ 0.8 = 1020. |
| `PrintBackground` (default false) | no equivalent; SelectPdf renders backgrounds | See [What does not carry over](#what-does-not-carry-over). |
| `PageRanges = "1-5, 8"` | `PageRanges = "1-5,8"` | |
| `DisplayHeaderFooter = true` | `DisplayHeader = true`, `DisplayFooter = true` | |
| `HeaderTemplate`, `FooterTemplate` (HTML) | `converter.Header.Add(new PdfHtmlSection(x, y, html, baseUrl))`, `converter.Footer.Add(...)`; `Header.Height` / `Footer.Height` in points (default 45) | [Headers and Footers](https://selectpdf.com/pdf-library/html/HtmlToPdfHeadersAndFooters.htm) |
| `<span class="pageNumber">`, `<span class="totalPages">` | `{page_number}`, `{total_pages}` in a `PdfTextSection` | `Header.FirstPageNumber` and `TotalPagesOffset` shift the numbers. |
| `date`, `title`, `url` classes | none | Put the value into the section's HTML or text from your code. |
| `PreferCSSPageSize = true` | none documented | Set the size with `PdfPageSize` / `PdfPageCustomSize`. |
| `Outline = true` (default false) | `PdfBookmarkOptions.CssSelectors = new[] { "h1", "h2" }` (+ `HierarchicalBookmarks = true`) | Bookmarks from elements you select ([Automatic Bookmarks](https://selectpdf.com/pdf-library/html/AutomaticBookmarks.htm)). |
| `Tagged = true` (default false) | `Tagged = true` (default false), with `Title` and `Language` | [Tagged](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_Tagged.htm) |

## Task by task

Every `after/` project sets the four settings that give it the layout `page.PdfAsync` uses by default: Letter
paper, no margins (SelectPdf's default too), print CSS media (`CssMediaType = Print`) and the page laid out at the
paper width (`WebPageWidth = 816`). The `before/` projects are the comparison repository's verified samples,
unchanged.

| Task | Before | After | What changed |
|---|---|---|---|
| T01 Hello world | [before](T01-hello-world/before/Program.cs) | [after](T01-hello-world/after/Program.cs) | The browser install, `Playwright.CreateAsync`, `LaunchAsync`, `NewPageAsync`, `SetContentAsync` and `PdfAsync` become `new HtmlToPdf()`, `ConvertHtmlString` and `Save`. The code is synchronous; the async methods are listed in the mapping. |
| T02 HTML string | [before](T02-html-string/before/Program.cs) | [after](T02-html-string/after/Program.cs) | The same HTML string goes to `ConvertHtmlString`. With the default-layout settings above, the text lands at the same positions on the page. |
| T03 URL | [before](T03-url/before/Program.cs) | [after](T03-url/after/Program.cs) | `GotoAsync` becomes `ConvertUrl`, which waits for the page to load (`NavigationTimeout`, 60 seconds by default). |
| T04 Page setup | [before](T04-page-setup/before/Program.cs) | [after](T04-page-setup/after/Program.cs) | `Format = "A4"`, `Landscape` and the `"20mm"` margins become `PdfPageSize`, `PdfPageOrientation` and margins of 57 points. `WebPageWidth = 971` (257 mm at 96 px per inch) lays the page out at the width between the margins, as the browser does. |
| T05 Header and footer | [before](T05-header-footer/before/Program.cs) | [after](T05-header-footer/after/Program.cs) | The 25 mm margins that hold the templates become a header and a footer 71 points high. The header template becomes a `PdfHtmlSection`; the footer becomes a `PdfTextSection` with `{page_number}` and `{total_pages}`, in 7.5 pt Helvetica (the template's 10 px). |
| T13 ASP.NET Core | [before](T13-aspnet-core/before/Program.cs) | [after](T13-aspnet-core/after/Program.cs) | The application-wide browser and the page per request go; each request creates an `HtmlToPdf` and returns `doc.Save()` with `Results.File`. |

## What you can add

`page.PdfAsync` is Playwright's only PDF API: it prints the current page, and there is no API for opening or
changing PDF files ([page.PdfAsync](https://playwright.dev/dotnet/docs/api/class-page#page-pdf),
[comparison page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/playwright#supported-tasks)).
Projects that need the tasks below usually pair Playwright with a second PDF library. SelectPdf covers them in the
same package; the verified samples are in the comparison repository.

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

- **Scripting the page.** Playwright drives a live page: any sequence of clicks, typing, navigation and
  `EvaluateAsync` calls that return values to .NET, then a print. SelectPdf takes the page as options: scripts
  injected before or after load (`StartupScript`, `PostLoadingScript`), clicks on selectors
  (`ClickElementsSelectors`), one form login (`LoginOptions`) and a readiness signal from the page (`WindowStatus`,
  `selectpdf.start()`). A multi-step flow (log in, search, open a result, then print) does not map; keep the browser
  for it, or render the final page's HTML in your application and pass it to `ConvertHtmlString`.
- **Waiting conditions.** `WaitUntilState.NetworkIdle`, locators and `WaitForFunctionAsync` have no direct
  equivalent. The page has to set `window.status` or call `selectpdf.start()`, or the conversion waits a fixed
  `ConversionDelay` after load.
- **Browser contexts and storage state.** Contexts, saved storage state, permissions, geolocation and locale
  emulation do not exist; each conversion gets the cookies and headers set in its options
  ([HTTP Cookies](https://selectpdf.com/pdf-library/html/HTTPCookies.htm)).
- **Defaults.** `page.PdfAsync` prints with print media on Letter paper at the paper width, and drops CSS
  backgrounds; SelectPdf converts with screen media on A4 at a 1024 px layout width scaled to the page, and keeps
  backgrounds ([page.PdfAsync](https://playwright.dev/dotnet/docs/api/class-page#page-pdf),
  [Media Types](https://selectpdf.com/pdf-library/html/MediaTypes.htm),
  [WebPageWidth](https://selectpdf.com/pdf-library/html/P_SelectPdf_Universal_HtmlToPdfOptions_WebPageWidth.htm)).
  Set `PdfPageSize`, `CssMediaType` and `WebPageWidth` as the `after/` projects do, or the output changes size and
  scale.
- **`PrintBackground = false`.** SelectPdf has no option that leaves out element backgrounds; `DrawBackground` is
  about the page background. Pages that relied on backgrounds being dropped can inject a rule such as
  `* { background: none !important; }` through `CustomCSS`.
- **`EmulateMediaAsync` beyond the media type.** `ColorScheme`, `ReducedMotion` and `ForcedColors` have no
  equivalent; `CssMediaType` chooses between `Screen` and `Print` only
  ([HtmlToPdfCssMediaType](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_HtmlToPdfCssMediaType.htm)).
- **`PreferCSSPageSize`.** SelectPdf's documentation sets the page size only with `PdfPageSize` and
  `PdfPageCustomSize`; a page that declares its size in `@page` needs that size repeated in code.
- **Header and footer templates.** A template mixes HTML and the `pageNumber` / `totalPages` / `date` / `title` /
  `url` classes in one fragment. In SelectPdf, page numbers come only from the `{page_number}` and `{total_pages}`
  placeholders of a `PdfTextSection`, drawn with a PDF font rather than CSS; the HTML part goes in a separate
  `PdfHtmlSection` placed in the same header or footer, and `date`, `title` and `url` are filled in by your code.
  `PdfHtmlSection` lays its HTML out at 1024 px by default, so set its `WebPageWidth` (as T05 does) to keep the
  template's font sizes. Playwright notes that script tags inside templates are not evaluated and page styles are
  not visible in them; a `PdfHtmlSection` is rendered as its own HTML document, so its scripts run
  (`JavaScriptEnabled`, default true), and it too sees only the styles in its own HTML
  ([page.PdfAsync](https://playwright.dev/dotnet/docs/api/class-page#page-pdf),
  [PdfHtmlSection](https://selectpdf.com/pdf-library/html/T_SelectPdf_Universal_PdfHtmlSection.htm)).
- **Outline.** `Outline = true` builds the outline from headings; SelectPdf builds bookmarks from the CSS selectors
  in `PdfBookmarkOptions`.
- **Firefox, WebKit and Intel Macs.** Playwright also drives Firefox and WebKit for other automation (its PDFs come
  from Chromium, [comparison page](https://github.com/csharp-pdf/pdf-libraries-compared/tree/main/playwright)) and
  supports macOS 14 and later; SelectPdf renders with its bundled Chromium and supports macOS on Apple Silicon only
  ([system requirements](https://playwright.dev/dotnet/docs/intro#system-requirements),
  [Installation](https://selectpdf.com/pdf-library/html/Installation.htm)).
- **Licence.** Playwright is free (MIT, with the bundled driver under Apache-2.0) with no page or watermark limits;
  SelectPdf is commercial, and its trial adds a banner to every page (see [Licensing](#licensing)).

## Deployment

| | Playwright | SelectPdf |
|---|---|---|
| Windows | Windows 11+, Windows Server 2019+ or WSL ([system requirements](https://playwright.dev/dotnet/docs/intro#system-requirements)); the browsers are installed per user profile or in `PLAYWRIGHT_BROWSERS_PATH`, and "take a few hundred megabytes of disk space" ([Browsers](https://playwright.dev/dotnet/docs/browsers#managing-browser-binaries)) | `SelectPdf.Universal.Native.win-x64` or `win-x86`; Windows containers need Windows Server Core with the core fonts, not Nano Server ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)) |
| Linux | Debian 12 and 13, Ubuntu 22.04, 24.04 and 26.04 (x86-64 and arm64), with the system packages from `install-deps` ([system requirements](https://playwright.dev/dotnet/docs/intro#system-requirements)) | `SelectPdf.Universal.Native.linux-x64` or `linux-arm64`, glibc 2.30 or later, no OS packages. Alpine (musl) converts only through SelectPdf's separate [Remote Engine](https://selectpdf.com/pdf-library/html/Remote-Engine.htm) container. Base images have no fonts; the built-in fonts cover Latin, Greek and Cyrillic, and `SelectPdf.Universal.Fonts` adds Noto fonts for other scripts ([Deployment with Docker](https://selectpdf.com/pdf-library/html/Deployment-Docker.htm)) |
| Docker | [playwright/docker/Dockerfile](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/playwright/docker/Dockerfile): Microsoft's `mcr.microsoft.com/playwright/dotnet:v1.63.0-noble` image, whose tag must match the package version and which includes the browsers and their system packages; it is built on .NET 10, so the .NET 8 runtime is added. The Docker page recommends `--init` and `--ipc=host` ([Docker](https://playwright.dev/dotnet/docs/docker)) | [selectpdf/docker/Dockerfile](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/selectpdf/docker/Dockerfile): the official `mcr.microsoft.com/dotnet/runtime:8.0` image, `dotnet publish -r linux-x64`, no `apt-get` step and no extra `docker run` flags |
| macOS | macOS 14 and later | Apple Silicon only (`osx-arm64`); on an Intel Mac, HTML to PDF is unavailable ([Installation](https://selectpdf.com/pdf-library/html/Installation.htm)) |

Benchmark results for both libraries, including Docker image runs, are on the comparison repository's
[2026-10-15 results page](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/benchmarks/2026-10-15.md).

## Licensing

| | Playwright | SelectPdf |
|---|---|---|
| Licence | MIT for the .NET package ([playwright-dotnet LICENSE](https://github.com/microsoft/playwright-dotnet/blob/main/LICENSE)); the Node.js driver bundled in the package is Apache-2.0 ([playwright LICENSE](https://github.com/microsoft/playwright/blob/main/LICENSE)) | Commercial, perpetual ([EULA](https://selectpdf.com/eula/)) |
| Price | Free | From USD 499 (Single Developer: 1 developer, 1 deployment machine, 1 year of maintenance); OEM licences remove the deployment-machine limit ([Pricing](https://selectpdf.com/pricing/)) |
| Free use | No limits | 30-day trial, fully functional, with a "Demo Version" banner on every page until a key is set ([Licensing](https://selectpdf.com/pdf-library/html/License.htm)). The separate free Community Edition (`SelectPdf.HtmlToPdf.Universal`) converts HTML to PDF only, up to 5 pages per document ([Community Edition](https://selectpdf.com/community-edition/)) |

Vendor pages read on 2026-10-16.
