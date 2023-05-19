using FileBroker.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace FileBroker.Web.Pages.Tools
{
    public class ConvertXMLtoJSONModel : PageModel
    {
        [BindProperty]
        public IFormFile FormFile { get; set; }

        public string InfoMessage { get; set; }
        public string ErrorMessage { get; set; }

        public ActionResult OnPostConvert()
        {
            var file = FormFile;
            if (file is not null)
            {
                var fileName = file.FileName;

                if (file.ContentType.ToLower() != "text/xml")
                {
                    ErrorMessage = $"Source file [{fileName}] is not XML?";
                    return null;
                }

                var xmlData = new StringBuilder();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        xmlData.AppendLine(reader.ReadLine());
                }

                var errors = new List<string>();
                string jsonContent = FileHelper.ConvertXmlToJson(xmlData.ToString(), errors);

                if (errors.Any())
                {
                    string lastError = errors.Last();
                    foreach(string error in errors)
                    {
                        ErrorMessage += error;
                        if (error != lastError)
                            ErrorMessage += "; ";
                    }

                }
                else
                {
                    byte[] bytes = Encoding.ASCII.GetBytes(jsonContent);
                    string jsonFileName = Path.GetFileNameWithoutExtension(fileName) + ".json";
                    InfoMessage = $"Successfully converted {fileName} to {jsonFileName}.";
                    return File(bytes, "application/json", jsonFileName);
                }
            }

            return null;
        }
    }
}
