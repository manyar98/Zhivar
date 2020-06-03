define(['angularAMD', 'toastr'], function (app, toastr) {
    app.service('messageService', ['$rootScope',
        function ($rootScope) {
            toastr.options = {
                closeButton: true,
                debug: false,
                newestOnTop: true,
                progressBar: true,
                positionClass: "toast-bottom-right",
                preventDuplicates: false,
                onclick: null,
                showDuration: "300",
                hideDuration: "1000",
                timeOut: "5000",
                extendedTimeOut: "1000",
                showEasing: "swing",
                hideEasing: "linear",
                showMethod: "fadeIn",
                hideMethod: "fadeOut",
                rtl: true
            }

            function enableButtonValidation() {
                var isModalForm = angular.element('body').hasClass('modal-open');
                var isTabularForm = angular.element('.tab-pane:visible').length == 1;
                var hasOneButtonValidation = angular.element('.button-validation').length == 1 && angular.element('.button-validation').isolateScope() !== undefined;

                if (isModalForm) {
                    if (angular.element('.modal').find('.button-validation').length == 1 && angular.element('.modal').find('.button-validation').isolateScope() !== undefined)
                        angular.element('.modal').find('.button-validation').isolateScope().disabled = false;
                }
                else if (isTabularForm) {
                    if (angular.element('.tab-pane:visible').find('.button-validation').length == 1 && angular.element('.tab-pane:visible').find('.button-validation').isolateScope() !== undefined)
                        angular.element('.tab-pane:visible').find('.button-validation').isolateScope().disabled = false;
                }
                else if (hasOneButtonValidation)
                    angular.element('.button-validation').isolateScope().disabled = false;
            }

            return {
                success: function (msg, title) {
                    enableButtonValidation();
                    if (msg !== "")
                        toastr.success(msg, title);
                },
                info: function (msg, title) {
                    enableButtonValidation();
                    if (msg !== "")
                        toastr.info(msg, title);
                },
                warning: function (msg, title) {
                    enableButtonValidation();
                    if (msg !== "")
                        toastr.warning(msg, title);
                },
                error: function (msg, title) {
                    enableButtonValidation();
                    if (msg !== "")
                        toastr.error(msg, title);
                },
                warningMessage: function (messages) {
                    enableButtonValidation();
                    if (messages.length > 0)
                        toastr.error(messages, "نمایش خطا");
                }
            }
        }
    ]);
});