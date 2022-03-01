declare let actionLogoutURL: string;

$(function () {

    $("#menu").menu();

    $("ul").addClass("rounded");

    $("#menu div").hover(function () {
        $(this).addClass("rounded"); 
    });

    $.get("/Home/GetDBInfo", function (dbInfo) {
        $("#dbInfo").html(dbInfo);
    });

    $.get("/Home/GetFOAEAVersion", function (foaeaVersion) {
        $("#foaeaVersion").text(foaeaVersion);
    });

    $("#leftMenu").show();

});


$(window).on("load", function () {
    $('#loadingImg').hide();
    $("#wb-dtmd").show();
});

function setupAutoLogout() {

    const ONE_SECOND: number = 1000;
    const ONE_MINUTE: number = 60 * ONE_SECOND;
    const FOAEA_TIMEOUT: number = 15 * ONE_MINUTE; 
    const WARNING_PERIOD: number = 15;

    let count = 0;

    let redirectTimeoutId = window.setTimeout(showWarningAndStartCountdownToLogout, FOAEA_TIMEOUT);

    window.addEventListener('keypress', function () { resetTimer() }, true);
    window.addEventListener('click', function () { resetTimer() }, true);
    window.addEventListener('mousemove', function () { resetTimer() }, true);

    function showWarningAndStartCountdownToLogout() {

        window.clearTimeout(redirectTimeoutId);
        $("#logoutWarningCountdown").text(WARNING_PERIOD);
        $("#logoutWarning").show();
        redirectTimeoutId = window.setTimeout(updateCountdownWarningAndLogoutWhenZero, ONE_SECOND);

    }

    function updateCountdownWarningAndLogoutWhenZero() {

        if (count < WARNING_PERIOD) {
            $("#logoutWarningCountdown").text(WARNING_PERIOD - count);
            redirectTimeoutId = window.setTimeout(updateCountdownWarningAndLogoutWhenZero, ONE_SECOND);
        }
        else {
            window.location.href = actionLogoutURL; // call logout action from home page
        }

        count++;

    }

    function resetTimer() {

        count = 0;
        $("#logoutWarning").hide();
        $("#logoutWarningCountdown").text(WARNING_PERIOD);
        window.clearTimeout(redirectTimeoutId);
        redirectTimeoutId = window.setTimeout(showWarningAndStartCountdownToLogout, FOAEA_TIMEOUT);

    }

}