"use strict";
var DataTableManager = /** @class */ (function () {
    function DataTableManager() {
    }
    DataTableManager.Setup = function (columnData, tableId) {
        if (tableId === void 0) { tableId = "datatablesGrid"; }
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
    };
    DataTableManager.Destroy = function (tableId) {
        if (tableId === void 0) { tableId = "datatablesGrid"; }
        var table = $("#" + tableId).DataTable({ retrieve: true });
        table.destroy();
    };
    return DataTableManager;
}());
//# sourceMappingURL=DatatableManager.js.map