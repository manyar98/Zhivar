define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'buttonValidation', 'textNumericChar'], function (app) {
    app.register.controller('changePasswordController', ['$scope', '$rootScope', '$state', 'dataService', 'messageService',
        function ($scope, $rootScope, $state, dataService, messageService) {

            $scope.model = {};
            $rootScope.applicationModule = "تغییر رمز عبور";
            $scope.applicationDescription = "برای تغییر رمز عبور خود، رمز عبور فعلی، رمز عبور جدید و تایید رمز جدید را وارد نمایید، سپس برروی دکمه ثبت کلیک نمایید."

            //TODO
            $scope.operationAccess = dataService.getOperationAccessRoute('/app/api/Login/GetChangePasswordOperationAccess');
            if (!$scope.operationAccess.canView)
                return;

            $scope.edit = function (model) {
                var countPassword = 0;

                if (model.newPassword.length > 30) {
                    messageService.error('رمز عبور معتبر نیست! ', 'نمایش خطا');
                    return;
                }

                if (model.newPassword.length <= 5) {
                    messageService.error('رمز عبور باید حداقل 6 کاراکتر  باشد! ', 'نمایش خطا');
                    return;
                }

                for (i = 0; i < model.newPassword.length; i++) {
                    var charCode = model.newPassword.charCodeAt(i);
                    if (charCode > 47 && charCode < 58)
                        countPassword++;
                }

                if (countPassword != 0 && countPassword < model.newPassword.length) {
                    if (model.newPassword == model.confirmPassword) {
                        dataService.postData("/app/api/Login/ChangePassword", { oldPassword: model.currentPassword, newPassword: model.newPassword }).then(function (data) {
                            messageService.success('عملیات با موفقیت انجام شد', '');
                            $scope.redirect();
                        });
                    }
                    else {
                        messageService.error('یکسان بودن رمز عبور و تایید آن الزامیست! ', 'نمایش خطا');
                    }
                }
                else {
                    messageService.error('رمز عبور وارد شده باید ترکیبی از حروف و عدد باشد! ', 'نمایش خطا');
                }
            };

            $scope.redirect = function () {
                $state.go('home');
            }
        }
    ])
});