define(['application', 'dataService', 'messageService', 'kendoGridFa'], function (app) {
    app.register.controller('roleSearchController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "مدیریت نقش ها";
            $scope.applicationDescription = "در این صفحه امکان جستجو نقش های تعریف شده، اضافه کردن نقش جدید، ویرایش و حذف نقش، انتسلب دسترسی به نقش وجود دارد."

            $scope.confirmDelete = { active: false, model: {} }
            $scope.id = 0;

            $scope.add = function () {
                $state.go("roleInfo", { id: 0, mode: "-1" });
            }

            $scope.addParent = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("roleInfo", { id: id, mode: "-1" });
            }

            $scope.edit = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("roleInfo", { id: id, mode: "1" });
            }

            $scope.destroy = function (e) {
                $scope.confirmDelete.model = this.dataItem($(e.currentTarget).closest("tr"));
                $scope.$apply(function () {
                    $scope.confirmDelete.active = true;
                });
            }

            $scope.operation = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("roleOperation", { id: id });
            }

            $scope.operationAccess = dataService.getOperationAccess("Role");
            $scope.roleOperationAccess = dataService.getOperationAccessRoute('/app/api/Role/GetRoleOperationOperationAccess');

            $scope.commands = [
                $scope.operationAccess.canInsert == true ? { name: "addoperation", text: "اضافه", click: $scope.addParent, imageClass: "k-add", } : "",
                $scope.operationAccess.canUpdate == true ? { imageClass: "k-edit", name: "editTree", text: "ویرایش", click: $scope.edit } : "",
                $scope.operationAccess.canDelete == true ? { imageClass: "k-delete", name: "deleteTree", text: "حذف", click: $scope.destroy } : "",
                $scope.roleOperationAccess.canInsert == true ? { imageClass: "k-i-hbars", name: "operation", text: "انتساب دسترسی", click: $scope.operation } : ""
            ];

            function loadTamplate() {
                return '<button type="button" class="k-button k-button-icontext" ng-click="add()"><span class="k-icon k-add"></span>اضافه کردن نقش به ریشه</button>';
            }

            if ($scope.operationAccess.canInsert)
                $scope.toolbarTemplate = loadTamplate();
            else
                $scope.toolbarTemplate = ' ';

            $scope.dataSource = $scope.operationAccess.canView == false ? null : new kendo.data.TreeListDataSource({
                serverPaging: false,
                serverSorting: true,
                serverFiltering: true,
                transport: {
                    read: {
                        type: "GET",
                        url: '/app/api/Role',
                        dataType: "json"
                    },
                },
                batch: false,
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
                            parentId: { type: "number", field: "ParentId", nullable: true },
                            Code: { type: "string", validation: { required: true } },
                            Name: { type: "string", validation: { required: true } },
                            IsActive: { type: "boolean" },
                            IsDeleted: { type: "boolean" }
                        },
                        expanded: false
                    }
                },

            });

            $scope.mainGridOptions = {
                dataSource: $scope.dataSource,
                sortable: true,
                pageable: true,
                filterable: {
                    extra: false,
                },
                columnMenu: false,
                dataBound: function () {
                    $(".k-treelist").find('button').attr('type', 'button');
                },
                selectable: "single", change: function (e) {
                    var grid = e.sender;
                    var selectedData = grid.dataItem(grid.select());
                    $scope.id = selectedData.ID;
                },
                columns: [
                    {
                        field: "Name",
                        title: "عنوان",
                    }, {
                        field: "Code",
                        title: " کد",
                        width: "250px"
                    }, {
                        title: "وضعیت",
                        field: "IsActive",
                        template: "#= IsActive? ' فعال' : ' غیر فعال' #",
                        filterable: false,
                        width: "100px"
                    }, {
                        command: $scope.commands.filter(function (n) { return n != "" }),
                        width: "450px"
                    }]
            };

            $scope.deleteRow = function () {
                $scope.confirmDelete.active = false;
                dataService.deleteEntity("/app/api/Role/", $scope.confirmDelete.model.ID).then(function (data) {
                    $scope.mainGridOptions.dataSource.read();
                    messageService.success('عملیات با موفقیت انجام شد', '');
                });
            };

        }
    ]);
});



