using FOAEA3.Model.Enums;

namespace FOAEA3.Web.Models
{
    public struct MenuAction
    {
        public string Appl_EnfSrv_Cd { get; set; }
        public string Appl_CtrlCd { get; set; }
        public MenuActionChoice Action { get; set; }
    }
}
