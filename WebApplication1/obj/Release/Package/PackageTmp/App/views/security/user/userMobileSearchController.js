"use strict";
define(['application', 'dataService', 'messageService', 'kendoGridFa'], function (app) {
    app.register.controller('userMobileSearchController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "  مدیریت کاربران موبایل";
            $scope.applicationDescription = "در این صفحه امکان جستجو کاربران موبایل وجود دارد."

            $scope.operationAccess = dataService.getOperationAccessRoute('/app/api/UserInfo/GetUserMobileOperationAccess');
 

            if (!$scope.operationAccess.canView)
                return;

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
                                    MobileNo: { type: "string", field: "MobileNo" },
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
                            field: "ID",
                            title: "شناسه",
                            width: 115
                        },
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
                            field: "MobileNo",
                            title: "شماره موبایل",
                        
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
                        }
                    ]
                };
            }

     

        }
    ]);
});


