define(['application', 'kendoUi', 'kendoGridFa'], function (app) {
    app.register.directive('editContractOperation', ['$compile', '$uibModal', 'messageService', function ($compile, $uibModal, messageService) {

        return {
            restrict: 'E',
            transclude: true,
            templateUrl: '/App/template/edit-contract-operation.html',
            scope: {
                contractstop: '=',
                //onsuccess: '=',
                open: '=',
                iscreated: '=',
                contractid: '=',
                isstopgroup: '=',
                sazeid: '='

            },
            link: function (scope, element, attrs) {
                scope.initEditReservation = function () {
                    scope.actRow2 = {
                        Quantity: 0
                    };
                    scope.invoiceItem = Hesabfa.newContractSazeItemObj();

                    scope.fileOptions = {
                        localization: {
                            select: "انتخاب تصویر"
                        },
                        multiple: false,
                        showFileList: false,
                        enabled: true,
                        select: function (files) {
                            var file = files.files[0];
                            var ok = file.extension.toLowerCase() === ".jpg" || file.extension.toLowerCase() === ".jpeg";
                            if (!ok) {
                                messageService.error("فقط تصویر با فرمت jpg  قابل قبول است.");
                                return;
                            }
                            else if (file.size > 204800) {
                                messageService.error("حداکثر سایز مجاز برای تصویر 200kb میباشد.");
                                return;
                            }

                            var reader = new FileReader();
                            reader.onload = function (e) {
                                scope.contractStop.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                                scope.contractStop.Blob = "data:image/jpeg;base64," + scope.modelModal.TasvirBlobBase64;
                                scope.contractStop.FileName = file.name;
                                scope.contractStop.FileSize = file.size;
                                scope.contractStop.MimeType = file.rawFile.type;
                                scope.$$phase || scope.$apply();
                            }
                            reader.readAsDataURL(file.rawFile);
                        }
                    }

                    if (!scope.iscreated) {
                        scope.iscreated = true;

                        scope.comboItemContractStop = new HesabfaCombobox({
                            items: [],
                            containerEle: document.getElementById("comboItemContractStop"),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboSazeTemplate,
                            inputClass: "form-control input-sm input-grid-factor",
                            matchBy: "item.DetailAccount.Id",
                            displayProperty: "{Title}",
                            searchBy: ["Title", "DetailAccount.Code"],
                            onSelect: scope.itemSelect,
                            onNew: scope.newItem,
                            onDelete: scope.deleteItem,
                            divider: true
                        });

                        scope.comboNoeEjareContractStop = new HesabfaCombobox({
                            items: [],
                            containerEle: document.getElementById("comboNoeEjareContractStop"),
                            toggleBtn: true,
                            newBtn: true,
                            deleteBtn: true,
                            itemClass: "hesabfa-combobox-item",
                            activeItemClass: "hesabfa-combobox-activeitem",
                            itemTemplate: Hesabfa.comboNoeEjareTemplate,
                            inputClass: "form-control input-sm input-grid-factor",
                            matchBy: "ID",
                            displayProperty: "{Title}",
                            searchBy: ["Title"],
                            onSelect: scope.noeEjareSelect,
                            onNew: scope.newNoeEjare,
                            // onDelete: scope.deleteItem,
                            divider: true
                        });
                    }




                    //if (!scope.reservation) {
                    scope.getNewReservationObject();
                    //} else {
                    //    scope.getNewReservationObject(scope.reservation.ID);
                    //}
                    $('#editContractStopModal').modal({ keyboard: false }, 'show');
                    // $("#editReservationModal .modal-dialog").draggable({ handle: ".modal-header" });
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initEditReservation();
                    scope.open = false;
                }, true);
                scope.submitReservation = function () {
                    scope.saveReservation();
                };
                scope.getNewReservationObject = function () {
                    scope.calling = true;

                    var model = {
                        isStopGroup: scope.isstopgroup,
                        sazeID: scope.sazeid,
                        contractID: scope.contractid
                    };

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(model),
                        url: "/app/api/Contract/loadStopContractData",
                        contentType: "application/json"
                    }).done(function (res) {

                        scope.calling = false;
                        scope.contractStop = res.data.contractStop;

                        //$('#fileShow').attr({ src: scope.contractStop.TasvirBlobBase64 });

                        scope.$apply();

                        var hideDiv = document.getElementById('hideme2');

                        if (!$('#comboItemContractStop').length) {
                            var comboItemContractStop = document.createElement('div');

                            comboItemContractStop.setAttribute("id", "comboItemContractStop");

                            $compile(comboItemContractStop)(scope);
                            hideDiv.appendChild(comboItemContractStop);

                            scope.comboItemContractStop = new HesabfaCombobox({
                                items: [],
                                containerEle: document.getElementById("comboItemContractStop"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboSazeTemplate,
                                inputClass: "form-control input-sm input-grid-factor",
                                matchBy: "item.DetailAccount.Id",
                                displayProperty: "{Title}",
                                searchBy: ["Title", "DetailAccount.Code"],
                                onSelect: scope.itemSelect,
                                onNew: scope.newItem,
                                onDelete: scope.deleteItem,
                                divider: true
                            });
                        }

                        if (!$('#inputStartDate2').length) {
                            var inputStartDate2 = document.createElement('input');

                            inputStartDate2.setAttribute("date-picker", "");
                            inputStartDate2.setAttribute("display-numbers", "");
                            inputStartDate2.setAttribute("ng-model", "actRow2.DisplayStartDate");
                            inputStartDate2.setAttribute("data-toggle", "popover");
                            inputStartDate2.setAttribute("data-placement", "top");
                            inputStartDate2.setAttribute("data-trigger", "hover");
                            inputStartDate2.setAttribute("data-content", "تاریخ شروع اکران...");
                            inputStartDate2.setAttribute("type", "text");
                            inputStartDate2.setAttribute("data-original-title", "");
                            inputStartDate2.setAttribute("title", "");
                            inputStartDate2.setAttribute("aria-describedby", "popover216080213");
                            inputStartDate2.setAttribute("class", "pdate form-control input-sm ng-valid ng-pristine ng-not-empty ng-touched");
                            inputStartDate2.setAttribute("id", "inputStartDate2");

                            $compile(inputStartDate2)(scope);
                            hideDiv.appendChild(inputStartDate2);
                        }

                        if (!$('#inputQuantity2').length) {
                            var inputQuantity2 = document.createElement('input');

                            inputQuantity2.setAttribute("display-numbers", "");
                            inputQuantity2.setAttribute("keyboard-filter", "float");
                            inputQuantity2.setAttribute("ng-model", "actRow2.Quantity");
                            inputQuantity2.setAttribute("data-toggle", "popover");
                            inputQuantity2.setAttribute("data-placement", "top");
                            inputQuantity2.setAttribute("data-trigger", "hover");
                            inputQuantity2.setAttribute("data-content", "مدت اجاره ...");
                            inputQuantity2.setAttribute("type", "text");
                            inputQuantity2.setAttribute("class", "input-grid-factor input-sm");
                            inputQuantity2.setAttribute("id", "inputQuantity2");

                            $compile(inputQuantity2)(scope);
                            hideDiv.appendChild(inputQuantity2);
                        }
                        if (!$('#comboNoeEjareContractStop').length) {
                            var comboNoeEjareContractStop = document.createElement('div');

                            comboNoeEjareContractStop.setAttribute("id", "comboNoeEjareContractStop");

                            $compile(comboNoeEjareContractStop)(scope);
                            hideDiv.appendChild(comboNoeEjareContractStop);

                            scope.comboNoeEjareContractStop = new HesabfaCombobox({
                                items: [],
                                containerEle: document.getElementById("comboNoeEjareContractStop"),
                                toggleBtn: true,
                                newBtn: true,
                                deleteBtn: true,
                                itemClass: "hesabfa-combobox-item",
                                activeItemClass: "hesabfa-combobox-activeitem",
                                itemTemplate: Hesabfa.comboNoeEjareTemplate,
                                inputClass: "form-control input-sm input-grid-factor",
                                matchBy: "ID",
                                displayProperty: "{Title}",
                                searchBy: ["Title"],
                                onSelect: scope.noeEjareSelect,
                                onNew: scope.newNoeEjare,
                                // onDelete: scope.deleteItem,
                                divider: true
                            });
                        }

                        scope.comboItemContractStop.items = res.data.items;
                        scope.comboNoeEjareContractStop.items = res.data.noeEjares;
                    });




                };
                scope.getReservation = function (id) {
                    scope.calling = true;

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/Reservation/GetReservationById",
                        contentType: "application/json"
                    }).done(function (res) {
                        var reservation = res.data;
                        scope.calling = false;
                        scope.reservation = reservation;
                        scope.$apply();
                    }).fail(function (error) {
                        window.location = '/error.html';
                    });
                };
                scope.readMultipleFiles = function (file) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        // bind the file content
                        $('#fileShow').attr({ src: e.target.result });

                        scope.contractStop.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                        //scope.contractStop.Blob = "data:image/jpeg;base64," + scope.contractStop.TasvirBlobBase64;
                        scope.contractStop.FileName = file.name;
                        scope.contractStop.FileSize = file.size;
                        scope.contractStop.MimeType = file.rawFile.type;
                        scope.$$phase || scope.$apply();
                    };
                    reader.readAsDataURL(file.rawFile);
                };
                scope.onSelect = function (e) {
                    $.each(e.files, function (index, value) {
                        var ok = value.extension === ".jpg"
                            || value.extension === ".JPEG"
                            || value.extension === ".jpg"
                            || value.extension === ".jpeg";

                        if (!ok) {
                            e.preventDefault();
                            messageService.error("فقط تصویر با فرمت jpg  قابل قبول است.");
                        }
                        else if (value.size > 204800) {
                            e.preventDefault();
                            messageService.error("حداکثر سایز مجاز برای تصویر 200KB میباشد.");
                        }
                        else {
                            scope.readMultipleFiles(value);
                        }
                    });
                };
                scope.saveReservation = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.calling = true;
                    scope.contractStop.ContractStopDetails[0].DisplayStartDate = scope.actRow2.DisplayStartDate;
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(scope.contractStop),
                        url: "/app/api/Contract/SaveContractStops",
                        contentType: "application/json"
                    }).done(function (res) {

                        scope.calling = false;


                        if (res.resultCode === 1) {
                            alertbox({ content: res.data, type: "error" });
                        }
                        else {
                            var savedItem = res.data;
                            scope.contractStop = savedItem;
                            $('#editContractStopModal').modal('hide');
                        }

                        scope.$apply();
                    }).fail(function (error) {
                        scope.calling = false;
                        scope.alertBoxVisible = true;
                        scope.alertMessage = error;
                        scope.$apply();
                    });
                };

                scope.moveRowEditor = function (index, $event) {
                    if (scope.actRow2 && scope.actRow2.RowNumber === index) return;
                    scope.actRow2 = scope.contractStop.ContractStopDetails[index];
                    $("#comboItemContractStop").prependTo("#tdItem2" + index);
                    $("#inputStartDate2").prependTo("#tdDate2" + index);
                    $("#inputQuantity2").prependTo("#tdStock2" + index);
                    $("#comboNoeEjareContractStop").prependTo("#tdNoeEjare2" + index);

                    if (scope.actRow2) {
                        scope.comboItemContractStop.setSelected(scope.actRow2.Saze);
                        scope.comboNoeEjareContractStop.setSelected(scope.actRow2.NoeEjare);
                    }

                    //applyScope(scope);
                    if ($event) $('#' + $event.currentTarget.id + " input").select();
                };
                scope.outRowEditor = function () {
                    $("#comboItemContractStop").prependTo("#hideme2");
                    $("#inputStartDate2").prependTo("#hideme2");
                    $("#inputQuantity2").prependTo("#hideme2");
                    $("#comboNoeEjareContractStop").prependTo("#hideme2");

                };

                scope.itemSelect = function (item, selectedBy) {
                    scope.activeIvoiceItem = scope.actRow2;


                    if (!item) {
                        scope.activeIvoiceItem.Saze = null;
                        return false;
                    }

                    var items = scope.contractStop.ContractStopDetails;
                    for (var i = 0; i < items.length; i++) {
                        if (items[i].Saze && items[i].Saze.ID === item.ID && (items[i].RowNumber !== scope.actRow2.RowNumber)) {
                            items[i].Quantity++;

                            //applyScope(scope);

                            DevExpress.ui.notify(Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"), "success", 3000);
                            scope.comboItemContractStop.setSelected(null);

                            return false;
                        }
                    }


                    scope.activeIvoiceItem.Saze = item;

                    //برای انتخاب نوع اجاره
                    scope.comboNoeEjareContractStop.setSelected(item.NoeEjare);
                    scope.activeIvoiceItem.NoeEjare = item.NoeEjare;


                    if (scope.activeIvoiceItem.Quantity === 0) scope.activeIvoiceItem.Quantity = 1;


                    //scope.$apply();
                    if (selectedBy === 'enter') {
                        if (scope.activeIvoiceItem.RowNumber === scope.contractStop.ContractStopDetails.length)
                            scope.addRow();
                        scope.moveRowEditor(scope.actRow2.RowNumber);
                        //            $(':input:eq(' + ($(':input').index(this)) + ')').focus();
                        //            $('#tdItem' + activeRow.RowNumber + ' input').focus();
                        $('#itemInputitemSelectInput').select();
                        $('#itemInputitemSelectInput').val('');
                        $('#itemInputitemSelectInput').focus();
                    }
                    return false;
                };

                scope.deleteItem = function () {
                    scope.actRow2.Saze = null;
                    scope.actRow2.Quantity = 0;
                };

                scope.noeEjareSelect = function (item, selectedBy) {

                    scope.activeIvoiceItem = scope.actRow2;

                    scope.activeIvoiceItem.NoeEjare = item;


                    //scope.$apply();


                };

                scope.addRow = function (n) {
                    if (!n) n = 1;
                    for (var i = 0; i < n; i++) {
                        var newInvoiceItem = {};
                        angular.copy(scope.invoiceItem, newInvoiceItem);
                        newInvoiceItem.RowNumber = scope.contractStop.ContractStopDetails.length;// + 1;
                        scope.contractStop.ContractStopDetails.push(newInvoiceItem);
                    }
                    //scope.$apply();
                };

                scope.deleteRow = function (invoiceItem) {
                    if (scope.contractStop.ContractStopDetails.length === 1) return;   // if only one remaind, do not delete
                    // prevent removing autocompletes from page
                    if (scope.actRow2.RowNumber === invoiceItem.RowNumber) {
                        if (invoiceItem.RowNumber > 1)
                            scope.moveRowEditor(invoiceItem.RowNumber - 2);
                        else
                            scope.moveRowEditor(invoiceItem.RowNumber);
                    }

                    // remove contract item from ContractStopDetails list
                    var items = scope.contractStop.ContractStopDetails;
                    findAndRemoveByPropertyValue(items, 'RowNumber', invoiceItem.RowNumber);
                    for (var i = 0; i < items.length; i++)  // add row numbers again
                        items[i].RowNumber = i + 1;

                    //scope.$apply();
                };

                scope.contactSelect = function (contact) {
                    if (contact) {
                        scope.contractStop.Contact = contact;
                    }
                };

                scope.addPhoto = function (id) {
                    var reqModel = {
                        DocTypeID: id,
                        RequestID: -1
                    };

                    openMoshaverTasvirInfoModal(reqModel);
                };

                //scope.fileOptions =  {
                //    localization: {
                //        select: "انتخاب تصویر"
                //    },
                //    multiple: false,
                //    showFileList: false,
                //    enabled: true,
                //    select: function (files) {
                //        var file = files.files[0];
                //        var ok = file.extension.toLowerCase() == ".jpg" || file.extension.toLowerCase() == ".jpeg";
                //        if (!ok) {
                //            messageService.error("فقط تصویر با فرمت jpg  قابل قبول است.");
                //            return;
                //        }
                //        else if (file.size > 204800) {
                //            messageService.error("حداکثر سایز مجاز برای تصویر 200kb میباشد.");
                //            return;
                //        }

                //        var reader = new FileReader();
                //        reader.onload = function (e) {
                //            scope.contractStop.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                //            scope.contractStop.Blob = "data:image/jpeg;base64," + scope.contractStop.TasvirBlobBase64;
                //            scope.contractStop.FileName = file.name;
                //            scope.contractStop.FileSize = file.size;
                //            scope.contractStop.MimeType = file.rawFile.type;
                //            scope.$$phase || scope.$apply();
                //        };
                //        reader.readAsDataURL(file.rawFile);
                //    }
                //}
            }
        };
    }]);
});