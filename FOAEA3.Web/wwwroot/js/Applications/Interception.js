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

    $(document).on('click', '#ClearPaymentPeriod', ResetPeriod)
               .on('change', '#Appl_Dbtr_Addr_CtryCd', LoadProvinces);

    LoadProvinces();

});