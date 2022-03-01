"use strict";
var Submitter;
(function (Submitter) {
    function GetProvinces(callBack, optionCallBack, setProvinceCode) {
        if (setProvinceCode === void 0) { setProvinceCode = false; }
        $.get(actionGetProvincesURL)
            .done(function (province) {
            $('#selectProvinces').empty();
            $.each(province, function (i, p) {
                var item = "<option value=\"" + p.prvCd + "\">" + p.prvTxt + "</option>";
                $('#selectProvinces').append(item);
            });
            if (typeof Submitter.initProvince !== 'undefined' && Submitter.initProvince !== null && Submitter.initProvince != "")
                $("#selectProvinces").val(Submitter.initProvince); // select given item
            if ($("#selectProvinces").val() == null) {
                $("#selectProvinces").val($("#selectProvinces option:first").val()); // select first item
            }
            if (setProvinceCode) {
                $("#provinceCode").val($("#selectProvinces").val());
            }
            if (callBack != null) {
                callBack(optionCallBack);
            }
        })
            .fail(function (data) {
            alert("Call to GetProvinces() Failed!");
        });
    }
    Submitter.GetProvinces = GetProvinces;
    function GetEnfServicesForProvince(callBack, optionCallBack) {
        var provCd = $("#selectProvinces option:selected").val();
        var actionURL = actionGetEnfServicesForProvinceURL + "?provCd=" + provCd;
        $.get(actionURL).done(function (enfService) {
            $('#selectEnfServices').empty();
            $.each(enfService, function (i, s) {
                var displayValue = PrepString(s.enfSrv_Cd) + ' | ' + PrepString(s.enfSrv_Nme.trim());
                var keyValue = s.enfSrv_Cd.trim();
                var item = '<option value="' + keyValue + '">' + displayValue + '</option>';
                $('#selectEnfServices').append(item);
            });
            if (typeof Submitter.initEnfSrv !== 'undefined' && Submitter.initEnfSrv !== null && Submitter.initEnfSrv != "")
                $("#selectEnfServices").val(Submitter.initEnfSrv); // select given item
            if ($("#selectEnfServices").val() == null)
                $("#selectEnfServices").val($("#selectEnfServices option:first").val()); // select first item
            if (callBack != null) {
                callBack(optionCallBack);
            }
        });
    }
    Submitter.GetEnfServicesForProvince = GetEnfServicesForProvince;
    function GetEnfOfficesForService(callBack, optionCallBack) {
        var enfService = $("#selectEnfServices option:selected").val();
        var actionURL = actionGetEnfOffForServiceURL + "?enfServiceCd=" + enfService;
        $.get(actionURL).done(function (enfOff) {
            $('#selectEnfOff').empty();
            $.each(enfOff, function (i, o) {
                var displayValue = PrepString(o.enfSrv_Cd) + ' | ' + PrepString(o.enfOff_City_LocCd);
                var keyValue = o.enfOff_City_LocCd.trim();
                var item = '<option value="' + keyValue + '">' + displayValue + '</option>';
                $('#selectEnfOff').append(item);
            });
            if (typeof Submitter.initEnfOff !== 'undefined' && Submitter.initEnfOff !== null && Submitter.initEnfOff != "")
                $("#selectEnfOff").val(Submitter.initEnfOff); // select given item
            if ($("#selectEnfOff").val() == null)
                $("#selectEnfOff").val($("#selectEnfOff option:first").val()); // select first item
            if (callBack != null) {
                callBack(optionCallBack);
            }
            ShowOffInfo();
            ShowHideSuffix();
        });
    }
    Submitter.GetEnfOfficesForService = GetEnfOfficesForService;
    function ShowOffInfo() {
        var enfOffice = $("#selectEnfOff option:selected").val();
        var enfService = $("#selectEnfServices option:selected").val();
        var actionURL = actionGetEnfOffInfoURL + "?enfOffCd=" + enfOffice + "&enfSrvCd=" + enfService;
        $.get(actionURL).done(function (enfOff) {
            var displayValue = "";
            if (enfOff != null) {
                displayValue = "[Abbreviation: " + enfOff.enfOff_AbbrCd + "] [City: " + enfOff.enfOff_Addr_CityNme + " ]";
            }
            $("#enfOffInfo").text(displayValue);
        });
    }
    Submitter.ShowOffInfo = ShowOffInfo;
    function ShowHideSuffix() {
        var selectedEnfService = $("#selectEnfServices").val();
        if ((selectedEnfService != null) && (selectedEnfService.length >= 2)) {
            var prefix = selectedEnfService.substring(0, 2);
            if (prefix == "FO")
                $("#suffixCode").show();
            else
                $("#suffixCode").hide();
        }
        else
            $("#suffixCode").hide();
    }
    Submitter.ShowHideSuffix = ShowHideSuffix;
    function PrepString(value) {
        return value.replace(/ /g, String.fromCharCode(160));
    }
    Submitter.PrepString = PrepString;
    function PrepStringAndPad(value, padLength) {
        var result = value;
        var padSpaces = "";
        for (var i = 0; i < (padLength - 1); i++) {
            padSpaces += " ";
        }
        result += padSpaces;
        result = result.substr(0, padLength - 1);
        return result.replace(/ /g, String.fromCharCode(160));
    }
    Submitter.PrepStringAndPad = PrepStringAndPad;
})(Submitter || (Submitter = {}));
//# sourceMappingURL=Submitter.js.map