window.onload = function () {
    setAbsenceCounter();
    RequestTmamStatus();
    numbersE2A();
}

function IsAllFieldsFilled() {
    var result =
        $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#absence-times").val() !== "" &&
        $("#date-from").val() !== "" &&
        $("#command-number").val() !== "" &&
        $("#command-date").val() !== ""
    return result;
}

function disableBtn() {
    if (IsAllFieldsFilled()) {
        $(".popup-submit-btn").removeAttr('disabled');
    } else {
        $(".popup-submit-btn").attr('disabled', 'disabled');
    }
}

function Add() {
    $.ajax({
        url: window.location.origin + "/Absence/Create",
        type: "POST",
        async: false,
        data: {
            "AbsenceDetail.PersonID": $("#person-name").val(),
            "AbsenceDetail.AbsenceTimes": parseInt($("#absence-times").val()),
            "AbsenceDetail.DateFrom": $("#date-from").val(),
            "AbsenceDetail.commandItem.Number": $("#command-number").val(),
            "AbsenceDetail.commandItem.Date": $("#command-date").val(),
        },
        success: function (result) {
            if (result == -1) {
                Swal.fire({
                    icon: 'error',
                    title: 'خطأ',
                    text: 'يوجد خطأ في تاريخ الغياب!',
                });
            } else {
                Swal.fire({
                    icon: 'success',
                    title: 'تم الحفظ!',
                    text: 'تم إضافة الغياب بنجاح!',
                }).then(() => {
                    closePop();
                    UpdateAbsencesTable();
                    IncreaseAbsenceCounter();
                    emptyFormField();
                });
            }
        }
    });
}

function emptyFormField() {
    $("#person-name").val(null);
    $("#person-rank").val(null);
    $("#absence-times").val(null);
    $("#date-from").val(null);
    $("#command-number").val(null);
    $("#command-date").val(null);
}

function UpdateAbsencesTable() {
    $.ajax({
        url: window.location.origin + "/Absence/GetAbsences",
        type: "GET",
        async: false,
        success: function (result) {
            fillAbsenceTable(result);
        }
    });
}

function fillAbsenceTable(result) {
    $("#absence-table").empty();

    var tableHead = `
        <thead>
            <th>م</th>
            <th>الرتبة / الدرجة</th>
            <th>الإسم </th>
            <th>التاريخ الغياب</th>
            <th>دفعة الغياب</th>
            <th >بند الأوامر</th>
            <th > تاريخ البند </th>
            <th> الاجراءات </th>
        </thead>`;
    $("#absence-table").append(tableHead);

    for (var index in result) {
        var tableItem = `
            <tbody style="font-size:14px;">
                <td class = "aribiano">${parseInt(index) + 1}</td>
                <td>${result[index]['AbsenceDetail']['Person']['Rank']['RankName']}</td>
                <td>${result[index]['AbsenceDetail']['Person']['FullName']}</td>
                <td class = "aribiano">${getDateFormated(result[index]['AbsenceDetail']['DateFrom'])}</td>
                <td>${result[index]['AbsenceDetail']['AbsenceTimes']}</td>
                <td>${result[index]['AbsenceDetail']['commandItem']['Number']}</td>
                <td class = "aribiano">${getDateFormated(result[index]['AbsenceDetail']['commandItem']['Date'])}</td>
                <td>
                    <button class="delete-btn " onclick="deleteAbsence(${result[index]["ID"]})">
                        <i class ="fas fa-trash"></i>
                    </button>
                </td>
            </tbody>`;
        $("#absence-table").append(tableItem);
    }
}

function setAbsenceCounter() {
    var listOfAbsenceNumbers = $("#absence-counter").text().split("/");
    var currentAbsenceCount = parseInt(listOfAbsenceNumbers[0]);
    var totalAbsenceCount = parseInt(listOfAbsenceNumbers[1]);
    if (totalAbsenceCount === currentAbsenceCount || timeOutCounter) {
        $("#add-absence-btn").attr('disabled', 'disabled');
    } else {
        $("#add-absence-btn").removeAttr('disabled');
    }
    console.log(listOfAbsenceNumbers);
}


function IncreaseAbsenceCounter() {
    var listOfAbsenceNumbers = $("#absence-counter").text().split("/");
    var currentAbsenceCount = parseInt(listOfAbsenceNumbers[0]);
    var totalAbsenceCount = parseInt(listOfAbsenceNumbers[1]);
    currentAbsenceCount += 1;
    if (totalAbsenceCount === currentAbsenceCount) {
        $("#add-absence-btn").attr('disabled', 'disabled');
    }
    var newStr = `${currentAbsenceCount} / ${totalAbsenceCount}`;
    $("#absence-counter").text(newStr);
}

function DecreaseAbsenceCounter() {
    var listOfAbsenceNumbers = $("#absence-counter").text().split("/");
    var currentAbsenceCount = parseInt(listOfAbsenceNumbers[0]);
    var totalAbsenceCount = parseInt(listOfAbsenceNumbers[1]);
    currentAbsenceCount -= 1;
    $("#add-absence-btn").removeAttr('disabled');
    var newStr = `${currentAbsenceCount} / ${totalAbsenceCount}`;
    $("#absence-counter").text(newStr);
}

function deleteAbsence(id) {
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: "لن تتمكن من التراجع عن هذا!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'نعم، احذفه!',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: window.location.origin + "/Absence/Delete",
                type: "POST",
                async: false,
                data: { "AbsenceID": id },
                success: function () {
                    UpdateAbsencesTable();
                    DecreaseAbsenceCounter();
                    Swal.fire({
                        icon: 'success',
                        title: 'تم الحذف!',
                        text: 'تم حذف المأمورية بنجاح!',
                    });
                }
            });
        }
    });
}

function openAbsencePopup() {
    $('#absenceModal').modal('show'); // This will show the modal using Bootstrap's modal method
}

function closePop() {
    $('#absenceModal').modal('hide');
}
