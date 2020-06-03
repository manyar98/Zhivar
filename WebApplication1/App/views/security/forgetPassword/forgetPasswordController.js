define(['application', 'dataService', 'messageService', 'kendoGridFa', 'buttonValidation'], function (app) {
    app.register.controller('forgetPasswordController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state', '$stateParams', 'defaultUrlOtherwise',
        function ($scope, $rootScope, dataService, messageService, $state, $stateParams, defaultUrlOtherwise) {

            $rootScope.applicationModule = "کد زمانک";

            $scope.mobileCaptcha = '';
            $rootScope.visibleForLogin = true;
            $scope.loginControlVisible = true;

            $scope.init = function () {
            }

            $scope.insertCaptchaMobile = function (mobileCaptcha) {
                var model = {
                    UserName: $rootScope.forgotPass.UserName,
                    Code: mobileCaptcha
                }
                dataService.postData('/app/api/Login/GetUserNameByForgotPassCode/', model).then(function (response) {
                    $rootScope.forgotPass.UserName = response;
                    $rootScope.forgotPass.Code = model.Code;
                    $scope.redirectToChangePass();
                }, function (error) {
                    if (error.resultCode == 5) {
                        messageService.warningMessage(error.failures);
                        $rootScope.forgotPass = {};
                        $state.go(defaultUrlOtherwise);
                    }
                });
            }

            $scope.redirectToChangePass = function () {
                var url = 'forgotPassChanging';
                $state.go(url);
            }
        }
    ])
});