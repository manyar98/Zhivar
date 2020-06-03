define(['application', 'dataService', 'messageService', 'kendoUi', 'momentJalaali', 'kendoGridFa'], function (app) {
    app.register.controller('activityDetailsController', ['$scope', '$rootScope', '$stateParams', 'dataService', 'messageService', '$state', '$sce',
        function ($scope, $rootScope, $stateParams, dataService, messageService, $state, $sce) {
            $rootScope.applicationModule = "اطلاعات رخداد";
            $scope.applicationDescription = "در این صفحه اطلاعات رخداد نمایش داده می شود.";
            $scope.operationAccess = dataService.getOperationAccess("ActivityLog");

            $scope.redirect = function () {
                var url = 'activitySearch';
                $state.go(url);
            }

            $scope.activityDetails = [];                       

            $scope.formatDate = function (value) {
                return moment(value).format('jYYYY/jMM/jDD - HH:mm');
            }

            $scope.initialize = function () {
                if ($scope.operationAccess.canView) {
                    var id = $stateParams.id;
                    if (id) {
                        dataService.getById('app/api/activityLog/showDetails?id=', id).then(function (data) {
                            $scope.activityDetails = data;
                            $scope.setGrid($scope.activityDetails);
                        });
                        dataService.getById('app/api/activityLog/GetByID?id=', id).then(function (data) {
                            $scope.gridDetails = "نمایش جزییات مربوط به " + " ' " + data.ActionPersianName + " ' " + "موجودیت " + " ' " + data.EntityName + " ' " + " در تاریخ " + " ' " + $scope.formatDate(data.RecordDateTime) + " ' " + "توسط کاربر " + " ' " + data.UserName + " ' :";
                        });
                    }
                }
            }

            $scope.setGrid = function (data) {
                $scope.dataSource = $scope.operationAccess.canView == false ? null : new kendo.data.DataSource({
                    data: data,
                    schema: {
                        model: {
                            fields: {
                                m_Item1: { type: "string", editable: false },
                                m_Item2: { type: "string", editable: false },
                            }
                        }
                    },
                    pageSize: 10,
                    serverPaging: true,
                    serverSorting: true,
                    serverFiltering: true,
                });

                $scope.mainGridOptions = {
                    dataSource: $scope.dataSource,
                    filterable: {
                        extra: false
                    },
                    height: 340,
                    groupable: false,
                    resizable: true,
                    scrollable: true,
                    sortable: {
                        mode: "single",
                        allowUnsort: true
                    },
                    pageable: false,
                    columns: [
                          {
                              field: "m_Item1",
                              title: "نام",
                              filterable:
                                {
                                    operators: {
                                        string: {
                                            eq: "برابر",
                                            neq: "مخالف"
                                        }
                                    },
                                }
                          },
                          {
                              field: "m_Item2",
                              title: "مقدار",
                              filterable:
                                  {
                                      cell:
                                          {
                                              dataSource: {},
                                          }
                                  }
                          },
                    ]
                };
            }
        }
    ]);
});