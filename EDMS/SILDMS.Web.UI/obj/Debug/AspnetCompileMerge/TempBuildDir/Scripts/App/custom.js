$(document).ready(function () {
    //Command: toastr["info"]('Page Loaded!')


    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }



});


function IsPositiveNumber(val)
{
    debugger;
    if(((val - 0) > 0) && (!isNaN(val)))
    {
        return true;
    }
    return false;
}

var convArrToObj = function (array) {
    var thisEleObj = new Object();
    if (typeof array == "object") {
        for (var i in array) {
            var thisEle = convArrToObj(array[i]);
            thisEleObj[i] = thisEle;
        }
    } else {
        thisEleObj = array;
    }
    return thisEleObj;
}

var applySecurity = function () {

    $.ajax({
        url: "/SecurityModule/Account/GetActionPermission?url=" + window.location.pathname,
        cache: true,
        success: function (data) {
            if (data !== "")
            {
                $(".pnlView").hide();
                $(".btnSave").hide();
                $(".btnEdit").hide();
                $(".btnPrint").hide();
                $(".btnInfoPrint").hide();
                $(".btnBatchPrint").hide();
                $(".btnLoad").hide();
                $(".btnScan").hide();
                $(".btnRemove").hide();
                $(".btnDownload").hide();
                $(".btnRotate").hide();
                $(".btnSearch").hide();
                $(".btnMakeVersion").hide();
                $(".btnDistribute").hide();
                $(".btnEmail").hide();
                $(".chkInfoCopy").attr('disabled', 'disabled');
                var strs = data.split("|");
                for (var i = 0; i < strs.length - 1; i++) {
                    if (strs[i] != null) {                        
                        $("." + strs[i]).show();
                        $("." + strs[i]).removeAttr('disabled');
                    }
                }
            }            
        }
    });

}


function GetCurrentDate() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!

    var yyyy = today.getFullYear();
    if (dd < 10) {
        dd = '0' + dd
    }
    if (mm < 10) {
        mm = '0' + mm
    }
    var today = dd + '/' + mm + '/' + yyyy;
    return today;

}

function CheckPositive() {

    if (($("#InvoiceAmount").val() == "") || (isNaN($("#InvoiceAmount").val()))) {
        return;
    }
    if (!IsPositiveNumber($('#InvoiceAmount').val())) {
        if (isNaN($('#InvoiceAmount').val())) {
            $('#InvoiceAmount').val('');
            toastr.error("Invoice Amount format is invalid");
        }
        else {
            toastr.error("Invoice Amount should be grater than zero");
        }
        $('#InvoiceAmount').val('');

        return;
    }
}

function FormatDigit() {
    if (!IsPositiveNumber($('#InvoiceAmount').val())) {
        $('#InvoiceAmount').val('');
    }
    else if (isNaN($('#InvoiceAmount').val())) {
        $('#InvoiceAmount').val('');
    }
    else
        $('#InvoiceAmount').val(($('#InvoiceAmount').val() - 0).toFixed(2));
}

function CheckFutureDate() {

    var EnteredDate = $('#InvoiceDateID').val(); // For JQuery
    var date = EnteredDate.substring(0, 2);
    var month = EnteredDate.substring(3, 5);
    var year = EnteredDate.substring(6, 10);
    var myDate = new Date(year, month - 1, date);
    var today = new Date();
    if (myDate > today) {
        $('#InvoiceDateID').val('');
        $('.datepicker').hide();
        toastr.error("Invoice Date should not grater than system date.");
    }
}

