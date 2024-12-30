var EditMode, ID, personType;

function disableBtn() {
    // Disable the submit button if any form field is empty
    if ($("#person-name").val() === "" ||
        $("#person-rank").val() === "" ||
        $("#person-MilID").val() === "") {
        $("#savePersonBtn").attr('disabled', 'disabled'); // Adjust to your specific submit button
    } else {
        $("#savePersonBtn").removeAttr('disabled');
    }
}
angular.module('EmailModule').filter('arabicNumbers', function () {
    return function (input) {
        if (!input) return input;

        const englishToArabicMap = {
            '0': '٠', '1': '١', '2': '٢', '3': '٣',
            '4': '٤', '5': '٥', '6': '٦', '7': '٧',
            '8': '٨', '9': '٩'
        };
        return input.toString().replace(/\d/g, function (d) {
            return englishToArabicMap[d];
        });
    };
});
function AddPerson(unitId, type) {
    // Adding a new person via AJAX POST request
    
    $.ajax({
        url: window.location.origin + "/Person/Create",
        type: "POST",
        data: {
            "UnitID": unitId,
            "MilID": $("#person-MilID").val(),
            "RankID": $("#person-rank").val(),
            "FullName": $("#person-name").val(),
            "Type": type
        },
        success: function () {
            clearFormFields();
            updatePersonTable(type);
            closePop(); // Close the modal after a successful add
            location.reload();

        },
        error: function () {
            alert("Failed to add person. Please try again.");
        }
    });
}


function openPersonPopup(id, type, editMode) {
    $("#personModal").modal("show"); // Open Bootstrap modal

    personType = type;
    EditMode = editMode;
    ID = id;

    if (EditMode) {
        // Populate form with existing person data for editing
        $.ajax({
            url: window.location.origin + "/Person/GetPerson",
            type: "GET",
            data: { "id": id },
            success: function (result) {
                $("#person-MilID").val(result['MilID']);
                $("#person-rank").val(result['RankID']);
                $("#person-name").val(result['FullName']);
                disableBtn(); // Ensure the button is correctly enabled/disabled
            },
            error: function () {
                alert("Failed to retrieve person data.");
            }
        });
    } else {
        // Clear fields if it's a new person (Add mode)
        clearFormFields();
    }
}

function savePersonChanges() {
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: "هل تريد حقاً تعديل بيانات هذا الشخص؟",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'نعم، قم بالتعديل!',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            if (EditMode) {
                // إذا كان في وضع التعديل
                EditPerson(ID, personType);
            } else {
                // إذا كان في وضع الإضافة
                AddPerson(ID, personType);
            }
        }
    });
}

function EditPerson(id, type) {
    // إرسال البيانات لتعديل الشخص
    $('#personModalLabel').text('تعديل البيانات   ');
    $.ajax({
        url: window.location.origin + "/Person/Edit",
        type: "POST",
        data: {
            "id": id,
            "MilID": $("#person-MilID").val(),
            "RankID": $("#person-rank").val(),
            "FullName": $("#person-name").val(),
            "Type": type
        },
        success: function () {
            Swal.fire(
                'تم التعديل!',
                'تم تعديل بيانات الشخص بنجاح.',
                'success'
            ).then(() => {
                location.reload();
                 // إعادة تحميل الصفحة بالكامل لتحديث البيانات
            });
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'فشل',
                text: 'حدث خطأ أثناء محاولة التعديل، الرجاء المحاولة مرة أخرى.',
            });
        }
    });
}


function closePop() {
    $("#personModal").modal("hide"); // Close Bootstrap modal
    clearFormFields(); // Reset fields
}

function clearFormFields() {
    // Clear all form fields
    $("#person-MilID").val("");
    $("#person-rank").val("");
    $("#person-name").val("");
    disableBtn(); // Ensure the save button is disabled after clearing
}

function updatePersonTable(type) {
    $.ajax({
        url: window.location.origin + "/Person/GetPersons",
        type: "GET",
        data: { "type": type },
        success: function (result) {
            FillTable(result, type);
            location.reload();
        },
        error: function () {
            alert("Failed to update person table.");
        }
    });
}

function updateAfterDelete(id, type) {
    if (confirm("هل انت متأكد من حذف الشخص ؟")) {
        $.ajax({
            url: window.location.origin + "/Person/Delete",
            type: "POST",
            data: { "id": id },
            success: function (result) {
                if (result === "False") {
                    alert("لا يمكنك مسح عنصر من قيادة الوحدة");
                } else {
                    updatePersonTable(type);
                }
            },
            error: function () {
                alert("Failed to delete person.");
            }
        });
    }
}

function FillTable(result, type) {
    var rankLabel = (type == 1) ? "الرتبة" : "الدرجة";
    var table = `
        <thead>
            <tr>
                <th>م</th>
                <th>الرقم العسكرى</th>
                <th>${rankLabel}</th>
                <th>الإسم</th>
                <th>الإجراءات</th>
            </tr>
        </thead>
        <tbody>`;

    result.forEach(function (item, index) {
        table += `
            <tr>
                <td>${index + 1}</td>
                <td>${item.MilID}</td>
                <td>${item.Rank.RankName}</td>
                <td>${item.FullName}</td>
                <td>
                    <button class="delete-btn" onclick="updateAfterDelete(${item.ID}, ${type})">حذف</button>
                    <button class="add-btn" onclick="openPersonPopup(${item.ID}, ${type}, true)">تعديل</button>
                </td>
            </tr>`;
    });

    table += `</tbody>`;
    $("#persons-table").html(table); // Update the table with new data
}
function updateAfterDelete(id, type) {
    // استخدام SweetAlert لعرض نافذة تأكيد الحذف
    Swal.fire({
        title: 'هل أنت متأكد؟',
        text: "لن تتمكن من استعادة هذا الشخص بعد الحذف!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'نعم، احذف!',
        cancelButtonText: 'إلغاء'
    }).then((result) => {
        if (result.isConfirmed) {
            // إذا وافق المستخدم على الحذف
            $.ajax({
                url: window.location.origin + "/Person/Delete",
                type: "POST",
                data: { "id": id },
                success: function (result) {
                    if (result === "False") {
                        Swal.fire({
                            icon: 'error',
                            title: 'خطأ',
                            text: 'لا يمكنك مسح عنصر من قيادة الوحدة',
                        });
                    } else {
                        Swal.fire(
                            'تم الحذف!',
                            'تم حذف الشخص بنجاح.',
                            'success'
                        ).then(() => {
                            // هنا يتم تحديث الصفحة بالكامل لتطبيق الحذف
                            location.reload(); // ريفرش للصفحة بالكامل
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'فشل',
                        text: 'حدث خطأ أثناء محاولة الحذف، الرجاء المحاولة مرة أخرى.',
                    });
                }
            });
        }
    });
}
