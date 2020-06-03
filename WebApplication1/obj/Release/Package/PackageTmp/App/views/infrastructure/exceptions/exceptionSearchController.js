define(['application', 'dataService', 'momentJalaali', 'kendoGridFa', 'persianDateTime', 'trustHtml'], function (app) {
    app.register.controller('exceptionSearchController', ['$scope', '$rootScope', 'dataService', '$state', '$uibModal',
        function ($scope, $rootScope, dataService, $state, $uibModal) {

            $rootScope.applicationModule = "خطاهای سیستم";
            $scope.applicationDescription = "در این صفحه امکان جستجوی خطا های سیستم وجود دارد برای مشاهده جزییات خطا بر روی دکمه نمایش جزییات کلیک نمایید.";

            $scope.operationAccess = dataService.getOperationAccess("ExceptionLog");
            if (!$scope.operationAccess.canView)
                return;

            $scope.exceptionTypes = [];

            $scope.loadExceptionDetailPopup = function (id) {
                if (id) {
                    dataService.getById('app/api/exceptionLog/showDetails?id=', id).then(function (data) {
                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'exceptionDetailPopup.html',
                            controller: exceptionDetailModalController,
                            size: 'lg',
                            windowClass: 'app-modal-window',
                            resolve: {
                                exceptionDetailModel: data
                            }
                        });
                    });
                }
            }

            function exceptionDetailModalController($scope, $uibModalInstance, exceptionDetailModel) {
                $scope.formInit = function () {
                    $scope.exceptionDetails = exceptionDetailModel;
                }

                $scope.closePopup = function () {
                    $uibModalInstance.close();
                }
            };

            $scope.showDetails = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $scope.loadExceptionDetailPopup(id);
            }

            $scope.commands = [
                $scope.operationAccess.canView == true ? { name: "showDetails", text: "نمايش جزئيات", click: $scope.showDetails } : "",
            ];

            $scope.initialize = function () {
                var prommissList = ['app/api/Enum/GetExceptionTypes'];
                dataService.callBackData(setGridOptions, prommissList, $scope.operationAccess);
            };

            $scope.dataSource = $scope.operationAccess.canView == false ? null : new kendo.data.DataSource({
                transport: {
                    read: {
                        type: "GET",
                        url: "/app/api/ExceptionLog",
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
                            ExceptionDate: { type: "date", editable: false },
                            Code: { type: "string", editable: false },
                            ExceptionMessage: { type: "string", editable: false },
                            ExceptionType: { field: "ExceptionType", type: "string", editable: false },
                        }
                    }
                },
                pageSize: 7,
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
            });

            setGridOptions = function (options) {
                options[0].forEach(function (item) {
                    var exType = {};
                    exType.value = item.Key, exType.text = item.Value;
                    $scope.exceptionTypes.push(exType);
                });

                $scope.mainGridOptions = {
                    dataSource: $scope.dataSource,
                    selectable: "single",
                    sortable: true,
                    pageable: {
                        refresh: true
                    },
                    filterable: {
                        extra: false
                    },
                    columns: [
                        {
                            field: "ExceptionDate",
                            title: "تاريخ",
                            width: "120px",
                            template: "<span ng-bind=\"dataItem.ExceptionDate | persianDateTime\"></span>",
                            filterable:
                                 {
                                     extra: true,
                                     ui: 'datepicker',
                                     operators: {
                                         date: {
                                             gt: "بزرگتر",
                                             lt: "کوچکتر"
                                         }
                                     }
                                 },
                        }, {
                            field: "Code",
                            title: "کد",
                            width: "200px",
                        }, {
                            field: "ExceptionMessage",
                            title: "پيغام",
                            width: "300px",
                        }, {
                            field: "ExceptionType",
                            title: "نوع",
                            width: "120px",
                            values: $scope.exceptionTypes,
                            filterable: {
                                operators: {
                                    string: {
                                        eq: "برابر",
                                        neq: "مخالف"
                                    }
                                },
                            }
                        }, {
                            field: "IP",
                            title: "آی پی",
                            width: "100px",
                        }, {
                            command: $scope.commands.filter(function (n) { return n != "" }),
                            title: "&nbsp;", width: "80px"
                        }
                    ]
                };
            }
        }
    ]);
});
