"""Build, run and check one project against its expect.json. Copied from csharp-pdf/pdf-libraries-compared
(tools/check_samples.py); check_guides.py uses check_sample().

    python tools/check_samples.py                  every library
    python tools/check_samples.py ironpdf          one library
    python tools/check_samples.py ironpdf T08      one library, one task (T14 = the docker/ folder)

Needs the .NET 8 SDK, Python 3 with pypdf, pymupdf and pyhanko, Docker for T14, and veraPDF for PDF/A checks
(VERAPDF = path of the veraPDF launcher; without it those checks are reported as skipped).
Exit code 1 if any check fails.
"""
import json
import os
import pathlib
import re
import shutil
import socket
import subprocess
import sys
import tempfile
import time
import urllib.request

import logging

import fitz
import pypdf

# pyHanko logs a traceback for the samples' self-signed certificates; the checks only need intact/valid.
logging.getLogger("pyhanko").setLevel(logging.CRITICAL)

ROOT = pathlib.Path(__file__).resolve().parents[1]
INPUTS = ROOT / "_inputs"


def words(s: str) -> str:
    return re.sub(r"\s+", " ", s).strip()


def run(cmd, cwd=None, timeout=900, env=None):
    return subprocess.run(cmd, cwd=cwd, capture_output=True, text=True, encoding="utf-8", errors="replace",
                          timeout=timeout, env=env)


def free_port() -> int:
    with socket.socket() as s:
        s.bind(("127.0.0.1", 0))
        return s.getsockname()[1]


def check_pdf(path: pathlib.Path, spec: dict) -> list[str]:
    problems = []
    if not path.exists():
        return [f"{path.name} was not written"]
    try:
        doc = fitz.open(path)
    except Exception as e:
        return [f"{path.name} does not open: {e}"]
    pages = [words(p.get_text()) for p in doc]
    alltext = " ".join(pages)
    if "pages" in spec and len(doc) != spec["pages"]:
        problems.append(f"{len(doc)} pages, expected {spec['pages']}")
    if "minPages" in spec and len(doc) < spec["minPages"]:
        problems.append(f"{len(doc)} pages, expected at least {spec['minPages']}")
    for t in spec.get("text", []):
        if words(t) not in alltext:
            problems.append(f"text {t!r} not found")
    for t in spec.get("textEveryPage", []):
        missing = [i + 1 for i, p in enumerate(pages) if not re.search(t, p)]
        if missing:
            problems.append(f"{t!r} missing on pages {missing}")
    if "pageSize" in spec:
        w, h = spec["pageSize"]
        for i, p in enumerate(doc):
            if abs(p.rect.width - w) > 2 or abs(p.rect.height - h) > 2:
                problems.append(f"page {i + 1} is {p.rect.width:.0f} x {p.rect.height:.0f} pt, expected {w} x {h}")
                break
    if "fields" in spec:
        fields = pypdf.PdfReader(path).get_fields() or {}
        for name, want in spec["fields"].items():
            got = fields.get(name, {}).get("/V")
            got = str(got).lstrip("/") if got is not None else None
            if got != want:
                problems.append(f"field {name!r} = {got!r}, expected {want!r}")
    if spec.get("signed"):
        problems += check_signature(path)
    if "pdfa" in spec:
        problems += check_pdfa(path, spec["pdfa"])
    return problems


def check_signature(path: pathlib.Path) -> list[str]:
    from pyhanko.pdf_utils.reader import PdfFileReader
    from pyhanko.sign.validation import validate_pdf_signature
    with open(path, "rb") as f:
        sigs = PdfFileReader(f, strict=False).embedded_signatures
        if len(sigs) != 1:
            return [f"{len(sigs)} signatures, expected 1"]
        status = validate_pdf_signature(sigs[0])
        if not status.intact:
            return ["signature is not intact (the signed bytes were changed)"]
        if not status.valid:
            return ["signature does not verify"]
    return []


def check_pdfa(path: pathlib.Path, level: str) -> list[str]:
    verapdf = os.environ.get("VERAPDF")
    if not verapdf:
        return ["SKIP: PDF/A not validated (set VERAPDF)"]
    env = dict(os.environ)
    if os.environ.get("VERAPDF_JAVA_HOME"):   # veraPDF's launcher finds Java through JAVA_HOME
        env["JAVA_HOME"] = os.environ["VERAPDF_JAVA_HOME"]
    r = run([verapdf, "--flavour", level.lower(), "--format", "text", str(path)], timeout=300, env=env)
    out = r.stdout + r.stderr
    if "PASS" in out:
        return []
    return [f"veraPDF PDF/A-{level}: " + (out.strip().splitlines() or ["no output"])[-1][:200]]


def check_image(path: pathlib.Path, size) -> list[str]:
    if not path.exists():
        return [f"{path.name} was not written"]
    try:
        pix = fitz.Pixmap(str(path))
    except Exception as e:
        return [f"{path.name} is not an image: {e}"]
    w, h = size
    if abs(pix.width - w) > w * 0.02 or abs(pix.height - h) > h * 0.02:
        return [f"{path.name} is {pix.width} x {pix.height}, expected about {w} x {h}"]
    return []


def check_outputs(work: pathlib.Path, expect: dict, stdout: str) -> list[str]:
    problems = []
    for spec in expect.get("outputs", []):
        f = work / spec["file"]
        problems += check_image(f, spec["image"]) if "image" in spec else check_pdf(f, spec)
    for t in expect.get("stdout", []):
        if words(t) not in words(stdout):
            problems.append(f"stdout does not contain {t!r}")
    return problems


def scratch() -> pathlib.Path:
    work = pathlib.Path(tempfile.mkdtemp(prefix="sample-"))
    for f in INPUTS.glob("*.pdf"):
        shutil.copy(f, work / f.name)
    return work


def check_sample(sample: pathlib.Path) -> tuple[str, list[str]]:
    expect = json.loads((sample / "expect.json").read_text(encoding="utf-8"))
    project = next(sample.glob("*.csproj"))
    b = run(["dotnet", "build", str(project), "-c", "Release", "--nologo", "-v", "q"])
    if b.returncode:
        lines = [l for l in (b.stdout + b.stderr).splitlines() if " error " in l]
        return "FAIL", ["build failed: " + (lines[0].strip() if lines else b.stderr[-300:])]
    work = scratch()
    try:
        if "web" in expect:
            port = free_port()
            p = subprocess.Popen(["dotnet", "run", "--project", str(project), "-c", "Release", "--no-build",
                                  "--urls", f"http://127.0.0.1:{port}"], cwd=work,
                                 stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL)
            try:
                url = f"http://127.0.0.1:{port}{expect['web']['path']}"
                for _ in range(120):
                    try:
                        with urllib.request.urlopen(url, timeout=300) as resp:
                            ctype = resp.headers.get("Content-Type", "")
                            (work / "output.pdf").write_bytes(resp.read())
                        break
                    except (ConnectionError, urllib.error.URLError):
                        time.sleep(1)
                else:
                    return "FAIL", ["the endpoint never answered"]
                problems = [] if ctype.startswith("application/pdf") else [f"Content-Type {ctype!r}"]
                problems += check_outputs(work, expect, "")
            finally:
                subprocess.run(["taskkill", "/PID", str(p.pid), "/T", "/F"], capture_output=True) if os.name == "nt" else p.kill()
        else:
            r = run(["dotnet", "run", "--project", str(project), "-c", "Release", "--no-build"], cwd=work)
            if r.returncode:
                return "FAIL", [f"exit code {r.returncode}: " + (r.stderr.strip() or r.stdout.strip())[-300:]]
            problems = check_outputs(work, expect, r.stdout)
    finally:
        shutil.rmtree(work, ignore_errors=True)
    return status(problems), problems


def check_docker(lib: pathlib.Path) -> tuple[str, list[str]]:
    expect = json.loads((lib / "docker" / "expect.json").read_text(encoding="utf-8"))
    image = f"pdf-libraries-compared/{lib.name}:sample"
    b = run(["docker", "build", "-f", str(lib / "docker" / "Dockerfile"), "-t", image, str(lib)], timeout=1800)
    if b.returncode:
        return "FAIL", ["docker build failed: " + (b.stderr.strip().splitlines() or [""])[-1][:300]]
    work = scratch()
    try:
        env_args = [a for k in ("IRONPDF_LICENSE_KEY", "ABCPDF_LICENSE_KEY", "DOCOTIC_LICENSE_KEY")
                    if os.environ.get(k) for a in ("-e", k)]
        r = run(["docker", "run", "--rm", *env_args, "-v", f"{work}:/out", image, "/out/output.pdf"], timeout=600)
        if r.returncode:
            return "FAIL", [f"container exit code {r.returncode}: " + (r.stderr.strip() or r.stdout.strip())[-300:]]
        problems = check_outputs(work, expect, r.stdout)
    finally:
        shutil.rmtree(work, ignore_errors=True)
        run(["docker", "rmi", "-f", image])
    return status(problems), problems


def status(problems: list[str]) -> str:
    real = [p for p in problems if not p.startswith("SKIP")]
    return "FAIL" if real else ("SKIP" if problems else "PASS")


def main() -> int:
    libs = [d for d in sorted(ROOT.iterdir()) if (d / "samples").is_dir() or (d / "docker").is_dir()]
    if len(sys.argv) > 1:
        libs = [d for d in libs if d.name == sys.argv[1]]
    task = sys.argv[2].upper() if len(sys.argv) > 2 else None
    failed = False
    for lib in libs:
        print(f"== {lib.name}")
        targets = [s for s in sorted((lib / "samples").glob("T*")) if (s / "expect.json").exists()]
        for s in targets:
            if task and not s.name.upper().startswith(task):
                continue
            st, problems = check_sample(s)
            failed |= st == "FAIL"
            print(f"  {st:4} {s.name}" + "".join(f"\n         - {p}" for p in problems))
        if (lib / "docker" / "expect.json").exists() and task in (None, "T14"):
            st, problems = check_docker(lib)
            failed |= st == "FAIL"
            print(f"  {st:4} T14-docker" + "".join(f"\n         - {p}" for p in problems))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
