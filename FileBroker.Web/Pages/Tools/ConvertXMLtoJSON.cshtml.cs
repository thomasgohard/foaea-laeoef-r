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
        public string JsonContent { get; set; }

        public void OnPostConvert()
        {
            var file = FormFile;
            if (file is not null)
            {
                var fileName = file.FileName;

                if (file.ContentType.ToLower() != "text/xml")
                {
                    ErrorMessage = $"Source file [{fileName}] is not XML?";
                    return;
                }

                var xmlData = new StringBuilder();
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    while (reader.Peek() >= 0)
                        xmlData.AppendLine(reader.ReadLine());
                }

                var errors = new List<string>();
                JsonContent = FileHelper.ConvertXmlToJson(xmlData.ToString(), ref errors);

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
            }

            return;
        }
    }
}
