namespace Submitter {

    $(document).ready(function (): void {

        $("#selectProvinces").change(function () { GetEnfServicesForProvince(GetEnfOfficesForService) });
        $("#selectEnfServices").change(function () { GetEnfOfficesForService(); });
        $("#selectEnfOff").change(function () { ShowOffInfo(); });

        $("#Subm_EnfSrvAuth_Ind").change(function () { EnableReadOnly(true); });
        $("#Subm_EnfOffAuth_Ind").change(function () { EnableReadOnly(true); });
        $("#Subm_SysMgr_Ind").change(function () { EnableReadOnly(false); });
        $("#Subm_CourtUsr_Ind").change(function () { EnableReadOnly(false); });
        $("#Subm_AppMgr_Ind").change(function () { EnableReadOnly(false); });
        $("#LimitedFinancialUser_Ind").change(function () { EnableReadOnly(false); });

        GetProvinces(GetEnfServicesForProvince, GetEnfOfficesForService);

        $("#lblUsedBy").hide();

        ShowHideSuffix();

    });

    function EnableReadOnly(enable: boolean): void {

        if (enable) {
            $("#lblReadOnly_Ind").removeClass("disabled");
            $("#ReadOnly_Ind").removeAttr("disabled");
        }
        else {
            $("#lblReadOnly_Ind").addClass("disabled");
            $("#ReadOnly_Ind").attr("disabled", "disabled");
        }

    }

}