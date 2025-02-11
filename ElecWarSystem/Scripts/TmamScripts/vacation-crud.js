window.onload = function () {
    setVacationCouter();
    RequestTmamStatus();
    numbersE2A();
}

function IsAllFieldsFilled() {
    var result =
        $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#vacation-type").val() !== "" &&
        $("#date-from").val() !== "" &&
        $("#date-to").val() !== ""
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
function Add() {
    // الحصول على التواريخ كقيم نصية
    var dateFrom = $("#date-from").val();
    var dateTo = $("#date-to").val();

    // التأكد أن التاريخ يتم إرساله بالتنسيق الصحيح بدون أي تحقق
    var formattedDateFrom = new Date(dateFrom).toISOString().split('T')[0]; // YYYY-MM-DD
    var formattedDateTo = new Date(dateTo).toISOString().split('T')[0];     // YYYY-MM-DD

    // make sure the from date is before the to date
    if (dateFrom > dateTo) {
        console.log("error");
        Swal.fire({
            icon: 'error',
            title: 'خطأ',
            text: 'يوجد خطأ في تاريخ الأجازة',
        });
        return;
    }

    // إرسال الطلب AJAX
    $.ajax({
        url: window.location.origin + "/Vacation/Create",
        type: "POST",
        async: false,
        data: {
            "VacationDetail.PersonID": $("#person-name").val(),
            "VacationDetail.VacationType": $("#vacation-type").val(),
            "VacationDetail.DateFrom": formattedDateFrom, // استخدام التاريخ المنسق
            "VacationDetail.DateTo": formattedDateTo     // استخدام التاريخ المنسق
        },
        success: function (result) {
            if (result == -1) {
                Swal.fire({
                    icon: 'error',
                    title: 'خطأ',
                    text: 'يوجد خطأ في تاريخ الأجازة',
                });
            } else {
                closePop();
                UpdateVacationsTable();
                IncreaseVacationCounter();
                emptyFormField();

                // SweetAlert لإظهار رسالة النجاح
                Swal.fire({
                    icon: 'success',
                    title: 'تمت الإضافة بنجاح!',
                    showConfirmButton: false,
                    timer: 1500
                });
            }
        }
    });
}




function emptyFormField() {
    $("#person-name").val(null)
    $("#person-rank").val(null)
    $("#vacation-type").val(null)
    $("#date-from").val(null)
    $("#date-to").val(null)
}

function UpdateVacationsTable() {
    $.ajax({
        url: window.location.origin + "/Vacation/GetVacation",
        type: "GET",
        async: false,
        success: function (result) {
            fillVacationTable(result);
        }
    })
}

function fillVacationTable(result) {
    $("#vacations-table").empty();

    var tableHead = `
        <thead>
            <th>م</th>
            <th>الرتبة / الدرجة</th>
            <th>الإسم </th>
            <th>نوع الأجازة</th>
            <th>بدء الأجازة</th>
            <th>عودة الأجازة</th>
            <th>الاجراءات</th>
        </thead>`;
    $("#vacations-table").append(tableHead);

    for (var index in result) {
        var tableItem = `
            <tbody style="font-size:14px;">
                <td class = "aribiano">${parseInt(index) + 1}</td>
                <td>${result[index]['VacationDetail']['Person']['Rank']['RankName']}</td>
                <td>${result[index]['VacationDetail']['Person']['FullName']}</td>
                <td>${result[index]['VacationDetail']['VacationType']}</td>
                <td class = "aribiano">${getDateFormated(result[index]['VacationDetail']['DateFrom'])}</td>
                <td class = "aribiano">${getDateFormated(result[index]['VacationDetail']['DateTo'])}</td>
                <td>
                    <button class="delete-btn" onclick="deleteVacation(${result[index]["ID"]})">
                        <i class ="fas fa-trash"></i>
                    </button>
                    
                </td>
            </tbody>`;
        $("#vacations-table").append(tableItem);
    }
}

function openVacationPopup() {
    $('#vacationModal').modal('show'); // This will correctly show the modal
}


function setVacationCouter() {
    var listOfVacationNumbers = $("#vacation-counter").text().split("/");
    var currentVacationCount = parseInt(listOfVacationNumbers[0]);
    var totalVacationCount = parseInt(listOfVacationNumbers[1]);
    if (totalVacationCount === currentVacationCount || timeOutCounter) {
        $("#add-vacation-btn").attr('disabled', 'disabled');
    } else {
        $("#add-vacation-btn").removeAttr('disabled');
    }
}

function IncreaseVacationCounter() {
    var listOfVacationNumbers = $("#vacation-counter").text().split("/");
    var currentVacationCount = parseInt(listOfVacationNumbers[0]);
    var totalVacationCount = parseInt(listOfVacationNumbers[1]);
    currentVacationCount += 1;
    if (totalVacationCount === currentVacationCount) {
        $("#add-vacation-btn").attr('disabled', 'disabled');
    }
    var newStr = `${currentVacationCount} / ${totalVacationCount}`;
    $("#vacation-counter").text(newStr);
}

function DecreaseVacationCounter() {
    var listOfVacationNumbers = $("#vacation-counter").text().split("/");
    var currentVacationCount = parseInt(listOfVacationNumbers[0]);
    var totalVacationCount = parseInt(listOfVacationNumbers[1]);
    currentVacationCount -= 1;
    $("#add-vacation-btn").removeAttr('disabled');
    var newStr = `${currentVacationCount} / ${totalVacationCount}`;
    $("#vacation-counter").text(newStr);
}

function closePop() {
    $('#vacationModal').modal('hide'); // This will correctly hide the modal
}
function deleteVacation(id) {
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: "لن تتمكن من التراجع عن هذا!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'نعم، احذفها!',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: window.location.origin + "/Vacation/Delete",
                type: "POST",
                async: false,
                data: {
                    "VacationID": id,
                },
                success: function () {
                    UpdateVacationsTable();
                    DecreaseVacationCounter();
                    Swal.fire(
                        'تم الحذف!',
                        'تم حذف الأجازة بنجاح.',
                        'success'
                    );
                }
            });
        }
    });
}

function closePop() {
    $('#vacationModal').modal('hide');
}
