"use strict";
$(document).ready(function () {
    var columnData = [
        { title: "Action        " },
        { title: "Category      " },
        { title: "Service       " },
        { title: "Submitter     " },
        { title: "Ctrl Code     " },
        { title: "Recipient     " },
        { title: "Date Received " },
        { title: "Expiry Date   " },
        { title: "Debtor SurName" },
        { title: "Source Ref Nr " },
        { title: "Justice Nr    " },
        { title: "Status        ", className: "text-left" },
        { title: "State         ", className: "text-left" }
    ];
    DataTableManager.Setup(columnData);
});
//# sourceMappingURL=ApplicationSearchResult.js.map