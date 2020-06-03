define(['application', 'dataService', 'messageService', 'kendoGridFa', 'buttonValidation', 'textNumericChar', 'captchaImage'], function (app) {
    app.register.controller('forgotPassChangingController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state', 'defaultUrlOtherwise',
        function ($scope, $rootScope, dataService, messageService, $state, defaultUrlOtherwise) {

            $rootScope.applicationModule = "تغییر رمز عبور";

            $scope.model = {};
            $rootScope.visibleForLogin = true;
            $scope.loginControlVisible = true;
            $scope.captchaOption = {
                height: '50px',
                width: '120px'
            }

            $scope.resetCaptcha = function () {
                $rootScope.$broadcast('callRegenarateCaptchaMethod');
                $scope.model.captcha = '';
            }
            $scope.changePass = function (model) {
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

                if (countPassword == 0 || countPassword >= model.newPassword.length) {
                    messageService.error('رمز عبور وارد شده باید ترکیبی از حروف و عدد باشد! ', 'نمایش خطا');
                    return;
                }

                if (model.newPassword != model.confirmPassword) {
                    messageService.error('یکسان بودن رمز عبور و تایید آن الزامیست! ', 'نمایش خطا');
                    return;
                }

                var req = {
                    UserName: $rootScope.forgotPass.UserName,
                    Code: $rootScope.forgotPass.Code,
                    Password: model.newPassword
                }

                dataService.postData("/app/api/Login/UpdateForgotPassword", req).then(function (data) {
                    $rootScope.forgotPass = {};
                    $state.go(defaultUrlOtherwise);
                }, function (error) {
                    $scope.resetCaptcha();
                        //if (error.resultCode == 4) {
                        //    $rootScope.visibleForLogin = true;
                        //}
                });
            }
        }
    ]);
});