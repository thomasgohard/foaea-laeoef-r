class DataTableManager {

    static Setup(columnData: DataTables.ColumnSettings[], tableId: string = "datatablesGrid"): void {

        $("#" + tableId).DataTable({
            searching: true,
            info: true,
            paging: true,
            processing: true,
            dom: 'l<"#search"f>rtip',
            columns: columnData,
            lengthMenu: [[20, 50, -1], [20, 50, "All"]]
        });
        $('#' + tableId).DataTable();

        $("#search div label").addClass("form-inline");
        $("#search div label input").addClass("form-control");

    }

    static Destroy(tableId: string = "datatablesGrid"): void {

        var table = $("#" + tableId).DataTable({ retrieve: true });
        table.destroy();
    }

}