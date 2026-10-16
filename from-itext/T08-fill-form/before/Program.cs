// iText Core (forms module): PdfAcroForm.GetField(name).SetValue(value). A check box takes the name of its "on"
// appearance state (here "Yes"), as in iText's form samples.
// https://github.com/itext/itext-publications-samples-dotnet/blob/develop/itext/itext.samples/itext/samples/sandbox/acroforms/FillForm.cs
// https://kb.itextpdf.com/itext/chapter-4-making-a-pdf-interactive
using iText.Forms;
using iText.Forms.Fields;
using iText.Kernel.Pdf;

var output = args.Length > 0 ? args[0] : "output.pdf";
using var pdf = new PdfDocument(new PdfReader("order-form.pdf"), new PdfWriter(output));
var form = PdfFormCreator.GetAcroForm(pdf, false);
form.GetField("name").SetValue("Jane Doe");
form.GetField("email").SetValue("jane@example.com");
form.GetField("plan").SetValue("Pro");
form.GetField("news").SetValue("Yes");
