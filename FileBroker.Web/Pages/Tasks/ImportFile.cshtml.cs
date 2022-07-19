using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBroker.Web.Pages.Tasks
{
    public class ImportFileModel : PageModel
    {
        [BindProperty]
        public IFormFile FormFile { get; set; }

        public string Message { get; set; }

        public void OnGet() 
        {
        }

        public void OnPostUpload()
        {
            var file = FormFile;
            if (file is not null)
            {
                var fileSize = file.Length / 1024;
                var fileName = file.FileName;
                Message = $"Loading {fileName} ({fileSize} KB)..." ;
            }
        }
    }
}
