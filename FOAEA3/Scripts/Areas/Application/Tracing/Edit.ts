$(document).ready(function (): void {

    $("#ddlComments").change(function () { UpdateComments(); });

});

function UpdateComments(): void
{
    let currentComment: string | undefined = $("#hiddenComment").val() as string;
    let addedComment: string = $("#ddlComments option:selected").text();

    if ((currentComment != undefined) && (currentComment != ""))
        currentComment += ((addedComment != "") ? ", " : "") + addedComment;
    else
        currentComment = addedComment;

    $("#Tracing\\.Appl_CommSubm_Text").val(currentComment);
}

