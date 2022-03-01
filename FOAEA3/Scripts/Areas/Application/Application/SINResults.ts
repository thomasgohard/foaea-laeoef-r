$(document).ready(function (): void {

    let columnDataSINResult = [
        { title: "Prov            " },
        { title: "Submitter       " },
        { title: "Ctrl Code       " },
        { title: "Prov Case #     " },
        { title: "Date Received   " },
        { title: "DOB Code        " },
        { title: "First Name Code " },
        { title: "Middle Name Code" },
        { title: "Surname Code    " },
        { title: "Mother's Name Code"},
        { title: "Sex Code        " },
        { title: "SIN Code        " },
        { title: "SIN Status      " },
    ];

    DataTableManager.Setup(columnDataSINResult);

});