const LOCAL_STORAGE_MYSEARCH_CONTROLS = "mySearchControls";
const MAX_MY_SEARCH_CONTROLS = 5;

$(document).on('wb-ready.wb', function () {

    SetupMySearch();
    SetupAdvancedSearch();

    var enfSrv = "";
    var ctrlCd = "";
    var action = "";

    $('input[type="checkbox"]').click(function () {

        var chkId = $(this).attr("id");

        var mySearch = localStorage.getItem(LOCAL_STORAGE_MYSEARCH_CONTROLS);
        var mySearchArray;
        if (mySearch != null)
            mySearchArray = JSON.parse(mySearch);
        else
            mySearchArray = [];

        if ($(this).is(':checked')) {
            // add to array if not already there
            if (mySearchArray.indexOf(chkId) == -1)
                mySearchArray.push(chkId);
        } else {
            // remove from array if there
            var pos = mySearchArray.indexOf(chkId);
            if (pos > -1)
                mySearchArray.splice(pos, 1);
        }

        var data = JSON.stringify(mySearchArray);
        localStorage.setItem(LOCAL_STORAGE_MYSEARCH_CONTROLS, data);
    });

    $("#details-panel1-lnk").click(function () {
        SetupMySearch();
    });

});

function ExecuteMenuOption(item)
{
    var itemValue = $(item).find(":selected").val();    // e.g. 'ON01-E483 Edit'
     
    const values = itemValue.split(" ");
    const applKey = values[0].split("-");
    enfSrv = applKey[0];
    ctrlCd = applKey[1];
    action = values[1];

    switch (action) {
        case "Suspend":
            $("#suspendKey").val(values[0]);
            $("#suspendKeyLabel").html(values[0]);            
            $("#suspendDialog").trigger("open.wb-overlay");
            break;
        case "Transfer":
            $("#transferDialog").trigger("open.wb-overlay");
            break;
        case "Cancel":
            $("#cancelDialog").trigger("open.wb-overlay");
            break;
        default:
            document.menuForm.submit();
            break;
    }
    
}

function SetupMySearch() {

    $('div .mySearchControl').hide();

    var mySearch = localStorage.getItem(LOCAL_STORAGE_MYSEARCH_CONTROLS);
    if (mySearch != null) {

        var mySearchArray = JSON.parse(mySearch);

        var count = 0;
        mySearchArray.every(controlId => {
            count++;
            var divName = controlId.replace('chk', 'mySearch_');
            if (divName.endsWith("Range")) {
                $("#" + divName + "From").show();
                $("#" + divName + "To").show();
            }
            else
                $("#" + divName).show();

            if (count < MAX_MY_SEARCH_CONTROLS)
                return true; 
            else
                return false;
        });


    }
}

function SetupAdvancedSearch() {

    var mySearch = localStorage.getItem(LOCAL_STORAGE_MYSEARCH_CONTROLS);

    if (mySearch != null) {

        var mySearchArray = JSON.parse(mySearch);
        mySearchArray.every(controlId => $('#' + controlId).prop("checked", true));

    }
}