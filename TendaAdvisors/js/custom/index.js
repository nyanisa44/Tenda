
$(document).ready(function () {

    /**/
    $("#popupMenu").click(function () {
        if ($("#bs-example-navbar-collapse-1").css("display") == "none")
            $("#bs-example-navbar-collapse-1").slideDown();
        else
            $("#bs-example-navbar-collapse-1").slideUp();
    });

});
