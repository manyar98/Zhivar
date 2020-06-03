define(['application', 'dataService', 'messageService', 'kendoGridFa'], function (app) {
    app.register.controller('userOperationSearchController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "جستجوی دسترسی کاربر";

            $scope.model = {};
            $scope.returnModel = {};
            $scope.searchResultShow = false;
            $scope.searchResultRedColor = false;

            $scope.operationAccess = dataService.getOperationAccess("Operation");

            $scope.init = function () { }

            $scope.search = function (model) {
                $scope.searchResultShow = false;
                dataService.postData('app/api/UserInfo/CheckSecurityAccess', model).then(function (data) {
                    $scope.searchResultShow = true;
                    $scope.returnModel.FullName = data.FullName;
                    $scope.returnModel.OprName = data.OprName;
                    $scope.returnModel.OprType = data.OprType;
                    $scope.returnModel.HasAccess = data.HasAccess;
                    $scope.returnModel.UserName = model.UserName;
                    $scope.returnModel.OperationCode = model.OperationCode;

                    if(data.HasAccess == "نمی باشد.")
                        $scope.searchResultRedColor = true;
                    else
                        $scope.searchResultRedColor = false;
                });
            }

            $scope.redirect = function () {
                $state.go('dfault');
            }
        }
    ])
})