"use strict";
define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'checkTreeList'], function (app) {
    app.register.controller('userRoleController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state) {

            $rootScope.applicationModule = "انتساب نقش";
            $scope.applicationDescription = "برای انتساب نقش به کاربر مدنظر، نقش یا نقش های لازم را از لیست انتخاب نمایید سپس روی دکمه ثبت کلیک نمایید.";

            $scope.UserId = $stateParams.id;
            $scope.FullName = "";
            //$scope.statusBind = true;
            //$scope.mainGridOptions = {};

            $scope.userRoleOperationAccess = dataService.getOperationAccessRoute('/app/api/UserInfo/GetUserRoleOperationAccess');

            dataService.getById('/app/api/UserInfo/', $stateParams.id).then(function (data) {
                 $scope.FullName = data.FullName;
                 $scope.LoadTreeCheckBox(data.UserRoles);
             });

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
                            url: '/app/api/Role',
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
                    checkListField: 'RoleId',
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

            //$scope.action = function () {
            //    if (!$scope.userOperationOperationAccess.canInsert)
            //        return;

            //    var result = $scope.mainTreeOptions.checkboxes.getAllCheckedItems();
            //    if (result.length > 0) {
            //        var list = [];
            //        result.forEach(function (value) {
            //            list.push({
            //                UserId: $stateParams.id,
            //                IsPermision: 1,
            //                OperationId: value.id
            //            });
            //        });
            //    }

            //    var model = {
            //        OperationUsers: list,
            //        UserId: $stateParams.id
            //    };
            //    $scope.update(model);
            //}

            //$scope.redirect = function () {
            //    var url = 'userSearch';
            //    if ($state.current.previousState && $state.current.previousState.name == url && $state.current.previousState.data)
            //        $state.current.previousState.data.fromChildState = true;
            //    else
            //        $state.get(url).data = {};
            //    $state.go(url);
            //}

            //$scope.update = function (model) {
            //    dataService.addEntity("/app/api/UserInfo/UpdateOperations", model).then(function (data) {
            //        messageService.success('عملیات با موفقیت انجام شد', '');
            //        $scope.redirect();
            //    });
            //}

            //$scope.dataSource = $scope.userRoleOperationAccess.canInsert == false ? null : new kendo.data.TreeListDataSource({
            //    transport: {
            //        read: {
            //            type: "GET",
            //            url: '/app/api/Role',
            //            dataType: "json"
            //        }
            //    },
            //    serverPaging: true,
            //    serverFiltering: true,
            //    batch: false,
            //    schema: {
            //        data: function (result) {
            //            return dataService.processResponse(result);
            //        },
            //        total: function (data) {
            //            return dataService.getCount(data);
            //        },
            //        model: {
            //            id: "ID",
            //            fields: {
            //                ID: { type: "number", editable: false, nullable: false },
            //                parentId: { type: "number", field: "ParentId", nullable: true },
            //                Code: { type: "string", validation: { required: true } },
            //                Name: { type: "string", validation: { required: true } },
            //            },
            //            expanded: false
            //        }
            //    }
            //});

            //$scope.loadTree = function (list) {
            //    $scope.mainGridOptions = {
            //        dataSource: $scope.dataSource,
            //        sortable: true,
            //        pageable: true,
            //        height: 335,
            //        filterable: {
            //            extra: false
            //        },
            //        checkbox: {
            //            field: 'RoleId', list: list
            //        },
            //        columnMenu: false,
            //        columns: [
            //             {
            //                 field: "Name",
            //                 title: "عنوان",
            //                 width: 270
            //             }, {
            //                 field: "Code",
            //                 title: "کد",
            //                 width: 870
            //             }
            //        ]
            //    };
            //}

            $scope.action = function () {
                if ($scope.userRoleOperationAccess.canInsert) {
                    var result = $scope.mainTreeOptions.checkboxes.getAllCheckedItems();
                    if (result.length > 0) {
                        var list = [];
                        $.each(result, function (index, value) {
                            var entity = {
                                RoleId: value.ID,
                                UserId: $scope.UserId,
                            }
                            list.push(entity);
                        });
                    }
                    var model = {
                        UserRoles: list,
                        UserId: $scope.UserId,
                    };
                    $scope.update(model);
                }
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
                dataService.addEntity("/app/api/UserInfo/UpdateUserRole", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});