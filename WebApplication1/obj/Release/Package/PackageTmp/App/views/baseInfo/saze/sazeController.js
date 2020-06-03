define(['application', 'combo', 'scrollbar', 'helper', 'editSaze', 'editContact', 'goroheSazeSelect', 'vahedSelect','noeSazeSelect','noeEjareSelect', 'dx', 'roweditor', 'gridHelper'], function (app) {
    app.register.controller('sazeController', ['$scope','$rootScope', '$location', '$compile',
        function ($scope, $rootScope, $location, $compile) {
	var grid;
	$scope.init = function () {
		$rootScope.pageTitle("رسانه ها");
		$('#businessNav').show();
		$scope.type = "all";
		$scope.nodeId = 0;

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: "grid-inventory",
			selection: { mode: "multiple" },
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
			columns: [
				{ caption: 'کـد', dataField: 'Code', width: 80, printWidth: 1.5, allowEditing: false },
				{ caption: 'گروه رسانه', dataField: 'GoroheName', printWidth: 5, allowEditing: false },
				{
				    caption: 'نام رسانه', printWidth: 7, dataField: 'Title', allowEditing: false, cellTemplate: function (cellElement, cellInfo) {
						cellElement.html("<a class='text-info txt-bold' href='#itemCard/" + cellInfo.data.Id + "'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
					}
				},
				{ caption: 'نوع رسانه', dataField: 'NoeSazeName', width: 120, printWidth: 2, printFormat: "#", allowEditing: false },
				{ caption: 'نوع اجاره', dataField: 'NoeEjareName', width: 120, printWidth: 2, printFormat: "#", allowEditing: false },
				{ caption: 'واحد', dataField: 'VahedName', width: 80, printWidth: 2, allowEditing: false },
				{
					caption: 'ویرایش', printVisible: false, dataField: '', width: 70, allowEditing: false, cellTemplate: function (cellElement, cellInfo) {
						cellElement.html("<span class='text-info txt-bold hand'>ویرایش</span>");
					}
				}
			],
			print: {
				fileName: "InventoryList.pdf",
				rowColumnWidth: 1.5,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: "فهرست رسانه"
				}
			},
			onCellClick: function (item) {
				if (!item.column || !item.data) return;
				if (item.column.caption === "ویرایش")
					$scope.editItem(item.data);
			}
		});

		$scope.getItems();

		$('#tabs a').click(function (e) {
			e.preventDefault();
			$(this).tab('show');
		});
		var inputFileFromExcel = document.getElementById('inputFileFromExcel');
		$(inputFileFromExcel).bind('change', function (e) {
			function clearFile() {
				inputFileFromExcel.type = "text";
				inputFileFromExcel.value = '';
				inputFileFromExcel.type = "file";
			}

			var file = e.target.files[0];
			var reader = new FileReader();
			reader.onload = function (e) {
				clearFile();
				function isExcel(data) {
					return true; return data.indexOf("excel;base64,") > -1;
				}
				var data = e.target.result;
				if (isExcel(data)) {
					questionbox({
						content: "آیا از انجام این عملیات مطمئن هستید؟ این عملیات غیر قابل بازگشت است.",
						onBtn1Click: function () { $scope.importItemsFromExcel(data); }
					});
				} else {
					alertbox({ content: 'لطفا فایل اکسل انتخاب کنید.' });
				}
			};

			reader.readAsDataURL(file);
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
	            //data: JSON.stringify(id),
	            url: "/app/api/Saze/GetAllByOrganId",
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
	$scope.viewItem = function (item) {
		$location.path("/itemCard/" + item.Id);
		return;
	};
	$scope.deleteItems = function () {
		var notDeletedCount = 0;
		function deleteItems(items, startIndex, count, successCallback, errorCallback) {
			var arr = [];
			for (var i = startIndex; i < items.length; i++) {
				arr.push(items[i].ID);
				if (arr.length === count)
					break;
			}
			$.ajax({
			    type: "POST",
			    data: JSON.stringify(arr),
			    url: "/app/api/Saze/DeleteSazeItems",
			    contentType: "application/json"
			}).done(function (res) {
			 
					//if (res.alertType === "danger") {
					//	errorCallback(res.message);
					//	return;
					//}
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
				var strMessage = deletedCount > 1 ? "رسانه ها " : "رسانه ";
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
			deleteItems(items, start, count, function() {
				setTimeout(function() {
					startDelete(items, start + count, count);
				}, 0);
			}, function(error) {
				alertbox({ content: error, type: "warning" });
				$scope.calling = false;
				$scope.$apply();
			});
		}
		var items = grid.getSelectedRowsData();
		if (items.length === 0) {
			alertbox({ content: "ابتدا يك يا چند رسانه را انتخاب كنيد" });
			return;
		}
		questionbox({
			content: "آيا از حذف رسانه های انتخاب شده مطمئن هستيد؟",
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
	$scope.enterBarcode = function () {
		$scope.searchBy = "barcode";
		$scope.searchValue = $scope.barcode;
		$scope.grid.goToPage(1);
	};
	$scope.searchBarcode = function () {
		$scope.grid.search($scope.barcode, $scope);
		clearTimeout($scope.timeoutBarcodeScan);
		$scope.timeoutBarcodeScan = setTimeout(function () {
			$("#barcodeScan").select();
		}, 3000);
	};
	$scope.importItemsFromExcel = function (file) {
		$scope.callingImport = true;
		callws(DefaultUrl.MainWebService + 'ImportItemsFromExcel', { excelFile: file })
            .success(function (result) {
            	$scope.callingImport = false;
            	$('#importExcelModal').modal('hide');
            	$scope.getItems();
            	applyScope($scope);
            	alertbox({
            		title: "ورود اطلاعات از اکسل",
            		content: "ورود اطلاعات از اکسل با موفقیت انجام شد.<br />" +
                        result[0] + " کالا/خدمات اضافه شد." + "<br />" +
                        result[1] + " کالا/خدمات بروزرسانی شد." + "<br />" +
                        "موجودی اول دوره " + result[2] + " کالا ثبت شد.<br />" +
                        "موجودی اول دوره " + result[3] + " کالا بروزرسانی شد."
            	});
            }).fail(function (error) {
            	$scope.callingImport = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.exportItemsToExcel = function () {
		if ($scope.callingImport) return;
		$scope.callingImport = true;
		$('#loadingModal').modal('show');
		callws(DefaultUrl.MainWebService + 'ExportItemsToExcel', {})
            .success(function (data) {
            	$scope.callingImport = false;
            	$('#loadingModal').modal('hide');
            	applyScope($scope);
            	var contentType = 'application/vnd.ms-excel';
            	var blob = b64toBlob(data, contentType);
            	if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            		window.navigator.msSaveOrOpenBlob(blob, "Hesabfa_Items.xlsx");
            	}
            	else {
            		var objectUrl = URL.createObjectURL(blob);
            		//                    window.open(objectUrl);
            		//                    var encodedUri = "data:application/vnd.ms-excel," + encodeURI(data);
            		var link = document.createElement("a");
            		link.href = objectUrl;
            		link.download = "Hesabfa_Items.xlsx";
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
	$scope.printBarcodeLabelDialog = function () {
		var gridSelectedItems = grid.getSelectedRowsData();
		if (gridSelectedItems.length === 0) {
			alertbox({ content: "ابتدا یک یا چند آیتم را انتخاب کنید." });
			return;
		}
		//				var items = [{ code: "10001", name: "کالای 1", barcode: "182738917398", count: 51, price: 123456789 },
		//				{ code: "10002", name: "کالای 2", barcode: "345563454234", count: 15, price: 150000 },
		//				{ code: "10003", name: "کالای 3", barcode: "454755635345", count: 31, price: 3758000 }];

		var items = [];
		for (var i = 0; i < gridSelectedItems.length; i++) {
			var r = gridSelectedItems[i];
			items.push({ code: r.DetailAccount.Code, name: r.Name, barcode: r.Barcode, count: 1, price: r.SellPrice });
		}

		function g(key, def) {
			var val = readCookie(key);
			if (typeof val == "undefined" || val == null)
				return def;
			return val;
		}
		var printTypes = ['چاپ روی کاغذ A4', 'چاپ لیبل'];
		if (!$scope.label) {
			$scope.label = {
			    title: g("lpt", $rootScope.currentBusiness.Name),
				width: Number(g("lpw", "50")),
				height: Number(g("lph", "30")),
				showItemName: g("lin", "true") === "true",
				showItemCode: g("lic", "true") === "true",
				showBarcode: g("lbb", "true") === "true",
				showPrice: g("lpp", "true") === "true",
				showBorder: g("lpb", "true") === "true",
				printType: Number(g("ltt", 0))
			}
		}
		$scope.label.items = items;

		$scope.printPopup = $("#printLabelPopup").dxPopup({
			rtlEnabled: true,
			showTitle: true,
			height: 'auto',
			title: "چاپ لیبل کالا",
			dragEnabled: true,
			closeOnOutsideClick: true,
			contentTemplate: function (contentElement) {
				var html = $('#printLabelPopupContent').html();
				$(html).appendTo(contentElement);
				$compile(contentElement.contents())($scope);
				$scope.showItemName = true;

				var el = $(".gridLabelCount", contentElement)[0];
				$scope.grid4 = dxGrid({
					element: el,
					height: 300,
					columnAutoWidth: true,
					editing: {
						mode: "cell",
						allowUpdating: true
					},
					columns: [
						{ caption: 'کد کالا', dataField: 'code', allowEditing: false },
						{ caption: 'نام کالا', dataField: 'name', allowEditing: false },
						{ caption: 'بارکد', dataField: 'barcode', allowEditing: false },
						{ caption: 'تعداد چاپ', dataField: 'count', dataType: "number" }
					]
				});
				$scope.grid4.fill(items);

				$(".chkItemName", contentElement).dxCheckBox({ text: 'درج نام کالا', value: $scope.label.showItemName, onValueChanged: function (data) { $scope.label.showItemName = data.value; } });
				$(".chkItemCode", contentElement).dxCheckBox({ text: 'درج کد کالا', value: $scope.label.showItemCode, onValueChanged: function (data) { $scope.label.showItemCode = data.value; } });
				$(".chkBarcode", contentElement).dxCheckBox({ text: 'درج بارکد کالا', value: $scope.label.showBarcode, onValueChanged: function (data) { $scope.label.showBarcode = data.value; } });
				$(".chkPrice", contentElement).dxCheckBox({ text: 'درج قیمت کالا', value: $scope.label.showPrice, onValueChanged: function (data) { $scope.label.showPrice = data.value; } });
				$(".chkBorder", contentElement).dxCheckBox({ text: 'ترسیم کادر', value: $scope.label.showBorder, onValueChanged: function (data) { $scope.label.showBorder = data.value; } });
				$(".radPrintType", contentElement).dxRadioGroup({
					items: printTypes,
					value: printTypes[$scope.label.printType],
					layout: "horizontal",
					onValueChanged: function (data) {
						for (var i = 0; i < printTypes.length; i++) {
							if (printTypes[i] === data.value)
								$scope.label.printType = i;
						}
					}
				});

			}
		}).dxPopup("instance");

		$scope.printPopup.show();
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
            
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});