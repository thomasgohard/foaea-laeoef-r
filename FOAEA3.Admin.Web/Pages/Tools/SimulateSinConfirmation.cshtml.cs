using FOAEA3.Admin.Business;
using FOAEA3.Admin.Web.Models;
using FOAEA3.Common.Helpers;
using FOAEA3.Model.Interfaces;
using FOAEA3.Model.Interfaces.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace FOAEA3.Admin.Web.Pages.Tools;

public class SimulateSinConfirmationModel : PageModel
{
    [BindProperty]
    public SimulateSinConfirmationData SimulateSinConfirmation { get; set; }

    private readonly IRepositories DB;
    private readonly IFoaeaConfigurationHelper Config;

    public SimulateSinConfirmationModel(IRepositories repositories)
    {
        Config = new FoaeaConfigurationHelper();
        DB = repositories;
    }

    public void OnGet()
    {
    }

    public async Task OnPost()
    {
        if (ModelState.IsValid)
        {
            var adminManager = new AdminManager(DB, Config);

            try
            {
                var success = await adminManager.ManuallyConfirmSIN(SimulateSinConfirmation.EnfService,
                                                                         SimulateSinConfirmation.ControlCode,
                                                                         SimulateSinConfirmation.Sin, User);

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
