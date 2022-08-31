using FOAEA3.Admin.Business;
using FOAEA3.Admin.Web.Models;
using FOAEA3.Model;
using FOAEA3.Model.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Admin.Web.Pages.Tools
{
    public class SimulateSinConfirmationModel : PageModel
    {
        [BindProperty]
        public SimulateSinConfirmationData SimulateSinConfirmation { get; set; }

        private readonly IRepositories Repositories;
        private readonly CustomConfig config;

        public SimulateSinConfirmationModel(IRepositories repositories, IOptions<CustomConfig> config)
        {
            this.config = config.Value;
            Repositories = repositories;
        }

        public void OnGet()
        {
        }

        public async Task OnPost()
        {

            if (ModelState.IsValid)
            {
                var adminManager = new AdminManager(Repositories, config);

                try
                {
                    var success = await adminManager.ManuallyConfirmSINAsync(SimulateSinConfirmation.EnfService, SimulateSinConfirmation.ControlCode,
                                                                  SimulateSinConfirmation.Sin);

                    if (success)
                        ViewData["Message"] = $"{SimulateSinConfirmation.EnfService}-{SimulateSinConfirmation.ControlCode}: SIN has been confirmed.";
                    else
                        ViewData["Error"] = "Error: " + adminManager.LastError;
                }
                catch (Exception e)
                {
                    ViewData["Error"] = "Error: " + e.Message;
                }

            }

        }
    }
}
