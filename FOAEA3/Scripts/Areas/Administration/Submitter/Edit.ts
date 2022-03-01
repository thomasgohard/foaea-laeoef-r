declare let actionGetSubjectsForSubmitterURL: string;

namespace Submitter {

    $(document).ready(function (): void {

        GetProvinces(GetEnfServicesForProvince, GetEnfOfficesForService, true);

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

    function DisableButtonGroupControl(controlName: string): void {
        $("#lbl" + controlName).addClass("disabled");
        $("#" + controlName).attr("disabled", "disabled");
        if ($("#lbl" + controlName).hasClass("active")) {
            $("#lbl" + controlName).removeClass("btn-outline-info").addClass("btn-info");
        }
    }

    function GetSubjectsForSubmitter(callBack?: Function, optionCallBack?: Function): void {

        let submCd: string = $("#Subm_SubmCd").val() as string;
        let actionURL: string = `${actionGetSubjectsForSubmitterURL}?submCd=${submCd}`;

        $.get(actionURL).done(function (subjects) {

            let subjectList: string = "";
            $.each(subjects, function (i, subject) {
                let activeState: string = (subject.allowedAccess) ? "A" : "I";
                let item: string = `${subject.subjectName} (${activeState})`;
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

}