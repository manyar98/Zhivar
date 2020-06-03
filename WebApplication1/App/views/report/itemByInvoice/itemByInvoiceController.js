define(['application','combo', 'scrollbar', 'helper', 'editItem', 'editContact', 'nodeSelect','dx','gridHelper'], function (app) {
    app.register.controller('itemByInvoiceController', ['$scope','$rootScope', '$stateParams',
        function ($scope, $rootScope, $stateParams) {

	var grid;
	var sale = $scope.sale = !($stateParams.type === "purchase");
	$scope.init = function () {
		$rootScope.pageTitle(sale ? 'گزارش فروش کالا و خدمات به تفکیک فاکتور و مشتری' : 'گزارش خرید کالا به تفکیک فاکتور و مشتری');
		$('#businessNav').show();
		$scope.fromDate = "1397/01/01"; //$scope.currentFinanYear.DisplayStartDate;
		$scope.toDate = "1397/12/29"; //$scope.currentFinanYear.DisplayEndDate;
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: 'grid-report-itemByInvoice',
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
			columns: [
				{ caption: 'تاریخ', dataField: 'Date', width: 90, printWidth: 2 },
				{
					caption: 'ش. فاکتور', dataField: 'Number', width: 100, printWidth: 2, cellTemplate: function (cellElement, cellInfo) {
						cellElement.html("<a class='text-info txt-bold' href='#viewInvoice/id=" + cellInfo.data.Id + "' target='_blank'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
					}
				},
				{
					caption: 'ش. ارجاع', dataField: 'Reference', width: 100, printWidth: 2, cellTemplate: function (cellElement, cellInfo) {
						cellElement.html("<a class='text-info txt-bold' href='#viewInvoice/id=" + cellInfo.data.Id + "' target='_blank'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
					}
				},
				{
					caption: sale ? 'مشتری' : 'فروشنده', dataField: 'Contact', printWidth: 6, cellTemplate: function (cellElement, cellInfo) {
						cellElement.html("<a class='text-info txt-bold' href='#contactCard/" + cellInfo.data.ContactId + "' target='_blank'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
					}
				},
				{ caption: 'تعداد', dataField: 'Quantity', format: "<b>*</b>", width: 60, printWidth: 1.5 },
				{ caption: 'مبلغ واحد', dataField: 'UnitPrice', dataType: "number", format: "#", width: 120, printWidth: 2, printFormat: "#" },
				{ caption: 'جمع', dataField: 'Sum', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'تخفیف', dataField: 'Discount', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'مالیات', dataField: 'Tax', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'مبلغ کل', dataField: 'TotalAmount', dataType: "number", format: "<b>#</b>", width: 120, printWidth: 2.5, printFormat: "#" }
			],
			print: {
				fileName: sale ? 'گزارش فروش کالا و خدمات به تفکیک فاکتور و مشتری' : 'گزارش خرید کالا به تفکیک فاکتور و مشتری' + ".pdf",
				rowColumnWidth: 1.5,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: sale ? 'گزارش فروش کالا و خدمات به تفکیک فاکتور و مشتری' : 'گزارش خرید کالا به تفکیک فاکتور و مشتری',
					       		left:  $scope.currentFinanYear.Name,
					center: function () {
						return $scope.selectedDateRange;
					}
				}
			},
			summary: {
				totalItems: [
					{
						column: "Quantity", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.farsiDigit(data.value);
						}
					},
					{
						column: "UnitPrice", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					},
					{
						column: "Sum", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					},
					{
						column: "Discount", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					},
					{
						column: "Tax", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					},
					{
						column: "TotalAmount", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					}
				]
			}
		});
		$scope.loadItems();
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
	};

	$scope.loadItems = function () {
	    $.ajax({
	        type:"POST",
	        url: "/app/api/KalaKhadmatApi/GetItems",
	        contentType: "application/json"
	    }).done(function(res){
	        var items = res.data;
            	$scope.comboItem.items = items;
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.getItemSalesByInvoice = function () {
		if ($scope.loading) return;
		var selectedItem = $scope.selectedItem;
		if (!selectedItem || !selectedItem.Id || selectedItem.Id === 0) {
			alertbox({ content: "ابتدا یک کالا یا خدمات را اتخاب کنید" });
			return;
		}
		grid.beginCustomLoading();
		$scope.loading = true;
		$.ajax({
		    type:"POST",
		    data: JSON.stringify(selectedItem.Id),
		    url: "/app/api/KalaKhadmatApi/GetItemSalesByInvoice",
		    contentType: "application/json"
		}).done(function(res){
		    var data = res.data;
		//callws(DefaultUrl.MainWebService + 'GetItemSalesByInvoice', { type: sale ? 0 : 1, itemId: selectedItem.Id, start: $scope.fromDate, end: $scope.toDate })
            	$scope.loading = false;
            	grid.fill(data);
            	grid.endCustomLoading();
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	grid.endCustomLoading();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.itemSelect = function (item) {
		if (item) {
			$scope.selectedItem = item;
			$scope.getItemSalesByInvoice();
		}
	};
	$scope.editRptTitle = function () {
		$scope.showTitleInput = true;
		$scope.$apply();
		$("#inputRptTitle").focus();
	};

	function getPageCount(pageSize) {
		var rpp = pageSize.name === "A4portrait" ? 22 : 15;
		return Math.ceil(($scope.grid.gridData.length + 5) / rpp);
	};
	function getPrintImage(rect, pageSize, pageNumber, business, currency, todayDate) {
		var rpp = pageSize.name === "A4portrait" ? 22 : 15;
		var mm = rect.height / pageSize.height; // page height in mm
		var scale = 3;
		var tempCanvas = document.createElement("canvas");
		document.body.appendChild(tempCanvas);
		tempCanvas.style.display = "none";
		tempCanvas.width = rect.width * scale;
		tempCanvas.height = rect.height * scale;

		var font = "iransans";
		var fontSize = 10;

		var gr = new graphics(tempCanvas);
		gr.scale(scale, scale);
		var rptPageCount = getPageCount(pageSize);

		gr.fillRect(rect.left, rect.top, rect.width, rect.height, "#fff", "1px", 0);

		var topMargin = 10;
		var bottomMargin = 10;
		var r = { left: 10 * mm, top: topMargin * mm, width: rect.width - 20 * mm, height: rect.height - bottomMargin * mm };
		var n = 0;
		var strName = business.LegalName === "" ? business.Name : business.LegalName;

		var titleFontSize = fontSize + "pt";
		var headerFontSize = fontSize + "pt";
		var addressFont = fontSize + "pt";
		if (pageSize.name === "A5") {
			titleFontSize = fontSize + "pt";
			headerFontSize = fontSize + "pt";
			addressFont = fontSize + "pt";
		}

		gr.drawText(r.left, r.top, r.width, r.height, strName, font, titleFontSize, "#000", "top", "center", true, false, false, true);
		gr.drawText(r.left, r.top + 30, r.width, r.height, $scope.rptTitle, font, titleFontSize, "gray", "top", "center", true, false, false, true);
		gr.drawText(r.left, r.top + 25, r.width, r.height, todayDate, font, fontSize - 1 + "pt", "#000", "top", "left", false, false, false, true);
		gr.drawText(r.left, r.top + 60, r.width, r.height, "از " + $scope.date1 + " تا " + $scope.date2, font, fontSize + "pt", "gray", "top", "center", false, false, false, true);

		// table
		n += 95;

		var rowStart = n;
		gr.fillRect(r.left, r.top + n, r.width, 50, "#dcdcdc", "1px", 0);
		gr.drawLine(r.left, r.top + n, r.width + 40, r.top + n, "black", "1px", "solid");
		gr.drawLine(r.left, r.top + n + 50, r.width + 40, r.top + n + 50, "black", "1px", "solid");
		//        gr.drawRect(r.left, r.top + n, r.width, 50, "black", "1px", "solid", 0);
		n += 5;
		// عنوان ستون ها
		gr.drawText(r.left, r.top + n, r.width - 10, r.height, "ردیف", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 60, r.height, "تاریخ", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 140, r.height, "شماره فاکتور", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 250, r.height, "مشتری", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		gr.drawText(r.left, r.top + n, r.width - 410, r.height, "تعداد", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		if (pageSize.name === "A4portrait") {
			gr.drawText(r.left, r.top + n, r.width - 490, r.height, "مبلغ واحد", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 20, r.width - 497, r.height, "(" + currency + ")", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 640, r.height, "مبلغ کل", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 20, r.width - 647, r.height, "(" + currency + ")", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
		} else {
		}

		n += 55;
		var a = pageNumber === 1 ? 0 : (pageNumber - 1) * rpp;
		var b = pageNumber === 1 ? rpp : pageNumber * rpp;
		var items = $scope.grid.gridData.slice(a, $scope.grid.gridData[b - 1] ? b : $scope.grid.gridData.length);
		var length = items.length;
		for (var i = 0; i < length; i++) {
			var item = items[i];
			var isBold = false;
			var rowFontSize = fontSize - 2 + "pt";
			if (pageSize.name === "A4landscape") { isBold = true; rowFontSize = fontSize - 1 + "pt" }

			var rowNo = (pageNumber === 1 ? 0 : (pageNumber - 1) * rpp) + i + 1;
			// فیلد ها
			gr.drawText(r.width - 10, r.top + n, 50, r.height, rowNo, font, rowFontSize, "black", "top", "center", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 60, r.height, $rootScope.farsiDigit(item.DisplayDate), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.width - 193, r.top + n, 72, 20, item.InvoiceNumber, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.width - 408, r.top + n, 223, 20, item.ContactName, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 425, r.height, $rootScope.farsiDigit(item.Quantity), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			if (pageSize.name === "A4portrait") {
				gr.drawText(r.left, r.top + n, r.width - 470, r.height, $scope.money(item.UnitPrice), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
				gr.drawText(r.left, r.top + n, r.width - 590, r.height, $scope.money(item.TotalAmount), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			} else {
			}

			gr.drawLine(r.left, r.top + n + 30, r.width + 40, r.top + n + 30, "black", "1px", "solid");
			n += 40;
		}
		// خطوط عمودی گزارش
		gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 10, r.top + rowStart, r.width - 10, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 85, r.top + rowStart, r.width - 85, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 180, r.top + rowStart, r.width - 180, r.top + n - 10, "black", "1px", "solid"); // خط بعد از شماره فاکتور
		gr.drawLine(r.width - 360, r.top + rowStart, r.width - 360, r.top + n - 10, "black", "1px", "solid");
		if (pageSize.name === "A4portrait") {
			gr.drawLine(r.width - 420, r.top + rowStart, r.width - 420, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.width - 540, r.top + rowStart, r.width - 540, r.top + n - 10, "black", "1px", "solid");
		} else {

		}

		gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");

		if (pageNumber === rptPageCount) {  // if was last page print footer
			n += 10;
			gr.drawText(200, r.top + n, 202, 20, "جمع", font, "9pt", "black", "top", "right", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "solid");

			n += 35;
			gr.drawText(300, r.top + n, 202, 20, "جمع کل", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.totalSum), font, "10pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "solid");
			n += 35;

			gr.drawText(300, r.top + n, 202, 20, "جمع تخفیف", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.totalDiscount), font, "10pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "double");
			n += 35;

			gr.drawText(300, r.top + n, 202, 20, "جمع مالیات", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.totalTax), font, "10pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "double");
			n += 35;

			gr.drawText(300, r.top + n, 202, 20, "مبلغ کل", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.totalAmount), font, "10pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "double");
			n += 35;

			gr.drawText(300, r.top + n, 202, 20, "مبلغ کل پس از کسر مالیات", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.pureAmount), font, "10pt", "black", "top", "left", true, false, false, true);
		}

		//         ژیور
		if (n <= r.height - 10)
			gr.drawText(r.left, r.height - 10, r.width, r.height, "ژیور - www.smbt.ir", font, "10pt", "silver", "top", "center", true, false, false, true);

		if (rptPageCount > 1)
			gr.drawText(r.left, r.height - 10, r.width, r.height, "صفحه " + pageNumber + " از " + rptPageCount, font, fontSize - 1 + "pt", "black", "top", "left", true, false, false, true);

		tempCanvas.ctx = gr.ctx;

		return tempCanvas;
	}
	$scope.print = function () {
		grid.print();
	};
	$scope.pdf = function () {
		grid.pdf();
	};
	$scope.excel = function () {
		grid.exportToExcel(false);
	};
	
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});