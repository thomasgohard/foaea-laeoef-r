using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileBroker.Web.Pages
{
    public class IndexModel : PageModel
    {
        public void OnPostImportFile()
        {
            Response.Redirect("/Tasks/ImportFile");
        }

        public void OnPostCreateOutbound()
        {

        }

        public void OnNightlyProcess()
        {

        }
    }
}