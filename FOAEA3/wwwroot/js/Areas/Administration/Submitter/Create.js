"use strict";
var Submitter;
(function (Submitter) {
    $(document).ready(function () {
        $("#selectProvinces").change(function () { Submitter.GetEnfServicesForProvince(Submitter.GetEnfOfficesForService); });
        $("#selectEnfServices").change(function () { Submitter.GetEnfOfficesForService(); });
        $("#selectEnfOff").change(function () { Submitter.ShowOffInfo(); });
        $("#Subm_EnfSrvAuth_Ind").change(function () { EnableReadOnly(true); });
        $("#Subm_EnfOffAuth_Ind").change(function () { EnableReadOnly(true); });
        $("#Subm_SysMgr_Ind").change(function () { EnableReadOnly(false); });
        $("#Subm_CourtUsr_Ind").change(function () { EnableReadOnly(false); });
        $("#Subm_AppMgr_Ind").change(function () { EnableReadOnly(false); });
        $("#LimitedFinancialUser_Ind").change(function () { EnableReadOnly(false); });
        Submitter.GetProvinces(Submitter.GetEnfServicesForProvince, Submitter.GetEnfOfficesForService);
        $("#lblUsedBy").hide();
        Submitter.ShowHideSuffix();
    });
    function EnableReadOnly(enable) {
        if (enable) {
            $("#lblReadOnly_Ind").removeClass("disabled");
            $("#ReadOnly_Ind").removeAttr("disabled");
        }
        else {
            $("#lblReadOnly_Ind").addClass("disabled");
            $("#ReadOnly_Ind").attr("disabled", "disabled");
        }
    }
})(Submitter || (Submitter = {}));
//# sourceMappingURL=Create.js.map