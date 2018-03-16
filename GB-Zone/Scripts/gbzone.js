/*
//script to execute some function in GBZone
*/


//functions


function OnSuccess(response) {
    alert(response.message);
}


function OnFailure(response) {

}

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


    //
    $("#text01,#text02,#text03").keyup(function (event) {
        if (event.keyCode == 13) {
            $("#btnapply").click();
        }
    });

});


