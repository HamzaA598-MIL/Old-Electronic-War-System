window.onload = function () {
    setSickLeaveCounter();
    RequestTmamStatus();
    numbersE2A();
    disableBtn();
}

// Function to check if all fields are filled
function IsAllFieldsFilled() {
    return $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#hospital-name").val() !== "" &&
        $("#hospital-date").val() !== "" &&
        $("#diagnosis").val() !== "" &&
        $("#date-from").val() !== "" &&
        $("#date-to").val() !== "";
}

// Function to enable/disable the submit button
function disableBtn() {
    if (IsAllFieldsFilled()) {
        $(".popup-submit-btn").removeAttr('disabled');
    } else {
        $(".popup-submit-btn").attr('disabled', 'disabled');
    }
}

// Function to add a new sick leave
function Add() {
    if (IsAllFieldsFilled()) {  // تأكد من أن جميع الحقول معبأة قبل محاولة الإضافة
        const dateFrom = $("#date-from").val();
        const dateTo = $("#date-to").val();

        if (dateFrom > dateTo) {
            Swal.fire({
                icon: 'error',
                title: 'خطأ',
                text: 'يوجد خطأ في تاريخ الأجازة المرضية',
            });
            return;
        }
        
        $.ajax({
            url: `${window.location.origin}/SickLeave/Create`,
            type: "POST",
            async: false,
            data: {
                "SickLeaveDetail.PersonID": $("#person-name").val(),
                "SickLeaveDetail.Hospital": $("#hospital-name").val(),
                "SickLeaveDetail.HospitalDate": $("#hospital-date").val(),
                "SickLeaveDetail.Diagnosis": $("#diagnosis").val(),
                "SickLeaveDetail.DateFrom": dateFrom,
                "SickLeaveDetail.DateTo": dateTo
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
                    UpdateErrandsTable();
                    IncreaseSickLeaveCounter();
                    clearFormFields();
                    Swal.fire({
                        icon: 'success',
                        title: 'تمت الإضافة',
                        text: 'تمت إضافة الأجازة المرضية بنجاح!',
                    });
                    closePop();
                }
            }
        });
        
    } else {
        Swal.fire({
            icon: 'warning',
            title: 'تحذير',
            text: 'يرجى تعبئة جميع الحقول قبل الإضافة.',
        });
    }
}

// Function to clear form fields after submission
function clearFormFields() {
    $("#person-name, #person-rank, #hospital-name, #hospital-date, #diagnosis, #date-from, #date-to").val(null);
}

// Function to update the sick leave table with fresh data
function UpdateErrandsTable() {
    $.ajax({
        url: `${window.location.origin}/SickLeave/GetSickLeave`,
        type: "GET",
        async: false,
        success: function (result) {
            fillSickLeaveTable(result);
        }
    });
}

// Function to fill the sick leave table with data
function fillSickLeaveTable(result) {
    const $table = $("#sick-leaves-table");
    $table.empty();
    const tableHead = `
        <thead>
            <tr>
                <th>م</th>
                <th>الرتبة / الدرجة</th>
                <th>الإسم</th>
                <th>المستشفى</th>
                <th>تاريخ دخول المستشفى</th>
                <th>التشخيص</th>
                <th>بدء الأجازة</th>
                <th>عودة الأجازة</th>
                <th>الإجراءات</th>
            </tr>
        </thead>`;

    $table.append(tableHead);
    if (result.length === 0) {
        // إضافة صف "لا توجد مستشفيات مضافة" إذا كانت النتيجة فارغة
        $("#hospital-table").append('<tr><td colspan="8" style="text-align:center;">لا توجد أجازات مضافة</td></tr>');
    } else {
        

        result.forEach((leave, index) => {
            const tableItem = `
            <tbody style="font-size:14px;">
                <tr>
                    <td class = "aribiano">${index + 1}</td>
                    <td>${leave.SickLeaveDetail.Person.Rank.RankName}</td>
                    <td>${leave.SickLeaveDetail.Person.FullName}</td>
                    <td>${leave.SickLeaveDetail.Hospital}</td>
                    <td >${getDateFormated(leave.SickLeaveDetail.HospitalDate)}</td>
                    <td>${leave.SickLeaveDetail.Diagnosis}</td>
                    <td >${getDateFormated(leave.SickLeaveDetail.DateFrom)}</td>
                    <td >${getDateFormated(leave.SickLeaveDetail.DateTo)}</td>
                    <td>
                        <button class="delete-btn" onclick="deleteSickLeave(${leave.ID})">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                </tr>
            </tbody>`;
            $table.append(tableItem);
        });
}
    

}
function getDateFormated(date) {
    // Convert the date from string to Date object
    date = new Date(parseInt(date.substr(6)));

    // Extract year, month, and day
    var yyyy = date.getFullYear();
    var mm = date.getMonth() + 1;
    var dd = date.getDate();

    // Pad month and day with zeros if less than 10
    if (mm < 10) mm = '0' + mm;
    if (dd < 10) dd = '0' + dd;

    // Return date in YYYY-MM-DD format with Arabic numerals
    var formattedDate = `${dd}-${mm}- ${yyyy}  `;

    // Replace English numerals with Arabic numerals
    return numbersEn2Ar(formattedDate);
}

// Helper function to convert English numerals to Arabic numerals
function numbersEn2Ar(input) {
    const enToAr = {
        '0': '٠', '1': '١', '2': '٢', '3': '٣',
        '4': '٤', '5': '٥', '6': '٦', '7': '٧',
        '8': '٨', '9': '٩'
    };

    return input.replace(/\d/g, function (digit) {
        return enToAr[digit];
    });
}
// Function to open the sick leave popup
function openSickLevelPopup() {
    $('#sickLeaveModal').modal('show');  // Ensure the modal is shown when the button is clicked
}

// Function to set the sick leave counter and check if the button should be disabled
function setSickLeaveCounter() {
    const [currentSickLeaveCount, totalSickLeaveCount] = $("#sick-leave-counter").text().split("/").map(Number);

    // Disable the add button if the current count equals the total count
    if (currentSickLeaveCount === totalSickLeaveCount || timeOutCounter) {
        $("#add-sick-leave-btn").attr('disabled', 'disabled');
    } else {
        $("#add-sick-leave-btn").removeAttr('disabled');
    }
}

// Function to increase the sick leave counter and disable the button if limits are reached
function IncreaseSickLeaveCounter() {
    const [currentSickLeaveCount, totalSickLeaveCount] = $("#sick-leave-counter").text().split("/").map(Number);
    const newCount = currentSickLeaveCount + 1;
    const newText = `${newCount} / ${totalSickLeaveCount}`;
    $("#sick-leave-counter").text(newText);

    // Disable the add button if the new count equals the total count
    if (newCount === totalSickLeaveCount) {
        $("#add-sick-leave-btn").attr('disabled', 'disabled');
    }
}

// Function to decrease the sick leave counter
function DecreaseSickLeaveCounter() {
    const [currentSickLeaveCount, totalSickLeaveCount] = $("#sick-leave-counter").text().split("/").map(Number);
    const newCount = currentSickLeaveCount - 1;
    const newText = `${newCount} / ${totalSickLeaveCount}`;
    $("#sick-leave-counter").text(newText);

    // Always enable the add button after decreasing the count
    $("#add-sick-leave-btn").removeAttr('disabled');
}

// Function to close the popup
function closePop() {
    document.querySelector("#sickLeaveModal").classList.remove("act");
}

// Function to delete a sick leave
function deleteSickLeave(id) {
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
                url: `${window.location.origin}/SickLeave/Delete`,
                type: "POST",
                async: false,
                data: { id },
                success: function () {
                    UpdateErrandsTable();
                    DecreaseSickLeaveCounter();
                    Swal.fire(
                        'تم الحذف!',
                        'تم حذف الأجازة المرضية بنجاح.',
                        'success'
                    );
                }
            });
        }
    });
}
