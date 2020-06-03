define(['application', 'dataService', 'messageService', 'buttonValidation', 'captchaImage', 'number', 'displayNumbers'], function (app) {
    app.register.controller('loginController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state', 'defaultUrlOtherwise',
        function ($scope, $rootScope, dataService, messageService, $state, defaultUrlOtherwise) {

            $rootScope.applicationModule = "ورود به سیستم";

            $scope.model = {};
            $rootScope.userModel = {};
            $rootScope.forgotPass = {};
            $rootScope.visibleForLoginView = true;

            $rootScope.control = {
                loginControlVisible: false,
                registrationControlVisible: true,
                defaultControlVisible: true,
            }

            $scope.captchaOption = {
                height: '50px',
                width: '120px'
            }

            var resetCaptcha = function () {
                $rootScope.$broadcast('callRegenarateCaptchaMethod');
                $scope.model.captcha = '';
            }
            $(".panel-loadPage").hide();
            $(".panel-after-loadpage").show();

            $scope.logoff = function () {
               // resetCaptcha();
                dataService.logOff(resetCaptcha);
            };

            $scope.login = function (model) {
                dataService.postData("/app/api/Login/Login", model).then(function (data) {
                    $rootScope.control.loginControlVisible = false;
                    $rootScope.visibleForLoginView = false;
                    $rootScope.userModel = data;
                    //$rootScope.userModelAuthenticationType();
                    //dataService.checkSession(false);
                    dataService.getMenus();
                    $state.go('home');
                },
                    function (error) {
                        if (error.resultCode == 4)
                            messageService.error(error.message);
                        resetCaptcha();
                    });
            }

            $scope.forgetPass = function (model) {
                if (angular.isUndefined(model.userName) || model.userName == "") {
                    messageService.error("نام کاربری الزامیست!", "خطا:");
                    return;
                }

                if (angular.isUndefined(model.captcha) || model.captcha == "") {
                    messageService.error("کد امنیتی الزامیست!", "خطا:");
                    return;
                }

                var req = { UserName: model.userName, Captcha: model.captcha };
                dataService.postData("/app/api/Login/SendForgotPasswordRequest/", req).then(function (data) {
                    $rootScope.forgotPass.UserName = req.UserName;
                    $state.go('forgetPassword');
                }, function (error) {
                    resetCaptcha();
                });
            }

            $scope.redirect2 = function (url) {
                if (url == null)
                    $state.go(defaultUrlOtherwise);
                else
                    $state.go(url);
            }

            $scope.redirect = function (url) {
                if (url == null)
                    $state.go(defaultUrlOtherwise);
                else
                    $state.go(url);
            }



        }
    ]);
});