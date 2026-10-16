// Fill the fields of an existing PDF form.
// Docs: https://ironpdf.com/how-to/edit-forms/
using IronPdf;

var output = args.Length > 0 ? args[0] : "output.pdf";

// IronPDF reads its licence key from License.LicenseKey (a free trial key works)
License.LicenseKey = Environment.GetEnvironmentVariable("IRONPDF_LICENSE_KEY");

PdfDocument pdf = PdfDocument.FromFile("order-form.pdf");

// Set text input form values
pdf.Form.FindFormField("name").Value = "Jane Doe";
pdf.Form.FindFormField("email").Value = "jane@example.com";

// Set the combobox value
pdf.Form.FindFormField("plan").Value = "Pro";

// Check the checkbox (its "on" value is Yes)
pdf.Form.FindFormField("news").Value = "Yes";

pdf.SaveAs(output);
