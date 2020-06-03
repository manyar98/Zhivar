define(['application', 'dataService', 'messageService'], function (app) {
    app.register.controller('roleInfoController', ['$scope', '$rootScope', '$stateParams', '$state', 'dataService', 'messageService',
        function ($scope, $rootScope, $stateParams, $state, dataService, messageService) {

            $rootScope.applicationModule = "اطلاعات نقش";
            $scope.applicationDescription = "برای ثبت نقش عنوان و کد نقش را وارد نمایید. وضعیت، مشخص کننده فعال یا عدم فعال بودن نقش می باشد";

            $scope.init = function () {

            }

            $scope.operationAccess = dataService.getOperationAccess("Role");

            $scope.model = {
                ID: '',
                ParentId: '',
                Name: '',
                Code: '',
                AppId: 1,
                IsActive: true,
                IsDeleted: false,
            };

            if ($stateParams.mode != '-1') {
                $scope.model.ID = $stateParams.id;
                dataService.getById('/app/api/Role/', $scope.model.ID).then(function (data) {
                    $scope.model = data;
                });
            } else {
                if ($stateParams.id != 0) {
                    $scope.model.ParentId = $stateParams.id;
                }
            }

            $scope.redirect = function () {
                var url = 'roleSearch';
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
                dataService.addEntity("/app/api/Role/", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

            $scope.edit = function (model) {
                dataService.updateEntity("/app/api/Role/", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});
