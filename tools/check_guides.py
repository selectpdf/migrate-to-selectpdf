"""Builds and runs every project in the guides and checks its output (the same checks as
csharp-pdf/pdf-libraries-compared; see tools/checker.py and GUIDES.md).

    python tools/check_guides.py                     every guide
    python tools/check_guides.py from-ironpdf        one guide
    python tools/check_guides.py from-ironpdf T08    one guide, one task

Each task folder holds `before/` (the library you use today), `after/` (SelectPdf) and, in some guides, other
routes (e.g. `compat/`); every one of them that has an expect.json is built, run and checked.
Exit code 1 if any check fails.
"""
import pathlib
import sys

sys.path.insert(0, str(pathlib.Path(__file__).resolve().parent))
import checker  # noqa: E402

ROOT = pathlib.Path(__file__).resolve().parents[1]


def main() -> int:
    guides = sorted(d for d in ROOT.glob("from-*") if d.is_dir())
    if len(sys.argv) > 1:
        guides = [g for g in guides if g.name == sys.argv[1]]
    task = sys.argv[2].upper() if len(sys.argv) > 2 else None
    failed = False
    for guide in guides:
        print(f"== {guide.name}")
        for t in sorted(guide.glob("T*")):
            if task and not t.name.upper().startswith(task):
                continue
            for route in sorted(p for p in t.iterdir() if (p / "expect.json").exists()):
                st, problems = checker.check_sample(route)
                failed |= st == "FAIL"
                print(f"  {st:4} {t.name}/{route.name}" + "".join(f"\n         - {p}" for p in problems))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
