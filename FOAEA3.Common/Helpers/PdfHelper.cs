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
            var standardFont = new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Bold);

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
                        }
                    }
                }

            foreach (var value in values)
                if (!foundFields.Contains(value.Key))
                    missingFields.Add(value.Key);

            pdfDoc.Form.IsFlatten = true;

            return pdfDoc;
        }
    }
}
