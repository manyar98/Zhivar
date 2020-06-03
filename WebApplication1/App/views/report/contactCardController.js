define(['application','combo', 'scrollbar', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('contactCardController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {

    var grid1, grid2, grid3, grid4;
    var selectedContactName = "";
    $scope.init = function () {
        $("#businessNav").show();
        var invoiceTypeLookup = {
            dataSource: [{ key: 0, val: "سند افتتاحیه" },{ key: 1, val: "فروش" }, { key: 2, val: "خرید" }, { key: 3, val: 'برگشت از فروش' }, { key: 4, val: 'برگشت از خرید' },
                { key: 5, val: "فاکتور اجاره" }, { key: 6, val: "فاکتور اجاره از طرف حساب" }, { key: 7, val: "رسید دریافت" },
                { key: 8, val: "رسید پرداخت" }, { key: 9, val: "هزینه" }, { key: 10, val: "انتقال" }], valueExpr: 'key', displayExpr: 'val'
        };

        $scope.fromDate =  $scope.currentFinanYear.DisplayStartDate;
        $scope.toDate = $scope.currentFinanYear.DisplayEndDate;

        $scope.contactId = $stateParams.id;

        $scope.comboContact = new HesabfaCombobox({
            items: [],
            containerEle: document.getElementById("comboContact"),
            toggleBtn: true,
            itemClass: "hesabfa-combobox-item",
            activeItemClass: "hesabfa-combobox-activeitem",
            itemTemplate: Hesabfa.comboContactTemplate,
            matchBy: "item.DetailAccount.Id",
            displayProperty: "{Name}",
            searchBy: ["Name", "DetailAccount.Code"],
            onSelect: $scope.contactSelect,
            divider: true
        });


        $scope.loadContacts();
        var selectedInvoices = [];
        var itemName = "";

        var gridPopup = $('#gridPopup').dxPopup({
            rtlEnabled: true,
            showTitle: true,
            height: 'auto',
            title: "تراکنش های کالا",
            dragEnabled: true,
            closeOnOutsideClick: true,
            contentTemplate: function (contentElement) {
                var el = $("<div>");
                grid4 = dxGrid({
                    element: el,
                    layoutKeyName: 'grid-report-contactCard-itemTransactions',
                    height: 300,
                    columnAutoWidth: true,
                    columns: [
						{
						    caption: 'نوع', dataField: 'Type', lookup: invoiceTypeLookup,
						    printCalcValue: function (data) {
						        for (var l in invoiceTypeLookup.dataSource) {
						            if (invoiceTypeLookup.dataSource[l].key === data.Type)
						                return invoiceTypeLookup.dataSource[l].val;
						        }
						        return "";
						    }, printWidth: 2,
						    cellTemplate: function (ce, ci) {
						        var val = ci.displayValue;
						        ce.html("<a href='#viewInvoice/id=" + ci.data.InvoiceId + "' class='text-primary txt-bold' target='_blank'>" + val + "</a>");
						    }
						},
						{ caption: 'شماره فاکتور', dataField: 'Number', printWidth: 2 },
						{ caption: 'شماره ارجاع', dataField: 'Reference', printWidth: 2 },
						{ caption: 'تاریخ', dataField: 'DateTime', printWidth: 2 },
						{ caption: 'تعداد/مقدار', dataField: 'Quantity', dataType: "number", printWidth: 2 },
						{ caption: 'واحد', dataField: 'Unit', dataType: "number", printWidth: 2 },
						{ caption: 'قیمت واحد', dataField: 'UnitPrice', dataType: "number", format: "#", printWidth: 2.5, printFormat: "#" },
						{ caption: 'تخفیف', dataField: 'Discount', dataType: "number", format: "#", printWidth: 2.5, printFormat: "#" },
						{ caption: 'مالیات', dataField: 'Tax', dataType: "number", format: "#", printWidth: 2.5, printFormat: "#" },
						{ caption: 'قیمت کل', dataField: 'TotalAmount', dataType: "number", format: "#", printWidth: 2.5, printFormat: "#" }
                    ],
                    print: {
                        fileName: "تراکنش های کالا.pdf",
                        rowColumnWidth: 1,
                        page: {
                            landscape: false
                        },
                        header: {
                            title1: $rootScope.currentBusiness.Name,
                            title2: "تراکنش های کالای " + itemName,
                            left: $scope.currentFinanYear.Name,
                            center: function () {
                                return $scope.selectedDateRange;
                            },
                            right: function () {
                                return selectedContactName;
                            }
                        }
                    }
                });
                grid4.fill(selectedInvoices);
                contentElement.append(el);
            }
        }).dxPopup("instance");


        grid1 = dxGrid({
            elementId: 'gridContainer1',
            layoutKeyName: 'grid-report-contactCard-statement',
            height: 500,
            wordWrapEnabled: true,
            sorting: { mode: 'none' },
            filterRow: { visible: false },
            headerFilter: { visible: false },
            columns: [
				{ caption: 'تاریخ', dataField: 'Date', width: 100, printWidth: 2 },
				{
				    caption: 'سند', dataField: 'DocNum', width: 80, cellTemplate: function (ce, ci) {
				        if (ci.data.DocNum === 0) {
				            ce.html("");
				            return;
				        }
				        var txt = Hesabfa.farsiDigit(ci.data.DocNum);
				        if (ci.data.DocId > 0)
				            ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
				        else
				            ce.html(txt);
				    }, printWidth: 1.5
				},
				{
				    caption: 'شرح', dataField: 'Description', cellTemplate: function (ce, ci) {
				        var txt = Hesabfa.farsiDigit(ci.data.Description);
				        if (ci.data.InvoiceId > 0)
				            ce.html("<a href='#viewInvoice/id=" + ci.data.InvoiceId + "' class='text-primary txt-bold' target='_blank'>" + txt + "</a>");
				        else if (ci.data.DocId > 0)
				            ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
				        else
				            ce.html(txt);
				    }, printWidth: 10
				},
				{ caption: 'ارجاع', dataField: 'Reference', width: 80, printWidth: 1.5 },
				{ caption: 'بدهکار', dataField: 'Debit', dataType: "number", format: "<span class='text-danger txt-bold'>#</span>", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'بستانکار', dataField: 'Credit', dataType: "number", format: "<span class='text-success txt-bold'>#</span>", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'باقیمانده', dataField: 'Remain', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'تشخیص', dataField: 'RemainType', width: 80, printWidth: 2 }
            ],
            print: {
                fileName: "صورتحساب.pdf",
                rowColumnWidth: 1.5,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "گزارش صورتحساب",
                    left:  $scope.currentFinanYear.Name,
                    center: function () {
                        return $scope.selectedDateRange;
                    },
                    right: function () {
                        return selectedContactName;
                    }
                }
            },
             summary: {
                 totalItems: [
               
                    {
                        column: "Debit",
                        summaryType: "sum",
                        customizeText: function (data) {
                            return Hesabfa.money(data.value);
                        }
                    },
                    {
                        column: "Credit",
                        summaryType: "sum",
                        customizeText: function (data) {
                            return Hesabfa.money(data.value);
                        }
                    },
                    //{
                    //    column: "Remain", summaryType: "sum",
                    //    customizeText: function (data) {
                    //        return Hesabfa.money(data.value);
                    //    }
                    //}
                ]
            }

        });


   
        var lookup = {
            dataSource: [{ key: 0, val: "سند افتتاحیه" }, { key: 1, val: "فروش" }, { key: 2, val: "خرید" }, { key: 3, val: 'برگشت از فروش' }, { key: 4, val: 'برگشت از خرید' },
            { key: 5, val: "فاکتور اجاره" }, { key: 6, val: "فاکتور اجاره از طرف حساب" }, { key: 7, val: "رسید دریافت" },
            { key: 8, val: "رسید پرداخت" }, { key: 9, val: "هزینه" }, { key: 10, val: "انتقال" }], valueExpr: 'key', displayExpr: 'val'
        };
        grid2 = dxGrid({
            elementId: 'gridContainer2',
            layoutKeyName: 'grid-report-contactCard-statementDetail',
            height: 500,
            wordWrapEnabled: true,
            sorting: { mode: 'none' },
            filterRow: { visible: false },
            headerFilter: { visible: false },
            pager: {
                showPageSizeSelector: false,
                allowedPageSizes: [10, 20, 50],
                showNavigationButtons: true
            },
            columns: [
                { caption: 'تاریخ', dataField: 'Date', width: 100, printWidth: 2 },


                {
                    caption: 'نوع',
                    dataField: 'Status',
                    width: 80,
                    printWidth: 1.5,
                    lookup: lookup,
                    hidingPriority: 6,
                    printCalcValue: function (data) {
                        for (var l in lookup.dataSource) {
                            if (lookup.dataSource[l].key === data.Status)
                                return lookup.dataSource[l].val;
                        }
                    }
                },

				{
				    caption: 'سند', dataField: 'DocNum', width: 80, cellTemplate: function (ce, ci) {
				        if (ci.data.DocNum === 0) {
				            ce.html("");
				            return;
				        }
				        var txt = Hesabfa.farsiDigit(ci.data.DocNum);
				        if (ci.data.DocId > 0)
				            ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
				        else
				            ce.html(txt);
				    }, printWidth: 1.5
				},
				{
				    caption: 'شرح', dataField: 'Description', cellTemplate: function (ce, ci) {
				        var data = ci.data;
				        var val = Hesabfa.farsiDigit(data.Description);
				        var txt;
				        //if (data.DocId !== -1)
				        //    txt = "class='text-primary txt-bold'>"  + val;
				        //else
				            txt = "class='text-info p-r-20'> - " + val;


				        //if (data.InvoiceId > 0)
				        //    ce.html("<a href='#viewInvoice/id=" + data.InvoiceId + "' target='_blank' " + txt + "</a>");
				        //else if (data.DocId > 0)
				        //    ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + data.DocId + ")' " + txt + "</a>");
				        //else
				         //   ce.html(txt);

                        				            ce.html("<a href='javascript:void(0);' " + txt + "</a>");
				    }, printWidth: 10
				},

				{ caption: 'تعداد', dataField: 'Count', dataType: "number", format: "#", width: 60, printWidth: 1, printFormat: "#" },
				{ caption: 'قیمت واحد', dataField: 'UnitPrice', dataType: "number", format: "#", width: 110, printWidth: 2, printFormat: "#" },
				{ caption: 'بدهکار', dataField: 'Debit', dataType: "number", format: "<span class='text-danger txt-bold'>#</span>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'بستانکار', dataField: 'Credit', dataType: "number", format: "<span class='text-success txt-bold'>#</span>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'باقیمانده', dataField: 'Remain', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'تشخیص', dataField: 'RemainType', width: 80, printWidth: 1.5 }
            ],
            print: {
                fileName: "صورتحساب به ریز فاکتور.pdf",
                rowColumnWidth: 1,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "صورتحساب به ریز فاکتور",
                    left: $scope.currentFinanYear.Name,
                    center: function () {
                        return $scope.selectedDateRange;
                    },
                    right: function () {
                        return selectedContactName;
                    }
                }
            }
        });

        grid3 = dxGrid({
            elementId: 'gridContainer3',
            layoutKeyName: 'grid-report-contactCard-itemsSellBuy',
            height: 500,
            columns: [
				{
				    caption: 'کد', dataField: 'Code', width: 100, printWidth: 2, cellTemplate: function (ce, ci) {
				        if (ci.data.Code === "") {
				            ce.html("");
				            return;
				        }
				        var txt = Hesabfa.farsiDigit(ci.data.Code);
				        ce.html("<a href='#itemCard/" + ci.data.Id + "' class='text-primary txt-bold' target='_blank'>" + txt + "</a>");
				    }
				},
				{ caption: 'نام کالا / خدمات', dataField: 'Name', printWidth: 8 },
				{ caption: 'تعداد خرید', dataField: 'SaleCount', dataType: "number", format: "<b>*</b>", width: 100, printWidth: 2 },
				{ caption: 'تعداد فروش', dataField: 'PurchaseCount', dataType: "number", format: "<b>*</b>", width: 100, printWidth: 2 },
				{ caption: 'خرید', dataField: 'SaleAmount', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'فروش', dataField: 'PurchaseAmount', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: '', dataField: 'detail', printVisible: false, width: 80, format: "<a href='javascript:void(0)'>جزئیات</a>" }
            ],
            print: {
                fileName: "صورتحساب خرید و فروش.pdf",
                rowColumnWidth: 1,
                page: {
                    landscape: false
                },
                header: {
                    title1: $rootScope.currentBusiness.Name,
                    title2: "صورتحساب خرید و فروش",
                    left: $scope.currentFinanYear.Name,
                    center: function () {
                        return $scope.selectedDateRange;
                    },
                    right: function () {
                        return selectedContactName;
                    }
                }
            },
            onCellClick: function (e) {
                if (e.column && e.column.dataField === 'detail') {
                    if (e.data.Id === 0)
                        return;
                    selectedInvoices = e.data.list;
                    if (grid4) {
                        grid4.fill(selectedInvoices);
                        grid4.clearFilter();
                    }
                    itemName = e.data.Name;
                    gridPopup.show();
                }
            },
            summary: {
                totalItems: [
                    {
                        column: "SaleCount",
                        summaryType: "sum",
                        customizeText: function (data) {
                            return Hesabfa.farsiDigit(data.value);
                        }
					},
					{
					    column: "PurchaseCount",
					    summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.farsiDigit(data.value);
					    }
					},
					{
					    column: "SaleAmount", summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.money(data.value);
					    }
					},
					{
					    column: "PurchaseAmount", summaryType: "sum",
					    customizeText: function (data) {
					        return Hesabfa.money(data.value);
					    }
					}
                ]
            }

        });

        if ($scope.contactId)
            $scope.getContactCard();

        $(function () { $('[data-toggle="popover"]').popover(); });
    };
    $scope.loadContacts = function () {
       $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url: "/app/api/Contact/GetAllByOrganId",
            contentType: "application/json"
       }).done(function (res) {
           var contacts = res.data;
    
                $scope.comboContact.items = contacts;
                $scope.$apply();
            }).fail(function (error) {
                $scope.loading = false;
                applyScope($scope);
                if ($scope.accessError(error)) return;
                alertbox({ content: error, type: "error" });
            });
    };

    $scope.contactSelect = function (e) {
        $scope.contactId = e.ID;
        selectedContactName = e.Name;
        $scope.getContactCard();
    }

    $scope.refreshGrid = function (i) {
        var grid = i === 1 ? grid1 : (i === 2 ? grid2 : (i === 3 ? grid3 : grid4));
        setTimeout(function () {
            grid.repaint();
            grid.fixScroll();
        }, 250);
    }

    function fillGrids(ledger, invoiceItems) {
        var list1 = [];
        var list2 = [];
        var list3 = [];
        var i;
        var dic = {};
        for (i = 0; i < ledger.length; i++) {
            var r = ledger[i];
            if (r.IsMain === true) {
                list1.push(r);
            }
        }

        for (i = 0; i < ledger.length; i++) {
            r = ledger[i];
            if (r.IsMain === false) {
                list2.push(r);
            }
        }

        for (i = 0; i < invoiceItems.length; i++) {
            var ii = invoiceItems[i];
            var o;
            if (!dic[ii.ItemId]) {
                o = {
                    Id: ii.ItemId,
                    Name: ii.Name,
                    Code: ii.Code,
                    SaleCount: 0,
                    PurchaseCount: 0,
                    SaleAmount: 0,
                    PurchaseAmount: 0,
                    list: []
                };
                dic[ii.ItemId] = o;
                list3.push(o);
            }
            else
                o = dic[ii.ItemId];
            if (ii.Type === 1) {
                o.SaleCount += ii.Quantity;
                o.SaleAmount += ii.TotalAmount;
            }
            if (ii.Type === 0) {
                o.PurchaseCount += ii.Quantity;
                o.PurchaseAmount += ii.TotalAmount;
            }
            if (ii.Type === 2) {
                o.PurchaseCount -= ii.Quantity;
                o.PurchaseAmount -= ii.TotalAmount;
            }
            if (ii.Type === 3) {
                o.SaleCount -= ii.Quantity;
                o.SaleAmount -= ii.TotalAmount;
            }
            o.list.push(ii);
        }

        grid1.fill(list1);
        grid2.fill(list2);
        grid3.fill(list3);
    }
    $scope.editContact = function (contact) {
        Hesabfa.pageParams = {};
        Hesabfa.pageParams.id = contact.Id;
        Hesabfa.pageParams.contact = contact;
        $location.path("/editContact");
        return;
    };
    $scope.getContactCard = function () {
        $scope.loading = true;
        grid1.beginCustomLoading();
        grid2.beginCustomLoading();
        grid3.beginCustomLoading();

        var model = {
            contactId: $scope.contactId, 
            start: $scope.fromDate, 
            end: $scope.toDate
        };

        $.ajax({
            type:"POST",
            data: JSON.stringify(model),
            url: "/app/api/Transaction/GetContactCard",
            contentType: "application/json"
        }).done(function (res) {

            var result = res.data;

                $scope.loading = false;

                $scope.comboContact.setSelected(result.contact);
                selectedContactName = result.contact.Name;
                $scope.contact = result.contact;
                $scope.sumDebit = result.sumDebit;
                $scope.sumCredit = result.sumCredit;

                $scope.saleApprovedCount = result.saleApprovedCount;
                $scope.billApprovedCount = result.billApprovedCount;
                $scope.saleOverDueCount = result.saleOverDueCount;
                $scope.billOverDueCount = result.billOverDueCount;

                $scope.saleApprovedSum = result.saleApprovedSum;
                $scope.billApprovedSum = result.billApprovedSum;
                $scope.saleOverDueSum = result.saleOverDueSum;
                $scope.billOverDueSum = result.billOverDueSum;



                $scope.sumInvoiceRentTo = result.sumInvoiceRentTo;
                $scope.countInvoiceRentTo = result.countInvoiceRentTo;

                $scope.sumInvoiceRentFrom = result.sumInvoiceRentFrom;
                $scope.countInvoiceRentFrom = result.countInvoiceRentFrom;

                $scope.sumInvoiceSell = result.sumInvoiceSell;
                $scope.countInvoiceSell = result.countInvoiceSell;

                $scope.sumInvoiceBuy = result.sumInvoiceBuy;
                $scope.countInvoiceBuy = result.countInvoiceBuy;

                fillGrids(result.ledger.Rows, result.invoiceItems);

                grid1.endCustomLoading();
                grid2.endCustomLoading();
                grid3.endCustomLoading();

                $rootScope.pageTitle($scope.contact.Name);
                applyScope($scope);
            }).fail(function (error) {
                $scope.loading = false;
                grid1.endCustomLoading();
                grid2.endCustomLoading();
                grid3.endCustomLoading();

                applyScope($scope);
                if ($scope.accessError(error)) { applyScope($scope); return; }
                alertbox({ content: error, type: "error" });
            });
    };
    $scope.printStatement = function () {
        var date1 = $("#date1").val();
        var date2 = $("#date2").val();
        printContactStatement($scope.transactions, $scope.sumDebit, $scope.sumCredit, $scope.balance, $scope.balanceType, $scope.contact, date1, date2, $rootScope.currentBusiness, $scope.getCurrency(), "2000/12/12");
    };
    $scope.pdfStatement = function () {
        var date1 = $("#date1").val();
        var date2 = $("#date2").val();
        pdfContactStatement($scope.transactions, $scope.sumDebit, $scope.sumCredit, $scope.balance, $scope.balanceType, $scope.contact, date1, date2, $rootScope.currentBusiness, $scope.getCurrency(), "2000/12/12");
    };
    $scope.exportToCsv = function () {
        var csvContent = "";
        var transactions = $scope.transactions;
        // headers
        csvContent += "ردیف" + "," + "تاریخ" + "," + "سند" + "," + "شرح" + "," + "بدهکار" + "," + "بستانکار" + "," + "تشخیص" + "," + "باقیمانده" + "\n";
        var length = transactions.length;
        for (var i = 0; i < length; i++) {
            var trans = transactions[i];
            var tashkhis = "";
            var dataString = i + 1 + "," + trans.Transaction.AccDocument.DisplayDate + "," + trans.Transaction.AccDocument.Id + "," + trans.Transaction.Description + "," + trans.Debit + "," + trans.Credit + "," + trans.RemainTypeString + "," + trans.Remain;
            csvContent += i < length ? dataString + "\n" : dataString;
        }
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            var contentType = 'text/csv';
            var blob = b64toBlob(window.btoa(unescape(encodeURI(csvContent))), contentType);
            window.navigator.msSaveOrOpenBlob(blob, "contact_transactions.csv");
        }
        else {
            var encodedUri = "data:text/csv;charset=utf-8," + encodeURI(csvContent);
            var link = document.createElement("a");
            link.href = encodedUri;
            link.download = "contact_transactions.csv";
            link.target = '_blank';
            document.body.appendChild(link);
            link.click();
        }
    };
    $scope.getBalance = function (contact) {
        if (!contact) return 0;
        contact.Balance = contact.Liability - contact.Credits;
        if (contact.Balance >= 0) return contact.Balance;
        else return contact.Balance * -1;
    };
    $scope.getTashkhis = function (contact) {
        if (!contact) return "";
        contact.Balance = contact.Liability - contact.Credits;
        if (contact.Balance > 0) return "بدهکار";
        else if (contact.Balance < 0) return "بستانکار";
        else return "-";
    };
    $scope.getBalanceType = function (type) {
        if (typeof type == "undefined" || type == null) return "-";
        if (type === 0) return "بدهکار";
        else if (type === 1) return "بستانکار";
        else return "-";
    };
    $scope.getBgByBalanceState = function (balance) {
        if (!balance) return "";
        if (balance > 0) return "bg-danger";
        else if (balance < 0) return "bg-success";
        else return "bg-info";
    };
    $scope.print = function () {
        grid1.print();
    };
    $scope.pdf = function () {
        grid1.pdf();
    };
    $scope.excel = function () {
        grid1.exportToExcel(false);
    };
    $scope.print2 = function () {
        grid2.print();
    };
    $scope.pdf2 = function () {
        grid2.pdf();
    };
    $scope.excel2 = function () {
        grid2.exportToExcel(false);
    };
    $scope.print3 = function () {
        grid3.print();
    };
    $scope.pdf3 = function () {
        grid3.pdf();
    };
    $scope.excel3 = function () {
        grid3.exportToExcel(false);
    };
    //
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});