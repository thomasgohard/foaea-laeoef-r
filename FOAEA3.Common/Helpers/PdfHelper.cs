using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Widget;

namespace Outgoing.FileCreator.Fed.Tracing
{
    public class PdfHelper
    {
        public static List<string> FillPdf(string templatePath, string outputPath, Dictionary<string, string> values, bool isEnglish = true)
        {
            var pdfDoc = CreatePdfFromTemplate(templatePath, values, out List<string> missingFields, out List<string> foundFields, isEnglish);

            if (File.Exists(outputPath))
                File.Delete(outputPath);

            pdfDoc.SaveToFile(outputPath);
            pdfDoc.Close();

            return missingFields;
        }

        public static (MemoryStream, List<string>) FillPdf(string templatePath, Dictionary<string, string> values, bool isEnglish = true)
        {
            var pdfDoc = CreatePdfFromTemplate(templatePath, values, out List<string> missingFields, out List<string> foundFields, isEnglish);

            var data = pdfDoc.SaveToStream(FileFormat.PDF);

            if (data is not null)
                return (data[0] as MemoryStream, missingFields);
            else
                return (null, null);
        }

        private static PdfDocument CreatePdfFromTemplate(string templatePath, Dictionary<string, string> values,
                                                         out List<string> missingFields, out List<string> foundFields,
                                                         bool isEnglish = true)
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

            WatermarkPDF(ref pdfDoc, isEnglish);

            pdfDoc.DocumentInformation.Title = "GeneratedPDF";
            pdfDoc.Form.IsFlatten = true;

            return pdfDoc;
        }

        private static void WatermarkPDF(ref PdfDocument pdf, bool isEnglish = true)
        {
            var fontSmall = new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Bold);
            var fontSmallItalic = new PdfFont(PdfFontFamily.Helvetica, 9f, PdfFontStyle.Italic | PdfFontStyle.Bold);
            var fontBig = new PdfFont(PdfFontFamily.Helvetica, 14f, PdfFontStyle.Bold);

            var brush = PdfBrushes.CornflowerBlue;

            string textLine1;
            string textLine2;
            string textLine2Italic;
            string textREPLICA;

            if (isEnglish)
            {
                textLine1 = "Important Notice: Edited by the Department of Justice Canada for the purposes of releasing information under";
                textLine2 = "Part I of the";
                textLine2Italic = "Family Orders and Agreements Enforcement Assistance Act";
            }
            else
            {
                textLine1 = "Avis important : Édité par le ministère de la Justice du Canada aux fins de la communication des renseignements";
                textLine2 = "en vertu de la partie I de la";
                textLine2Italic = "Loi d'aide à l'exécution des ordonnances et des ententes familiales";
            }
            textREPLICA = "REPLICA";

            foreach (PdfPageBase page in pdf.Pages)
            {
                page.Canvas.Save();

                page.Canvas.DrawString(textLine1, fontSmall, brush, 24, -1);
                page.Canvas.DrawString(textLine2, fontSmall, brush, 24, 9);
                page.Canvas.DrawString(textLine2Italic, fontSmallItalic, brush, isEnglish ? 76 : 140, 9);
                page.Canvas.DrawString(textREPLICA, fontBig, brush, 520, 0);

                page.Canvas.Restore();
            }
        }
    }
}
