$(document).on('wb-ready.wb', function () {

    function LoadProvinces() {
        var countryCode = $("#Appl_Dbtr_Addr_CtryCd").val();
        var enforcementProvince = $("#Appl_EnfSrv_Cd").val().substring(0, 2);
        $("#Appl_Dbtr_Addr_PrvCd").empty();
        $.getJSON(`?handler=SelectProvinceForCountry&countryCode=${countryCode}`, (data) => {
            $.each(data, function (i, item) {
                var selected = "";
                if (item.prvCd == enforcementProvince)
                    selected = "selected";
                $("#Appl_Dbtr_Addr_PrvCd").append(`<option value="${item.prvCd}" ${selected}>${item.prvTxtE}</option>`);
            });
        });
    }

    function ResetPeriod() {
        $('input[name="InterceptionApplication.IntFinH.PymPr_Cd"]').prop("checked", false).change();
    }

    function ResetCumulative() {
        $('input[name="InterceptionApplication.IntFinH.IntFinH_CmlPrPym_Ind"]').prop("checked", false).change();
    }

    function LoadRecalcDate() {
        var periodCode = $("#PymPr_Cd:checked").val();
        $("#IntFinH_NextRecalcDate_Cd").empty();
        switch (periodCode) {
            case "A":
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="0"></option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="1">Sunday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="2">Monday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="3">Tuesday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="4">Wednesday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="5">Thursday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="6">Friday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="7">Saturday</option>`);
                break;
            case "B":
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="0"></option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="1">First available Sunday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="2">First available Monday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="3">First available Tuesday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="4">First available Wednesday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="5">First available Thursday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="6">First available Friday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="7">First available Saturday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="8">Second available Sunday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="9">Second available Monday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="10">Second available Tuesday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="11">Second available Wednesday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="12">Second available Thursday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="13">Second available Friday</option>`);
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="14">Second available Saturday</option>`);
                break;
            case "C":
                $("#IntFinH_NextRecalcDate_Cd").append(`<option value="0"></option>`);
                for (let i = 1; i <= 31; i++) {
                    $("#IntFinH_NextRecalcDate_Cd").append(`<option value="${i}">${i}</option>`);
                }
                break;
        }
    }

    $(document)
        .on('click', '#ClearPaymentPeriod', ResetPeriod)
        .on('click', '#ClearCumulative', ResetCumulative)
        .on('change', '#Appl_Dbtr_Addr_CtryCd', LoadProvinces)
        .on('change', '#PymPr_Cd', LoadRecalcDate);

    LoadProvinces();

});