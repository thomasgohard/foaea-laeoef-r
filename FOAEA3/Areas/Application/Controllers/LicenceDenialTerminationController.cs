using Microsoft.AspNetCore.Mvc;

namespace FOAEA3.Areas.Application.Controllers
{
    [Area("Application")]
    public class LicenceDenialTerminationController : Controller
    {

        public IActionResult Create()
        {
            return View();
        }
    }
}