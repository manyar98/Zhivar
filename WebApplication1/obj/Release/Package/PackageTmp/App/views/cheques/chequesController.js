var gridPopupChequeRelatedDocs;
define(['application', 'digitGroup', 'combo', 'scrollbar', 'number', 'dx', 'roweditor', 'helper', 'gridHelper', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker'], function (app) {
    app.register.controller('chequesController', ['$scope','$rootScope','$stateParams',
        function ($scope,$rootScope,$stateParams) {

            var receiptDateObj = new AMIB.persianCalendar('receiptDate'); 

    var grid, grid4;
    $scope.init = function () {
        $rootScope.pageTitle("بارگیری چک ها...");
        $("#businessNav").show();

        var show = $stateParams.show; //  $scope.getRouteQuery($routeParams.params, "show");
        $scope.chequeNumber = $stateParams.number; //$scope.getRouteQuery($routeParams.params, "number");

        if (show && show === "receivables") {
            $scope.getCheques(0);
            $scope.type = 0;
            $scope.tab = null;
        }
        else if (show && show === "payables") {
            $scope.getCheques(1);
            $scope.type = 1;
        } else {
            $scope.getCheques(0);
            $scope.type = 0;
            $scope.tab = null;
        }

        $scope.ReferenceNumber = "";
        $scope.description = "";
        $scope.getCashes();
        $scope.getBanks();

        $scope.comboBank = new HesabfaCombobox({
            items: [],
            containerEle: document.getElementById("comboBank"),
            toggleBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: Hesabfa.comboBankTemplate,
            divider: true,
            matchBy: "bank.DetailAccount.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "Code"],
            onSelect: $scope.bankSelect
        });
        $scope.comboDepositBank = new HesabfaCombobox({
            items: [],
            containerEle: document.getElementById("comboDepositBank"),
            toggleBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: Hesabfa.comboBankTemplate,
            divider: true,
            matchBy: "bank.DetailAccount.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "Code"],
            onSelect: $scope.bankSelect
        });
        $scope.comboCash = new HesabfaCombobox({
            items: $scope.cashes,
            containerEle: document.getElementById("comboCash"),
            toggleBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: "<div> {DetailAccount.Code} &nbsp;-&nbsp; {Name} </div>",
            matchBy: "cash.DetailAccount.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "Code"],
            onSelect: $scope.cashSelect,
        });

        $("#tabs a").click(function (e) {
            e.preventDefault();
            $(this).tab("show");
        });
        var lookup = { dataSource: [{ key: 0, val: "عادی" }, { key: 1, val: "در جریان وصول" }, { key: 2, val: $scope.type === 0 ? "وصول شده" : "پاس شده" }, { key: 3, val: "برگشت خورده" }, { key: 4, val: "عودت شده" }, { key: 5, val: "خرج شده" }], valueExpr: 'key', displayExpr: 'val' };
        function getStatusIcon(cheque) {
            if (cheque.Status === 0) return "<span class='fa fa-check txt-silver pull-center'></span>"; 
            else if (cheque.Status === 1) return "<span class='fa fa-hourglass txt-silver pull-center'></span>";
            else if (cheque.Status === 2) return "<span class='fa fa-check text-success pull-center'></span>";
            else if (cheque.Status === 3) return "<span class='fa fa-reply red pull-center'></span>";
            else if (cheque.Status === 4) return "<span class='fa fa-reply blue pull-center'></span>";
            else if (cheque.Status === 5) return "<span class='fa fa-reply green pull-center'></span>";
            else return ""; 
         
        }
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
            elementId: 'gridContainer',
            layoutKeyName: "grid-cheque-" + show,
            selection: { mode: "multiple" },
            height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 100),
            columns: [
                {
                    caption: '',
                    width: 30,
                    dataField: '',
                    allowHeaderFiltering: false,
                    allowSorting: false,
                    cellTemplate: function (cellElement, cellInfo) {
                        cellElement.html(getStatusIcon(cellInfo.data));
                    },
                    printWidth: 3
                },
				{ caption: 'شماره چک', dataField: 'ChequeNumber', width: 105, printWidth: 4 },
				{ caption: 'مبلغ چک', dataField: 'Amount', dataType: "number", format: "#", width: 120, printWidth: 3, printFormat: "#" },
				{ caption: 'تاریخ چک', dataField: 'DisplayDate', width: 100, printWidth: 3 },
				{ caption: $scope.type === 0 ? 'دریافت از' : 'پرداخت به', dataField: 'Contact.Name', printWidth: 6 },
				{ caption: 'بانک', dataField: 'BankName', printWidth: 3 },
				{ caption: 'شعبه', dataField: 'BankBranch', printWidth: 3 },
			    {
			        caption: 'وضعیت', dataField: 'Status', width: 135, printWidth: 3, lookup: lookup,
			        printCalcValue: function (data) {
			            for (var l in lookup.dataSource) {
			                if (lookup.dataSource[l].key === data.Status)
			                    return lookup.dataSource[l].val;
			            }
			            return "";
			        }
			    },
				{ caption: 'بانک واگذار شده', dataField: 'DepositBankName', printWidth: 5 },
				{
				    caption: 'اسناد', dataField: '', printVisible: false, cellTemplate: function (cellElement, cellInfo) {
				        cellElement.html("<span class='text-info txt-bold hand'>اسناد</span>");
				    }
				}
            ],
            onCellClick: function (item) {
                if (!item || !item.column || !item.data) return;
                if (item.columnIndex === 10)
                    $scope.getRelatedCheques(item.data);
            },
            print: {
                fileName: "ChequeList.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: $scope.type === 0 ? "چک های دریافتی" : "چک های پرداختنی"
                }
            }
        });
        gridPopupChequeRelatedDocs = $('#gridPopup').dxPopup({
            rtlEnabled: true,
            showTitle: true,
            height: 'auto',
            title: "اسناد چک",
            dragEnabled: true,
            closeOnOutsideClick: true,
            contentTemplate: function (contentElement) {
                var el = $("<div>");
                var statusLookup = { dataSource: [{ key: 0, val: "پیش نویس" }, { key: 1, val: "تایید شده" }], valueExpr: 'key', displayExpr: 'val' };
                grid4 = dxGrid({
                    element: el,
                    height: 300,
                    columnAutoWidth: true,
                    columns: [
						{ caption: 'شماره سند', dataField: 'Number', dataType: "number", width: 90, printWidth: 3 },
						{ caption: 'عطف', dataField: 'Number2', dataType: "number", width: 90, printWidth: 2 },
						{ caption: 'تاریخ', dataField: 'DisplayDate', width: 90, printWidth: 3 },
						{ caption: 'شــرح', dataField: 'Description', printWidth: 7 },
					    {
					        caption: 'وضعیت', dataField: 'Status', width: 100, printWidth: 3, lookup: statusLookup,
					        printCalcValue: function (data) {
					            for (var l in statusLookup.dataSource) {
					                if (statusLookup.dataSource[l].key === data.Status)
					                    return statusLookup.dataSource[l].val;
					            }
					            return "";
					        }
					    },
					    {
					        caption: 'نوع', dataField: 'IsManual', width: 100, printWidth: 3, lookup: { dataSource: [{ key: true, val: "دستی" }, { key: false, val: "اتوماتیک" }], valueExpr: 'key', displayExpr: 'val' },
					        printCalcValue: function (data) {
					            return data.IsManual ? "دستی" : "اتوماتیک";
					        }
					    },
						{ caption: 'بدهکار', dataField: 'Debit', width: 120, printWidth: 3, dataType: "number", format: "#", printFormat: "#" },
						{ caption: 'بستانکار', dataField: 'Credit', width: 120, printWidth: 3, dataType: "number", format: "#", printFormat: "#" },
						{
						    caption: 'نمایش', dataField: '', width: 80, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
						        cellElement.html("<a href='javascript:void(0);' onclick='gridPopupChequeRelatedDocs.hide();angular.element(this).scope().viewDocumentModal(" + cellInfo.data.ID + ")' class='text-primary txt-bold'>نمایش</a>");
						    }
						}
                    ],
                    print: {
                        fileName: "RelatedDocs.pdf",
                        rowColumnWidth: 1.5,
                        page: {
                            landscape: false
                        },
                        header: {
                            title1: $rootScope.currentBusiness.Name,
                            title2: "اسناد چک"
                        }
                    }
                });
                contentElement.append(el);
            }
        }).dxPopup("instance");

        applyScope($scope);
    };
    $scope.getCheques = function (type) {
        $scope.loading = true;

        $.ajax({
            type: "POST",
            data: JSON.stringify(type),
            url: "/app/api/Cheque/GetChequesAndStats",
            contentType: "application/json"
        }).done(function (res) {
            var result = res;
                $scope.loading = false;
                $scope.cheques = result.data;
                grid.fill(result.data);
                $scope.overdueStat = result.overdueStat;
                $scope.inProgressStat = result.inProgressStat;
                //            	var filter = $scope.getRouteQuery($routeParams.params, "filter");
                //            	if (filter && filter === "overdue") {
                //            		$scope.tab = 1;
                //            		$("#tabs li:eq(2) a").tab("show");
                //            		grid.applyFilter("Overdue", true); $scope.grid.applySort();
                //            	}
                //            	else if (filter && filter === "inprogress") {
                //            		$scope.tab = 2;
                //            		$("#tabs li:eq(3) a").tab("show");
                //            		$scope.grid.applyFilter("Status", 1); $scope.grid.applySort();
                //            	}

                if ($scope.type === 0) $rootScope.pageTitle("چک های دریافتی");
                if ($scope.type === 1) $rootScope.pageTitle("چک های پرداختی");

                if ($scope.chequeNumber) {
                    grid.filter(["ChequeNumber", "=", $scope.chequeNumber]);
                    grid.selectAll();
                }

                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getRelatedCheques = function (cheque) {
        $scope.loading = true;

        $.ajax({
            type: "POST",
            data: JSON.stringify(cheque.ID),
            url: "/app/api/Document/GetChequeRelatedDocuments",
            contentType: "application/json"
        }).done(function (res) {
            var data = res.data;
       
                $scope.loading = false;
                gridPopupChequeRelatedDocs.show();
                grid4.fill(data);
                applyScope($scope);
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getCashes = function () {
        $.ajax({
            type: "POST",
            url: "/app/api/Cash/GetAllByOrganId",
            contentType: "application/json"
        }).done(function (res) {
            var cashes = res.data;
                $scope.cashes = cashes;
                $scope.comboCash.items = cashes;
                $scope.$apply();
                if ($scope.cashId) $scope.addCashPay();
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.getBanks = function () {

        $.ajax({
            type: "POST",
            url: "/app/api/Bank/GetAllByOrganId",
            contentType: "application/json"
        }).done(function (res) {
            var banks = res.data;
                $scope.banks = banks;
                $scope.comboBank.items = banks;
                $scope.comboDepositBank.items = banks;
                $scope.$apply();
                if ($scope.bankId) $scope.addBankPay();
            }).fail(function (error) {
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.bankSelect = function (bank) {
        $scope.selectedBankDetailAccount = bank.DetailAccount;
    };
    $scope.cashSelect = function (cash) {
        $scope.selectedCashDetailAccount = cash.DetailAccount;
    };
    $scope.selectTab = function (tab) {
        switch (tab) {
            case null, 'all':
                grid.clearFilter();
                break;
            case "normal":
                grid.filter(["Status", "=", 0]);
                break;
            case "overdue":
                grid.filter(["Overdue", "=", true], "and", ["Status", "=", 0]);
                break;
            case "inProgress":
                grid.filter(["Status", "=", 1]);
                break;
            case "passed":
                grid.filter(["Status", "=", 2]);
                break;
            case "sold":
                grid.filter(["Status", "=", 5]);
                break;
            default:
                grid.clearFilter();
                break;
        }
        applyScope($scope);
    };

    $scope.search = function () {
        var key = $scope.searchKey;
        if (!key || key === " ") {
            var activeTab = $("ul#tabs li.active").index() - 1;
            if (activeTab === -1) {
                $scope.grid.removeFilter();
                $scope.$apply();
            }
            else $scope.grid.applyFilter("Status", activeTab);
            return;
        }
        if (!isNaN(parseInt(key, 10)))
            $scope.grid.applyFilter("ChequeNumber", key, true, $scope, true);
        else
            $scope.grid.applyFilter("ContactName", key, true, $scope, true);
    };
    $scope.chequeReceipt = function () {
        if (!$scope.validateChequeSelection())
            return false;
        var cheque = grid.getSelectedRowsData()[0];
        if (cheque.Status === 2) {
            alertbox({ content: "این چک قبلاً پاس شده است" });
            return false;
        }
        $scope.Receive = "cash";
        $scope.modalAlert = false;
        if ($scope.type === 0)
            $scope.chequeReceiptType = cheque.Status === 0 ? "received" : "deposit";
        else {
            $scope.chequeReceiptType = "paid";
        }
        $scope.receiptDate = cheque.DisplayDate;
        $("#chequeReceiptModal").modal({ keyboard: false }, "show");
    };
    $scope.chequeDeposit = function () {
        if (!$scope.validateChequeSelection())
            return;
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalAlert = false;
        if ($scope.type === 0 && cheque.Status === 0)
            $("#chequeDepositModal").modal({ keyboard: false }, "show");
    };
    $scope.chequeReturn = function () {
        if (!$scope.validateChequeSelection())
            return;
        var cheque = grid.getSelectedRowsData()[0];
        if (cheque.Status === 2) {
            alertbox({ content: "این چک وصول شده است" });
            return;
        }
        if (cheque.Status === 4) {
            alertbox({ content: "این چک قبلاً عودت داده شده است" });
            return false;
        }
        $scope.modalAlert = false;
        $scope.chequeReturn = true;
        $scope.chequeNotPass = false;
        applyScope($scope);
        $("#chequeReturnModal").modal({ keyboard: false }, "show");
    };
    $scope.validateChequeSelection = function () {
        var selectedItems = grid.getSelectedRowsData();
        if (selectedItems.length === 0) {
            alertbox({ content: "هیچ چکی انتخاب نشده است" });
            return false;
        }
        if (selectedItems.length > 1) {
            alertbox({ content: "فقط یک چک را انتخاب کنید" });
            return false;
        }
        if (selectedItems[0].Status === 5) {
            alertbox({ content: "این چک قبلاً خرج شده است" });
            return false;
        }
        return true;
    };

    // وصول چک پرداختی
    $scope.submitPaidChequeReceipt = function () {
        if ($scope.modalCalling) return;
        // validate client side
        if (!$scope.receiptDate) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "تاریخ وصول چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalCalling = true;

        var model = {
            change: "PaidChequeReceipt",
            chequeId: cheque.ID,
            date: $scope.receiptDate,
            detailAccount: null,
            description: "",
            receiveType: "",
            reference: ""
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/Cheque/ChangeChequeStatus",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.modalCalling = false;
                $("#chequeReceiptModal").modal("hide");
                result.ContactName = result.Contact.Name;
                if (cheque.Overdue) $scope.overdueStat--;

                var finded = findById($scope.cheques, result.ID);
                if (finded) {
                    findAndReplace(grid._options.dataSource, result.ID, result);
                    grid.refresh();
                    DevExpress.ui.notify("وصول چک پرداختی با موفقیت ثبت شد", "success", 3000);
                }

                grid.clearSelection();
                applyScope($scope);
            }).fail(function (error) {
                $scope.modalCalling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                alertbox({ content: error, type: "warning" });
                applyScope($scope);
            });
    };
    // وصول چک دریافتی
    $scope.submitReceivedChequeReceipt = function () {
        if ($scope.modalCalling) return;

        // validate client side
        if (!$scope.receiptDate) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "تاریخ وصول چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        if ($scope.Receive === "") {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "نوع دریافت را مشخص کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        if ($scope.Receive === "cash" && $scope.selectedCashDetailAccount == null) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "صندوق را انتخاب کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        if ($scope.Receive === "bank" && $scope.selectedBankDetailAccount == null) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "بانک را انتخاب کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalCalling = true;

        var model = {
            change: "ReceivedChequeReceipt",
            chequeId: cheque.ID,
            date: $scope.receiptDate,
            detailAccount: $scope.Receive === "cash" ? $scope.selectedCashDetailAccount : $scope.selectedBankDetailAccount,
            description: "",
            receiveType: $scope.Receive,
            reference: ""
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/Cheque/ChangeChequeStatus",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;

            $scope.modalCalling = false;
            $("#chequeReceiptModal").modal("hide");
                result.ContactName = result.Contact.Name;
                result.DepositBankName = result.DepositBank ? result.DepositBank.Name : "";
                if (cheque.Overdue) $scope.overdueStat--;

                var finded = findById($scope.cheques, result.ID);
                if (finded) {
                    findAndReplace(grid._options.dataSource, result.ID, result);
                    grid.refresh();
                    DevExpress.ui.notify("وصول چک دریافتی با موفقیت ثبت شد", "success", 3000);
                }

                grid.clearSelection();
                applyScope($scope);
            }).fail(function (error) {
                $scope.modalCalling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                $scope.modalAlert = true;
                $scope.modalAlertMessage = error;
                $scope.modalAlertType = "danger";
                applyScope($scope);
            });
    };
    // وصول چک واگذار شده به بانک
    $scope.submitDepositChequeReceipt = function () {
        if ($scope.modalCalling) return;

        // validate client side
        if (!$scope.receiptDate) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "تاریخ وصول چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalCalling = true;

        var model = {
            change: "DepositChequeReceipt",
            chequeId: cheque.ID,
            date: $scope.receiptDate,
            detailAccount: null,
            description: "",
            receiveType: "",
            reference: ""
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/Cheque/ChangeChequeStatus",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.modalCalling = false;
                $("#chequeReceiptModal").modal("hide");
                result.ContactName = result.Contact.Name;
                result.DepositBankName = result.DepositBank ? result.DepositBank.Name : "";
                $scope.inProgressStat--;
                if (cheque.Overdue) $scope.overdueStat--;
                var finded = findById($scope.cheques, result.ID);
                if (finded) {
                    findAndReplace(grid._options.dataSource, result.ID, result);
                    grid.refresh();
                    DevExpress.ui.notify("وصول چک واگذار شده با موفقیت ثبت شد", "success", 3000);
                }

                grid.clearSelection();
                applyScope($scope);
            }).fail(function (error) {
                $scope.modalCalling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                $scope.modalAlert = true;
                $scope.modalAlertMessage = error;
                $scope.modalAlertType = "danger";
                applyScope($scope);
            });
    };
    // واگذاری چک به بانک
    $scope.submitChequeDeposit = function () {
        if ($scope.modalCalling) return;
        // validate client side
        if (!$scope.depositDate) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "تاریخ واگذاری چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        if ($scope.selectedBankDetailAccount == null) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "بانک را انتخاب کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalCalling = true;

        var model = {
            change: "ReceivedChequeDeposit",
            chequeId: cheque.ID,
            date: $scope.depositDate,
            detailAccount: $scope.selectedBankDetailAccount,
            description: "",
            receiveType: "",
            reference: $scope.ReferenceNumber
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/Cheque/ChangeChequeStatus",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.modalCalling = false;
                $("#chequeDepositModal").modal("hide");
                result.ContactName = result.Contact.Name;
                result.DepositBankName = result.DepositBank ? result.DepositBank.Name : "";
                $scope.inProgressStat++;
                if (cheque.Overdue) $scope.overdueStat--;
                var finded = findById($scope.cheques, result.ID);
                if (finded) {
                    findAndReplace(grid._options.dataSource, result.ID, result);
                    grid.refresh();
                    DevExpress.ui.notify("واگذاری چک به بانک با موفقیت ثبت شد", "success", 3000);
                }

                grid.clearSelection();
                applyScope($scope);
            }).fail(function (error) {
                $scope.modalCalling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                $scope.modalAlert = true;
                $scope.modalAlertMessage = error;
                $scope.modalAlertType = "danger";
                applyScope($scope);
            });
    };
    // عودت دادن چک پرداختی
    $scope.submitPaidChequeReturn = function () {
        if ($scope.modalCalling) return;
        // validate client side
        if (!$scope.returnDate) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "تاریخ عودت چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        if ($scope.description === "") {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "شرح عودت چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalCalling = true;

        var model = {
            change: "PaidChequeReturn",
            chequeId: cheque.ID,
            date: $scope.returnDate,
            detailAccount: null,
            description: $scope.description,
            receiveType: "",
            reference: ""
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/Cheque/ChangeChequeStatus",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.modalCalling = false;
                $("#chequeReturnModal").modal("hide");
                if (result.Contact) result.ContactName = result.Contact.Name;
                if (cheque.Overdue) $scope.overdueStat--;

                var finded = findById($scope.cheques, result.Id);
                if (finded) {
                    findAndReplace(grid._options.dataSource, result.Id, result);
                    grid.refresh();
                    DevExpress.ui.notify("عودت چک پرداختی با موفقیت ثبت شد", "success", 3000);
                }

                grid.clearSelection();
                applyScope($scope);
            }).fail(function (error) {
                $scope.modalCalling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                $scope.modalAlert = true;
                $scope.modalAlertMessage = error;
                $scope.modalAlertType = "danger";
                applyScope($scope);
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };
    // عودت دادن چک دریافتی
    $scope.submitReceivedChequeReturn = function () {
        if ($scope.modalCalling) return;
        // validate client side
        if (!$scope.returnDate) {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "تاریخ عودت چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        if ($scope.description === "") {
            $scope.modalAlert = true;
            $scope.modalAlertMessage = "شرح عودت چک را وارد کنید";
            $scope.modalAlertType = "warning";
            $scope.$apply();
            return;
        }
        var cheque = grid.getSelectedRowsData()[0];
        $scope.modalCalling = true;

        var model = {
            change: "ReceivedChequeReturn",
            chequeId: cheque.ID,
            date: $scope.returnDate,
            detailAccount: null,
            description: $scope.description,
            receiveType: "",
            reference: ""
        };

        $.ajax({
            type: "POST",
            data: JSON.stringify(model),
            url: "/app/api/Cheque/ChangeChequeStatus",
            contentType: "application/json"
        }).done(function (res) {
            var result = res.data;
                $scope.modalCalling = false;
                $("#chequeReturnModal").modal("hide");
                result.ContactName = result.Contact.Name;
                result.DepositBankName = result.DepositBank ? result.DepositBank.Name : "";
                if (cheque.Overdue) $scope.overdueStat--;

                var finded = findById($scope.cheques, result.ID);
                if (finded) {
                    findAndReplace(grid._options.dataSource, result.ID, result);
                    grid.refresh();
                    DevExpress.ui.notify("عودت چک دریافتی با موفقیت ثبت شد", "success", 3000);
                }

                grid.clearSelection();
                applyScope($scope);
            }).fail(function (error) {
                $scope.modalCalling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                $scope.modalAlert = true;
                $scope.modalAlertMessage = error;
                $scope.modalAlertType = "danger";
                applyScope($scope);
            });
    };
    // برگشت خوردن چک دریافتی
    $scope.submitChequeNotPass = function () { };
    // تغییر وضعیت چک پرداختی به پاس نشده
    $scope.chequeChangeToNotPass = function () {
        if (!$scope.validateChequeSelection())
            return false;
        var cheque = grid.getSelectedRowsData()[0];
        if (cheque.Status === 0) {
            alertbox({ content: "این چک در وضعیت پاس نشده قرار دارد" });
            return false;
        }
        if (cheque.Status !== 2) {
            alertbox({ content: "این چک تا کنون پاس نشده است" });
            return false;
        }

        questionbox({
            content: "آیا از تغییر وضعیت این چک به پاس نشده مطمئن هستید؟",
            onBtn1Click: function () {
                if ($scope.calling) return false;
                $scope.calling = true;

                var model = {
                    change: "PaidChequeToNotPass",
                    chequeId: cheque.ID,
                    date: "",
                    detailAccount: null,
                    description: "",
                    receiveType: "",
                    reference: ""
                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Cheque/ChangeChequeStatus",
                    contentType: "application/json"
                }).done(function (res) {
                    var result = res.data;
                        $scope.calling = false;
                        result.ContactName = result.Contact.Name;

                        var finded = findById($scope.cheques, result.ID);
                        if (finded) {
                            findAndReplace(grid._options.dataSource, result.ID, result);
                            grid.refresh();
                            DevExpress.ui.notify("تغییر وضعیت چک پرداختی به پاس نشده با موفقیت انجام شد", "success", 3000);
                        }

                        $('#tabs a:first').tab('show') // Select first tab
                        grid.clearSelection();
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        $scope.modalAlert = true;
                        $scope.modalAlertMessage = error;
                        $scope.modalAlertType = "danger";
                        applyScope($scope);
                    });
            }
        });
    };
    // تغییر وضعیت چک دریافتی به وصول نشده
    $scope.ReceivedChequeChangeToNotPass = function () {
        if (!$scope.validateChequeSelection())
            return false;
        var cheque = grid.getSelectedRowsData()[0];
        if (cheque.Status === 0) {
            alertbox({ content: "این چک در وضعیت وصول نشده قرار دارد" });
            return false;
        }
        var strMessage = "توجه: سند وصول چک و سند واگذاری چک به بانک در صورت وجود حذف خواهند شد و وضعیت چک به حالت عادی بر می گردد.";
        if (cheque.Status === 5) strMessage = "توجه: سند خرج چک حذف یا اصلاح خواهد شد و وضعیت چک به حالت عادی بر می گردد.";
        questionbox({
            content: "آیا از تغییر وضعیت این چک به وصول نشده مطمئن هستید؟" + "<br/>" + strMessage,
            onBtn1Click: function () {
                if ($scope.calling) return false;
                $scope.calling = true;

                var model = {
                    change: "ReceivedChequeToNotPass",
                    chequeId: cheque.ID,
                    date: "",
                    detailAccount: null,
                    description: "",
                    receiveType: "",
                    reference: ""
                };

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(model),
                    url: "/app/api/Cheque/ChangeChequeStatus",
                    contentType: "application/json"
                }).done(function (res) {
                    var result = res.data;

                    $scope.calling = false;
                        result.ContactName = result.Contact.Name;

                        var finded = findById($scope.cheques, result.ID);
                        if (finded) {
                            findAndReplace(grid._options.dataSource, result.ID, result);
                            grid.refresh();
                            DevExpress.ui.notify("تغییر وضعیت چک دریافتی به وصول نشده با موفقیت انجام شد", "success", 3000);
                        }

                        $('#tabs a:first').tab('show') // Select first tab
                        grid.clearSelection();
                        applyScope($scope);
                    }).fail(function (error) {
                        $scope.calling = false;
                        if ($scope.accessError(error)) { applyScope($scope); return; }
                        alertbox({ content: error, type: "warning" });
                        applyScope($scope);
                    });
            }
        });
    };
    // حذف چک
    $scope.deleteCheque = function () {
        if (!$scope.validateChequeSelection())
            return false;
        var cheque = grid.getSelectedRowsData()[0];
        if (cheque.Status === 2) {
            alertbox({ content: "امکان حذف چکی که پاس شده است وجود ندارد." });
            return false;
        }
        if (cheque.Status !== 0) {
            alertbox({ content: "فقط چکی که در وضعیت عادی قرار داد قابل حذف می باشد." });
            return false;
        }

        $scope.calling = true;
        callws(DefaultUrl.MainWebService + "GetChequeRelatedDocumentsNumber", { chequeId: cheque.Id })
            .success(function (docsNumbers) {
                $scope.calling = false;
                $scope.docsNumbers = docsNumbers;

                questionbox({
                    title: "هشدار",
                    content: "آیا از حذف این چک مطمئن هستید؟" + "<br>"
                        + "<input id='deleteChequeDocs' type='checkbox' checked='checked'> حذف سند (یا اسناد) مرتبط با چک" + "<br><br>"
                        + "شماره سند (یا اسناد) مرتبط: " + $scope.docsNumbers + "<br>"
                        + "<p class='bg-warning small' style='padding:5px;'><span class='fa fa-warning' aria-hidden='true'></span>&nbsp" +
                        "توجه: اگر چک دریافتی یا پرداختی در قالب یک سند ترکیبی همراه با چندین دریافت و پرداخت دیگر ثبت شده است توصیه میشود سند یا اسناد مربوطه حذف نشده و بصورت دستی ویرایش شوند در غیر این صورت تمام تراکنش های مرتبط با سند حذف خواهند شد." +
                        "</p>",
                    onBtn1Click: function () {
                        if ($scope.calling) return false;
                        $scope.calling = true;
                        $scope.deleteChequeDocs = $('#deleteChequeDocs').is(":checked");
                        callws(DefaultUrl.MainWebService + "DeleteCheque", {
                            chequeId: cheque.Id,
                            deleteChequeDocs: $scope.deleteChequeDocs
                        })
                            .success(function (result) {
                                $scope.calling = false;
                                $scope.grid.removeItem(result);
                                $scope.alert = true;
                                $scope.alertMessage = "چک انتخاب شده با موفقیت حذف شد";
                                $scope.alertType = "success";
                                grid.clearSelection();
                                applyScope($scope);
                            }).fail(function (error) {
                                $scope.calling = false;
                                if ($scope.accessError(error)) { applyScope($scope); return; }
                                $scope.modalAlert = true;
                                $scope.modalAlertMessage = error;
                                $scope.modalAlertType = "danger";
                                applyScope($scope);
                            }).loginFail(function () {
                                window.location = DefaultUrl.login;
                            });
                    }
                });
            }).fail(function (error) {
                $scope.calling = false;
                if ($scope.accessError(error)) { applyScope($scope); return; }
                $scope.modalAlert = true;
                $scope.modalAlertMessage = error;
                $scope.modalAlertType = "danger";
                applyScope($scope);
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };
    $scope.exportToCsv = function () {
        var csvContent = "";
        var cheques = $scope.grid.gridData;
        // headers
        csvContent += "ردیف" + "," + "شماره چک" + "," + "مبلغ چک" + "," + "تاریخ چک" + "," + "طرف حساب" + "," + "بانک" + "," + "شعبه" + "," + "وضعیت" + ($scope.type == 0 && $scope.tab == 2 ? "," + "بانک واگذار شده" : "") + "\n";
        var length = cheques.length;
        for (var i = 0; i < length; i++) {
            var cheque = cheques[i];
            var dataString = i + 1 + "," + cheque.ChequeNumber + "," + cheque.Amount + "," + cheque.DisplayDate + "," + cheque.ContactName + "," + cheque.BankName + "," + cheque.BankBranch + "," + ($scope.type == 0 ? Hesabfa.ChequeStatus[cheque.Status] : Hesabfa.ChequeStatusPaid[cheque.Status]) + ($scope.type == 0 && $scope.tab == 2 ? ',' + cheque.DepositBankName : '');
            csvContent += i < length ? dataString + "\n" : dataString;
        }
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            var contentType = 'text/csv';
            var blob = b64toBlob(window.btoa(unescape(encodeURI(csvContent))), contentType);
            window.navigator.msSaveOrOpenBlob(blob, "cheques.csv");
        }
        else {
            var encodedUri = "data:text/csv;charset=utf-8," + encodeURI(csvContent);
            var link = document.createElement("a");
            link.href = encodedUri;
            link.download = "cheques.csv";
            link.target = '_blank';
            document.body.appendChild(link);
            link.click();
        }
    };
    $scope.exportChequesToExcel = function () {
        if ($scope.callingImport) return;
        $scope.callingImport = true;
        $('#loadingModal').modal('show');
        callws(DefaultUrl.MainWebService + 'ExportChequesToExcel', {})
            .success(function (data) {
                $scope.callingImport = false;
                $('#loadingModal').modal('hide');
                $scope.$apply();
                var fileName = "Cheques";

                var contentType = 'application/vnd.ms-excel';
                var blob = b64toBlob(data, contentType);
                if (window.navigator && window.navigator.msSaveOrOpenBlob) {
                    window.navigator.msSaveOrOpenBlob(blob, fileName + ".xlsx");
                }
                else {
                    var objectUrl = URL.createObjectURL(blob);
                    //                    window.open(objectUrl);
                    //                    var encodedUri = "data:application/vnd.ms-excel," + encodeURI(data);
                    var link = document.createElement("a");
                    link.href = objectUrl;
                    link.download = fileName + ".xlsx";
                    link.target = '_blank';
                    document.body.appendChild(link);
                    link.click();
                }

            }).fail(function (error) {
                $scope.callingImport = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error });
            }).loginFail(function () {
                window.location = DefaultUrl.login;
            });
    };

    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
    //


        }])
});