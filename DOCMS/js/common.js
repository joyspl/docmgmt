var ddlAjaxCallAsync = true;
var regexEmail = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*/;
var dontBlock = false;

var loadingMessage = "<div id='loader-wrapper'><div id='loader'></div><div class='loader-section section-left'></div><div class='loader-section section-right'></div></div>";
var loadingMessageMini = "<div class='row'><div class='col-sm-4'>&nbsp;</div><div class='col-sm-4'><div class='loaderzee'></div></div><div class='col-sm-4'>&nbsp;</div>";
$(document).ajaxStart(function () {
    if (!dontBlock) {
        $.blockUI({ message: loadingMessage, baseZ: 999999999 })
    }
}).ajaxStop($.unblockUI).ajaxError($.unblockUI);

$(window).load(function () {
    $.unblockUI();
});

$(document.body).on("keydown", ".number", function (e) {
    if ($.inArray(e.keyCode, [46, 8, 9, 27, 32, 13, 110, 188, 189, 190, 191]) !== -1
        || (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true))
        || (e.keyCode >= 35 && e.keyCode <= 40)) {
        return;
    }
    if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57))
        && (e.keyCode < 96 || e.keyCode > 105)) {
        e.preventDefault();
    }
});

function ValidateEmail(email) {
    if (regexEmail.test(email)) {
        return true;
    } else {
        return false;
    }
}

function rotateBase64Image90deg(base64Image, isClockwise) {
    // create an off-screen canvas
    var offScreenCanvas = document.createElement('canvas');
    offScreenCanvasCtx = offScreenCanvas.getContext('2d');

    // cteate Image
    var img = new Image();
    img.src = base64Image;

    // set its dimension to rotated size
    offScreenCanvas.height = img.width;
    offScreenCanvas.width = img.height;

    // rotate and draw source image into the off-screen canvas:
    if (isClockwise) {
        offScreenCanvasCtx.rotate(90 * Math.PI / 180);
        offScreenCanvasCtx.translate(0, -offScreenCanvas.width);
    } else {
        offScreenCanvasCtx.rotate(-90 * Math.PI / 180);
        offScreenCanvasCtx.translate(-offScreenCanvas.height, 0);
    }
    offScreenCanvasCtx.drawImage(img, 0, 0);

    // encode image to data-uri with base64
    return offScreenCanvas.toDataURL("image/jpeg", 100);
}

function alertInfo(alertType, alertText, isModal) {
    new Noty({
        type: alertType,
        layout: "topCenter",
        theme: "nest",
        text: alertText,
        timeout: 5000,
        progressBar: true,
        closeWith: ["click", "button"],
        animation: {
            open: "noty_effects_open",
            close: "noty_effects_close"
        },
        id: false,
        force: true,
        killer: true,
        queue: "global",
        container: false,
        buttons: [],
        sounds: {
            sources: [],
            volume: 1,
            conditions: []
        },
        titleCount: {
            conditions: []
        },
        modal: isModal
    }).show()
}