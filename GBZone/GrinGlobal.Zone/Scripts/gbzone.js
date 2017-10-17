/*
//script to execute some function in GBZone
*/
$(document).ready(function () {

    //apply mask for BOX textbox
    $('.ggbox').mask("AAAA-AAA-##", { placeholder: "CAR-SEC-BOX" });

    //validation in filters
    $("#formFilter").validate({
        rules: {
            text01: {
                required: false,
                number: true
            },
            text02: {
                required: false,
                minlength: 11,
                maxlength: 11
            },
            text03: {
                required: false,
                minlength: 1,
                maxlength: 20
            }
        },
        submitHandler: function (form) {
            form.submit();
        }
    });

    //apply for Apply button that is in Navigation view in Shared View folder
    $("#btnapply").click(function () {

        var radioValue = $("input[name='radios']:checked").val();


        if (radioValue) {
            var textId = '#text' + radioValue.trim();
            var textValue = $(textId).val().trim();
            //var parameter = $(textId).name();
            window.location.href = window.location.origin + '/gbzone/Home/ShowView?chooseSelected=' + radioValue + '&value=' + textValue;
        }
    });

    //
    $("#text01,#text02,#text03").keyup(function (event) {
        if (event.keyCode == 13) {
            $("#btnapply").click();
        }
    });

    // Initialize footable
    $('#viewTable').footable({
        "paging": {
            "enabled": true,
            "countFormat": "Page {CP} of {TP}",
            "current": 1,
            "limit": 3,
            "position": "center",
            "size": 10
        },
        "sorting": {
            "enabled": true
        },
        "filtering": {
            "enabled": true,
            "dropdownTitle": "Search in:",
            "placeholder": "Search...",
            "position": "left"
        }
    });
});
