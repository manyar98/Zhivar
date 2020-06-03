define(['application', 'kendoGridFa', 'dataService'], function (app) {
    app.register.controller('cacheInfoController', ['$scope', '$rootScope', 'dataService', '$state','messageService',
        function ($scope, $rootScope, dataService, $state, messageService) {

            $rootScope.applicationModule = "مدیریت حافظه نهان";
            $scope.applicationDescription = "";

            $scope.operationAccess = dataService.getOperationAccessRoute('/app/api/CacheInfo/GetCacheInfoOperationAccess');
            if (!$scope.operationAccess.canView)
                return;

            $scope.confirmDelete = { active: false, model: '' }

            $scope.init = function () {

            }

            $scope.deleteRow = function () {
                $scope.confirmDelete.active = false;
                dataService.deleteEntity('/app/api/CacheInfo/DeleteItemInListCache?Key=',$scope.confirmDelete.model.Key).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.mainGridOptions.dataSource.read();
                });
            }

            function destroy(e) {
                $scope.confirmDelete.model = this.dataItem($(e.currentTarget).closest("tr"));
                $scope.confirmDelete.active = true;
                $scope.$$phase || $scope.$apply();
            }

            $scope.commands = [
               $scope.operationAccess.canDelete == true ? { text: "حذف", imageClass: "k-icon k-delete", click: destroy } : ""
            ];

            $scope.mainGridOptions = {
                dataSource: new kendo.data.DataSource({
                    transport: {
                        read: {
                            url: "/app/api/CacheInfo/GetCacheInfoList",
                            dataType: "json"
                        },
                    },
                    schema: {
                        data: function (result) {
                            return dataService.processResponse(result);
                        },
                        total: function (data) {
                            return dataService.getCount(data);
                        },
                        model: {
                            id: "Key",
                            fields: {
                                ID: { type: "string" },
                                Name: { type: "string" },
                            },
                        }
                    },
                    pageSize: 9,
                    serverPaging: false,
                    serverSorting: false,
                    serverFiltering: false
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
                         field: "Key",
                         title: "کلید مقدار حافظه نهان"
                     }, {
                         command: $scope.commands.filter(function (n) { return n != "" }),
                         title: "&nbsp;",
                         width: 350
                     }
                ]
            };

        }
    ]);
});


