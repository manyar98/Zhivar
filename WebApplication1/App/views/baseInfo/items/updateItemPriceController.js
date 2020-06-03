define(['application', 'combo', 'scrollbar', 'helper', 'editCash', 'editBank', 'dx', 'roweditor', 'gridHelper'], function (app) {
    app.register.controller('updateItemPriceController', ['$scope','$rootScope', '$location', '$compile',
        function ($scope, $rootScope, $location, $compile) {

    var grid;
    var percentPopup, pricePopup;
    var itemsToUpdate = {};
    $scope.init = function () {
        $rootScope.pageTitle("ثبت سریع قیمت کالاها و خدمات");
        $('#businessNav').show();
        $scope.type = "all";
        $scope.nodeId = 0;
        $scope.percent = 0;
        $scope.price = 0;
        $scope.percentType = "افزایش";
        $scope.percentApply = "قیمت فروش";
        $scope.round = "-";

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: "grid-update-price-list",
            selection: { mode: "multiple" },
            editing: { mode: "cell", allowUpdating: true },
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
            columns: [
				{ caption: 'کـد', dataField: 'DetailAccount.Code', width: 80, printWidth: 1.5, allowEditing: false },
				{ caption: 'گروه', dataField: 'DetailAccount.Node.Name', printWidth: 5, allowEditing: false },
				{ caption: 'نام کالا یا خدمات', printWidth: 7, dataField: 'Name', allowEditing: false },
				{ caption: 'قیمت خرید', dataField: 'BuyPrice', dataType: 'number', format: '#', width: 120, printWidth: 2, printFormat: "#", allowEditing: true },
				{ caption: 'قیمت فروش', dataField: 'SellPrice', dataType: 'number', format: '#', width: 120, printWidth: 2, printFormat: "#", allowEditing: true }
            ],
            print: {
                fileName: "InventoryPriceList.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "فهرست قیمت کالاها و خدمات"
                }
            },
            onRowUpdated: function (e) {
                var item = e.key;
                itemsToUpdate[item.Id] = item;
            }
        });

        //$("#cmbPercentType").dxSelectBox({
        //    dataSource: ["Item 1", "Item 2", "Item 3"],
        //    searchEnabled: true,
        //    //rtlEnabled: true,
        //    //items: ["افزایش", "کاهش"],
        //    //bindingOptions: {
        //    //    value: "percentType"
        //    //}
        //});

        $("#cmbPercentType").dxSelectBox({
            acceptCustomValue: false,
            accessKey: null,
            activeStateEnabled: true,
            dataSource: ["افزایش", "کاهش"],
            deferRendering: true,
            disabled: false,
            displayExpr: undefined,
            dropDownButtonTemplate: "dropDownButton",
            elementAttr: {},
            fieldTemplate: null,
            focusStateEnabled: true,
            grouped: false,
            groupTemplate: "group",
            height: undefined,
            hint: undefined,
            hoverStateEnabled: true,
            inputAttr: {},
            isValid: true,
            items: null,
            itemTemplate: "item",
            maxLength: null,
            minSearchLength: 0,
            name: "",
            noDataText: "No data to display",
            onChange: null,
            onClosed: null,
            onContentReady: null,
            onCopy: null,
            onCustomItemCreating: function (e) { if (!e.customItem) { e.customItem = e.text; } },
            onCut: null,
            onDisposing: null,
            onEnterKey: null,
            onFocusIn: null,
            onFocusOut: null,
            onInitialized: null,
            onInput: null,
            onItemClick: null,
            onKeyDown: null,
            onKeyPress: null,
            onKeyUp: null,
            onOpened: null,
            onOptionChanged: null,
            onPaste: null,
            onSelectionChanged: null,
            onValueChanged: null,
            opened: false,
            openOnFieldClick: true,
            placeholder: "Select",
            readOnly: false,
            rtlEnabled: false,
            searchEnabled: false,
            searchExpr: null,
            searchMode: "contains",
            searchTimeout: 500,
            showClearButton: false,
            showDataBeforeSearch: false,
            showDropDownButton: true,
            showSelectionControls: false,
            spellcheck: false,
            stylingMode: "outlined",
            tabIndex: 0,
            validationError: undefined,
            validationMessageMode: "auto",
            value: null,
            valueChangeEvent: "change",
            valueExpr: this,
            visible: true,
            width: undefined
        });
 

        //$scope.cmbPercentType = {
        //    rtlEnabled: true,
        //    items: ["افزایش", "کاهش"],
        //    bindingOptions: {
        //        value: "percentType"
        //    }
        //};

        $scope.cmbPercentApply = {
            rtlEnabled: true,
            items: ["قیمت فروش", "قیمت خرید"],
            bindingOptions: {
                value: "percentApply"
            }
        };

        $scope.cmbPriceType = {
            rtlEnabled: true,
            items: ["افزایش", "کاهش"],
            bindingOptions: {
                value: "percentType"
            }
        };

        $scope.cmbPriceApply = {
            rtlEnabled: true,
            items: ["قیمت فروش", "قیمت خرید"],
            bindingOptions: {
                value: "percentApply"
            }
        };

        $scope.cmbRound = {
            rtlEnabled: true,
            items: ["-", "1 رقم", "2 رقم", "3 رقم", "4 رقم", "5 رقم"],
            bindingOptions: {
                value: "round"
            }
        };

        percentPopup = $("<div/>").appendTo("body").dxPopup({
            rtlEnabled: true,
            showTitle: true,
            height: 'auto',
            width: 300,
            title: "افزایش یا کاهش قیمت",
            dragEnabled: true,
            closeOnOutsideClick: true,
            contentTemplate: function (contentElement) {
                var html = $('#updatePercentPopup').html();
                $(html).appendTo(contentElement);
                $compile(contentElement.contents())($scope);
            }
        }).dxPopup("instance");

        pricePopup = $("<div/>").appendTo("body").dxPopup({
            rtlEnabled: true,
            showTitle: true,
            height: 'auto',
            width: 300,
            title: "افزایش یا کاهش قیمت",
            dragEnabled: true,
            closeOnOutsideClick: true,
            contentTemplate: function (contentElement) {
                var html = $('#updatePricePopup').html();
                $(html).appendTo(contentElement);
                $compile(contentElement.contents())($scope);
            }
        }).dxPopup("instance");

        $scope.getItems();

        $('#tabs a').click(function (e) {
            e.preventDefault();
            $(this).tab('show');
        });

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
            url: "/app/api/Item/GetAllByOrganId",
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
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.selectTab = function (tab) {
        if (tab === "all") {
            grid.clearFilter();
        }
        else if (tab === "product") {
            grid.filter(["ItemType", "=", 0]);
        }
        else if (tab === "service") {
            grid.filter(["ItemType", "=", 1]);
        }
        $scope.type = tab;
        applyScope($scope);
    };
    $scope.updateByPercent = function () {
        var items = grid.getSelectedRowsData();
        if (items.length === 0) {
            alertbox({ content: "ابتدا يك يا چند کالا را انتخاب كنيد" });
            return;
        }
        percentPopup.show();
    }
    $scope.updateByPrice = function () {
        var items = grid.getSelectedRowsData();
        if (items.length === 0) {
            alertbox({ content: "ابتدا يك يا چند کالا را انتخاب كنيد" });
            return;
        }
        pricePopup.show();
    }
    $scope.doUpdate = function (type) {
        percentPopup.hide();
        pricePopup.hide();
        var items = grid.getSelectedRowsData();
        if (type === 0) {
            if ($scope.percent < 0 || $scope.percent >= 1000) {
                alertbox({ content: "درصد غیرمجاز است." });
                return;
            }
        }
        if (type === 1) {
            if ($scope.price < 0 || $scope.price >= 1000000000) {
                alertbox({ content: "مبلغ غیرمجاز است." });
                return;
            }
        }
        var p = Number(type === 0 ? $scope.percent : $scope.price);
        if ($scope.percentType !== "افزایش")
            p = -p;
        for (var i = 0; i < items.length; i++) {
            var item = items[i];
            var price = $scope.percentApply === "قیمت فروش" ? item.SellPrice : item.BuyPrice;
            if (type === 0)
                price = Math.max(0, price * p / 100 + price);
            if (type === 1)
                price = Math.max(0, price + p);

            price = Math.round(price * 100) / 100;
            if (!Hesabfa.isDecimalCurrency())
                price = Math.round(price);
            if ($scope.round === "1 رقم")
                price = Math.round(price / 10) * 10;
            if ($scope.round === "2 رقم")
                price = Math.round(price / 100) * 100;
            if ($scope.round === "3 رقم")
                price = Math.round(price / 1000) * 1000;
            if ($scope.round === "4 رقم")
                price = Math.round(price / 10000) * 10000;
            if ($scope.round === "5 رقم")
                price = Math.round(price / 100000) * 100000;

            if ($scope.percentApply === "قیمت فروش")
                item.SellPrice = price;
            else
                item.BuyPrice = price;
            itemsToUpdate[item.Id] = item;
        }
        grid.refresh();
    }
    $scope.save = function () {
        if ($scope.calling) return;
        $scope.calling = true;
        var list = [];
        for (var i in itemsToUpdate) {
            list.push(itemsToUpdate[i]);
        }
        callws(DefaultUrl.MainWebService + "UpdateItemPrice", { items: list })
            .success(function (data) {
                $scope.calling = false;
                itemsToUpdate = {};
                DevExpress.ui.notify("تغییرات ذخیره شد", "success", 3000);
                applyScope($scope);
            }).fail(function (error) {
                $scope.calling = false;
                $scope.$apply();
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    }
    
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});