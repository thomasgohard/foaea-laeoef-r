namespace Submitter {

    $(document).ready(function (): void {

        $("#selectProvinces").change(function () { GetEnfOfficesForSelectedProvince(GetSubmitters); });
        $("#chkActiveOnly").change(function () { GetSubmitters(); });
        $("#selectEnfOff").change(function () { GetSubmitters(); });
        $(".addNew").click(function () {

            let selectedProvince: string = $("#selectProvinces").val() as string;
            let selectedOffSrv: string = $("#selectEnfOff").val() as string;
            let selectedOffice: string = "";
            let selectedService: string = "";

            if (selectedOffSrv.indexOf("-") > 0) {
                let values: string[] = selectedOffSrv.split("-");
                selectedOffice = values[1];
                selectedService = values[0];
            }

            let linkRef: string | undefined = $(this).attr("href");
            $(this).attr("href", `${linkRef}?provCd=${selectedProvince}&offCd=${selectedOffice}&srvCd=${selectedService}`);

        });

        GetProvinces(GetEnfOfficesForSelectedProvince, GetSubmitters);

    });

    export function GetEnfOfficesForSelectedProvince(callBack?: Function, optionCallBack?: Function): void {

        let provCd: string = $("#selectProvinces option:selected").val() as string;
        let actionURL: string = `${actionGetEnfOffForProvinceURL}?provCd=${provCd}`;

        $.get(actionURL).done(function (enfOff) {

            $('#selectEnfOff').empty();

            $('#selectEnfOff').append('<option value="-1">** All **</option>');
            $.each(enfOff, function (i, o) {
                let displayValue: string = PrepString(o.enfSrv_Cd) + ' | ' + PrepString(o.enfOff_City_LocCd) +
                    ' | ' + PrepStringAndPad(o.enfOff_Addr_CityNme, 20) +
                    ' | ' + PrepStringAndPad(o.enfOff_Nme, 25) + ' | ' + PrepString(o.actvSt_Cd);
                let keyValue: string = o.enfSrv_Cd.trim() + "-" + o.enfOff_City_LocCd.trim()
                let item: string = `<option value="${keyValue}">${displayValue}</option>`;
                $('#selectEnfOff').append(item);
            });

            if (typeof initEnfOff !== 'undefined' && initEnfOff !== null && initEnfOff != "")
                $("#selectEnfOff").val(initEnfOff); // select given item

            if ($("#selectEnfOff").val() == null) {
                $("#selectEnfOff").val($("#selectEnfOff option:first").val()!); // select first item
            }

            if (callBack != null) {
                callBack(optionCallBack);
            }

        });

    }

    export function GetSubmitters(): void {

        let columnData = [
            { title: "Submitter Code" },
            { title: "Office" },
            { title: "First Name" },
            { title: "Surname" },
            { title: "Status" }
        ];

        let onlyActive: boolean = $("#chkActiveOnly").is(":checked");
        let provCd: string = $("#selectProvinces option:selected").val() as string;
        let enfOffKey: string = $("#selectEnfOff option:selected").val() as string;
        let actionURL: string = `${actionGetSubmittersForProvinceURL}?provCd=${provCd}&onlyActive=${onlyActive}&enfOffKey=${enfOffKey}`;
        $.get(actionURL).done(
            function (submitters): void {

                DataTableManager.Destroy();

                $("#submitterDataRows").empty();

                $.each(submitters, function (i, s) {
                    let rowData = `<tr>
                                   <td>${s.subm_SubmCd}</td>
                                   <td>${s.enfSrv_Cd}</td>
                                   <td>${s.subm_FrstNme}</td>
                                   <td>${s.subm_SurNme}</td>
                                   <td>${s.actvSt_Cd}</td>
                               </tr>`;
                    $("#submitterDataRows").append(rowData);
                });

                DataTableManager.Setup(columnData);

                // set default order to be Status, Surname, First Name
                let table = $("#datatablesGrid").DataTable();
                table.order([[4, "asc"], [3, "asc"], [2, "asc"]])
                     .draw();

            });

    }

    $('#datatablesGrid tbody').on('click', 'tr', function (): void {

        let table: DataTables.Api = $('#datatablesGrid').DataTable();
        let idx: number = table.row(this).index();
        let id: string = table.cell(idx, 0).data();
        let provCd: string = $("#selectProvinces option:selected").val() as string;
        if (provCd == undefined)
            provCd = $("selectProvinces option:first").val() as string;

        document.location.href = `${actionEditSubmitterURL}?submCd=${id}&provCd=${provCd}`;

    });
}