define(['application', 'dataService', 'messageService', 'trustHtml'], function (app) {
    app.register.controller('errorController', ['$scope', '$rootScope', '$stateParams', '$state', 'dataService', 'messageService', '$window',
        function ($scope, $rootScope, $stateParams, $state, dataService, messageService, $window) {

            $rootScope.applicationModule = "نمایش خطا";
            $scope.applicationDescription = "لطفا کد خطا را به همراه توضیحات به مرکز پشتیبانی اطلاع دهید";

            $scope.init = function () {
                $scope.errorMessage = $stateParams.message;
                $scope.errorStackTrace = $stateParams.stackTrace;
            }

            $scope.redirect = function () {
                if ($rootScope.userModel.UserName) {
                    $rootScope.visibleForLoginView = false;
                    $rootScope.visibleForAnonymousUser = false;
                }

                $window.history.back();
            };

        }
    ]);
});
