"use strict";
$(document).ready(function () {
    $("#ddlComments").change(function () { UpdateComments(); });
});
function UpdateComments() {
    var currentComment = $("#hiddenComment").val();
    var addedComment = $("#ddlComments option:selected").text();
    if ((currentComment != undefined) && (currentComment != ""))
        currentComment += ((addedComment != "") ? ", " : "") + addedComment;
    else
        currentComment = addedComment;
    $("#Tracing\\.Appl_CommSubm_Text").val(currentComment);
}
//# sourceMappingURL=Edit.js.map