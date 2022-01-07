$(document).ready(function () {
    $('.timepicker').datetimepicker({
        format: 'HH:mm',
    });

    $('.datepicker').datetimepicker({
        format: 'DD/MM/YYYY'
    });

    $('.datepickerMonthly').datetimepicker({
        format: 'MM/YYYY'
    });

    $(".datepickerStartAdd").on("dp.change", function (e) {
        $('.datepickerEndAdd').data("DateTimePicker").minDate(e.date);
    });
    $(".datepickerEndAdd").on("dp.change", function (e) {
        $('.datepickerStartAdd').data("DateTimePicker").maxDate(e.date);
    });

    $(".datepickerStart").on("dp.change", function (e) {
        $('.datepickerEnd').data("DateTimePicker").minDate(e.date);
    });
    $(".datepickerEnd").on("dp.change", function (e) {
        $('.datepickerStart').data("DateTimePicker").maxDate(e.date);
    })

    $("#HideAddPlan").click(function (e) {
        e.preventDefault();
        $("#addForm").toggle(600);
    });
})

// Tooltip
//$(".more_info").click(function () {
//    var $title = $(this).find(".title");
//    if (!$title.length) {
//        $(this).append('<span class="title">' + $(this).attr("title") + '</span>');
//    } else {
//        $title.remove();
//    }
//});

$('.dropdown-toggle').dropdown()
//console.log("%c หากพบปัญหาการใช้งานกรุณาแจ้ง IT ขอบคุณครับ", "color: red; font-size: 30px;");