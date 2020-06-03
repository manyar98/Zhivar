define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'checkTreeList'], function (app) {
    app.register.controller('operationUserController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state) {

            $rootScope.applicationModule = "انتساب دسترسی";
            $scope.applicationDescription = "برای انتساب دسترسی به کاربر انتخاب شده، می توانید  با استفاده از لیست  کپی دسترسی، دسترسی های یک کاربر را به کاربر دیگر انتساب داد، همچنین می توانید دسترسی های لازم را از لیست نمایش داده شده انتخاب نمایید.";

            $scope.userOperationOperationAccess = dataService.getOperationAccessRoute('/app/api/UserInfo/GetUserOperationOperationAccess');

            $scope.initComplete = function () {
                dataService.getById('/app/api/UserInfo/', $stateParams.id).then(function (data) {
                    $scope.FullName = data.FullName;
                    $scope.LoadTreeCheckBox(data.UserOperations);
                });
            }

            $scope.LoadTreeCheckBox = function (list) {
                $scope.mainTreeOptions.checkboxes.checkList = list;
            };

            $scope.treeHtmlOptions = {
                tabindex: 1,
                class: "treeHtmlOptions",
                style: {}
            };

            $scope.mainTreeOptions = {
                dataSource: new kendo.data.TreeListDataSource({
                    transport: {
                        read: {
                            type: "GET",
                            url: '/app/api/Operation/GetOperationAndMenusAsync',
                            dataType: "json"
                        }
                    },
                    schema: {
                        data: function (result) {
                            return dataService.processResponse(result);
                        },
                        total: function (data) {
                            return dataService.getCount(data);
                        },
                        model: {
                            id: "ID",
                            fields: {
                                ID: { type: "number", editable: false },
                                parentId: { type: "number", field: "ParentId", nullable: true },
                                Name: { type: "string", editable: false },
                                Code: { type: "string", editable: false }
                            },
                            expanded: false
                        }
                    },
                    batch: false,
                    serverPaging: false,
                    serverSorting: true,
                    serverFiltering: true
                }),
                dataBound: function (e) {
                },
                enable: true,
                checkboxes: {
                    editable: true,
                    checkListField: 'OperationId',
                    parentIsImportant: true,
                    check: function (checkedItem, elm) {
                        checkedItem;
                    }
                },
                columns: [
                    {
                        field: "Name",
                        title: "عنوان",
                        width: 400
                    }, {
                        field: "Code",
                        title: " کد",
                        width: 700
                    }
                ]
            }

            $scope.action = function () {
                //$scope.mainTreeOptions.enable = true;
                //$scope.mainTreeOptions.checkboxes.editable = false;
                //var result2 = $scope.mainTreeOptions.checkboxes.getNewCheckedModels();
                //$scope.treeList.setDataSource(new kendo.data.TreeListDataSource());
                if (!$scope.userOperationOperationAccess.canInsert)
                    return;

                var result = $scope.mainTreeOptions.checkboxes.getAllCheckedItems();
                if (result.length > 0) {
                    var list = [];
                    result.forEach(function (value) {
                        list.push({
                            UserId: $stateParams.id,
                            IsPermision: 1,
                            OperationId: value.id
                        });
                    });
                }

                var model = {
                    OperationUsers: list,
                    UserId: $stateParams.id
                };
                $scope.update(model);
            }

            $scope.redirect = function () {
                var url = 'userSearch';
                if ($state.current.previousState && $state.current.previousState.name == url && $state.current.previousState.data)
                    $state.current.previousState.data.fromChildState = true;
                else
                    $state.get(url).data = {};
                $state.go(url);
            }

            $scope.update = function (model) {
                dataService.addEntity("/app/api/UserInfo/UpdateOperations", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});

