"use strict";
var Submitter;
(function (Submitter) {
    $(document).ready(function () {
        Submitter.GetProvinces(Submitter.GetEnfServicesForProvince, Submitter.GetEnfOfficesForService, true);
        $("#selectProvinces").attr("disabled", "disabled");
        $("#selectEnfServices").attr("disabled", "disabled");
        $("#selectEnfOff").attr("disabled", "disabled");
        $("#suffixCode").hide();
        DisableButtonGroupControl("Subm_EnfSrvAuth_Ind");
        DisableButtonGroupControl("Subm_EnfOffAuth_Ind");
        DisableButtonGroupControl("Subm_SysMgr_Ind");
        DisableButtonGroupControl("Subm_AppMgr_Ind");
        DisableButtonGroupControl("Subm_CourtUsr_Ind");
        DisableButtonGroupControl("LimitedFinancialUser_Ind");
        DisableButtonGroupControl("ReadOnly_Ind");
        GetSubjectsForSubmitter();
    });
    function DisableButtonGroupControl(controlName) {
        $("#lbl" + controlName).addClass("disabled");
        $("#" + controlName).attr("disabled", "disabled");
        if ($("#lbl" + controlName).hasClass("active")) {
            $("#lbl" + controlName).removeClass("btn-outline-info").addClass("btn-info");
        }
    }
    function GetSubjectsForSubmitter(callBack, optionCallBack) {
        var submCd = $("#Subm_SubmCd").val();
        var actionURL = actionGetSubjectsForSubmitterURL + "?submCd=" + submCd;
        $.get(actionURL).done(function (subjects) {
            var subjectList = "";
            $.each(subjects, function (i, subject) {
                var activeState = (subject.allowedAccess) ? "A" : "I";
                var item = subject.subjectName + " (" + activeState + ")";
                if (subjectList != "")
                    subjectList += "<br />";
                subjectList += item;
            });
            $("#usedBy").html(subjectList);
            if (callBack != null) {
                callBack(optionCallBack);
            }
        });
    }
})(Submitter || (Submitter = {}));
//# sourceMappingURL=Edit.js.map