using FOAEA3.API.broker;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.Areas.Application.Controllers
{
    [Area("Application")]
    public class ApplicationSearchController : Controller
    {
        // GET: ApplicationSearch
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuickSearch(IFormCollection collection)
        {
            QuickSearchData quickSearchData = new QuickSearchData
            {
                EnfService = collection["EnfService"],
                ControlCode = collection["ControlCode"],
                LastName = collection["LastName"],
                FirstName = collection["FirstName"],
                Status = collection["Status"],
                Category = collection["Category"],
                SIN = collection["SIN"],
                EnfSourceRefNumber = collection["SourceRefNumber"],
                JusticeNumber = collection["JusticeNumber"]
            };
            if (short.TryParse(collection["State"], out short state))
                quickSearchData.State = (ApplicationState)state;

            var searchAPI = new ApplicationSearchesAPI();
            var result = searchAPI.Search(quickSearchData);

            return View("ApplicationSearchResult", result);
        }

    }
}