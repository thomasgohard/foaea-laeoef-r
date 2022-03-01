declare let actionGetProvincesURL: string;
declare let actionGetEnfServicesForProvinceURL: string;
declare let actionGetEnfOffForProvinceURL: string;
declare let actionGetEnfOffForServiceURL: string;
declare let actionGetEnfOffInfoURL: string;
declare let actionGetSubmittersForProvinceURL: string;
declare let actionEditSubmitterURL: string;

namespace Submitter {

    export let initProvince: string;
    export let initEnfSrv: string;
    export let initEnfOff: string;

    export function GetProvinces(callBack?: Function, optionCallBack?: Function, setProvinceCode: boolean = false): void {

        $.get(actionGetProvincesURL)
            .done(function (province) {

                $('#selectProvinces').empty();

                $.each(province, function (i, p) {
                    let item = `<option value="${p.prvCd}">${p.prvTxt}</option>`;
                    $('#selectProvinces').append(item);
                });

                if (typeof initProvince !== 'undefined' && initProvince !== null && initProvince != "")
                    $("#selectProvinces").val(initProvince); // select given item

                if ($("#selectProvinces").val() == null) {
                    $("#selectProvinces").val($("#selectProvinces option:first").val()!); // select first item
                }

                if (setProvinceCode) {
                    $("#provinceCode").val($("#selectProvinces").val()!);
                }

                if (callBack != null) {
                    callBack(optionCallBack);
                }

            })
            .fail(function (data) {

                alert("Call to GetProvinces() Failed!");

            });
    }

    export function GetEnfServicesForProvince(callBack?: Function, optionCallBack?: Function): void {

        let provCd: string = $("#selectProvinces option:selected").val() as string;
        let actionURL: string = actionGetEnfServicesForProvinceURL + "?provCd=" + provCd;
        $.get(actionURL).done(function (enfService) {

            $('#selectEnfServices').empty();

            $.each(enfService, function (i, s) {
                let displayValue: string = PrepString(s.enfSrv_Cd) + ' | ' + PrepString(s.enfSrv_Nme.trim());
                let keyValue: string = s.enfSrv_Cd.trim();
                let item: string = '<option value="' + keyValue + '">' + displayValue + '</option>';
                $('#selectEnfServices').append(item);
            });

            if (typeof initEnfSrv !== 'undefined' && initEnfSrv !== null && initEnfSrv != "")
                $("#selectEnfServices").val(initEnfSrv); // select given item

            if ($("#selectEnfServices").val() == null)
                $("#selectEnfServices").val($("#selectEnfServices option:first").val()!); // select first item

            if (callBack != null) {
                callBack(optionCallBack);
            }

        });

    }

    export function GetEnfOfficesForService(callBack?: Function, optionCallBack?: Function): void {

        let enfService: string = $("#selectEnfServices option:selected").val() as string;
        let actionURL: string = actionGetEnfOffForServiceURL + "?enfServiceCd=" + enfService;
        $.get(actionURL).done(function (enfOff) {

            $('#selectEnfOff').empty();

            $.each(enfOff, function (i, o) {
                var displayValue: string = PrepString(o.enfSrv_Cd) + ' | ' + PrepString(o.enfOff_City_LocCd);
                var keyValue: string = o.enfOff_City_LocCd.trim()
                var item: string = '<option value="' + keyValue + '">' + displayValue + '</option>';
                $('#selectEnfOff').append(item);
            });

            if (typeof initEnfOff !== 'undefined' && initEnfOff !== null && initEnfOff != "")
                $("#selectEnfOff").val(initEnfOff); // select given item

            if ($("#selectEnfOff").val() == null)
                $("#selectEnfOff").val($("#selectEnfOff option:first").val()!); // select first item

            if (callBack != null) {
                callBack(optionCallBack);
            }

            ShowOffInfo();
            ShowHideSuffix();

        });

    }

    export function ShowOffInfo(): void {

        let enfOffice: string = $("#selectEnfOff option:selected").val() as string;
        let enfService: string = $("#selectEnfServices option:selected").val() as string;
        let actionURL: string = `${actionGetEnfOffInfoURL}?enfOffCd=${enfOffice}&enfSrvCd=${enfService}`;
        $.get(actionURL).done(function (enfOff) {
            var displayValue: string = "";
            if (enfOff != null) {
                displayValue = "[Abbreviation: " + enfOff.enfOff_AbbrCd + "] [City: " + enfOff.enfOff_Addr_CityNme + " ]";
            }
            $("#enfOffInfo").text(displayValue);
        });

    }

    export function ShowHideSuffix(): void {
        let selectedEnfService: string = $("#selectEnfServices").val() as string;
        if ((selectedEnfService != null) && (selectedEnfService.length >= 2)) {
            let prefix: string = selectedEnfService.substring(0, 2);
            if (prefix == "FO")
                $("#suffixCode").show();
            else
                $("#suffixCode").hide();
        }
        else
            $("#suffixCode").hide();
    }

    export function PrepString(value: string): string {

        return value.replace(/ /g, String.fromCharCode(160));

    }

    export function PrepStringAndPad(value: string, padLength: number): string {

        let result: string = value;
        let padSpaces: string = "";

        for (let i: number = 0; i < (padLength - 1); i++) {
            padSpaces += " ";
        }

        result += padSpaces;

        result = result.substr(0, padLength - 1);

        return result.replace(/ /g, String.fromCharCode(160));

    }

}