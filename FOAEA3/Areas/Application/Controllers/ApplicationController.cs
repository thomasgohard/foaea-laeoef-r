using FOAEA3.API.broker;
using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.Areas.Application.Controllers
{
    [Area("Application")]
    public class ApplicationController : Controller
    {

        public IActionResult Events(string id)
        {
            var applAPI = new ApplicationsAPI();

            return View(applAPI.GetEvents(id));
        }

        public IActionResult SINResults(string id)
        {
            var applAPI = new ApplicationsAPI();

            var model = applAPI.GetSINResultsWithHistory(id);

            return View(model);
        }
    }
}