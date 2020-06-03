define(['application', 'kendoUi', 'kendoGridFa', 'dataService', 'messageService'], function (app) {
    app.register.controller('operationInfoController', ['$scope', '$rootScope', '$stateParams', '$state', 'dataService', 'messageService',
        function ($scope, $rootScope, $stateParams, $state, dataService, messageService) {

            $rootScope.applicationModule = "اطلاعات دسترسی";
            $scope.applicationDescription = "برای انجام عملیات روی دسترسی، ابتدا اطلاعات را وارد / ویرایش نموده سپس برروی دکمه ثبت کلیک نمایید.";

            $scope.operationAccess = dataService.getOperationAccess("Operation");

            $scope.model = {
                ID: '',
                ParentId: '',
                Name: '',
                Code: '',
                AppId: 1,
                OperationType: 1,
                IsActive: true,
                IsDeleted: false,
                OrderNo: '',
                Tag1: '',//URL
                Tag2: '',//Icon Class
                Tag3: '',//External Link
                Tag4: '',
            };

            $scope.init = function () {
                if (angular.isUndefined($scope.operationAccess.canInsert) || $scope.operationAccess.canInsert == false)
                    return;

                if ($stateParams.mode != '-1') {
                    //Edit
                    $scope.model.ID = $stateParams.id;

                    dataService.getById('/app/api/Operation/', $scope.model.ID).then(function (data) {
                        $scope.model = data;
                    });
                }
                else if ($stateParams.id != 0) {
                    $scope.model.ParentId = $stateParams.id;
                }
            }

            $scope.redirect = function () {
                var url = 'operationSearch';
                $state.go(url);
            }

            $scope.action = function (model) {
                if ($scope.operationAccess.canInsert) {
                    if (model.Code == "" || model.Name == "") {
                        return;
                    }

                    model.IsActive = model.IsActive == 1 ? true : false;

                    if (model.ID != "") {
                        $scope.edit(model);
                    }
                    else {
                        $scope.add(model);
                    }
                }
            }

            $scope.add = function (model) {
                model.OperationType = 1;
                dataService.addEntity("/app/api/Operation/", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

            $scope.edit = function (model) {
                dataService.updateEntity("/app/api/Operation/", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});
