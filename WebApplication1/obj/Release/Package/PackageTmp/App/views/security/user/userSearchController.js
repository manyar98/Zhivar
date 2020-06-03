"use strict";
define(['application', 'dataService', 'messageService', 'kendoGridFa'], function (app) {
    app.register.controller('userSearchController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "مدیریت کاربران";
            $scope.applicationDescription = "در این صفحه امکان جستجو کاربران تعریف شده، اضافه کردن کاربر جدید، ویرایش و حذف کاربر، انتساب دسترسی و انتساب نقش به کاربر وجود دارد."

            $scope.operationAccess = dataService.getOperationAccess("UserInfo");
            $scope.userRoleOperationAccess = dataService.getOperationAccessRoute('/app/api/UserInfo/GetUserRoleOperationAccess');
            $scope.userOperationOperationAccess = dataService.getOperationAccessRoute('/app/api/UserInfo/GetUserOperationOperationAccess');

            if (!$scope.operationAccess.canView)
                return;

            $scope.confirmDelete = { active: false, ID: '' }

            $scope.edit = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("userInfo", { id: id });
            }

            $scope.add = function () {
                $state.go("userInfo", { id: "-1" });
            }

            function destroy(e) {
                $scope.confirmDelete.ID = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $scope.confirmDelete.active = true;
                $scope.$$phase || $scope.$apply();
            }

            $scope.role = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("userRole", { id: id });
            }

            $scope.operation = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("operationUser", { id: id });
            }

            $scope.commands = [
                $scope.operationAccess.canUpdate == true ? { text: "ویرایش", imageClass: "k-icon k-edit", name: "edit", click: $scope.edit } : "",
                $scope.operationAccess.canDelete == true ? { text: "حذف", imageClass: "k-icon k-delete", name: "delete", click: destroy } : "",
                $scope.userRoleOperationAccess.canInsert == true ? { imageClass: "k-icon k-i-rotatecw", text: "انتساب نقش", name: "role", click: $scope.role } : "",
                $scope.userOperationOperationAccess.canInsert == true ? { imageClass: "k-icon k-i-hbars", text: "انتساب دسترسی", name: "operation", click: $scope.operation } : "",
            ];

            function loadTamplate() {
                return '<button type="button" class="k-button k-button-icontext" ng-click="add()"><span class="k-icon k-add"></span>اضافه کردن کاربر جدید</button>';
            }

            $scope.toolbarTemplate = $scope.operationAccess.canInsert ? loadTamplate() : ' ';

            $scope.init = function () {
                var promiselist = ['/app/api/Enum/GetAuthenticationTypes'];
                dataService.callBackData($scope.loadGridData, promiselist, $scope.operationAccess);
            }

            $scope.loadGridData = function (options) {
                var authenticationTypes = [];
                options[0].forEach(function (item) {
                    var authenticationType = {};
                    authenticationType.value = item.Key;
                    authenticationType.text = item.Value;
                    authenticationTypes.push(authenticationType);
                });

                $scope.mainGridOptions = {
                    dataSource: new kendo.data.DataSource({
                        transport: {
                            read: {
                                url: "/app/api/UserInfo",
                                dataType: "json"
                            },
                            parameterMap: function (options, operation) {
                                if (operation == "read") {
                                    if (!$state.current.data.fromChildState)
                                        $state.current.data.gridOptions = options;
                                    else if (angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                                        $state.current.data.fromChildState = false;
                                        options = $state.current.data.gridOptions;
                                    }
                                }

                                return options;
                            }
                        },
                        requestStart: function (e) {
                            if (e.type == "read") {
                                if ($state.current.data.fromChildState == true && angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                                    e.sender._page = $state.current.data.gridOptions.page;
                                    e.sender._skip = $state.current.data.gridOptions.skip;
                                    e.sender._filter = $state.current.data.gridOptions.filter;
                                    e.sender._sort = $state.current.data.gridOptions.sort;
                                }
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
                                    ID: { type: "number", editable: false, nullable: false },
                                    UserName: { type: "string" },
                                    FirstName: { type: "string" },
                                    LastName: { type: "string" },
                                    IsActive: { type: "boolean" },
                                    AuthenticationType: { type: "string", field: "AuthenticationType" },
                                    RoleNamesStr: { type: "string" }
                                },
                            }
                        },
                        pageSize: 7,
                        serverPaging: true,
                        serverSorting: true,
                        serverFiltering: true
                    }),
                    selectable: "single",
                    sortable: true,
                    filterable: {
                        extra: false,
                    },
                    pageable: {
                        refresh: true
                    },
                    columns: [
                        {
                            field: "UserName",
                            title: "نام کاربری",
                            width: 115
                        }, {
                            field: "FullName",
                            title: "نام و نام خانوادگی",
                            sortable: true,
                            width: 140
                        }, {
                            field: "RoleNamesStr",
                            title: "نقش",
                            sortable: false,
                            width: 100
                        }, {
                            field: "AuthenticationType",
                            title: "نوع کاربر",
                            values: authenticationTypes,
                            width: 100
                        }, {
                            title: "وضعیت",
                            field: "IsActive",
                            template: "#= IsActive? ' فعال' : ' غیر فعال' #",
                            filterable: {
                                messages: {
                                    isTrue: " فعال می باشند",
                                    isFalse: " غیرفعال می باشند"
                                }
                            },
                            width: 90
                        }, {
                            command: $scope.commands.filter(function (n) { return n != "" }),
                            title: "&nbsp;",
                            width: 300
                        }
                    ]
                };
            }

            $scope.deleteRow = function () {
                $scope.confirmDelete.active = false;
                dataService.deleteEntity("/app/api/UserInfo/", $scope.confirmDelete.ID).then(function (data) {
                    $scope.mainGridOptions.dataSource.read();
                    messageService.success('عملیات با موفقیت انجام شد', '');
                });
            };

        }
    ]);
});


