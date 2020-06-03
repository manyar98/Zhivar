define(['application', 'dataService', 'messageService', 'captchaImage', 'buttonValidation', 'textNumericChar'], function (app) {
    app.register.controller('registerController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state', '$sce',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state, $sce) {

            $rootScope.applicationModule = "ثبت نام کسب و کار";

            $scope.model = {
                Password: '',
                ConfirmPassword: ''
            };
            $scope.captchaOption = {
                height: '50px',
                width: '120px'
            }
            $rootScope.userModel = {};
            $rootScope.visibleForLogin = true;
            $scope.loginControlVisible = true;

            if ($stateParams && $stateParams.model && $stateParams.model != null) {
                $scope.model = $stateParams.model;
            }

            var resetCaptcha = function () {
                $rootScope.$broadcast('callRegenarateCaptchaMethod');
                $scope.model.captcha = '';
            }

            $scope.logoff = function () {
                dataService.logOff(resetCaptcha);
            };

            $scope.register = function (model) {
                countPassword = 0;
                countShomarehParvane = 0;

                //if (model.ShomarehParvane.length > 30) {
                //    messageService.error('شماره پروانه تأسیس داروخانه معتبر نیست! ', 'نمایش خطا');
                //    resetCaptcha();
                //    return;
                //}

                if (model.Password.length > 30) {
                    messageService.error('رمز عبور معتبر نیست! ', 'نمایش خطا');
                    resetCaptcha();
                    return;
                }

                if (model.Password.length > 5) {
                    for (i = 0; i < model.Password.length; i++) {
                        var charCode = model.Password.charCodeAt(i);
                        if (charCode > 47 && charCode < 58)
                            countPassword++;
                    }

                    if (countPassword != 0 && countPassword < model.Password.length) {
                        if ($scope.model.Password == $scope.model.ConfirmPassword) {
                            dataService.postData("/app/api/Register/InitiateRegister", model).then(function (data) {
                                var params = { model: model };
                                $state.go('finalRegisteration', params);
                            },
                                function (error) {
                                    resetCaptcha();
                                });
                        }
                        else {
                            messageService.error('یکسان بودن رمز عبور و تایید آن الزامیست! ', 'نمایش خطا');
                            resetCaptcha();
                        }
                    }
                    else {
                        messageService.error('رمز عبور وارد شده باید ترکیبی از حروف و عدد باشد! ', 'نمایش خطا');
                        resetCaptcha();
                    }
                }
                else {
                    messageService.error('رمز عبور باید حداقل 6 کاراکتر  باشد! ', 'نمایش خطا');
                    resetCaptcha();
                }
            }

            $scope.redirect = function () {
                var url = 'default';
                $state.go(url);
            }

            $scope.trustAsHtml = function (value) {
                return $sce.trustAsHtml(value)
            }

            $(window).keypress(function (e) {
                if (e.keyCode == 13) {
                    $scope.$apply(function () {
                        $scope.register($scope.model);
                    });
                }
            });

        }
    ]);
});


