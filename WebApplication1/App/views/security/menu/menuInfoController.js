define(['application', 'kendoUi', 'kendoGridFa', 'dataService', 'messageService'], function (app) {
    app.register.controller('menuInfoController', ['$scope', '$rootScope', '$stateParams', '$state', 'dataService', 'messageService',
        function ($scope, $rootScope, $stateParams, $state, dataService, messageService) {

            $rootScope.applicationModule = "اطلاعات منو";
            $scope.applicationDescription = "برای انجام عملیات روی آیتم منو، ابتدا اطلاعات را وارد / ویرایش نموده سپس برروی دکمه ثبت کلیک نمایید.";

            $scope.operationAccess = dataService.getOperationAccess("Menu");

            $scope.model = {
                ID: '',
                ParentId: '',
                Name: '',
                Code: '',
                AppId: 1,
                OperationType: 2,
                IsActive: true,
                IsDeleted: false,
                OrderNo: '',
                Tag1: '',//URL
                Tag2: '',//Icon Class
                Tag3: 1,//External Link
                Tag4: '',//Text Style
            };

            $scope.OperationType = 0;
            $scope.subMenu = false;

            $scope.init = function () {
                if (angular.isUndefined($scope.operationAccess.canInsert) || $scope.operationAccess.canInsert == false)
                    return;

                if ($stateParams.mode != '-1') {
                    //Edit
                    $scope.model.ID = $stateParams.id;

                    dataService.getById('/app/api/Menu/', $scope.model.ID).then(function (data) {
                        $scope.model = data;

                        if (data.OperationType == 3) {
                            $scope.model.Tag3 = data.Tag3.toLowerCase() == "true" ? 2 : 1;
                            $scope.subMenu = true;
                        }
                    });
                }
                else {
                    if ($stateParams.id != 0) {
                        $scope.model.ParentId = $stateParams.id;
                        dataService.getById('/app/api/Menu/', $scope.model.ParentId).then(function (data) {
                            if (data.OperationType == 2 || data.OperationType == 3) {
                                $scope.subMenu = true;
                                $scope.OperationType = 3;
                            }
                        });
                    }
                    else if ($stateParams.id == 0) {
                        $scope.OperationType = 2;
                    }
                }
            }

            $scope.redirect = function () {
                var url = 'menuSearch';
                $state.go(url);
            }

            $scope.action = function (model) {
                if ($scope.operationAccess.canInsert) {
                    if (model.Code == "" || model.Name == "") {
                        return;
                    }

                    model.IsActive = model.IsActive == 1 ? true : false;

                    if (model.OperationType == 3 || $scope.OperationType == 3)
                        model.Tag3 = model.Tag3 == 2 ? "true" : "false";
                    else {
                        model.Tag3 = '';
                    }

                    if (model.ID != "") {
                        $scope.edit(model);
                    }
                    else {
                        $scope.add(model);
                    }
                }
            }

            $scope.add = function (model) {
                model.OperationType = $scope.OperationType;
                dataService.addEntity("/app/api/Menu/", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

            $scope.edit = function (model) {
                dataService.updateEntity("/app/api/Menu/", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});
