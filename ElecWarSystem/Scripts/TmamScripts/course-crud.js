(function () {
    var mod = angular.module("CourseMod", []);
    mod.controller("CourseCrud", function ($scope) {
        $scope.Name = "تمام الفرق و الدورات التعليمية";
        $scope.Courses = GetCourses();
        var numbers = GetNumbers();
        $scope.CoursesTotal = numbers["total"];
        $scope.CoursesEntered = numbers["entered"];
        setCourseAddStatus($scope.CoursesTotal, $scope.CoursesEntered);

        $scope.Add = function () {

            if ($('#date-from').val() >= $('#date-to').val()) {
                Swal.fire({
                    icon: 'error',
                    title: 'خطأ',
                    text: 'يوجد خطأ فى تاريخ الفرقة',
                    confirmButtonText: 'حسنًا'
                });
                return;
            }

            $.ajax({
                url: window.location.origin + "/Course/Create",
                type: "POST",
                async: false,
                data: {
                    "CourseDetails.PersonID": $("#person-name").val(),
                    "CourseDetails.CourseName": $("#course-name").val(),
                    "CourseDetails.CoursePlace": $("#course-place").val(),
                    "CourseDetails.DateFrom": $("#date-from").val(),
                    "CourseDetails.DateTo": $("#date-to").val(),
                    "CourseDetails.CommandItem.Number": $("#command-number").val(),
                    "CourseDetails.CommandItem.Date": $("#command-date").val()
                },
                success: function (result) {
                    if (result == -1) {
                        // SweetAlert للتنبيه عند وجود خطأ في تاريخ الفرقة
                        Swal.fire({
                            icon: 'error',
                            title: 'خطأ',
                            text: 'يوجد خطأ فى تاريخ الفرقة',
                            confirmButtonText: 'حسنًا'
                        });
                    } else {
                        $('#courseModal').modal('hide'); // إغلاق الـ modal
                        $scope.Courses = GetCourses();
                        emptyFormField();
                        $scope.CoursesEntered = $scope.CoursesEntered + 1;
                        setCourseAddStatus($scope.CoursesTotal, $scope.CoursesEntered);

                        // SweetAlert لنجاح العملية
                        Swal.fire({
                            icon: 'success',
                            title: 'تم الحفظ بنجاح',
                            confirmButtonText: 'حسنًا'
                        });
                    }
                }
            });
        };

        $scope.delete = function (id) {
            $.ajax({
                url: window.location.origin + "/Course/Delete",
                type: "POST",
                async: false,
                data: {
                    "id": id,
                },
                success: function () {
                    $scope.Courses = GetCourses();
                    $scope.CoursesEntered = $scope.CoursesEntered - 1;
                    setCourseAddStatus($scope.CoursesTotal, $scope.CoursesEntered);

                    // إذا لم يكن هناك أي صفوف بعد الحذف، أضف صفًا يحتوي على الرسالة
                    //if ($scope.Courses.length === 0) {
                    //    // إضافة رسالة عدم وجود بيانات إلى الجدول
                    //    const tableBody = document.querySelector("#courses-table tbody");
                    //    const noDataRow = document.createElement("tr");
                    //    const noDataCell = document.createElement("td");

                    //    noDataCell.setAttribute("colspan", "9"); // عدد الأعمدة في الجدول
                    //    noDataCell.textContent = "لا يوجد تمام مسجل";
                    //    noDataCell.classList.add("text-center"); // لتوسيط النص

                    //    noDataRow.appendChild(noDataCell);
                    //    tableBody.appendChild(noDataRow);
                    //}

                    // SweetAlert للتنبيه عند الحذف
                    Swal.fire({
                        icon: 'success',
                        title: 'تم الحذف بنجاح',
                        confirmButtonText: 'حسنًا'
                    });
                }
            });
        };


        $scope.openCoursePopup = function () {
            $('#courseModal').modal('show'); // فتح الـ modal
        };

        $scope.closePop = function () {
            $('#courseModal').modal('hide'); // إغلاق الـ modal
        };
    });
})();

function validateCourseName() {
    disableBtn();
    if ($("#course-name").val().length >= 25) {
        $("#course-name-warn").removeAttr('hidden');
    } else {
        $("#course-name-warn").attr('hidden', 'hidden');
    }
}

function setCourseAddStatus(total, entered) {
    if (total == entered) {
        $("#add-course-btn").attr('disabled', 'disabled');
    } else {
        $("#add-course-btn").removeAttr('disabled');
    }
}

function GetCourses() {
    var courses = [];
    $.ajax({
        url: window.location.origin + "/Course/GetCourses",
        type: "GET",
        async: false,
        success: function (result) {
            courses = result;

            for (var i in courses) {
                courses[i]["CourseDetails"]["DateFrom"] = getDateFormated(courses[i]["CourseDetails"]["DateFrom"]);
                courses[i]["CourseDetails"]["DateTo"] = getDateFormated(courses[i]["CourseDetails"]["DateTo"]);
                courses[i]["CourseDetails"]["CommandItem"]["Date"] = getDateFormated(courses[i]["CourseDetails"]["CommandItem"]["Date"]);
            }
        }
    });
    return courses;
}

function emptyFormField() {
    $("#person-rank").val(null);
    $("#person-name").val(null);
    $("#course-name").val(null);
    $("#course-place").val(null);
    $("#date-from").val(null);
    $("#date-to").val(null);
    $("#command-number").val(null);
    $("#command-date").val(null);
}

function disableBtn() {
    if (IsAllFieldsFilled()) {
        $(".popup-submit-btn").removeAttr('disabled');
    } else {
        $(".popup-submit-btn").attr('disabled', 'disabled');
    }
}

function IsAllFieldsFilled() {
    var result =
        $("#person-name").val() !== "" &&
        $("#person-rank").val() !== "" &&
        $("#course-place").val() !== "" &&
        $("#course-name").val() !== "" &&
        $("#date-from").val() !== "" &&
        $("#date-to").val() !== "" &&
        $("#command-number").val() !== "" &&
        $("#command-date").val() !== "";
    return result;
}

function GetNumbers() {
    var numbers = [];
    $.ajax({
        url: window.location.origin + "/Course/GetNumbers",
        type: "GET",
        async: false,
        success: function (result) {
            numbers = result;
        }
    });
    return numbers;
}
