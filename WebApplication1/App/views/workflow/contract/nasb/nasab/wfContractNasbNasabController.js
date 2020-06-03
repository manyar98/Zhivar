define(['application', 'kendoGridFa', 'combo', 'scrollbar', 'helper', 'editChap', 'editSaze', 'editContact', 'nodeSelect', 'goroheSazeSelect', 'vahedSelect', 'noeSazeSelect', 'noeEjareSelect', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker', 'maskedinput', 'messageService', 'buttonValidation'], function (app) {
    app.register.controller('wfContractNasbNasabController', ['$scope', '$rootScope', '$stateParams', '$location', '$state', 'messageService', 'dataService', '$uibModal',
        function ($scope, $rootScope, $stateParams, $location, $state, messageService, dataService, $uibModal) {


            var contractSazeId = $stateParams.id;

            $scope.init = function () {


                $scope.loadInvoice(contractSazeId);


                //dataService.getPagedData('/app/api/DocType/GetDocType', { noeDocID: noeDoc }).then(function (data) {
                //    if (data) {
                //        var noeTasvir = [];
                //        $scope.noeTasvir = data;

                //        data.forEach(function (type) {

                //            noeTasvir.push({ value: type.Key, text: type.Value });
                //            $scope.noeTasvirs.push({ value: type.Key, text: type.Value });
                //        });
                //        setGridOptions();
                //        if (angular.isDefined($rootScope.userModel.Tag3)) {
                //            if ($rootScope.userModel.Tag3 != "-1") {

                //                dataService.getById('/app/api/MarzNeshin/', $rootScope.userModel.Tag3).then(function (data) {
                //                    if (data) {
                //                        $scope.model = data;

                //                    }
                //                });
                //            }
                //        }
                //    }
                //});
                var model = {
                    noeDocID: 1
                }

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/DocType/GetDocType",
                    contentType: "application/json"
                }).done(function (res) {
                    var data = res.data;

                    if (data) {
                        var noeTasvir = [];
                        $scope.noeTasvir = data;

                        angular.forEach($scope.noeTasvir.records, function (key, value) {
                            noeTasvir.push({ value: key.Key, text: key.Value });
                            $scope.noeTasvirs.push({ value: key.Key, text: key.Value });
                        });

                        //data.forEach(function (item) {

                        //    noeTasvir.push({ value: item.Key, text: item.Value });
                        //    $scope.noeTasvirs.push({ value: item.Key, text: item.Value });
                        //});
                        setGridOptions();
                        //if (angular.isDefined($rootScope.userModel.Tag3)) {
                        //    if ($rootScope.userModel.Tag3 != "-1") {

                        //        dataService.getById('/app/api/MarzNeshin/', $rootScope.userModel.Tag3).then(function (data) {
                        //            if (data) {
                        //                $scope.model = data;

                        //            }
                        //        });
                        //    }
                        //}
                    }



                }).fail(function (error) {

                });

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
                    $scope.Contract_Saze.ContractSazeImages = result.ContractSazeImages;

                    totalFiles = [];
                    for (var i = 0; i < $scope.Contract_Saze.ContractSazeImages.length; ++i) {

                        var tasvirBlobBase64 = $scope.Contract_Saze.ContractSazeImages[i].Blob.replace("data:image/jpeg;base64,", "");

                        totalFiles.push({
                            src: "data:image/jpeg;base64," + tasvirBlobBase64,
                            file: {
                                name: $scope.Contract_Saze.ContractSazeImages[i].FileName,
                                size: $scope.Contract_Saze.ContractSazeImages[i].FileSize,
                                type: $scope.Contract_Saze.ContractSazeImages[i].MimeType
                            }
                        });



                    }

                }).fail(function (error) {
                    $scope.loading = false;
                    $scope.invoice = null;
                    applyScope($scope);
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error, title: "خطا", type: "error" });
                });
            };
            //$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
            $scope.save = function () {

                var model = [];
                for (var i = 0; i < totalFiles.length; ++i) {


                    model.push({
                        ContractSazeId: contractSazeId,
                        TasvirBlobBase64: totalFiles[i].src.replace("data:image/jpeg;base64,", ""),
                        Blob: null,
                        FileName: totalFiles[i].file.name,
                        FileSize: totalFiles[i].file.size,
                        MimeType: totalFiles[i].file.type

                    });



                }

                if (model == null || model.length == 0) {
                    messageService.error('الصاق نمودن تصویر  الزامیست', '');
                    return;
                }

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Contract/SaveContractSazeImages",
                    contentType: "application/json"
                }).done(function (res) {

                    var model = res.data;
                    messageService.success('اطلاعات مورد نظر با موفقیت ذخیره گردید.', '');


                }).fail(function (error) {
                    messageService.error('عملیات با شکست مواجه شد.', '');

                });
            }

            $scope.init();


            ///payvast

            $scope.operationAccess =
                {
                    canView: true,
                    canInsert: true,
                    canUpdate: true,
                    canDelete: true,
                }
            //dataService.getOperationAccess('Contract');
            if (!$scope.operationAccess.canView)
                return;

            $scope.confirmDelete = { active: false, model: '' }
            var noeDoc = "ContractTarah";
            $scope.noeTasvirs = [];
            $scope.model = {}
            $scope.initialize = function () {

            }
            $scope.action = function () {

                //if ($scope.mainGridOptions.dataSource.data().length < 2) {
                //    messageService.error("بارگذاری تصاویر شناسنامه و تصویر شخص در این مرحله الزامیست");
                //    return;
                //}
                //else
                //    $state.go('pmznPayment');

            };
            $scope.redirect = function () {
                $state.go('pmznInformation');
            }
            function edit(e) {
                var row = this.dataItem($(e.currentTarget).closest("tr"));
                var req = {
                    ID: row.ID
                };
                openMoshaverTasvirInfoModal(req);
            }

            function Magnify(e) {
                var row = this.dataItem($(e.currentTarget).closest("tr"));
                var req = {
                    ID: row.ID
                };
                openTasvirModal(req);
            }

            $scope.add = function () {
                var req = {
                    ID: ''
                };
                openMoshaverTasvirInfoModal(req);
            }
            function openMoshaverTasvirInfoModal(req) {
                var uibModalInstance = $uibModal.open({
                    templateUrl: 'moshaverTasavirInfoPopup.html',
                    controller: function ($scope, $uibModalInstance, request, controllerScope) {
                        $scope.modelModal = {
                            ID: '',
                            RecordID: contractSazeId,
                            DocTypeID: '',
                            MimeType: '',
                            FileName: '',
                            FileSize: '',
                            Blob: '',

                        }
                        $scope.formInit = function () {
                            $scope.modalTitle = request.ID ? "ویرایش" : "افزودن";
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
                                });
                            }

                            setDataOptions();

                        }

                        function setDataOptions() {
                            $scope.modelModal.noeTasvirOptions = {
                                dataSource: controllerScope.noeTasvir.records || [],
                                dataTextField: "Value",
                                dataValueField: "Key",
                                optionLabel: "لطفا نوع تصویر را انتخاب نمایید",
                                valuePrimitive: true,
                                change: function (e) {
                                }
                            }
                        }
                        $scope.fileOptions = {
                            localization: {
                                select: "انتخاب تصویر"
                            },
                            multiple: false,
                            showFileList: false,
                            enabled: true,
                            select: function (files) {
                                var file = files.files[0];
                                // var ok = file.extension.toLowerCase() == ".jpg" || file.extension.toLowerCase() == ".jpeg";
                                // if (!ok) {
                                // messageService.error("فقط تصویر با فرمت jpg  قابل قبول است.");
                                //    return;
                                // }
                                //  else 
                                if (file.size > 10240000) {
                                    messageService.error("حداکثر سایز مجاز برای تصویر 10 مگابایت میباشد.");
                                    return;
                                }
                                var reader = new FileReader();
                                reader.onload = function (e) {
                                    $scope.modelModal.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                                    $scope.modelModal.Blob = "data:image/jpeg;base64," + $scope.modelModal.TasvirBlobBase64;
                                    $scope.modelModal.FileName = file.name;
                                    $scope.modelModal.FileSize = file.size;
                                    $scope.modelModal.MimeType = file.rawFile.type;
                                    $scope.$$phase || $scope.$apply();
                                }
                                reader.readAsDataURL(file.rawFile);
                            }
                        }

                        $scope.action = function (modelModal) {
                            if (modelModal.TasvirBlobBase64 === null || modelModal.TasvirBlobBase64 === "") {
                                messageService.error('وارد نمودن تصویر مدرک الزامیست', '');
                                return;
                            }
                            modelModal.Blob = null;

                            modelModal.RecordID = contractSazeId;
                            modelModal.Blob = "";
                            modelModal.DocTypeID = 1;

                            if (modelModal.Nam === '' || angular.isUndefined(modelModal.Nam))
                                modelModal.Nam = modelModal.FileName;

                            if (modelModal.ID === '' || angular.isUndefined(modelModal.ID))
                                addTasvir(modelModal);
                            else
                                editTasvir(modelModal);
                        }

                        function addTasvir(modelModal) {
                            dataService.addEntity("/app/api/MadarekPayvast", modelModal).then(function (data) {
                                messageService.success('عملیات با موفقیت انجام شد', '');
                                $scope.closePopup();
                                controllerScope.mainGridOptions.dataSource.read();
                            }, function (e) {
                                if (e.resultCode === 4)
                                    $scope.closePopup();
                            });
                        }


                        function editTasvir(modelModal) {

                            dataService.updateEntity("/app/api/MadarekPayvast", modelModal).then(function (data) {
                                messageService.success('عملیات با موفقیت انجام شد', '');
                                $scope.closePopup();
                                controllerScope.mainGridOptions.dataSource.read();
                            }, function (e) {
                                if (e.resultCode === 4)
                                    $scope.closePopup();
                            });
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

            function openTasvirModal(req) {
                var uibModalInstance = $uibModal.open({
                    templateUrl: 'TasvirPopup.html',
                    controller: function ($scope, $uibModalInstance, request, controllerScope) {
                        $scope.modelModal = {
                            ID: '',
                            DarkhastID: $rootScope.userModel.Tag3,
                            DocTypeID: '',
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

            function destroy(e) {
                $scope.confirmDelete.model = this.dataItem($(e.currentTarget).closest("tr"));
                $scope.confirmDelete.active = true;
                $scope.$$phase || $scope.$apply();
            }

            $scope.deleteRow = function () {
                $scope.confirmDelete.active = false;
                dataService.deleteEntity("/app/api/MadarekPayvast/", $scope.confirmDelete.model.ID).then(function (data) {
                    messageService.success('عملیات با موفقیت انجام شد', '');
                    $scope.mainGridOptions.dataSource.read();
                });
            }

            $scope.commands = [
                $scope.operationAccess.canView == true ? { name: "magnify", text: "مشاهده تصویر", imageClass: "k-icon k-i-custom", click: Magnify } : "",
                $scope.operationAccess.canUpdate == true ? { name: "edit", text: "ویرایش", imageClass: "k-icon k-edit", click: edit } : "",
                $scope.operationAccess.canDelete == true ? { text: "حذف", imageClass: "k-icon k-delete", click: destroy } : ""
            ];

            function loadTamplate() {
                return '<button type="button" class="k-button k-button-icontext" ng-click="add()"><span class="k-icon k-add"></span>افزودن تصویر</button>';
            }
            $scope.toolbarTemplate = $scope.operationAccess.canInsert ? loadTamplate() : '';

            function setGridOptions() {


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

                        },

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
            }

        }])
}); 