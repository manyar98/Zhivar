define(['application','combo', 'scrollbar', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('itemCardController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope,$stateParams, $location) {

	var grid1, grid2, grid4, chart1;
	var contactName, selectedItemName;
	$scope.init = function () {
		$("#businessNav").show();
		var invoiceTypeLookup = { dataSource: [{ key: 0, val: "فروش" }, { key: 1, val: "خرید" }, { key: 2, val: 'برگشت از فروش' }, { key: 3, val: 'برگشت از خرید' }, { key: 4, val: 'ضایعات' }], valueExpr: 'key', displayExpr: 'val' }

		$scope.fromDate = "1397/01/01",// $scope.currentFinanYear.DisplayStartDate;
		$scope.toDate = "1397/12/29",// $scope.currentFinanYear.DisplayEndDate;

		$scope.itemId = $stateParams.id;

		$scope.comboItem = new HesabfaCombobox({
			items: [],
			containerEle: document.getElementById("comboItem"),
			toggleBtn: true,
			itemClass: "hesabfa-combobox-item",
			activeItemClass: "hesabfa-combobox-activeitem",
			itemTemplate: Hesabfa.comboItemTemplate,
			matchBy: "item.DetailAccount.Id",
			displayProperty: "{Name}",
			searchBy: ["Name", "DetailAccount.Code"],
			onSelect: $scope.itemSelect,
			divider: true
		});


		$scope.loadItems();
		var selectedInvoices = [];

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
					layoutKeyName: 'grid-report-itemCard-contactTransactions',
					height: 300,
					columnAutoWidth: true,
					columns: [
						{
							caption: 'نوع', dataField: 'Type', lookup: invoiceTypeLookup, printWidth: 2, cellTemplate: function (ce, ci) {
								var val = ci.displayValue;
								ce.html("<a href='#viewInvoice/id=" + ci.data.InvoiceId + "' class='text-primary txt-bold' target='_blank'>" + val + "</a>");
							}, printCalcValue: function (data) {
								for (var l in invoiceTypeLookup.dataSource) {
									if (invoiceTypeLookup.dataSource[l].key === data.Type)
										return invoiceTypeLookup.dataSource[l].val;
								}
								return "";
							}
						},
						{ caption: 'شماره فاکتور', dataField: 'Number', printWidth: 2 },
						{ caption: 'شماره ارجاع', dataField: 'Reference', printWidth: 2 },
						{ caption: 'تاریخ', dataField: 'DateTime', printWidth: 2 },
						{ caption: 'تعداد/مقدار', dataField: 'Quantity', dataType: "number", format: "<b>*</b>", printWidth: 1.5 },
						{ caption: 'واحد', dataField: 'Unit', dataType: "number", printWidth: 1.5 },
						{ caption: 'قیمت واحد', dataField: 'UnitPrice', dataType: "number", format: "#", printWidth: 2, printFormat: "#" },
						{ caption: 'تخفیف', dataField: 'Discount', dataType: "number", format: "#", printWidth: 2, printFormat: "#" },
						{ caption: 'مالیات', dataField: 'Tax', dataType: "number", format: "#", printWidth: 2, printFormat: "#" },
						{ caption: 'قیمت کل', dataField: 'TotalAmount', dataType: "number", format: "<b>#</b>", printWidth: 2.5, printFormat: "#" }
					],
					print: {
						fileName: "تراکنش های کالا.pdf",
						rowColumnWidth: 1.5,
						page: {
							landscape: false
						},
						header: {
							title1: $rootScope.currentBusiness.Name,
							title2: function () {
								return "تراکنش های کالای " + selectedItemName;
							},
							left: $scope.currentFinanYear.Name,
							center: function () {
								return $scope.selectedDateRange;
							},
							right: function () {
								return contactName;
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
			layoutKeyName: 'grid-report-itemCard-statement',
			height: 500,
			wordWrapEnabled: true,
			sorting: { mode: 'none' },
			filterRow: { visible: false },
			headerFilter: { visible: false },
			columns: [
				{ caption: 'تاریخ', dataField: 'DateTime', width: 100, printWidth: 2 },
				{
					caption: 'سند', dataField: 'Number', width: 80, printWidth: 1.5, cellTemplate: function (ce, ci) {
						var txt = Hesabfa.farsiDigit(ci.data.Number);
						if (ci.data.DocId > 0)
							ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
						else
							ce.html(txt);
					}
				},
				{
					caption: 'شرح', dataField: 'Text', printWidth: 8, cellTemplate: function (ce, ci) {
						var txt = Hesabfa.farsiDigit(ci.data.Text);
						if (ci.data.InvoiceId > 0)
							ce.html("<a href='#viewInvoice/id=" + ci.data.InvoiceId + "' class='text-primary txt-bold' target='_blank'>" + txt + "</a>");
						else if (ci.data.DocId > 0)
							ce.html("<a href='javascript:void(0);' onclick='angular.element(this).scope().viewDocumentModal(" + ci.data.DocId + ")' class='text-primary txt-bold'>" + txt + "</a>");
						else
							ce.html(txt);
					}
				},
				{ caption: 'ورودی', dataField: 'In', dataType: "number", format: "<span class='text-success txt-bold'>#</span>", width: 120, printWidth: 2 },
				{ caption: 'خروجی', dataField: 'Out', dataType: "number", format: "<span class='text-danger txt-bold'>#</span>", width: 120, printWidth: 2 },
				{ caption: 'مانده', dataField: 'Remain', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'قیمت', dataField: 'Amount', format: '#', width: 120, printWidth: 2, printFormat: "#" }
			],
			print: {
				fileName: "کاردکس کالا.pdf",
				rowColumnWidth: 1.5,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: "کاردکس کالا",
					left: $scope.currentFinanYear.Name,
					center: function () {
						return $scope.selectedDateRange;
					},
					right: function () {
						return selectedItemName;
					}
				}
			}
		});

		grid2 = dxGrid({
			elementId: 'gridContainer2',
			layoutKeyName: 'grid-report-itemCard-contactSellBuy',
			height: 500,
			columns: [
				{
					caption: 'کد', dataField: 'Code', width: 100, printWidth: 2, cellTemplate: function (ce, ci) {
						var txt = Hesabfa.farsiDigit(ci.data.Code);
						ce.html("<a href='#contactCard/" + ci.data.Id + "' class='text-primary txt-bold' target='_blank'>" + txt + "</a>");
					}
				},
				{ caption: 'نام شخص', dataField: 'Name', printWidth: 8 },
				{ caption: 'تعداد خرید', dataField: 'SaleCount', dataType: "number", format: "<b>*</b>", width: 100, printWidth: 1.5 },
				{ caption: 'تعداد فروش', dataField: 'PurchaseCount', dataType: "number", format: "<b>*</b>", width: 100, printWidth: 1.5 },
				{ caption: 'خرید', dataField: 'SaleAmount', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'فروش', dataField: 'PurchaseAmount', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: '', dataField: 'detail', width: 80, format: "<a href='javascript:void(0)'>جزئیات</a>", printVisible: false }
			], print: {
				fileName: "مشتریان و فروشندگان کالا.pdf",
				rowColumnWidth: 1.5,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: "گزارش مشتریان و فروشندگان کالا",
					left: $scope.currentFinanYear.Name,
					center: function () {
						return $scope.selectedDateRange;
					},
					right: function () {
						return selectedItemName;
					}
				}
			},
			onCellClick: function (e) {
				if (e.column.dataField === 'detail') {
					selectedInvoices = e.data.list;
					if (grid4) {
						grid4.fill(selectedInvoices);
						grid4.clearFilter();
					}
					contactName = e.data.Name;
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

		chart1 = $("#chart1").dxChart({
			dataSource: [],
			height: 300,
			commonPaneSettings: {
				backgroundColor: '#f1f1f1'
			},
			commonSeriesSettings: {
				argumentField: "Month",
				type: "bar"
			},
			argumentAxis: {
				label: {
					overlappingBehavior: "rotate",
					rotationAngle: -30
				},
				grid: { visible: true }
			},
			series: [
				{ valueField: "In", name: "ورودی" },
				{ valueField: "Out", name: "خروجی" }
			],
			legend: {
				verticalAlignment: "bottom",
				horizontalAlignment: "center",
				margin: 0,
				itemTextPosition: "left"
			},
			tooltip: {
				enabled: true
			}
		}).dxChart("instance");

		if ($scope.itemId)
			$scope.getItemCard();

		$(function () { $('[data-toggle="popover"]').popover(); });
	};
	$scope.loadItems = function () {
     $.ajax({
	        type:"POST",
	        url: "/app/api/Item/GetItems",
	        contentType: "application/json"
	    }).done(function(res){
	        var items = res.data;
		//callws(DefaultUrl.MainWebService + 'GetItems', { type: 'all' })
        //    .success(function (items) {
            	$scope.comboItem.items = items;
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};

	$scope.itemSelect = function (e) {
		$scope.itemId = e.ID;
		selectedItemName = e.Name;
		$scope.getItemCard();
	}

	$scope.refreshGrid = function (i) {
		var grid = i === 1 ? grid1 : (i === 2 ? grid2 : (i === 3 ? grid3 : grid4));
		setTimeout(function () {
			grid.repaint();
			grid.fixScroll();
		}, 250);
	}

	function fillGrids(ledger, invoiceItems) {
		var list = [];
		var i;
		var dic = {};
		for (i = 0; i < invoiceItems.length; i++) {
			var ii = invoiceItems[i];
			var o;
			if (!dic[ii.ContactId]) {
				o = {
					Id: ii.ContactId,
					Name: ii.Name,
					Code: ii.Code,
					SaleCount: 0,
					PurchaseCount: 0,
					SaleAmount: 0,
					PurchaseAmount: 0,
					list: []
				};
				dic[ii.ContactId] = o;
				list.push(o);
			}
			else
				o = dic[ii.ContactId];
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

		for (i = 0; i < list.length; i++) {
			var l = list[i];
			l.SaleCount = Math.round(l.SaleCount * 100) / 100;
			l.PurchaseCount = Math.round(l.PurchaseCount * 100) / 100;
		}

		grid1.fill(ledger);
		grid2.fill(list);
	}
	$scope.editItem = function (contact) {
		Hesabfa.pageParams = {};
		Hesabfa.pageParams.id = item.Id;
		Hesabfa.pageParams.item = item;
		$location.path("/editItem");
		return;
	};
	$scope.getItemCard = function () {
		$scope.loading = true;
		grid1.beginCustomLoading();
		grid2.beginCustomLoading();
		$.ajax({
		    type:"POST",
            data: JSON.stringify($scope.itemId),
		    url: "/app/api/Item/GetItemCard",
		    contentType: "application/json"
		}).done(function(res){
		    var result = res.data;

            	$scope.loading = false;

            	$scope.comboItem.setSelected(result.item);
            	$scope.item = result.item;

            	chart1.option('dataSource', result.chart);
            	fillGrids(result.list, result.invoiceItems);

            	grid1.endCustomLoading();
            	grid2.endCustomLoading();
            	$rootScope.pageTitle($scope.item.Name);
            	applyScope($scope);
            }).fail(function (error) {
            	$scope.loading = false;
            	grid1.endCustomLoading();
            	grid2.endCustomLoading();
            	if ($scope.accessError(error)) { applyScope($scope); return; }
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.getBalance = function (contact) {
		if (!contact) return 0;
		contact.Balance = contact.Liability - contact.Credits;
		if (contact.Balance >= 0) return contact.Balance;
		else return contact.Balance * -1;
	};
	$scope.getTashkhis = function (contact) {
		if (!contact) return 0;
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
    
    $rootScope.loadCurrentBusinessAndBusinesses($scope.init);

}])
});