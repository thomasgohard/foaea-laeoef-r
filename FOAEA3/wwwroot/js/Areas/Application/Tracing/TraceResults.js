"use strict";
$(document).ready(function () {
    var columnDataTraceResult = [
        { title: "Appl Ref Nr " },
        { title: "Date Most Recently<br />Received in FOAEA" },
        { title: "Originator" },
        { title: "Address Type<br />(E = Employer / R = Residential)" },
        { title: "Last Update Date<br />by Federal Partner" },
        { title: "Address" },
        { title: "Cycle Number of Results<br />(first cycle = 0, etc...)" }
    ];
    DataTableManager.Setup(columnDataTraceResult);
});
//# sourceMappingURL=TraceResults.js.map