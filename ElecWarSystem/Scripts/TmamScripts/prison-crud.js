window.onload = function () {
    setPrisonCouter();
    RequestTmamStatus();
    numbersE2A();
}
function openPrisonPopup() {
    $("#prisonModal").modal('show');
}
function closePop() {
    $("#prisonModal").modal('hide');
}

function IsAllFieldsFilled() {
    var result =
        $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#crime").val() !== "" &&
        $("#punishment").val() !== "" &&
        $("#punisher").val() !== "" &&
        $("#prison-place").val() !== "" &&
        $("#date-from").val() !== "" &&
        $("#date-to").val() !== "" &&
        $("#command-number").val() !== "" &&
        $("#command-date").val() !== ""

    return result;
}

function disableBtn() {
    if (IsAllFieldsFilled()) {
        $(".popup-submit-btn").removeAttr('disabled');
    }
    else {
        $(".popup-submit-btn").attr('disabled', 'disabled');
    }
}

function emptyFormField() {
    $("#person-name").val(null)
    $("#person-rank").val(null)
    $("#crime").val(null)
    $("#punishment").val(null)
    $("#punisher").val(null)
    $("#date-from").val(null)
    $("#date-to").val(null)
    $("#prison-place").val(null)
    $("#command-number").val(null)
    $("#command-date").val(null)
}
function Add() {

    if ($('#date-from').val() >= $('#date-to').val()) {
        Swal.fire("خطأ", "يوجد خطأ فى تاريخ السجن", "error");
        return;
    }

    $.ajax({
        url: window.location.origin + "/Prison/Create",
        type: "POST",
        async: false,
        data: {
            "PrisonDetails.PersonID": $("#person-name").val(),
            "PrisonDetails.Crime": $("#crime").val(),
            "PrisonDetails.Punishment": $("#punishment").val(),
            "PrisonDetails.Punisher": $("#punisher").val(),
            "PrisonDetails.DateFrom": $("#date-from").val(),
            "PrisonDetails.DateTo": $("#date-to").val(),
            "PrisonDetails.PrisonPlace": $("#prison-place").val(),
            "PrisonDetails.CommandItem.Number": $("#command-number").val(),
            "PrisonDetails.CommandItem.Date": $("#command-date").val()
        },
        success: function (result) {
            if (result == -1) {
                Swal.fire("خطأ", "يوجد خطأ فى تاريخ السجن", "error");
            }
            else {
                Swal.fire({
                    title: "تم بنجاح",
                    text: "تم إضافة السجين بنجاح",
                    icon: "success",
                    confirmButtonText: "موافق"
                }).then(function () {
                    closePop();
                    emptyFormField();
                    UpdatePrisonsTable();
                    IncreasePrisonCounter();
                });
            }
        },
        error: function () {
            Swal.fire("خطأ", "حدث خطأ أثناء إضافة السجين", "error");
        }
    });
}





function UpdatePrisonsTable() {
    $.ajax({
        url: window.location.origin + "/Prison/GetPrisons",
        type: "GET",
        async: false,
        success: function (result) {
            fillPrisonTable(result);
        }
    })
}

function fillPrisonTable(result) {
    $("#prison-table").empty();

    var tableHead = `
        <thead>
            <th>م</th>
            <th>الرتبة / الدرجة</th>
            <th>الإسم </th>
            <th>الجريمة</th>
            <th>العقوبة</th>
            <th>الآمر بالعقوبة</th>
            <th>مدة التنفذ من</th>
            <th>مدة التنفذ إلى</th>
            <th>مكان التنفيذ</th>
            <th>بند الأوامر</th>
            <th>تاريخ البند </th>
            <th>الاجراءات</th>
        </thead>`;
    $("#prison-table").append(tableHead);

    for (var index in result) {
        var tableItem = `
            <tbody style="font-size:14px;">
                <td>${parseInt(index) + 1}</td>
                <td>${result[index]['PrisonDetails']['Person']['Rank']['RankName']}</td>
                <td>${result[index]['PrisonDetails']['Person']['FullName']}</td>
                <td>${result[index]['PrisonDetails']['Crime']}</td>
                <td>${result[index]['PrisonDetails']['Punishment']}</td>
                <td>${result[index]['PrisonDetails']['Punisher']}</td>
                <td>${getDateFormated(result[index]['PrisonDetails']['DateFrom'])}</td>
                <td>${getDateFormated(result[index]['PrisonDetails']['DateTo'])}</td>
                <td>${result[index]['PrisonDetails']['PrisonPlace']}</td>
                <td>${result[index]['PrisonDetails']['CommandItem']['Number']}</td>
                <td>${getDateFormated(result[index]['PrisonDetails']['CommandItem']['Date'])}</td>
                <td>
                    <button class="delete-btn" onclick="deletePrison(${result[index]["ID"]})">
                        
                        <i class = "fas fa-trash"></i>
                    </button>
                    
                </td>
            </tbody>`;
        $("#prison-table").append(tableItem);
    }
}




function setPrisonCouter() {
    var listOfPrisonNumbers = $("#prison-counter").text().split("/");
    var currentPrisonCount = parseInt(listOfPrisonNumbers[0]);
    var totalPrisonCount = parseInt(listOfPrisonNumbers[1]);
    if (totalPrisonCount === currentPrisonCount || timeOutCounter) {
        $("#add-prison-btn").attr('disabled', 'disabled');
    } else {
        $("#add-prison-btn").removeAttr('disabled');
    }
}

function IncreasePrisonCounter() {
    var listOfPrisonNumbers = $("#prison-counter").text().split("/");
    var currentPrisonCount = parseInt(listOfPrisonNumbers[0]);
    var totalPrisonCount = parseInt(listOfPrisonNumbers[1]);
    currentPrisonCount += 1;
    if (totalPrisonCount === currentPrisonCount) {
        $("#add-prison-btn").attr('disabled', 'disabled');
    }
    var newStr = `${currentPrisonCount} / ${totalPrisonCount}`;
    $("#prison-counter").text(newStr);
}

function DecreasePrisonCounter() {
    var listOfPrisonNumbers = $("#prison-counter").text().split("/");
    var currentPrisonCount = parseInt(listOfPrisonNumbers[0]);
    var totalPrisonCount = parseInt(listOfPrisonNumbers[1]);
    currentPrisonCount -= 1;
    $("#add-prison-btn").removeAttr('disabled');
    var newStr = `${currentPrisonCount} / ${totalPrisonCount}`;
    $("#prison-counter").text(newStr);
}

function deletePrison(id) {
    Swal.fire({
        title: "هل أنت متأكد؟",
        text: "لن تتمكن من استرجاع البيانات بعد الحذف!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: "نعم، احذف!",
        cancelButtonText: "إلغاء"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: window.location.origin + "/Prison/Delete",
                type: "POST",
                async: false,
                data: { "id": id },
                success: function () {
                    Swal.fire("تم الحذف!", "تم حذف السجين بنجاح.", "success");
                    UpdatePrisonsTable();
                    DecreasePrisonCounter();
                },
                error: function () {
                    Swal.fire("خطأ", "حدث خطأ أثناء الحذف", "error");
                }
            });
        }
    });
}
