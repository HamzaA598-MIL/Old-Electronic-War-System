(function () {
    var EmailModule = angular.module("EmailModule", []);

    EmailModule.controller("EmailController", function ($scope) {
        $scope.export = false;
        $scope.exportClasses = "btn btn-block btn-default";
        $scope.importClasses = "btn btn-block btn-primary";
        $scope.pageSize = 50;
        $scope.currentPage = 1;
        $scope.searchQuery = "";
        $scope.filteredEmails = [];

        $scope.loadEmails = function () {
            $scope.currentPage = 1;
            $scope.Recieved = getEmails(false);
            $scope.Sended = getEmails(true);
            $scope.filteredEmails = $scope.Recieved;
        };

        $scope.searchEmails = function () {
            if ($scope.searchQuery.trim() !== "") {
                $scope.filteredEmails = searchEmailsInServer($scope.searchQuery);
            } else {
                $scope.filteredEmails = $scope.export ? $scope.Sended : $scope.Recieved;
            }
        };

        function searchEmailsInServer(query) {
            var emails = [];
            $.ajax({
                url: "SearchEmails",
                type: "GET",
                async: true,
                data: { "query": query },
                success: function (result) {
                    emails = result;
                },
                error: function (err) {
                    console.error("Error searching emails", err);
                }
            });
            return emails;
        }

        $scope.currentPage = 1;
        $scope.pageSize = 20;

        $scope.loadMoreEmails = function () {
            if ($scope.loadingEmails) return;
            $scope.loadingEmails = true;

            setTimeout(function () {
                $scope.currentPage++;
                $scope.$apply(function () {
                    $scope.loadingEmails = false;
                });
            }, 1000);
        };

        $scope.loadExportedEmails = function (exported = false) {
            $scope.export = exported;
            [$scope.exportClasses, $scope.importClasses] = [$scope.importClasses, $scope.exportClasses];

            var url = new URL(window.location);
            url.searchParams.set('tab', exported ? 'sent' : 'received');
            window.history.pushState({}, '', url);

            if ($scope.export) {
                $scope.Sended = getEmails(true);
                $scope.filteredEmails = $scope.Sended;
            } else {
                $scope.Recieved = getEmails(false);
                $scope.filteredEmails = $scope.Recieved;
            }
        };

        $(document).ready(function () {
            $(window).scroll(function () {
                if ($(window).scrollTop() + $(window).height() >= $(document).height() - 100) {
                    angular.element($("#EmailsDiv")).scope().loadMoreEmails();
                }
            });
        });
        $scope.getUnreadCount = function () {
            $.ajax({
                url: "CountOfUnReadEmails",
                type: "GET",
                async: true,
                success: function (result) {
                    $scope.$apply(function () {
                        $scope.unreadCount = result;
                    });
                },
                error: function (err) {
                    console.error("Error fetching unread email count", err);
                }
            });
        };

        // استدعاء الدالة عند التحميل
        $scope.getUnreadCount();

        $scope.openDetailsEmail = function (id) {
            window.location.href = "Details/" + id;
        };

        $scope.StarEmail = function (id) {
            $.ajax({
                url: "StarEmail",
                type: "POST",
                data: { "id": id },
                success: function () { },
                error: function (error) {
                    console.error("Error starring email", error);
                }
            });
        };

        $scope.delete = function (id) {
            Swal.fire({
                title: 'هل أنت متأكد من الحذف؟',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'نعم، احذف!',
                cancelButtonText: 'إلغاء'
            }).then((result) => {
                if (result.isConfirmed) {
                    Swal.fire({
                        title: 'جارٍ الحذف...',
                        text: 'يرجى الانتظار قليلاً',
                        allowOutsideClick: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });

                    $.ajax({
                        url: "Delete",
                        type: "POST",
                        async: true,
                        data: { "id": id },
                        success: function (result) {
                            $scope.$apply(function () {
                                if ($scope.export) {
                                    $scope.Sended = $scope.Sended.filter(email => email.id !== id);
                                } else {
                                    $scope.Recieved = $scope.Recieved.filter(email => email.id !== id);
                                }
                            });
                            Swal.fire('تم الحذف!', 'تم حذف البريد الإلكتروني بنجاح.', 'success');
                        },
                        error: function () {
                            Swal.fire('خطأ!', 'تعذر الاتصال بالخادم. حاول مرة أخرى.', 'error');
                        }
                    });
                }
            });
        };

        $scope.setActiveTabFromQuery = function () {
            var params = new URLSearchParams(window.location.search);
            var tab = params.get('tab');
            if (tab === 'sent') {
                $scope.loadExportedEmails(true);
            } else {
                $scope.loadExportedEmails(false);
            }
        };

        $scope.setActiveTabFromQuery();
        window.addEventListener('popstate', function () {
            $scope.setActiveTabFromQuery();
            $scope.$apply();
        });
    });

    function getEmails(exported = false) {
        var emails = [];
        $.ajax({
            url: "GetEmails",
            type: "GET",
            async: false,
            data: { "export": exported },
            success: function (result) {
                emails = result;
            }
        });
        return emails;
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
})();

function DownloadFile(id) {
    window.location.href = window.location.origin + "/Document/Download/" + id;
}


function disableBtn() {
    $("#btn-add").attr('disabled', 'disabled');
}

//function getUnreadCount() {
//    var unreadCount = 0;
//    $.ajax({
//        url: "CountOfUnReadEmails",
//        type: "GET",
//        async: false,
//        success: function (result) {
//            unreadCount = result;
//        }
//    });
//    return unreadCount;
//}
