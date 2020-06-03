define(['application', 'kendoGridFa', 'dataService', 'messageService', 'combo', 'scrollbar', 'helper', 'editChap', 'editSaze', 'editContact', 'nodeSelect', 'goroheSazeSelect', 'vahedSelect', 'noeSazeSelect', 'noeEjareSelect', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'maskedinput'], function (app) {
    app.register.controller('wfContractModirController', ['$scope', '$stateParams', '$location', '$state', 'dataService', 'messageService', '$uibModal',
        function ($scope, $stateParams, $location, $state, dataService, messageService, $uibModal) {

            var contractSazeId = $stateParams.id;

            $scope.operationAccess = {
                canView : true,
            };
            $scope.init = function () {


               
                $scope.loadInvoice($stateParams.id);
                $scope.setGridOptions();
            };

            $scope.loadInvoice = function (id) {
                $scope.name = "id:" + id;
                $scope.loading = true;
                $.ajax({
                    type: "POST",
                    data: JSON.stringify(id),
                    url: "/app/api/Contract/LoadContractSazeTransObj",
                    contentType: "application/json"
                }).done(function (res) {
                    var result = res.data;
                    $scope.loading = false;
                    $scope.Contract_Saze = result.Contract_Saze;
                    $scope.Contract_Saze.Contarct_Saze_Bazareabs = result.Contarct_Saze_Bazareabs;
                    $scope.Contract_Saze.Contract_Saze_Tarahs = result.Contract_Saze_Tarahs;
                    $scope.Contract_Saze.Contract_Saze_Chapkhanes = result.Contract_Saze_Chapkhanes;
                    $scope.Contract_Saze.Contract_Saze_Nasabs = result.Contract_Saze_Nasabs;

                }).fail(function (error) {
                    $scope.loading = false;
                    $scope.invoice = null;
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, title: "خطا", type: "error" });
                });
            };
            //$rootScope.loadCurrentBusinessAndBusinesses($scope.init);


            function openTasvirModal(req) {
                var uibModalInstance = $uibModal.open({
                    templateUrl: 'TasvirPopup.html',
                    controller: function ($scope, $uibModalInstance, request, controllerScope) {
                        $scope.modelModal = {
                            ID: '',
                            MoshaverID: '',
                            NoeTasvirID: '',
                            MimeType: '',
                            FileName: '',
                            FileSize: '',
                            Blob: '',
                            TasvirBlobBase64: '',
                            TooltipCaption: '',
                            FileSpaceThumbnail: '',
                            FileNameThumbnail: '',
                            FileSizeThumbnail: '',
                        }
                        $scope.formInit = function () {
                            $scope.modalTitle = "نمایش تصویر";

                            if (!(req.ID == 0 || angular.isUndefined(req.ID))) {
                                dataService.getById('app/api/MadarekPayvast/', req.ID).then(function (data) {
                                    $scope.modelModal = data;
                                    if (data.Blob) {
                                        $scope.modelModal.TasvirBlobBase64 = data.Blob.replace("data:image/jpeg;base64,", "");
                                        $scope.modelModal.Blob = "data:image/jpeg;base64," + $scope.modelModal.TasvirBlobBase64;
                                        $scope.modelModal.FileName = data.FileName;
                                        $scope.modelModal.FileSize = data.FileSize;
                                        $scope.modelModal.MimeType = "image/jpeg";
                                    }
                                    var widthModalDialog = Number($scope.modelModal.WidthImageOrginal) + 30;
                                    var heightModalDialog = Number($scope.modelModal.HeightImageOrginal)


                                    $(" div.modal-dialog").attr("style", "width:" + widthModalDialog + "px !important");
                                    if (heightModalDialog < 300)
                                        $(" div.modal-dialog").attr("style", "width:" + widthModalDialog + "px !important" + ";" + "margin:" + 12 + "% auto !important");
                                });
                            }
                        }
                        $scope.closePopup = function () {
                            $uibModalInstance.close();
                        }
                    },
                    windowClass: 'app-modal-window',
                    resolve: {
                        request: function () {
                            return req;
                        },
                        controllerScope: function () {
                            return $scope;
                        }
                    }
                });
            };

            function Magnify(e) {
                var row = this.dataItem($(e.currentTarget).closest("tr"));
                var req = {
                    ID: row.ID
                };
                openTasvirModal(req);
            }


            $scope.commands = [
                //$scope.operationAccess.canView == true ?
                { name: "magnify", text: "مشاهده تصویر", imageClass: "k-icon k-i-custom", click: Magnify }
                //: "",
                //$scope.operationAccess.canUpdate == true ? { name: "edit", text: "ویرایش", imageClass: "k-icon k-edit", click: edit } : "",
                //$scope.operationAccess.canDelete == true ? { text: "حذف", imageClass: "k-icon k-delete", click: destroy } : ""
            ];

            function loadTamplate() {
                return '<button type="button" class="k-button k-button-icontext" ng-click="add()"><span class="k-icon k-add"></span>افزودن تصویر</button>';
            }
            $scope.toolbarTemplate = $scope.operationAccess.canInsert ? loadTamplate() : '';
            //$scope.toolbarTemplate =  loadTamplate() ;
            $scope.setGridOptions = function setGridOptions(options) {
                //var noeTasvir = [];
                //$scope.noeTasvirs = [];
                //$scope.noeTasvir = options[0];
                //if (options[0])
                //    options[0].forEach(function (type) {
                //        noeTasvir.push({ value: type.ID, text: type.Nam });
                //        $scope.noeTasvirs.push({ value: type.ID, text: type.Nam });
                //    });



                $scope.mainGridOptions = {
                    dataSource: new kendo.data.DataSource({
                        transport: {
                            read: {
                                url: '/app/api/MadarekPayvast',
                                dataType: "json"
                            },
                            parameterMap: function (options, operation) {
                                if (operation === "read") {
                                    if (options.filter && options.filter.filters) {
                                        options.filter.filters.push({ field: "RecordID", operator: "eq", value: contractSazeId });
                                    }
                                    else {
                                        options.filter = { filters: [{ field: "RecordID", operator: "eq", value: contractSazeId }] };
                                    }
                                }
                                return options;
                            }
                            //parameterMap: function (options, operation) {
                            //	if (operation == "read") {
                            //		if (!$state.current.data.fromChildState)
                            //			$state.current.data.gridOptions = options;
                            //		else if (angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                            //			$state.current.data.fromChildState = false;
                            //			options = $state.current.data.gridOptions;
                            //		}
                            //	}
                            //	return options;
                            //}
                        },
                        //requestStart: function (e) {
                        //    if (e.type == "read") {
                        //        if ($state.current.data.fromChildState == true && angular.isDefined($state.current.data.gridOptions) && !angular.equals({}, $state.current.data.gridOptions)) {
                        //            e.sender._page = $state.current.data.gridOptions.page;
                        //            e.sender._skip = $state.current.data.gridOptions.skip;
                        //            e.sender._filter = $state.current.data.gridOptions.filter;
                        //            e.sender._sort = $state.current.data.gridOptions.sort;
                        //        }
                        //    }
                        //},
                        schema: {
                            data: function (data) {
                                return dataService.processResponse(data);
                            },
                            total: function (data) {
                                return dataService.getCount(data);
                            },
                            model: {
                                id: "ID",
                                fields: {
                                    ID: { type: "number", editable: false, nullable: false },
                                    RecordID: { type: "number" },
                                    DocTypeID: { type: "number" }

                                }
                            }
                        },
                        pageSize: 9,
                        serverPaging: true,
                        serverSorting: true,
                        serverFiltering: true
                    }),
                    filterable: {
                        extra: false
                    },
                    groupable: false,
                    resizable: true,
                    selectable: "single",
                    scrollable: true,
                    sortable: {
                        mode: "single",
                        allowUnsort: true
                    },
                    pageable: {
                        refresh: true
                    },
                    columns: [
                        {
                            field: "FileName",
                            title: "نام فایل",
                            width: 100
                        },
                        {
                            command: $scope.commands.filter(function (n) { return n !== "" }),
                            title: "&nbsp;",
                            width: 200
                        }
                    ]
                };
            };

            $scope.init();


        }])
}); 