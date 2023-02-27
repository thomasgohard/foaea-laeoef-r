using FOAEA3.Common.Brokers.Administration;
using FOAEA3.Common.Brokers;
using FOAEA3.Model;
using FOAEA3.Model.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FOAEA3.Web.Pages.Applications
{
    public class InterceptionFoaeaPageModel : FoaeaPageModel
    {
        public string EnfServiceDescription { get; set; }

        [BindProperty]
        public HoldbackConditionData NewCondition { get; set; }

        [BindProperty]
        public InterceptionApplicationData InterceptionApplication { get; set; }

        public InterceptionFoaeaPageModel(IHttpContextAccessor httpContextAccessor, ApiConfig apiConfig) :
                                                                                            base(httpContextAccessor, apiConfig)
        {

        }

        public JsonResult OnGetSelectProvinceForCountry(string countryCode)
        {
            ValidProvinces = AllProvinces.Where(m => m.PrvCtryCd == countryCode).OrderBy(m => m.PrvTxtE).ToList();

            return new JsonResult(ValidProvinces);
        }

        protected List<MessageData> GetValidationErrors()
        {
            var errorMessages = new List<MessageData>();
            foreach (var value in ModelState.Values)
            {
                if (value.Errors.Any())
                    foreach (var error in value.Errors)
                        errorMessages.Add(new MessageData(EventCode.UNDEFINED, null, error.ErrorMessage, MessageType.Error));
            }

            return errorMessages;
        }

        public void OnPostAddEntry()
        {
            InterceptionApplication.HldbCnd.Add(new HoldbackConditionData()
            {
                Appl_EnfSrv_Cd = InterceptionApplication.Appl_EnfSrv_Cd,
                Appl_CtrlCd = InterceptionApplication.Appl_CtrlCd,
                HldbCnd_MxmPerChq_Money = NewCondition.HldbCnd_MxmPerChq_Money,
                HldbCnd_SrcHldbAmn_Money = NewCondition.HldbCnd_SrcHldbAmn_Money,
                HldbCnd_SrcHldbPrcnt = NewCondition.HldbCnd_SrcHldbPrcnt,
                HldbCtg_Cd = NewCondition.HldbCtg_Cd,
                EnfSrv_Cd = NewCondition.EnfSrv_Cd
            });
        }        

    }
}
