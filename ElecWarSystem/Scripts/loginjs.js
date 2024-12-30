$(document).ready(function () {
    $('#loginForm').on('submit', function (e) {
        e.preventDefault(); // منع الإرسال الافتراضي للنموذج

        var username = $('input[name="username"]').val().trim();
        var password = $('input[name="password"]').val().trim();
        var isValid = true;

        // التحقق من أن الحقول ليست فارغة
        if (username === "") {
            Swal.fire({
                icon: 'error',
                title: 'خطأ',
                text: 'يرجى إدخال إسم المستخدم.',
            });
            isValid = false;
        }

        if (password === "") {
            Swal.fire({
                icon: 'error',
                title: 'خطأ',
                text: 'يرجى إدخال كلمة السر.',
            });
            isValid = false;
        }

        if (password.length < 3) {
            Swal.fire({
                icon: 'error',
                title: 'خطأ',
                text: 'كلمة السر يجب أن تكون 3 أحرف على الأقل.',
            });
            isValid = false;
        }

        if (isValid) {
            // إرسال الطلب AJAX إذا كانت الحقول صحيحة
            $.ajax({
                url: '/User/Login',
                async: false,
                type: 'POST',
                data: {
                    username: username,
                    password: password
                },
                success: function (response) {
                    // التعامل مع الرد بصيغة JSON
                    if (response.status === 1) {
                        // الحصول على قيمة ملف تعريف ارتباط Roles للتحقق من دور المستخدم
                        var userRole = getCookie("Roles");

                        if (userRole == "1") { // Analyzer
                            window.location.href = response.redirectUrl; // Redirection for Analyzer
                        }
                        else if (userRole == "2" || userRole == "3") { // Admin or Viewer
                            window.location.href = response.redirectUrl; // Redirection for Admin or Viewer
                        }
                        else {
                            window.location.href = response.redirectUrl; // Default redirection
                        }
                    } else if (response.status === -2) {
                        Swal.fire({
                            icon: 'error',
                            title: 'خطأ',
                            text: response.message,
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'خطأ',
                            text: response.message,
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'خطأ',
                        text: 'حدث خطأ أثناء محاولة تسجيل الدخول. يرجى المحاولة مرة أخرى.',
                    });
                }
            });
        }
    });

    // Helper function to get cookie value
    function getCookie(name) {
        var value = "; " + document.cookie;
        var parts = value.split("; " + name + "=");
        if (parts.length === 2) return parts.pop().split(";").shift();
    }
});
