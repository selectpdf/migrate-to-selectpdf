# How the guides are written

## Layout

```
from-<library>/
  README.md                the guide: what changes, the mapping table, what does not carry over
  T01-hello-world/
    before/                the task with the library you use today
    after/                 the same task with SelectPdf
  T02-html-string/
  ...
```

Task numbers and names are those of the standard task set in
[csharp-pdf/pdf-libraries-compared](https://github.com/csharp-pdf/pdf-libraries-compared#the-standard-task-set).
A guide has a task folder for every task **both** libraries support. Tasks only SelectPdf supports have no
folder: the guide's "What you can add" section links SelectPdf's sample for them in the comparison repository.

Each `before/` and `after/` folder is a complete console project (`Sample.csproj`, `Program.cs`, `expect.json`),
built and run by `tools/check_guides.py` with the checks described in the comparison repository's
[SAMPLES.md](https://github.com/csharp-pdf/pdf-libraries-compared/blob/main/_template/SAMPLES.md). `before/`
starts from that library's verified sample in the comparison repository; `after/` starts from SelectPdf's. They
are edited only so that the pair does the same thing (the same input, page size, margins, header text) — where the
two samples take different routes (for example one converts HTML and the other an existing PDF), `after/` follows
the `before/` route — and both must pass the same `expect.json` checks.

## The guide (README.md)

1. **Who it is for** — one or two sentences: which projects this helps, and when staying is the better choice.
2. **Price and licensing** — prices side by side for common situations (one developer, SaaS or redistribution, a team, unlimited), and both licence models, each linked to the vendor's page with the date read.
3. **Packages and setup** — before and after: NuGet packages, licence keys, the namespace.
4. **Mapping** — a table from the library's types, methods and settings to SelectPdf's, with units (points,
   millimetres, pixels) stated where they differ.
5. **Task by task** — for each task folder: two or three sentences on what changed, linking `before/` and
   `after/`.
6. **What you can add** — tasks SelectPdf supports and the library does not, linked to SelectPdf's samples.
7. **What does not carry over** — options without an equivalent, behaviour that differs (defaults, units,
   timeouts, page-number placeholders), and the workaround where there is one. Honest and specific.
8. **Benchmark** — the library and SelectPdf side by side on every axis of the latest benchmark run (speed,
   throughput, memory, checks, fidelity, standards, tools, Docker), generated from the raw data by
   [`analysis/pair_sections.py`](https://github.com/csharp-pdf/pdf-benchmarks/blob/main/analysis/pair_sections.py)
   in pdf-benchmarks. The section is replaced on every run and never edited by hand.
9. **Deployment** — Windows, Linux and Docker differences, linking both libraries' Docker samples in the
   comparison repository.

## Rules

- **Factual and checkable.** Every statement about either library links its source (vendor docs, NuGet,
  the comparison repository's samples or benchmark data) or a project in this repository.
- **No speed claims** against another library in the text, and no "drop-in", "no code changes" or "just swap the
  package" claims. Benchmark numbers appear only in the generated Benchmark section, complete and in both
  directions, never quoted selectively.
- **Target:** `SelectPdf.Universal` (current version). The classic `Select.Pdf` library is mentioned only for
  .NET Framework projects.
- **Licence keys** come from environment variables, never from code: `IRONPDF_LICENSE_KEY` for IronPDF;
  SelectPdf runs as a trial without a key and reads none in these projects.
