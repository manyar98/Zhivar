define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','gridHelper'], function (app) {
    app.register.controller('purchaseController', ['$scope', '$rootScope','$stateParams',
        function ($scope, $rootScope, $stateParams) {

	var grid, cmb;
	var sale = !($stateParams.type === "purchase");
	var title = $scope.title = sale ? "فروش" : "خرید";
	var statuses = ['نمایش همه', 'نمایش کالاها', 'نمایش خدمات'];
	function applyFilter() {
		var val = cmb.option("value");
		if (val === statuses[0])
			grid.clearFilter();
		if (val === statuses[1])
			grid.filter(["IsGoods", "=", true]);
		if (val === statuses[2])
			grid.filter(["IsGoods", "=", false]);
	}

	$scope.init = function () {
		$rootScope.pageTitle(sale ? "گزارش فروش به تفکیک کالا و خدمات" : "گزارش خرید به تفکیک کالا");
		$('#businessNav').show();
		$scope.fromDate = "1397/01/01"; //$scope.currentFinanYear.DisplayStartDate;
		$scope.toDate = "1397/12/29"; //$scope.currentFinanYear.DisplayEndDate;
        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: 'grid-report-salesByItem',
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 70),
			columns: [
				{
					caption: 'کد', dataField: 'Code', width: 60, printWidth: 1.5, cellTemplate: function (cellElement, cellInfo) {
						cellElement.html("<a class='text-info' href='#itemCard/" + cellInfo.data.Id + "' target='_blank'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
					}
				},
				{ caption: 'گروه', printWidth: 5, dataField: 'NodeName' },
				{ caption: 'نام کالا / خدمات', printWidth: 7, dataField: 'Name' },
				{ caption: 'ت. ' + title, dataField: 'Stock', dataType: "number", format: "<b>#</b>", width: 100, printWidth: 2 },
				{ caption: 'مبلغ ' + title, dataField: 'Amount', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" },
				{ caption: 'ت. برگشت از ' + title, dataField: 'StockReturn', dataType: "number", format: "<b>#</b>", width: 100, printWidth: 3 },
				{ caption: 'مبلغ برگشت از ' + title, dataField: 'AmountReturn', dataType: "number", format: "#", width: 120, printWidth: 3.5, printFormat: "#" },
				{ caption: 'قیمت میانگین', dataField: 'Avg', dataType: "number", format: "#", width: 120, printWidth: 2.5, printFormat: "#" }
			]
            , print: {
            	fileName: "گزارش خرید و فروش.pdf",
            	rowColumnWidth: 1.5,
            	page: {
            		landscape: false
            	},
            	header: {
            	    title1: $rootScope.currentBusiness.Name,
            		title2: sale ? "گزارش فروش به تفکیک کالا و خدمات" : "گزارش خرید به تفکیک کالا",
            		left: $scope.currentFinanYear.Name,
            		center: function () {
            			return $scope.selectedDateRange;
            		}
            	}
            }, summary: {
            	totalItems: [
					{
						column: "Stock",
						summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.farsiDigit(data.value);
						}
					},
					{
						column: "Amount", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					},
					{
						column: "StockReturn", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					},
					{
						column: "AmountReturn", summaryType: "sum",
						customizeText: function (data) {
							return Hesabfa.money(data.value);
						}
					}
            	]
            }
		});

		$scope.getItemsAndServiceSales();
	};
	cmb = $("#selectStatus").dxSelectBox({
		dataSource: statuses,
		value: statuses[0],
		rtlEnabled: true,
		onValueChanged: function (data) {
			applyFilter();
		}
	}).dxSelectBox("instance");

	$scope.getItemsAndServiceSales = function () {
		if ($scope.loading) return;
		$scope.loading = true;
		grid.beginCustomLoading();
		$.ajax({
		    type:"POST",
		    url: "/app/api/Item/GetItemsAndServicePurchases",
		    contentType: "application/json"
		}).done(function(res){
		    var data = res.data;
            	$scope.loading = false;
            	grid.endCustomLoading();
            	grid.fill(data);
            	applyFilter();
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	grid.endCustomLoading();
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};

	function getPageCount(pageSize) {
		var rpp = pageSize.name === "A4portrait" ? 22 : 15;
		return Math.ceil(($scope.grid.gridData.length + 4) / rpp);
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
		if (pageSize.name === "A4portrait") {
			gr.drawText(r.left, r.top + n, r.width - 10, r.height, "ردیف", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 60, r.height, "کد " + ($scope.type === "product" ? "کالا" : ""), font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 130, r.height, "گروه", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 250, r.height, "نام " + $scope.getItemColumnTitle(), font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 410, r.height, "تعداد فروش", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 520, r.height, "مبلغ فروش", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 20, r.width - 527, r.height, "(" + currency + ")", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n, r.width - 640, r.height, "قیمت میانگین", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
			gr.drawText(r.left, r.top + n + 20, r.width - 657, r.height, "(" + currency + ")", font, fontSize - 1 + "pt", "black", "top", "right", true, false, false, true);
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
			gr.drawText(r.left, r.top + n, r.width - 60, r.height, $rootScope.farsiDigit(item.Code), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
			if (pageSize.name === "A4portrait") {
				gr.drawText(r.width - 143, r.top + n, 72, 20, item.NodeName, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
				gr.drawText(r.width - 368, r.top + n, 223, 20, item.Name, font, rowFontSize, "black", "top", "right", isBold, false, false, true);
				gr.drawText(r.left, r.top + n, r.width - 410, r.height, $rootScope.farsiDigit(item.salesQuantity), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
				gr.drawText(r.left, r.top + n, r.width - 500, r.height, $scope.money(item.salesAmount), font, rowFontSize, "black", "top", "right", isBold, false, false, true);
				gr.drawText(r.left + 20, r.top + n, r.left + 60, r.height, $scope.calculateAveragePrice(item), font, rowFontSize, "black", "top", "right", isBold, false, false, true);

			} else {
			}

			gr.drawLine(r.left, r.top + n + 30, r.width + 40, r.top + n + 30, "black", "1px", "solid");
			n += 40;
		}
		// خطوط عمودی گزارش
		gr.drawLine(r.width + 40, r.top + rowStart, r.width + 40, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 10, r.top + rowStart, r.width - 10, r.top + n - 10, "black", "1px", "solid");
		gr.drawLine(r.width - 65, r.top + rowStart, r.width - 65, r.top + n - 10, "black", "1px", "solid");
		if (pageSize.name === "A4portrait") {
			gr.drawLine(r.width - 140, r.top + rowStart, r.width - 140, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.width - 360, r.top + rowStart, r.width - 360, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.width - 450, r.top + rowStart, r.width - 450, r.top + n - 10, "black", "1px", "solid");
			gr.drawLine(r.width - 580, r.top + rowStart, r.width - 580, r.top + n - 10, "black", "1px", "solid");
		} else {

		}

		gr.drawLine(r.left, r.top + rowStart, r.left, r.top + n - 10, "black", "1px", "solid");

		if (pageNumber === rptPageCount) {  // if was last page print footer
			n += 10;
			gr.drawText(200, r.top + n, 202, 20, "جمع", font, "9pt", "black", "top", "right", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "solid");

			n += 35;
			gr.drawText(300, r.top + n, 202, 20, "فروش کالا", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.sumItems), font, "10pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "solid");
			n += 35;

			gr.drawText(300, r.top + n, 202, 20, "سایر فروش ها", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.otherSalesSum), font, "10pt", "black", "top", "left", true, false, false, true);
			gr.drawLine(r.left, r.top + n + 27, (r.width / 2) + 50, r.top + n + 27, "gray", "1px", "double");
			n += 35;

			gr.drawText(300, r.top + n, 202, 20, "کل فروش", font, "9pt", "black", "top", "left", true, false, false, true);
			gr.drawText(200, r.top + n, 95, 20, "(" + currency + ")", font, "8pt", "black", "top", "right", false, false, false, true);
			gr.drawText(60, r.top + n, 220, 20, $scope.money($scope.sumTotal), font, "10pt", "black", "top", "left", true, false, false, true);
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