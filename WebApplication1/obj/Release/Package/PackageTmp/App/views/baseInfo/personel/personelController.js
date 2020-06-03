define(['application', 'combo', 'scrollbar', 'helper', 'editPersonel', 'editContact', 'nodeSelect', 'roweditor', 'gridHelper', 'dx', 'dataService'], function (app) {
    app.register.controller('personelController', ['$scope', '$rootScope', '$location', '$compile', '$state', 'dataService',
        function ($scope, $rootScope, $location, $compile, $state, dataService) {
            var grid;
            $scope.init = function () {
                $rootScope.pageTitle("پرسنل");
                $('#businessNav').show();

                $scope.nodeId = 0;
                $scope.iscreated = false;

                grid = dxGrid({
                    columnFixing: {
                        enabled: true
                    },
                    elementId: 'gridContainer',
                    layoutKeyName: "grid-inventory",
                    selection: { mode: "multiple" },
                    height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
                    columns: [
                      
                        { caption: 'نام', dataField: 'FirstName', width: 130, printWidth: 1.5, allowEditing: false },
                        { caption: 'نام خانوادگی', dataField: 'LastName', width: 130, printWidth: 5, allowEditing: false },
                        { caption: 'کد ملی', dataField: 'NationalCode', width: 130, printWidth: 5, allowEditing: false },
                        { caption: 'موبایل', dataField: 'MobileNo', width: 130, printWidth: 5, allowEditing: false },
                        { caption: 'سمت', dataField: 'OnvanPosition', width: 130, printWidth: 5, allowEditing: false },
                        { caption: 'از تاریخ', dataField: 'DisplayAzTarikh', width: 130, printWidth: 5, allowEditing: false },
                        {
                            caption: 'ویرایش', printVisible: false, dataField: '', allowEditing: false, cellTemplate: function (cellElement, cellInfo) {
                                cellElement.html("<span class='text-info txt-bold hand'>ویرایش</span>");
                            }
                        }
                    ],
                    print: {
                        fileName: "\PersonelList.pdf",
                        rowColumnWidth: 1.5,
                        page: {
                            landscape: false
                        },
                        header: {
                            title1: $rootScope.currentBusiness.Name,
                            title2: "فهرست پرسنل"
                        }
                    },
              
                    onCellClick: function (item) {
                        if (!item.column || !item.data) return;
                        if (item.column.caption === "ویرایش")
                            $scope.editItem(item.data);
                    }
                });

                $scope.getItems();

                $scope.positions = [];

                $scope.setPosition(null);
  

                $scope.startNodeSelect = true;
                applyScope($scope);
                $('[data-toggle="popover"]').popover();
            };
            $scope.getItems = function () {
                if ($scope.pageLoading) return;
                $scope.pageLoading = true;
                grid.beginCustomLoading();

                $.ajax({
                    type: "POST",
                    data: JSON.stringify("all"),
                    url: "/app/api/Personel/GetAllByOrganId",
                    contentType: "application/json"
                }).done(function (res) {
                    var data = res.data;
                    $scope.pageLoading = false;
                    grid.endCustomLoading();
                    $scope.items = data;
                    grid.fill(data);
                    applyScope($scope);
                }).fail(function (error) {
                    $scope.pageLoading = false;
                    grid.endCustomLoading();
                    $scope.$apply();
                    if ($scope.accessError(error)) return;
                    alertbox({ content: "خطای رخ داده است.", type: "error" });
                });
            };
          
            $scope.addItem = function () {
                $scope.alert = false;
                $scope.item = null;
                $scope.editItemModal = true;
            };
            $scope.editItem = function (item) {
                $scope.alert = false;
                $scope.item = item;
                $scope.editItemModal = true;
                applyScope($scope);
            };
            $scope.getEditedItem = function (item) {
                if (!item) return;
                $scope.alert = true;
                $scope.alertType = 'success';
                //		item.Name = item.Name;
                //		item.NodeName = item.DetailAccount.Node.Name;
                var finded = findById($scope.items, item.Id);
                if (finded) {
                    findAndReplace(grid._options.dataSource, item.Id, item);
                    grid.refresh();
                    DevExpress.ui.notify("تغییرات آیتم ذخیره شد", "success", 3000);
                }
                else {
                    $scope.items.push(item);
                    grid.refresh();
                    DevExpress.ui.notify("آیتم جدید ذخیره شد", "success", 3000);
                }
                $scope.editItemModal = false;
                $scope.$apply();
            };
    
            $scope.deleteItems = function () {
                var notDeletedCount = 0;
                function deleteItems(items, startIndex, count, successCallback, errorCallback) {
                    var arr = [];
                    for (var i = startIndex; i < items.length; i++) {
                        arr.push(items[i]);
                        if (arr.length === count)
                            break;
                    }
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(arr),
                        url: "/app/api/Personel/DeletePersonel",
                        contentType: "application/json"
                    }).done(function (res) {

                        $scope.calling = false;
                        if (res.resultCode === 1) {
                            alertbox({ content: res.data, type: "warning" });
                            return;
                        }

                     
                        notDeletedCount += (arr.length - res.data.length);
                        for (var i = 0; i < res.data.length; i++)
                            findAndRemove(grid._options.dataSource, res.data[i]);
                        grid.refresh();
                        successCallback();
                    }).fail(function (error) {
                        errorCallback(error);
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                    });

                }

                function startDelete(items, start, count) {
                    if (start >= items.length) {
                        var deletedCount = items.length - notDeletedCount;
                        $scope.calling = false;
                        var strMessage = deletedCount > 1 ? "کارمندان " : "کارمند ";
                        strMessage += "انتخاب شده حذف " + (deletedCount > 1 ? " شدند." : " شد.");
                        if (notDeletedCount > 0) {
                            strMessage += "<br />تعداد " + notDeletedCount + " آیتم بعلت ثبت تراکنش و کنترل موجودی حذف نشد.";
                            alertbox({ content: strMessage, type: "warning" });
                        }
                        else
                            DevExpress.ui.notify(strMessage, "success", 3000);
                        $scope.$apply();
                        return;
                    }
                    deleteItems(items, start, count, function () {
                        setTimeout(function () {
                            startDelete(items, start + count, count);
                        }, 0);
                    }, function (error) {
                        alertbox({ content: error, type: "warning" });
                        $scope.calling = false;
                        $scope.$apply();
                    });
                }
                var items = grid.getSelectedRowsData();
                if (items.length === 0) {
                    alertbox({ content: "ابتدا يك يا چند کارمند را انتخاب كنيد" });
                    return;
                }
                questionbox({
                    content: "آيا از حذف کارمندان انتخاب شده مطمئن هستيد؟",
                    onBtn1Click: function () {
                        if ($scope.calling) return;
                        $scope.calling = true;
                        $scope.$apply();
                        startDelete(items, 0, 200);
                    }
                });
            };
            $scope.search = function () {
                $scope.grid.search($scope.searchValue, $scope);
            };
            $scope.nodeSelect = function (selectedNode) {
                if (!selectedNode) return;
                var data = $scope.items;
                var filteredData = [];
                for (var i = 0; i < data.length; i++) {
                    var c = data[i];
                    if (c.DetailAccount.Node.Id === selectedNode.Id)
                        filteredData.push(c);
                }
                $scope.grid.data = filteredData;
                $scope.grid.init();
            };
            $scope.nodeSelectToItems = function (selectedNode) {
                if (!selectedNode) return;
                if ($scope.calling) return;
                var selectedItems = $scope.grid.getSelectedItems();
                if (selectedItems.length === 0) {
                    alertBoxVisible({ content: "هیچ آیتمی انتخاب نشده است." });
                    return;
                }
                var selectedItemsIds = "";
                for (var i = 0; i < selectedItems.length; i++) {
                    selectedItemsIds += selectedItems[i].Id + ",";
                }
                selectedItemsIds = selectedItemsIds.replace(/,\s*$/, "");
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + 'ChangeItemsCategory', { items: selectedItemsIds, nodeId: selectedNode.Id })
                    .success(function () {
                        $scope.calling = false;
                        $scope.grid.goToPage($scope.grid.currentPage);
                        applyScope($scope);
                    })
                    .fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        $scope.alert = true;
                        $scope.alertMessage = error;
                        $scope.alertType = 'danger';
                        applyScope($scope);
                    })
                    .loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
        
     
   
            $scope.activeClass = function (n) {
                $("#dropdownSearch li").removeClass("active");
                $("#dropdownSearch li").eq(n).addClass("active");
                if (n === 0) { $scope.searchBy = 'name'; $scope.placeHolderStr = "جستجو با کد یا نام..."; $("#searchInput").select(); }
                else if (n === 1) { $scope.searchBy = 'barcode'; $scope.placeHolderStr = "جستجو با بارکد..."; $("#barcodeScan").select(); }
                $rootScope.setUISetting("itemsActiveSearch", n + "");
            };
            $scope.filterDialog = function () {
                $('#modalFilterItems').modal('show');
            };
            $scope.addFilters = function () {
                var data = $scope.items;
                var filteredItems = [];
                if ($scope.itemCodeFrom && $scope.itemCodeTo && $scope.itemCodeFrom !== "" && $scope.itemCodeTo !== "") {
                    var codeFrom = parseInt($scope.itemCodeFrom);
                    var codeTo = parseInt($scope.itemCodeTo);
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        var itemCode = parseInt(item.DetailAccount.Code);
                        if (itemCode >= codeFrom && itemCode <= codeTo)
                            filteredItems.push(item);
                    }
                    $scope.grid.data = filteredItems;
                    $scope.grid.init();
                }
                if ($scope.itemBarcodeFrom && $scope.itemBarcodeTo && $scope.itemBarcodeFrom !== "" && $scope.itemBarcodeTo !== "") {
                    var barcodeFrom = parseInt($scope.itemBarcodeFrom);
                    var barcodeTo = parseInt($scope.itemBarcodeTo);
                    for (var i = 0; i < data.length; i++) {
                        var item = data[i];
                        var itemBarcode = parseInt(item.Barcode);
                        if (itemBarcode >= barcodeFrom && itemBarcode <= barcodeTo)
                            filteredItems.push(item);
                    }
                    $scope.grid.data = filteredItems;
                    $scope.grid.init();
                }
                if ($scope.stockFilterCount && $scope.stockFilterCount !== "") {
                    if ($scope.stockFilterCondition === "bigger") {
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            if (item.Stock > parseInt($scope.stockFilterCount) && item.ItemType === 0)
                                filteredItems.push(item);
                        }
                    }
                    else if ($scope.stockFilterCondition === "smaller") {
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            if (item.Stock < parseInt($scope.stockFilterCount) && item.ItemType === 0)
                                filteredItems.push(item);
                        }
                    }
                    else if ($scope.stockFilterCondition === "equal") {
                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            if (item.Stock === parseInt($scope.stockFilterCount) && item.ItemType === 0)
                                filteredItems.push(item);
                        }
                    }
                    $scope.grid.data = filteredItems;
                    $scope.grid.init();
                }
                $('#modalFilterItems').modal('hide');
                $scope.otherFilters = "true";
            };
            $scope.removeFilters = function () {
                $scope.otherFilters = "";
                $scope.itemCodeFrom = "";
                $scope.itemCodeTo = "";
                $scope.itemBarcodeFrom = "";
                $scope.itemBarcodeTo = "";
                $scope.stockFilterCount = "";
                $('#modalFilterItems').modal('hide');
                $scope.grid.data = $scope.items;
                $scope.grid.init();
            };
            $scope.addOrRemoveSelectedItemsToFilter = function (removeIds) {
                if (removeIds && $scope.otherFilters !== "" && $scope.otherFilters.indexOf("id,") > -1) {
                    var splited = $scope.otherFilters.split("{~}");
                    $scope.otherFilters = "";
                    for (var j = 0; j < splited.length; j++) {
                        if (splited[j].indexOf("id,") > -1) continue;
                        $scope.otherFilters = splited[j] + "{~}";
                    }
                    return;
                }
                else if (removeIds) return;

                var selectedItems = $scope.grid.getSelectedItems();
                if (selectedItems.length > 0) {
                    var strIds = "id,";
                    for (var i = 0; i < selectedItems.length; i++) {
                        strIds += selectedItems[i].Id + ",";
                    }
                    strIds = strIds.slice(0, -1);
                    $scope.otherFilters += strIds + "{~}";
                } else $scope.otherFilters = $scope.otherFilters.replace("id,", ",");
            };
    
            $scope.printLabels = function () {
                $scope.grid4.saveEditData();
                $scope.printPopup.hide();

                setTimeout(function () {
                    var items = [];
                    for (var i = 0; i < $scope.label.items.length; i++) {
                        var item = $scope.label.items[i];
                        for (var j = 0; j < item.count; j++) {
                            items.push(item);
                        }

                    }

                    createCookie("lpt", $scope.label.title, 1000);
                    createCookie("lpw", $scope.label.width, 1000);
                    createCookie("lph", $scope.label.height, 1000);
                    createCookie("lin", $scope.label.showItemName, 1000);
                    createCookie("lic", $scope.label.showItemCode, 1000);
                    createCookie("lbb", $scope.label.showBarcode, 1000);
                    createCookie("lpp", $scope.label.showPrice, 1000);
                    createCookie("lpb", $scope.label.showBorder, 1000);
                    createCookie("ltt", $scope.label.printType, 1000);

                    pdfBarcodeLabelNew($scope.label, items, $scope.getCurrency());
                }, 100);
            };

            $scope.print = function () {
                grid.print();
            };
            $scope.pdf = function () {
                grid.pdf();
            };
        
            $scope.setPosition = function (positionId) {
                dataService.getPagedData('/app/api/Personel/GetPositionByOrganizationId').then(function (data) {
                    $scope.positions = data.Positions;
                    //scope.model.RoleID = positionId;

                });
            }

            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});