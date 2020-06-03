define(['application', 'dataService', 'messageService', 'kendoGridFa', 'momentJalaali'], function (app) {
    app.register.controller('onlineUserSearchController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "کاربران آنلاین";
            $scope.applicationDescription = "در این صفحه امکان مشاهده کاربران آنلاین و خروج آنها از سامانه وجود دارد."
            $scope.stores = [];

            $scope.operationAccess = dataService.getOperationAccess("OnlineUser");
            $scope.commands = [$scope.operationAccess.canUpdate == true ? { text: "خروج", imageClass: "k-icon k-delete", name: "logOut", click: destroy } : ""];

            $scope.init = function () {
                $scope.setControlsData(null);
            }

            $scope.confirmDelete = { active: false, model: {} }

            function destroy(e) {
                $scope.confirmDelete.model = this.dataItem($(e.currentTarget).closest("tr"));
                $scope.confirmDelete.active = true;
                $scope.$apply();
            }

            $scope.logOut = function () {
                $scope.confirmDelete.active = false;
                dataService.postData("/app/api/OnlineUser/Logoff", { userToken: $scope.confirmDelete.model.Token }).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.mainGridOptions.dataSource.read();
                });
            };

            $scope.dataSource = new kendo.data.DataSource({
                transport: {
                    read: {
                        url: "/app/api/OnlineUser",
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
                            UserID: { type: "number" },
                            UserName: { type: "string" },
                            FirstName: { type: "string" },
                            LastName: { type: "string" },
                            Token: { type: "string" },
                            LastLoginDate: { type: "date" }
                        },
                    }
                },
                pageSize: 7,
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true
            });

            $scope.setControlsData = function (options) {
                $scope.loadGrid();
            };

            $scope.loadGrid = function () {
                $scope.mainGridOptions = {
                    dataSource: $scope.operationAccess.canView == false ? null : $scope.dataSource,
                    selectable: "single",
                    sortable: true,
                    filterable: {
                        extra: false,
                    },
                    pageable: {
                        refresh: true
                    },
                    columns: [{
                        field: "UserName",
                        title: "نام کاربری",
                    }, {
                        field: "FirstName",
                        title: "نام",
                    }, {
                        field: "LastName",
                        title: "نام خانوادگی",
                    }, {
                        title: "تاریخ و زمان ورود",
                        field: "LastLoginDate",
                        template: "#= moment(LastLoginDate).format('jYYYY/jMM/jDD - HH:mm')#",
                        filterable: {
                            extra: true,
                            ui: 'datepicker',
                            operators: {
                                date: {
                                    gt: "بزرگتر",
                                    lt: "کوچکتر"
                                }
                            }
                        },
                    }
                    , {
                        command: $scope.commands.filter(function (n) { return n != "" }),
                        title: "&nbsp;",
                        width: "120px"
                    }]
                };
            }
        }
    ])
})