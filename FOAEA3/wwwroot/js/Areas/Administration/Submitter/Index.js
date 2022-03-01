"use strict";
var Submitter;
(function (Submitter) {
    $(document).ready(function () {
        $("#selectProvinces").change(function () { GetEnfOfficesForSelectedProvince(GetSubmitters); });
        $("#chkActiveOnly").change(function () { GetSubmitters(); });
        $("#selectEnfOff").change(function () { GetSubmitters(); });
        $(".addNew").click(function () {
            var selectedProvince = $("#selectProvinces").val();
            var selectedOffSrv = $("#selectEnfOff").val();
            var selectedOffice = "";
            var selectedService = "";
            if (selectedOffSrv.indexOf("-") > 0) {
                var values = selectedOffSrv.split("-");
                selectedOffice = values[1];
                selectedService = values[0];
            }
            var linkRef = $(this).attr("href");
            $(this).attr("href", linkRef + "?provCd=" + selectedProvince + "&offCd=" + selectedOffice + "&srvCd=" + selectedService);
        });
        Submitter.GetProvinces(GetEnfOfficesForSelectedProvince, GetSubmitters);
    });
    function GetEnfOfficesForSelectedProvince(callBack, optionCallBack) {
        var provCd = $("#selectProvinces option:selected").val();
        var actionURL = actionGetEnfOffForProvinceURL + "?provCd=" + provCd;
        $.get(actionURL).done(function (enfOff) {
            $('#selectEnfOff').empty();
            $('#selectEnfOff').append('<option value="-1">** All **</option>');
            $.each(enfOff, function (i, o) {
                var displayValue = Submitter.PrepString(o.enfSrv_Cd) + ' | ' + Submitter.PrepString(o.enfOff_City_LocCd) +
                    ' | ' + Submitter.PrepStringAndPad(o.enfOff_Addr_CityNme, 20) +
                    ' | ' + Submitter.PrepStringAndPad(o.enfOff_Nme, 25) + ' | ' + Submitter.PrepString(o.actvSt_Cd);
                var keyValue = o.enfSrv_Cd.trim() + "-" + o.enfOff_City_LocCd.trim();
                var item = "<option value=\"" + keyValue + "\">" + displayValue + "</option>";
                $('#selectEnfOff').append(item);
            });
            if (typeof Submitter.initEnfOff !== 'undefined' && Submitter.initEnfOff !== null && Submitter.initEnfOff != "")
                $("#selectEnfOff").val(Submitter.initEnfOff); // select given item
            if ($("#selectEnfOff").val() == null) {
                $("#selectEnfOff").val($("#selectEnfOff option:first").val()); // select first item
            }
            if (callBack != null) {
                callBack(optionCallBack);
            }
        });
    }
    Submitter.GetEnfOfficesForSelectedProvince = GetEnfOfficesForSelectedProvince;
    function GetSubmitters() {
        var columnData = [
            { title: "Submitter Code" },
            { title: "Office" },
            { title: "First Name" },
            { title: "Surname" },
            { title: "Status" }
        ];
        var onlyActive = $("#chkActiveOnly").is(":checked");
        var provCd = $("#selectProvinces option:selected").val();
        var enfOffKey = $("#selectEnfOff option:selected").val();
        var actionURL = actionGetSubmittersForProvinceURL + "?provCd=" + provCd + "&onlyActive=" + onlyActive + "&enfOffKey=" + enfOffKey;
        $.get(actionURL).done(function (submitters) {
            DataTableManager.Destroy();
            $("#submitterDataRows").empty();
            $.each(submitters, function (i, s) {
                var rowData = "<tr>\n                                   <td>" + s.subm_SubmCd + "</td>\n                                   <td>" + s.enfSrv_Cd + "</td>\n                                   <td>" + s.subm_FrstNme + "</td>\n                                   <td>" + s.subm_SurNme + "</td>\n                                   <td>" + s.actvSt_Cd + "</td>\n                               </tr>";
                $("#submitterDataRows").append(rowData);
            });
            DataTableManager.Setup(columnData);
            // set default order to be Status, Surname, First Name
            var table = $("#datatablesGrid").DataTable();
            table.order([[4, "asc"], [3, "asc"], [2, "asc"]])
                .draw();
        });
    }
    Submitter.GetSubmitters = GetSubmitters;
    $('#datatablesGrid tbody').on('click', 'tr', function () {
        var table = $('#datatablesGrid').DataTable();
        var idx = table.row(this).index();
        var id = table.cell(idx, 0).data();
        var provCd = $("#selectProvinces option:selected").val();
        if (provCd == undefined)
            provCd = $("selectProvinces option:first").val();
        document.location.href = actionEditSubmitterURL + "?submCd=" + id + "&provCd=" + provCd;
    });
})(Submitter || (Submitter = {}));
//# sourceMappingURL=Index.js.map