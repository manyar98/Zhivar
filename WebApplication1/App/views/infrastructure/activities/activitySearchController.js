define(['application', 'dataService', 'momentJalaali', 'kendoGridFa', 'persianDateTime'], function (app) {
    app.register.controller('activitySearchController', ['$scope', '$rootScope', 'dataService', '$state', '$uibModal', '$filter',
        function ($scope, $rootScope, dataService, $state, $uibModal, $filter) {

            $rootScope.applicationModule = "رخدادهای سیستم";
            $scope.applicationDescription = "در این صفحه امکان جستجوی رخداد های سیستم وجود دارد برای مشاهده جزییات رخداد بر روی دکمه نمایش جزییات کلیک نمایید.";
            $scope.activityTypes = [];

            $scope.operationAccess = dataService.getOperationAccess("ActivityLog");

            $scope.loadActivityDetailPopup = function (id) {
                if (id) {
                    dataService.getById('app/api/activityLog/showDetails?id=', id).then(function (data) {
                        var modalInstance = $uibModal.open({
                            animation: true,
                            templateUrl: 'activityDetailPopup.html',
                            controller: $scope.activityDetailModalController,
                            size: 'lg',
                            windowClass: 'app-modal-window',
                            resolve: {
                                activityDetailModel: function () {
                                    return data;
                                },
                                id: id
                            }
                        });
                    });
                }
            }

            $scope.activityDetailModalController = function ($scope, $uibModalInstance, activityDetailModel, id) {
                $scope.formInit = function () {
                    dataService.getById('app/api/activityLog/GetByID?id=', id).then(function (data) {
                        $scope.gridDetails = "نمایش جزییات مربوط به " + " ' " + data.ActionPersianName + " ' " + "موجودیت " + " ' " + data.EntityName + " ' " + " در تاریخ " + " ' " + $filter('persianDateTime')(data.RecordDateTime) + " ' " + "توسط کاربر " + " ' " + data.UserName + " ' :";
                    });

                    setGrid(activityDetailModel);
                }

                function setGrid(data) {
                    $scope.mainGridOptionsModal = {
                        dataSource: new kendo.data.DataSource({
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
                        }),
                        filterable: {
                            extra: false
                        },
                        groupable: false,
                        resizable: true,
                        scrollable: true,
                        sortable: {
                            mode: "single",
                            allowUnsort: true
                        },
                        pageable: false,
                        height: 568,
                        columns: [
                              {
                                  field: "m_Item1",
                                  title: "نام",
                                  filterable: {
                                      operators: {
                                          string: {
                                              eq: "برابر",
                                              neq: "مخالف"
                                          }
                                      },
                                  }
                              }, {
                                  field: "m_Item2",
                                  title: "مقدار",
                                  filterable: {
                                      cell: {
                                          dataSource: {},
                                      }
                                  }
                              }
                        ]
                    };
                }

                $scope.closePopup = function () {
                    $uibModalInstance.close();
                }
            };

            $scope.showDetails = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $scope.loadActivityDetailPopup(id);
            }

            $scope.commands = [
                $scope.operationAccess.canView == true ? { name: "showDetails", text: "نمايش جزئيات", click: $scope.showDetails } : "",
            ];

            $scope.initialize = function () {
                var prommissList = ['app/api/Enum/GetActivityTypes'];
                dataService.callBackData(setGridOptions, prommissList);
            };

            function setGridOptions(options) {
                options[0].forEach(function (item) {
                    var actType = {};
                    actType.value = item.Key, actType.text = item.Value;
                    $scope.activityTypes.push(actType);
                });

                $scope.mainGridOptions = {
                    dataSource: new kendo.data.DataSource({
                        transport: {
                            read: {
                                type: "GET",
                                url: "/app/api/ActivityLog",
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
                                    EntityID: { type: "string", editable: false },
                                    EntityName: { type: "string", editable: false },
                                    Action: { type: "string", editable: false },
                                    RecordDateTime: { type: "date", editable: false },
                                    UserID: { type: "number", editable: false },
                                    UserName: { type: "string", editable: false },
                                    ClientIP: { type: "string", editable: false }
                                }
                            }
                        },
                        pageSize: 7,
                        serverPaging: true,
                        serverSorting: true,
                        serverFiltering: true,
                    }),
                    selectable: "single",
                    sortable: true,
                    //height: 428,
                    pageable: {
                        refresh: true
                    },
                    filterable: {
                        extra: false
                    },
                    columns: [
                        {
                            field: "EntityID",
                            title: "شناسه موجودیت",
                            width: 140
                        }, {
                            field: "EntityName",
                            title: "نام موجودیت",
                            width: 180
                        }, {
                            field: "Action",
                            title: "نوع رخداد",
                            values: $scope.activityTypes,
                            filterable: {
                                operators: {
                                    string: {
                                        eq: "برابر",
                                        neq: "مخالف"
                                    }
                                },
                            },
                            width: 120
                        }, {
                            field: "RecordDateTime",
                            title: "تاريخ و ساعت",
                            width: 130,
                            template: "<span ng-bind=\"dataItem.RecordDateTime | persianDateTime\"></span>",
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
                            field: "UserID",
                            title: "شناسه کاربر",
                            width: 120,
                            filterable: {
                                operators: {
                                    number: {
                                        eq: "برابر",
                                        neq: "مخالف"
                                    }
                                },
                            }
                        }, {
                            field: "UserName",
                            title: "نام کاربری",
                            width: 120,

                        }, {
                            field: "ClientIP",
                            title: "آدرس IP",
                            width: 120,

                        }, {
                            command: $scope.commands.filter(function (n) { return n != "" }),
                            title: "&nbsp;",
                            width: 100
                        }
                    ]
                };
            }
        }
    ]);
});
