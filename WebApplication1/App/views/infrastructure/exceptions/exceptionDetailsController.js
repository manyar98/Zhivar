define(['application', 'dataService', 'messageService', 'kendoUi', 'persianDateTime', 'trustHtml'], function (app) {
    app.register.controller('exceptionDetailsController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state) {

            $rootScope.applicationModule = "اطلاعات خطای سیستم";
            $scope.applicationDescription = "در این صفحه اطلاعات خطای سیستم نمایش داده می شود.";

            $scope.operationAccess = dataService.getOperationAccess("ExceptionLog");

            $scope.redirect = function () {
                var url = 'exceptionSearch';
                $state.go(url);
            }

            $scope.exceptionDetails = [];

            $scope.initialize = function () {
                if ($scope.operationAccess.canView) {
                    var id = $stateParams.id;
                    if (id != "") {
                        dataService.getById('app/api/exceptionLog/showDetails?id=', id).then(function (data) {
                            $scope.exceptionDetails = data;
                        });
                    }
                }
            }
        }
    ]);
});