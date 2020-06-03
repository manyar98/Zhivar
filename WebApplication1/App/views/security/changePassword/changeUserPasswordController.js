define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'captchaImage'], function (app) {
    app.register.controller('changeUserPasswordController', ['$scope', '$rootScope', '$state', 'dataService', 'messageService',
        function ($scope, $rootScope, $state, dataService, messageService) {

            $rootScope.applicationModule = "تغییر رمز عبور کاربران";
            $scope.applicationDescription = "برای تغییر رمز عبور کاربر، نام کاربری، رمز عبور و تأیید آن را وارد نموده روی دکمه ثبت کلیک نمایید."

            $scope.operationAccess = dataService.getOperationAccessRoute('/app/api/UserInfo/GetChangeUserPasswordOperationAccess');
            if (!$scope.operationAccess.canUpdate)
                return;

            $scope.model = {};
            $scope.captchaOption = {
                height: '50px',
                width: '120px'
            }

            $scope.resetCaptcha = function () {
                $rootScope.$broadcast('callRegenarateCaptchaMethod');
                $scope.model.captcha = '';
            }
            $scope.confirmDelete = { active: false };

            $scope.init = function () { }

            $scope.changePass = function () {
                $scope.confirmDelete.active = true;
            }

            $scope.finalChangePass = function () {
                $scope.$apply(function () {
                    $scope.confirmDelete.active = false;
                });

                if ($scope.model.newPassword != $scope.model.confirmPassword) {
                    messageService.error('یکسان بودن رمز عبور جدید و تأیید آن الزامیست!', '');
                    return;
                }

                var req = {
                    UserName: $scope.model.username,
                    NewPassword: $scope.model.newPassword,
                    Captcha: $scope.model.captcha

                }

                dataService.postData("/app/api/Login/ChangeUserPassword", req).then(function (data) {
                    if (data) {
                        $scope.resetCaptcha();
                        messageService.success('عملیات با موفقیت انجام شد', '');
                    }
                },
                    function (error) {
                        $scope.resetCaptcha();

                    }
                );

            };

            $scope.redirect = function () {
                $state.go('home');
            }
        }
    ])
});