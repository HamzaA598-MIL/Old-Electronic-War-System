window.onload = function () {
    setHospitalCounter();
    RequestTmamStatus();
    numbersE2A();
};

function openHospitalPopup() {
    // فتح الـ modal باستخدام Bootstrap
    const hospitalModal = new bootstrap.Modal(document.getElementById('hospitalPopup'));
    hospitalModal.show();
}

function closePop() {
    // إغلاق الـ modal باستخدام Bootstrap
    const hospitalModal = bootstrap.Modal.getInstance(document.getElementById('hospitalPopup'));
    if (hospitalModal) {
        hospitalModal.hide();
    } else {
        console.error("العنصر #hospitalPopup غير موجود في DOM.");
    }
}

function disableBtn() {
    // تفعيل أو تعطيل زر الحفظ بناءً على تعبئة الحقول
    if (IsAllFieldsFilled()) {
        $(".popup-submit-btn").removeAttr('disabled');
    } else {
        $(".popup-submit-btn").attr('disabled', 'disabled');
    }
}

function IsAllFieldsFilled() {
    // التحقق من أن جميع الحقول المطلوبة تم تعبئتها
    return $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#hospital-name").val() !== "" &&
        $("#hospital-date").val() !== "" &&
        $("#diagnosis").val() !== "" &&
        $("#recommendations").val() !== "";
}

function Add() {
    // إضافة مستشفى جديد إذا تم تعبئة جميع الحقول
    if (IsAllFieldsFilled()) {
        $.ajax({
            url: window.location.origin + "/Hospital/Create",
            type: "POST",
            data: {
                "HospitalDetails.PersonID": $("#person-name").val(),
                "HospitalDetails.Hospital": $("#hospital-name").val(),
                "HospitalDetails.Diagnosis": $("#diagnosis").val(),
                "HospitalDetails.Recommendations": $("#recommendations").val(),
                "HospitalDetails.DateFrom": $("#hospital-date").val()
            },
            success: function (result) {
                if (result === -1) {
                    Swal.fire({
                        icon: 'error',
                        title: 'خطأ',
                        text: 'يوجد خطأ في تاريخ دخول المستشفى !!',
                        confirmButtonText: 'حسناً'
                    });
                } else {
                    closePop();  // إغلاق الـ modal بعد الإضافة الناجحة
                    Swal.fire({
                        icon: 'success',
                        title: 'تم الحفظ بنجاح!',
                        text: 'تمت إضافة المستشفى بنجاح.',
                        confirmButtonText: 'حسناً'
                    }).then(() => {
                        // إزالة أي طبقة `backdrop` زائدة من الـ modal
                        $('.modal-backdrop').remove();
                        $('body').removeClass('modal-open');
                        $('body').css('padding-right', '');
                    });

                    UpdateHospitalsTable(); // تحديث الجدول بعد الإضافة
                    IncreaseHospitalCounter(); // زيادة عداد المستشفيات
                    emptyFormField(); // تفريغ الحقول
                }
            }
        });
    } else {
        Swal.fire({
            icon: 'error',
            title: 'خطأ',
            text: 'يرجى ملء جميع الحقول المطلوبة!',
            confirmButtonText: 'حسناً'
        });
    }
}

function closePop() {
    // إغلاق الـ modal باستخدام Bootstrap
    const hospitalModal = bootstrap.Modal.getInstance(document.getElementById('hospitalPopup'));
    if (hospitalModal) {
        hospitalModal.hide();
        // إزالة طبقة الـ backdrop في حال لم تُزل تلقائيًا
        $('.modal-backdrop').remove();
        $('body').removeClass('modal-open');
        $('body').css('padding-right', '');
    } else {
        console.error("العنصر #hospitalPopup غير موجود في DOM.");
    }
}


function emptyFormField() {
    // تفريغ جميع الحقول بعد الحفظ
    $("#person-name, #person-rank, #hospital-name, #hospital-date, #diagnosis, #recommendations").val(null);
}

function UpdateHospitalsTable() {
    // تحديث جدول المستشفيات
    $.ajax({
        url: window.location.origin + "/Hospital/GetHospital",
        type: "GET",
        success: function (result) {
            fillHospitalTable(result);
        }
    });
}

function fillHospitalTable(result) {
    // تعبئة الجدول بالبيانات الواردة من الخادم
    $("#hospital-table").empty();
    var tableHead = `
        <thead>
            <tr>
                <th>م</th>
                <th>الرتبة / الدرجة</th>
                <th>الإسم </th>
                <th>المستشفى</th>
                <th>تاريخ دخول المستشفى</th>
                <th>التشخيص الطبى</th>
                <th>التوصيات الممنوحة</th>
                <th>الاجراءات</th>
            </tr>
        </thead>`;
    $("#hospital-table").append(tableHead);

    if (result.length === 0) {
        // إضافة صف "لا توجد مستشفيات مضافة" إذا كانت النتيجة فارغة
        $("#hospital-table").append('<tr><td colspan="8" style="text-align:center;">لا توجد مستشفيات مضافة</td></tr>');
    } else {
        result.forEach((item, index) => {
            var tableItem = `
                <tbody style="font-size:14px;">
                    <tr>
                        <td>${index + 1}</td>
                        <td>${item.HospitalDetails.Person.Rank.RankName}</td>
                        <td>${item.HospitalDetails.Person.FullName}</td>
                        <td>${item.HospitalDetails.Hospital}</td>
                        <td class = "aribiano">${getDateFormated(item.HospitalDetails.DateFrom)}</td>
                        <td>${item.HospitalDetails.Diagnosis}</td>
                        <td>${item.HospitalDetails.Recommendations}</td>
                        <td>
                            <button class="delete-btn" onclick="deleteHospital(${item.ID})">
                                <i class="fas fa-trash"></i>
                            </button>
                        </td>
                    </tr>
                </tbody>`;
            $("#hospital-table").append(tableItem);
        });
    }
}

function setHospitalCounter() {
    // إعداد عداد المستشفيات
    var [currentHospitalCount, totalHospitalCount] = $("#hospital-counter").text().split("/").map(Number);
    if (currentHospitalCount >= totalHospitalCount) {
        $("#add-hospital-btn").attr('disabled', 'disabled');
    } else {
        $("#add-hospital-btn").removeAttr('disabled');
    }
}

function IncreaseHospitalCounter() {
    // زيادة عدد المستشفيات بعد الإضافة
    var [currentHospitalCount, totalHospitalCount] = $("#hospital-counter").text().split("/").map(Number);
    currentHospitalCount += 1;
    if (currentHospitalCount >= totalHospitalCount) {
        $("#add-hospital-btn").attr('disabled', 'disabled');
    }
    $("#hospital-counter").text(`${currentHospitalCount} / ${totalHospitalCount}`);
}

function DecreaseHospitalCounter() {
    // تقليل عدد المستشفيات بعد الحذف
    var [currentHospitalCount, totalHospitalCount] = $("#hospital-counter").text().split("/").map(Number);
    currentHospitalCount -= 1;
    $("#hospital-counter").text(`${currentHospitalCount} / ${totalHospitalCount}`);
    $("#add-hospital-btn").removeAttr('disabled');
}

function deleteHospital(id) {
    // تأكيد الحذف وإرسال الطلب
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: "لن تتمكن من التراجع عن هذا الإجراء!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'نعم، احذفه!',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: window.location.origin + "/hospital/Delete",
                type: "POST",
                data: { id: id },
                success: function () {
                    UpdateHospitalsTable();
                    DecreaseHospitalCounter();
                    Swal.fire({
                        icon: 'success',
                        title: 'تم الحذف!',
                        text: 'تم حذف المستشفى بنجاح.',
                        confirmButtonText: 'حسناً'
                    });
                }
            });
        }
    });
}
