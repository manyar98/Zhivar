define(['application', 'dataService', 'messageService', 'kendoUi', 'kendoGridFa', 'checkTreeList'], function (app) {
    app.register.controller('roleOperationController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state) {

            $rootScope.applicationModule = "انتساب دسترسی";
            $rootScope.applicationDescription = "برای انتساب دسترسی به نقش انتخاب شده، می توانید  با استفاده از لیست کپی دسترسی، دسترسی های یک نقش را به نقش دیگر انتساب دهید، همچنین میتوانید دسترسی های لازم را از لیست نمایش داده شده انتخاب نمایید";

            $scope.RoleId = $stateParams.id;
            $scope.statusBind = true;
            $scope.roleOperations = [{ ID: -1, Name: "لطفا نقش را انتخاب نمایید" }];
            $scope.roleUser = "-1"
            //$scope.mainGridOptions = {};

            $scope.roleOperationAccess = dataService.getOperationAccessRoute('/app/api/Role/GetRoleOperationOperationAccess');

            $scope.initialize = function () {
                dataService.getAll('/app/api/Role/GetRoleInfoAsync').then(function (data) {
                    $scope.setControlsData(data);
                });
            }

            $scope.setControlsData = function (data) {
                data.forEach(function (item) {
                    $scope.roleOperations.push({ ID: item.Key, Name: item.Value });
                });

                $scope.getRoleById($stateParams.id, true)
            }

            $scope.getRoleById = function (id, hasName) {
                dataService.getById('/app/api/Role/', id).then(function (data) {
                    if (hasName) {
                        $scope.FullName = data.Name;
                    }
                    $scope.LoadTreeCheckBox(data.RoleOperations);
                });
            }

            $scope.changeRole = function (id) {
                if (id && id != -1) {
                    $scope.getRoleById(id, false)
                }
                else if (id == -1) {
                    $scope.loadTree([]);
                    if (!$scope.$$phase)
                        $scope.$apply();
                }
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
                    serverFiltering: true,
                    
                }),
                dataBound: function (e) {
                },
                height: "450px",
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

            //$scope.dataSource = new kendo.data.TreeListDataSource({
            //    transport: {
            //        read: {
            //            type: "GET",
            //            url: '/app/api/Operation/GetOperationAndMenusAsync',
            //            dataType: "json"
            //        }
            //    },
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
            //    },
            //    serverPaging: false,
            //    serverFiltering: true,
            //});

            //$scope.loadTree = function (list) {
            //    $scope.mainGridOptions = {
            //        dataSource: $scope.dataSource,
            //        sortable: true,
            //        pageable: true,
            //        selectable: 'row',
            //        height: 335,
            //        filterable: {
            //            extra: false
            //        },
            //        checkbox: { field: 'OperationId', list: list },
            //        columns: [
            //            {
            //                field: "Name",
            //                title: "عنوان",
            //                width: 300
            //            }, {
            //                field: "Code",
            //                title: " کد",
            //                width: 870
            //            }
            //        ]
            //    };
            //}

            $scope.action = function () {
                if ($scope.roleOperationAccess.canInsert) {
                    var result = $scope.mainTreeOptions.checkboxes.getAllCheckedItems();
                    if (result.length > 0) {
                        var list = [];
                        $.each(result, function (index, value) {
                            var entity = {
                                RoleId: $scope.RoleId,
                                IsPermision: 1,
                                OperationId: value.ID
                            }
                            list.push(entity);
                        });
                    }
                    var model = {
                        RoleOperations: list,
                        RoleId: $scope.RoleId,
                    };
                    $scope.update(model);
                }
            }

            $scope.redirect = function () {
                var url = 'roleSearch';
                $state.go(url);
            }

            $scope.update = function (model) {
                dataService.addEntity("/app/api/Role/UpdateOperations", model).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.redirect();
                });
            }

        }
    ]);
});

