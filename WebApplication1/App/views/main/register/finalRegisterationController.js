define(['application', 'dataService', 'messageService', 'jquery-realperson', 'buttonValidation'], function (app) {
    app.register.controller('finalRegisterationController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state', '$sce',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state, $sce) {

            $rootScope.applicationModule = "وارد نمودن کد تایید ";
            $scope.redirect = function () {
                $scope.model.Password = '';
                $scope.model.ConfirmPassword = '';
                $scope.model.MobileCaptcha = '';

                var params = { model: $scope.model };
                $state.go('register', params);
            }

            $scope.model = {

                UserName: '',
                Password: '',
                ConfirmPassword: '',
                MobileNo: '',
                MobileCaptcha: '',
                Captcha: ''
            };

            if (!$stateParams || !$stateParams.model || $stateParams.model == null) {
                $scope.redirect();
            }
            else {
                $scope.model = $stateParams.model

                $rootScope.userModel = {};
                $rootScope.visibleForLogin = true;
                $scope.loginControlVisible = true;

            }

            $scope.finalRegisteration = function (model) {
                dataService.postData("/app/api/Register/FinalRegister", model).then(function (data) {
                    $rootScope.userModel = data;
                    $rootScope.visibleForLogin = false;
                    $scope.loginControlVisible = false;
                    dataService.getMenus();
                    $state.go('moshahedeDarkhastMoteghaziSearch');
                },
                    function (error) {
                    });
            }

            $scope.trustAsHtml = function (value) {
                return $sce.trustAsHtml(value)
            }

            $(window).keypress(function (e) {
                if (e.keyCode == 13) {
                    $scope.$apply(function () {
                        $scope.finalRegisteration($scope.model);
                    }
                    );
                }
            });
        }]);
});


