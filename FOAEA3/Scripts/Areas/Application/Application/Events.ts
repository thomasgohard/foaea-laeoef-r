
$(document).ready(function (): void {

    let columnData = [
        { title: "Category        " },
        { title: "Service         " },
        { title: "Submitter       " },
        { title: "Control Code    " },
        { title: "Recipient       " },
        { title: "Date/Time       " },
        { title: "Reason Code     " },
        { title: "Reason Text     ", className: "text-left" },
        { title: "State           ", className: "text-left" },
        { title: "Update Submitter" }
    ];

    DataTableManager.Setup(columnData);

});

