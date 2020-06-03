define(['application', 'dataService', 'messageService', 'kendoGridFa'], function (app) {
    app.register.controller('menuSearchController', ['$scope', '$rootScope', 'dataService', 'messageService', '$state',
        function ($scope, $rootScope, dataService, messageService, $state) {

            $rootScope.applicationModule = "مدیریت منو";
            $scope.applicationDescription = "در این صفحه امکان جستجو آیتم های تعریف شده منو، اضافه کردن آیتم جدید، ویرایش و حذف آیتم ها وجود دارد."

            $scope.id = 0;
            $scope.confirmDelete = { active: false, model: '' }

            $scope.add = function () {
                $state.go("menuInfo", { id: 0, mode: "-1" });
            }

            $scope.addParent = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("menuInfo", { id: id, mode: "-1" });
            }

            $scope.edit = function (e) {
                var id = this.dataItem($(e.currentTarget).closest("tr")).ID;
                $state.go("menuInfo", { id: id, mode: "1" });
            }

            $scope.destroy = function (e) {
                $scope.confirmDelete.model = this.dataItem($(e.currentTarget).closest("tr"));
                $scope.confirmDelete.active = true;
                $scope.$apply();
            }

           // $scope.operationAccess = dataService.getOperationAccess("Menu");
             $scope.operationAccess ={
                 canView: true,
                 canInsert: true,
                 canUpdate:true,
                 canDelete: true
             };

            $scope.commands = [
                $scope.operationAccess.canInsert == true ? { imageClass: "k-add", name: "addoperation", text: "اضافه", click: $scope.addParent } : "",
                $scope.operationAccess.canUpdate == true ? { imageClass: "k-edit", name: "editTree", text: "ویرایش", click: $scope.edit } : "",
                $scope.operationAccess.canDelete == true ? { imageClass: "k-delete", name: "deleteTree", text: "حذف", click: $scope.destroy } : ""
            ];

            function loadTamplate() {
                return '<button type="button" class="k-button k-button-icontext" ng-click="add()"><span class="k-icon k-add"></span>اضافه کردن آیتم به ریشه</button>';
            }

            $scope.toolbarTemplate = $scope.operationAccess.canInsert ? loadTamplate() : '';

            $scope.dataSource = $scope.operationAccess.canView == false ? null : new kendo.data.TreeListDataSource({
                serverPaging: false,
                serverSorting: true,
                serverFiltering: true,
                transport: {
                    read: {
                        type: "GET",
                        url: '/app/api/Menu',
                        dataType: "json"
                    },
                },
                batch: false,
                schema: {
                    data: function (result) {
                        return dataService.processResponse(result);
                    },
                    total: function (result) {
                        return dataService.getCount(result);
                    },
                    model: {
                        id: "ID",
                        fields: {
                            ID: { type: "number", editable: false, nullable: false },
                            parentId: { type: "number", field: "ParentId", nullable: true },
                            OrderNo: { type: "number" },
                            Name: { type: "string" },
                            Code: { type: "string" },
                            IsActive: { type: "boolean" },
                            IsDeleted: { type: "boolean" }
                        },
                        expanded: false
                    }
                }
            });

            $scope.mainGridOptions = {
                dataSource: $scope.dataSource,
                sortable: true,
                pageable: true,
                selectable: "row",
                filterable: {
                    extra: false,
                },
                columnMenu: false,
                dataBound: function () {
                    $(".k-treelist").find('button').attr('type', 'button');
                },
                change: function (e) {
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
                        title: "کد",
                        width: 400
                    }, {
                        field: "OrderNo",
                        title: "ترتیب",
                        filterable: false,
                        width: 100
                    }, {
                        title: "وضعیت",
                        field: "IsActive",
                        template: "#= IsActive? ' فعال' : ' غیر فعال' #",
                        filterable: false,
                        width: 100
                    }, {
                        command: $scope.commands.filter(function (n) { return n != "" }),
                        title: "",
                        width: 270
                    }
                ]
            };

            $scope.deleteRow = function () {
                $scope.confirmDelete.active = false;
                dataService.deleteEntity("/app/api/Menu/", $scope.confirmDelete.model.ID).then(function (data) {
                    $scope.mainGridOptions.dataSource.read();
                    messageService.success('عملیات با موفقیت انجام شد', '');
                });
            };

            $scope.selectedRow = function (id) {
                setTimeout(function () {
                    var index = 0;
                    var entity = $.grep($scope.mainGridOptions.dataSource.view(), function (e, i) { if (e.id == id) { index = i; return e } })[0];
                    treelist = $("#treelist").data("kendoTreeList");
                    row = treelist.content.find("tr").eq(index);
                    treelist.select(row);
                }, 1500)
            }


        }
    ]);
});



