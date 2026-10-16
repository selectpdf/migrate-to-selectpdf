// Fill the fields of an existing PDF form.
// Sample: https://selectpdf.com/demo-universal/PdfFormFilling
using SelectPdf.Universal;

var output = args.Length > 0 ? args[0] : "output.pdf";

// load the pdf form manager
PdfFormManager form = new PdfFormManager();
form.Load("order-form.pdf");

// fill the fields
PdfFormFieldTextBox name = (PdfFormFieldTextBox)form.Fields["name"];
name.Text = "Jane Doe";

PdfFormFieldTextBox email = (PdfFormFieldTextBox)form.Fields["email"];
email.Text = "jane@example.com";

PdfFormFieldComboBox plan = (PdfFormFieldComboBox)form.Fields["plan"];
plan.SelectedValue = "Pro";

PdfFormFieldCheckBox news = (PdfFormFieldCheckBox)form.Fields["news"];
news.Checked = true;

// save and close the pdf document
form.Save(output);
form.Close();
