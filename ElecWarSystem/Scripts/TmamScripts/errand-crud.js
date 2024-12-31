

window.onload = function () {
    setErrandCounter();
    RequestTmamStatus();
    numbersE2A();
    UpdateErrandTable();  // لتحديث الجدول عند التحميل
};

// التأكد من تعبئة جميع الحقول
function IsAllFieldsFilled() {
    var result =
        $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#errand-place").val() !== "" &&
        $("#errand-commandor").val() !== "" &&
        $("#date-from").val() !== "" &&
        $("#date-to").val() !== "";
    return result;
}

// تمكين الزر عند تعبئة الحقول
function disableBtn() {
    if (IsAllFieldsFilled()) {
        $(".popup-submit-btn").removeAttr('disabled');
    } else {
        $(".popup-submit-btn").attr('disabled', 'disabled');
    }
}

// إضافة مأمورية جديدة
function Add() {

    if ($("#date-from").val() >= $("#date-to").val()) {
        Swal.fire("خطأ", "يوجد خطأ فى تاريخ المأموريات", "error");
        return;
    }

    $.ajax({
        url: window.location.origin + "/Errand/Create",
        type: "POST",
        async: true,
        data: {
            "ErrandDetail.PersonID": $("#person-name").val(),
            "ErrandDetail.ErrandPlace": $("#errand-place").val(),
            "ErrandDetail.ErrandCommandor": $("#errand-commandor").val(),
            "ErrandDetail.DateFrom": $("#date-from").val(),
            "ErrandDetail.DateTo": $("#date-to").val()
        },
        success: function (result) {
            if (result == -1) {
                Swal.fire("خطأ", "يوجد خطأ فى تاريخ المأموريات", "error");
            } else {
                closePop();
                UpdateErrandTable();
                IncreaseErrandCounter();
                emptyFormField();
                Swal.fire("نجاح", "تمت إضافة المأمورية بنجاح", "success");
            }
        }
    });
}



// إفراغ الحقول بعد الإضافة أو التعديل
function emptyFormField() {
    $("#person-name").val(null);
    $("#person-rank").val(null);
    $("#errand-place").val(null);
    $("#errand-commandor").val(null);
    $("#date-from").val(null);
    $("#date-to").val(null);
}

// تحديث الجدول بعد كل عملية
function UpdateErrandTable() {
    $.ajax({
        url: window.location.origin + "/Errand/GetErrands",  // تأكد أن هذا هو المسار الصحيح
        type: "GET",
        success: function (result) {
            fillErrandTable(result);
        },
        error: function () {
            Swal.fire("خطأ", "حدث خطأ أثناء جلب البيانات", "error");
        }
    });
}

// عرض البيانات في الجدول
function fillErrandTable(result) {
    $("#errand-table").empty();
    var tableHead = `
        <thead>
            <th>م</th>
            <th>الرتبة / الدرجة</th>
            <th>الإسم </th>
            <th>جهة المأمورية</th>
            <th>الأمر</th>
            <th>التاريخ من</th>
            <th>التاريخ إلى</th>
            <th>الإجراءات</th>
        </thead>`;
    $("#errand-table").append(tableHead);

    for (var index in result) {
        var tableItem = `
            <tbody style="font-size:14px;">
                <td class =  "aribiano">${parseInt(index) + 1}</td >
                <td>${result[index]['ErrandDetail']['Person']['Rank']['RankName']}</td>
                <td>${result[index]['ErrandDetail']['Person']['FullName']}</td>
                <td>${result[index]['ErrandDetail']['ErrandPlace']}</td>
                <td>${result[index]['ErrandDetail']['ErrandCommandor']}</td>
                <td class =  "aribiano">${getDateFormated(result[index]['ErrandDetail']['DateFrom'])}</td>
                <td class =  "aribiano">${getDateFormated(result[index]['ErrandDetail']['DateTo'])}</td>
                <td>
                <button class="delete-btn" onclick="deleteErrand(${result[index]['ID']})">
                    <i class="fas fa-trash"></i>
                </button>
                </td>
            </tbody>`;
        $("#errand-table").append(tableItem);
    }
}

function openErrandPopup() {
    $('#errandModal').modal('show');
}

// إغلاق المودال
function closePop() {
    $('#errandModal').modal('hide');
}

// حذف مأمورية
function deleteErrand(id) {
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: "لن يمكنك التراجع عن هذا!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'نعم، احذفه!',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: window.location.origin + "/Errand/Delete",
                type: "POST",
                data: { "errandID": id },
                success: function () {
                    
                    UpdateErrandTable();
                    DecreaseErrandCounter();
                    Swal.fire("تم الحذف!", "تم حذف المأمورية بنجاح", "success");
                },
                error: function () {
                    Swal.fire("خطأ", "حدث خطأ أثناء حذف البيانات", "error");
                    
                }
            });
        }
    });
}

function setErrandCounter() {
    var listOfErrandNumbers = $("#errand-counter").text().split("/");
    var currentErrandCount = parseInt(listOfErrandNumbers[0]);
    var totalErrandCount = parseInt(listOfErrandNumbers[1]);
    if (totalErrandCount === currentErrandCount || timeOutCounter) {
        $("#add-errand-btn").attr('disabled', 'disabled');
    } else {
        $("#add-errand-btn").removeAttr('disabled');
    }
    console.log(listOfErrandNumbers);
}

function IncreaseErrandCounter() {
    var listOfErrandNumbers = $("#errand-counter").text().split("/");
    var currentErrandCount = parseInt(listOfErrandNumbers[0]);
    var totalErrandCount = parseInt(listOfErrandNumbers[1]);
    currentErrandCount += 1;
    if (totalErrandCount === currentErrandCount) {
        $("#add-errand-btn").attr('disabled', 'disabled');
    }
    var newStr = `${currentErrandCount} / ${totalErrandCount}`;
    $("#errand-counter").text(newStr);

}
function DecreaseErrandCounter() {
    var listOfErrandNumbers = $("#errand-counter").text().split("/");
    var currentErrandCount = parseInt(listOfErrandNumbers[0]);
    var totalErrandCount = parseInt(listOfErrandNumbers[1]);
    currentErrandCount -= 1;
    $("#add-errand-btn").removeAttr('disabled');
    var newStr = `${currentErrandCount} / ${totalErrandCount}`;
    $("#errand-counter").text(newStr);
}

