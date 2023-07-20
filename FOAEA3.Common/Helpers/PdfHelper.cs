using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Widget;

namespace Outgoing.FileCreator.Fed.Tracing
{
    public class PdfHelper
    {
        public static List<string> FillPdf(string templatePath, string outputPath, Dictionary<string, string> values)
        {
            var pdfDoc = CreatePdfFromTemplate(templatePath, values, out List<string> missingFields, out List<string> foundFields);

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            pdfDoc.SaveToFile(outputPath);
            pdfDoc.Close();

            return missingFields;
        }

        public static (MemoryStream, List<string>) FillPdf(string templatePath, Dictionary<string, string> values)
        {
            var pdfDoc = CreatePdfFromTemplate(templatePath, values, out List<string> missingFields, out List<string> foundFields);

            var data = pdfDoc.SaveToStream(FileFormat.PDF);

            if (data is not null)
                return (data[0] as MemoryStream, missingFields);
            else
                return (null, null);
        }

        private static PdfDocument CreatePdfFromTemplate(string templatePath, Dictionary<string, string> values,
                                                         out List<string> missingFields, out List<string> foundFields)
        {
            var pdfDoc = new PdfDocument();
            pdfDoc.LoadFromFile(templatePath);

            missingFields = new List<string>();
            foundFields = new List<string>();
            var standardFont = new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Regular);

            var form = pdfDoc.Form as PdfFormWidget;

            if (form is not null)
                for (int i = 0; i < form.FieldsWidget.List.Count; i++)
                {
                    var field = form.FieldsWidget.List[i] as Spire.Pdf.Fields.PdfField;

                    if (field is not null)
                    {
                        string fieldType = field.GetType().Name.ToUpper();
                        switch (fieldType)
                        {
                            case "PDFTEXTBOXFIELDWIDGET":
                                var textBox = field as PdfTextBoxFieldWidget;
                                if (textBox is not null)
                                {
                                    var fieldName = textBox.Name.ToUpper();
                                    if (values.ContainsKey(fieldName))
                                    {
                                        textBox.Font = standardFont;
                                        textBox.Text = values[fieldName];
                                        foundFields.Add(fieldName);
                                    }
                                }
                                break;
                            case "PDFCHECKBOXWIDGETFIELDWIDGET":
                                var checkBox = field as PdfCheckBoxWidgetFieldWidget;
                                if (checkBox is not null)
                                {
                                    var fieldName = checkBox.Name.ToUpper();
                                    if (values.ContainsKey(fieldName))
                                    {
                                        checkBox.Checked = values[fieldName] == "1";
                                        foundFields.Add(fieldName);
                                    }
                                }
                                break;
                            case "PDFCOMBOBOXWIDGETFIELDWIDGET":
                                var comboBox = field as PdfComboBoxWidgetFieldWidget;
                                if (comboBox is not null)
                                {
                                    var fieldName = comboBox.Name.ToUpper();
                                    if (values.ContainsKey(fieldName))
                                    {
                                        // var comboValues = comboBox.Values;
                                        comboBox.SelectedValue = values[fieldName];
                                    }
                                }
                                break;
                            case "PDFRADIOBUTTONLISTFIELDWIDGET":
                                var radioButton = field as PdfRadioButtonListFieldWidget;
                                if (radioButton is not null)
                                {
                                    var fieldName = radioButton.Name.ToUpper();
                                    if (values.ContainsKey(fieldName))
                                    {
                                        // var comboValues = comboBox.Values;
                                        radioButton.SelectedValue = values[fieldName];
                                    }
                                }
                                break;
                            default:
                                // TODO: Log invalid types?
                                break;
                        }
                    }
                }

            foreach (var value in values)
                if (!foundFields.Contains(value.Key))
                    missingFields.Add(value.Key);

            WatermarkPDF(ref pdfDoc);

            pdfDoc.DocumentInformation.Title = "GeneratedPDF";
            pdfDoc.Form.IsFlatten = true;

            return pdfDoc;
        }

        private static void WatermarkPDF(ref PdfDocument pdf)
        {
            var font = new PdfFont(PdfFontFamily.Helvetica, 12f, PdfFontStyle.Regular);

            string text = "REPLICA created by the Department of Justice Canada Family Law Assistance Services Information Technology Team \n for the purposes of sharing information under Part I of the Family Orders and Agreements Enforcement Assistance Act \n and the Release of Information for Family Orders and Agreements Enforcement Assistance Regulations";

            var textSize = font.MeasureString(text);

            float offsetWidth = (float)(textSize.Width * Math.Sqrt(2) / 4);
            float offsetHeight = (float)(textSize.Height * Math.Sqrt(2) / 4);

            foreach (PdfPageBase page in pdf.Pages)
            {
                page.Canvas.Save();
                page.Canvas.SetTransparency(0.8f);
                page.Canvas.TranslateTransform(page.Canvas.Size.Width / 2 - offsetWidth - offsetHeight,
                                               page.Canvas.Size.Height / 2 + offsetWidth - offsetHeight);
                page.Canvas.RotateTransform(-45);
                page.Canvas.DrawString(text, font, PdfBrushes.DarkGray, 0, 0);
                page.Canvas.Restore();
            }
        }
    }
}
