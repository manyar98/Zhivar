define(['application', 'combo', 'scrollbar', 'helper', 'editItem', 'editContact', 'nodeSelect', 'dx', 'digitGroup', 'number', 'combo', 'displayNumbers', 'decimalPointZero', 'keyboardFilter', 'digitGroup', 'datePicker','maskedinput'], function (app) {
    app.register.controller('invoiceController', ['$scope','$rootScope', '$stateParams', '$location','$state',
        function ($scope, $rootScope, $stateParams, $location, $state) {

	//$scope.message = "omid mohammadi";
      
	var invoiceDueDateObj = new AMIB.persianCalendar('invoiceDueDate'); 
    var invoiceDateObj = new AMIB.persianCalendar('invoiceDate');
    $scope.automaticNumber = true;

	$("#automaticNumberSwitch").dxSwitch({
	    onText: 'اتوماتیک',
	    offText: 'دستی',
	    width: 80,
	    onValueChanged: function (data) {
	
	        $scope.automaticNumber = data.value;
	        //$scope.calculateInvoice(true);
	    }

	});

	$("#taxSwitch").dxSwitch({
	    onText: 'محاسبه مالیات',
	    offText: 'بدون مالیات',
	    width: 110,
	    value: true,
	    onValueChanged: function (data) {
	        $scope.autoCalTax = data.value;
	        $scope.calculateInvoice(true);
	    }

	});


    $scope.taxSwitchChecked = function (data) {

        $scope.autoCalTax = data;
        $scope.calculateInvoice(true);
    }

	$scope.init = function () {
	    $('#editDetailAccountModal').modal({ keyboard: false }, 'show');
		$rootScope.pageTitle("بارگیری فاکتور...");
		$("#businessNav").show();
		$("#contactSelect").hide();
		$scope.actRow = {
			Description: '',
			Quantity: 0,
			UnitPrice: 0,
			Discount: 0,
			Tax: 0
		};
		//if (!$scope.$scope.Hesabfa.businesses)
		//	$scope.getCurrentUserAndBusinessInfo($scope.updatePage);
		//else $scope.updatePage();

		$scope.alert = false;
		$scope.alertMessage = "";
		$("#barcodeOK").hide();
		$("#barcodeNoOK").hide();

        	$scope.comboContact = new HesabfaCombobox({
			items: [],
			containerEle: document.getElementById("comboContact"),
			toggleBtn: true,
			newBtn: true,
			deleteBtn: true,
			itemClass: "hesabfa-combobox-item",
			activeItemClass: "hesabfa-combobox-activeitem",
			itemTemplate: $scope.Hesabfa.comboContactTemplate,
			divider: true,
			matchBy: "contact.DetailAccount.ID",
			displayProperty: "{Name}",
			searchBy: ["Name", "Code"],
			onSelect: $scope.contactSelect,
			onNew: $scope.newContact
		});
        	$scope.comboItem = new HesabfaCombobox({
        	    items: [],
        	    containerEle: document.getElementById("comboItem"),
        	    toggleBtn: true,
        	    newBtn: true,
        	    deleteBtn: true,
        	    itemClass: "hesabfa-combobox-item",
        	    activeItemClass: "hesabfa-combobox-activeitem",
        	    itemTemplate: $scope.Hesabfa.comboItemTemplate,
        	    inputClass: "form-control input-sm input-grid-factor",
        	    matchBy: "item.DetailAccount.Id",
        	    displayProperty: "{Name}",
        	    searchBy: ["Name", "DetailAccount.Code"],
        	    onSelect: $scope.itemSelect,
        	    onNew: $scope.newItem,
        	    onDelete: $scope.deleteItem,
        	    divider: true
        	});

		// اگر جدید بود
		if ($stateParams.id === "new" || $stateParams.id === "newBill" ||
			$stateParams.id === "saleReturn" || $stateParams.id === "purchaseReturn" || $stateParams.id === "newWaste") {
			$scope.loadInvoiceData(0);
			$scope.automaticNumber = false;
		}
			// اگر مرجوعی یک فاکتور بخصوص بود
		//else if ($stateParams.id.indexOf("saleReturnId") > -1) {
		//	$scope.saleReturnId = $stateParams.id.split("=")[1];
		//	$scope.loadInvoiceData(0);
		//}
		//else if ($stateParams.id.indexOf("purchaseReturnId") > -1) {
		//	$scope.purchaseReturnId = $stateParams.id.split("=")[1];
		//	$scope.loadInvoiceData(0);
		//}
		//	// اگر کپی یک فاکتور بود
		//else if ($stateParams.id.indexOf("saleCopyId") > -1) {
		//	$scope.saleCopyId = $stateParams.id.split("=")[1];
		//	$scope.loadInvoiceData(0);
		//}
		//else if ($stateParams.id.indexOf("purchaseCopyId") > -1) {
		//	$scope.purchaseCopyId = $stateParams.id.split("=")[1];
		//	$scope.loadInvoiceData(0);
		//}
		//	// اگر جدید برای یک مشتری یا فروشنده خاص بود
		//else if ($stateParams.id.indexOf("new_contactId") > -1 || $stateParams.id.indexOf("newBill_contactId") > -1) {
		//	$scope.contactId = $stateParams.id.split("=")[1];
		//	$scope.loadInvoiceData(0);
		//}
			// اگر ویرایش یک فاکتور بود
		else
			$scope.loadInvoiceData($stateParams.id);

		$(".input-grid-factor").keydown(function (event) {
			if (event.which === 38) {    // up key
				$scope.goToTopRow($scope.actRow, event);
			}
			else if (event.which === 40) {  // down key
				$scope.goToBottomRow($scope.actRow, event);
			}
		});
		$("#inputTax").keydown(function (event) {
			if (event.which === 9) {    // tab key
				$scope.tabToNextRow($scope.actRow, event);
			}
		});
		$("#barcodeScan").keydown(function (event) {
			if (event.which === 13) {    // enter key
				$scope.enterBarcode($scope.actRow, event);
			}
		});

		var t = document.getElementById("comboItem");

        	
		var sendInvoiceSms = $rootScope.getUISetting("sendInvoiceSms");
		$scope.sendSmsByApprove = !sendInvoiceSms || sendInvoiceSms === "false" ? false : true;
		var showInvoiceTips = $rootScope.getUISetting("showInvoiceTips");
		$scope.showInvoiceTips = showInvoiceTips && showInvoiceTips === "true" ? true : false;
		if (showInvoiceTips == null) $scope.showInvoiceTips = true;
		var allowItemRepeat = $rootScope.getUISetting("allowItemRepeatInvoice");
		$scope.allowItemRepeat = allowItemRepeat && allowItemRepeat === "true" ? true : false;

	

		//$('[data-toggle="popover"]').popover();
		//$.getScript("/App/printReports.js", function () { });

		applyScope($scope);
		$(function () {
		    $.getScript("/App/printReports.js", function () { });
		});
	};

 
	$scope.loadInvoiceData = function (id) {
	    $scope.loading = true;

	   // var result = testApi();

	  //  callws("/app/api/FactorApi/loadInvoiceData", { id: id })
	  //   .success(function (result) {

	    $(function () {
	        //var id = 0;

	        $.ajax({
	            type: "POST",
	            data: JSON.stringify(id),
	            url: "/app/api/Invoice/loadInvoiceData",
	            contentType: "application/json"
	        }).done(function (res) {
	            //return res;
	            var result = res.data;
	            var moveItemEdit = false;
	            $scope.invoiceSettings = result.invoiceSettings;
	            $scope.footerNote = result.invoiceSettings.footerNote;
	            $scope.autoCalTax = $scope.invoiceSettings.autoAddTax;
	           applyScope($scope);

	            if ($scope.saleReturnId)
	                $scope.getInvoice4SaleReturn($scope.saleReturnId);
	            else if ($scope.purchaseReturnId)
	                $scope.getInvoice4PurchaseReturn($scope.purchaseReturnId);
	            else if ($scope.saleCopyId)
	                $scope.getInvoice4Copy($scope.saleCopyId);
	            else if ($scope.purchaseCopyId)
	                $scope.getInvoice4Copy($scope.purchaseCopyId);
	            else {
	                $scope.loading = false;
	                moveItemEdit = true;
	            }
	            $scope.invoice = result.invoice;
	            $scope.contacts = result.contacts;
	            $scope.comboContact.items = result.contacts;
	            $scope.items = result.items;
	            $scope.comboItem.items = result.items;
	            $scope.itemNodes = result.itemNodes;
	            $scope.invoiceItem = $scope.Hesabfa.newInvoiceItemObj();
	            $scope.item = $scope.Hesabfa.newItemObj();
	            $scope.contact = $scope.Hesabfa.newContactObj();
	            $scope.currentInvoiceStatus = $scope.invoice.Status;

	            $scope.sms = result.defaultSms;
	            $scope.showNote = $scope.invoice.Note !== "" ? true : false;

	            $("#imgLogo").attr("src", $scope.invoiceSettings.businessLogo);
	            var nextInvoiceNumber;
	            if ($stateParams.id === "new" || $scope.saleCopyId) {
	                nextInvoiceNumber = result.invoiceNextNumber;
	                if (nextInvoiceNumber) 
                        $scope.invoice.Number = nextInvoiceNumber;
	            }
	            else if ($stateParams.id === "newBill" || $scope.purchaseCopyId) $scope.invoice.InvoiceType = 1;
	            else if ($stateParams.id === "saleReturn" || $scope.saleReturnId) $scope.invoice.InvoiceType = 2;
	            else if ($stateParams.id === "purchaseReturn" || $scope.purchaseReturnId) $scope.invoice.InvoiceType = 3;
	            else if ($stateParams.id === "newWaste" || $scope.purchaseReturnId) {
	            	$scope.invoice.InvoiceType = 4;
	            	var products = [];
	            	var length = $scope.items.length;
	            	for (var i = 0; i < length; i++) {
	            		if ($scope.items[i].ItemType === 0)
	            			products.push($scope.items[i]);
	            	}
	            	$scope.items = products;
	            	$scope.comboItem.items = $scope.items;
	            }
	            //else if ($stateParams.id.indexOf("new_contactId") > -1) {
	            //	nextInvoiceNumber = result.invoiceNextNumber;
	            //	if (nextInvoiceNumber) $scope.invoice.Number = nextInvoiceNumber;
	            //	$scope.invoice.Contact = findById($scope.contacts, $scope.contactId);
	            //	$scope.contactInput = $scope.invoice.Contact.Name;
	            //}
	            //else if ($stateParams.id.indexOf("newBill_contactId") > -1) {
	            //	$scope.invoice.InvoiceType = 1;
	            //	$scope.invoice.Contact = findById($scope.contacts, $scope.contactId);
	            //	$scope.contactInput = $scope.invoice.Contact.Name;
	            //}

	            if ($scope.invoice.ID > 0 && $scope.invoice.Contact)
	                $scope.comboContact.setSelected($scope.invoice.Contact);

	            if ($scope.invoice.Status === 0) {
	                var settingAutoSave = $rootScope.getUISetting("editInvoiceAutoSave");
	                if (settingAutoSave && settingAutoSave === "true") {
	                    $scope.setAutoSave(true);
	                    $scope.autoSaveInvoice = true;
	                }
	            }

	            if ($scope.invoice.InvoiceType === 4)
	                $scope.autoCalTax = false;

	            $scope.setEditInvoicePageTitle();
	            applyScope($scope);
	            $scope.calculateInvoice();
	            angular.element(document).ready(function () {
	                if (moveItemEdit) $scope.moveRowEditor(0);
	            });
            
	            //}).fail(function (error) {
	            //	$scope.loading = false;
	            //	applyScope($scope);
	            //	if ($scope.accessError(error)) return;
	            //	alertbox({ content: error, title: "خطا" });
	            //}).loginFail(function () {
	            //	window.location = DefaultUrl.login;
	            //});
	            // Do something with the result :)
	        });
	    })
	     
	};
	$scope.getInvoice4SaleReturn = function (id) {
		callws(DefaultUrl.MainWebService + "GetInvoice", { id: id })
            .success(function (invoice) {
            	$scope.loading = false;
            	$scope.invoice.InvoiceItems = invoice.InvoiceItems;
            	var items = $scope.invoice.InvoiceItems;
            	for (var i = 0; i < items.length; i++) items[i].Id = 0;
            	$scope.invoice.Contact = invoice.Contact;
            	$scope.invoice.ContactTitle = invoice.ContactTitle;
            	$scope.comboContact.setSelected(invoice.Contact);
            	$scope.invoice.Reference = invoice.Number;
            	$scope.calculateInvoice();
            	$scope.$apply();
            	angular.element(document).ready(function () {
            		$scope.moveRowEditor(0);
            	});
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.getInvoice4PurchaseReturn = function (id) {
		callws(DefaultUrl.MainWebService + "GetInvoice", { id: id })
            .success(function (invoice) {
            	$scope.loading = false;
            	$scope.invoice.InvoiceItems = invoice.InvoiceItems;
            	var items = $scope.invoice.InvoiceItems;
            	for (var i = 0; i < items.length; i++) items[i].Id = 0;
            	$scope.invoice.Contact = invoice.Contact;
            	$scope.invoice.ContactTitle = invoice.ContactTitle;
            	$scope.comboContact.setSelected(invoice.Contact);
            	$scope.invoice.Reference = invoice.Reference;
            	$scope.calculateInvoice();
            	$scope.$apply();
            	angular.element(document).ready(function () {
            		$scope.moveRowEditor(0);
            	});
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.getInvoice4Copy = function (id) {
		callws(DefaultUrl.MainWebService + "GetInvoice", { id: id })
            .success(function (invoice) {
            	$scope.loading = false;
            	$scope.invoice.InvoiceItems = invoice.InvoiceItems;
            	var items = $scope.invoice.InvoiceItems;
            	for (var i = 0; i < items.length; i++) items[i].Id = 0;
            	$scope.invoice.Contact = invoice.Contact;
            	$scope.comboContact.setSelected(invoice.Contact);
            	$scope.contactInput = invoice.Contact.Name;
            	$scope.invoice.ContactTitle = invoice.ContactTitle;
            	$scope.calculateInvoice();
            	$scope.$apply();
            	angular.element(document).ready(function () {
            		$scope.moveRowEditor(0);
            	});
            }).fail(function (error) {
            	$scope.loading = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.moveRowEditor = function (index, $event) {
		if ($scope.actRow && $scope.actRow.RowNumber  === index) return;
		$scope.actRow = $scope.invoice.InvoiceItems[index];
		$("#comboItem").prependTo("#tdItem" + index);
		$("#inputDescription").prependTo("#tdDescription" + index);
		$("#inputQuantity").prependTo("#tdStock" + index);
		$("#inputUnitPrice").prependTo("#tdUnitPrice" + index);
		$("#inputDiscount").prependTo("#tdDiscount" + index);
		$("#inputTax").prependTo("#tdTax" + index);
		if ($scope.actRow)
			$scope.comboItem.setSelected($scope.actRow.Item);
		applyScope($scope);
		if ($event) $('#' + $event.currentTarget.id + " input").select();
	};
	$scope.outRowEditor = function () {
		$("#comboItem").prependTo("#hideme");
		$("#inputDescription").prependTo("#hideme");
		$("#inputQuantity").prependTo("#hideme");
		$("#inputUnitPrice").prependTo("#hideme");
		$("#inputDiscount").prependTo("#hideme");
		$("#inputTax").prependTo("#hideme");
	};
	$scope.tabToNextRow = function (actRow, event) {
		$scope.calculateInvoice();
		if ($scope.invoice.InvoiceItems.length > actRow.RowNumber) {
			event.preventDefault();
			$scope.moveRowEditor(actRow.RowNumber);
			$("#itemInputitemSelectInput").select();
		}
	};
	$scope.enterBarcode = function (actRow) {
		if ($scope.barcode === "") return;
		var length = $scope.items.length;
		for (var i = 0; i < length; i++) {
			if ($scope.items[i].Barcode === $scope.barcode) {
				if ($scope.actRow && $scope.actRow.Item)
					$scope.moveRowEditor($scope.actRow.RowNumber);
				$scope.itemSelect($scope.items[i], "enter", actRow);
				$("#barcodeScan").select();
				$("#barcodeOK").toggle("blink");
				$("#barcodeOK").toggle("blink");
				$("#barcodeOK").toggle("blink");
				$("#barcodeOK").toggle("blink");
				return;
			}
		}
		$("#barcodeScan").select();
		$("#barcodeNoOK").toggle("blink");
		$("#barcodeNoOK").toggle("blink");
		$("#barcodeNoOK").toggle("blink");
		$("#barcodeNoOK").toggle("blink");
	};
	$scope.goToTopRow = function (actRow, event) {
		$scope.calculateInvoice();
		if (actRow.RowNumber > 1) {
			event.preventDefault();
			$scope.moveRowEditor(actRow.RowNumber - 2);
			event.target.select();
		}
	};
	$scope.goToBottomRow = function (actRow, event) {
		$scope.calculateInvoice();
		if (actRow.RowNumber < $scope.invoice.InvoiceItems.length) {
			event.preventDefault();
			$scope.moveRowEditor(actRow.RowNumber);
			event.target.select();
		}
	};
	$scope.setEditInvoicePageTitle = function () {
		if ($scope.invoice.InvoiceType === 0) $rootScope.pageTitle("فاکتور" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
		else if ($scope.invoice.InvoiceType === 1) $rootScope.pageTitle("صورتحساب" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
		else if ($scope.invoice.InvoiceType === 2) $rootScope.pageTitle("برگشت از فروش" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
		else if ($scope.invoice.InvoiceType === 3) $rootScope.pageTitle("برگشت از خرید" + ($scope.invoice.Contact ? " " + $scope.invoice.Contact.Name : ""));
		else if ($scope.invoice.InvoiceType === 4) $rootScope.pageTitle("ضایعات کالا");
	};
	$scope.selectFromTree = function () {
		$scope.itemSelectModal = true;
	};

	$scope.newContact = function () {
		$scope.alert = false;
		$scope.contact = null;
		$scope.editContactModal = true;
		$scope.$apply();
	};
	$scope.getEditedContact = function (contact) {
		if (!contact) return;
		$scope.contacts.push(contact);
		$scope.editContactModal = false;
		$scope.invoice.Contact = contact;
		$scope.invoice.ContactTitle = contact.Name;
		$scope.setEditInvoicePageTitle();
		$scope.comboContact.setSelected(contact);
		$scope.$apply();
	};

	$scope.itemSelect = function (item, selectedBy) {
		$scope.activeIvoiceItem = $scope.actRow;
		$scope.activeIvoiceItem.calcTax = true;

		if (!item) {
			$scope.activeIvoiceItem.Item = null;
			$scope.activeIvoiceItem.ItemInput = "";
			$scope.$apply();
			return false;
		}
		// first check repetation
		if (!$scope.allowItemRepeat) {
			var items = $scope.invoice.InvoiceItems;
			for (var i = 0; i < items.length; i++) {
				if (items[i].Item && items[i].Item.ID === item.ID && (items[i].RowNumber !== $scope.actRow.RowNumber)) {
					items[i].Quantity++;
					$scope.calculateInvoice();
					applyScope($scope);
					// repeated item then stay, add count and select current input text then notify user
					DevExpress.ui.notify($scope.Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"), "success", 3000);
					//					notification(Hesabfa.farsiDigit("آیتم تکراری به تعداد ردیف " + (i + 1) + " اضافه شد"));
					$scope.comboItem.setSelected(null);
					return false;
				}
			}
		}

		$scope.activeIvoiceItem.Item = item;
		if (!item.DetailAccount) {
			$scope.activeIvoiceItem.Item.DetailAccount = {};
			$scope.activeIvoiceItem.Item.DetailAccount.Id = item.DetailAccountId;
		}
		$scope.activeIvoiceItem.ItemInput = item.Name;
		if ($scope.activeIvoiceItem.Quantity === 0) $scope.activeIvoiceItem.Quantity = 1;
		if ($scope.invoice.InvoiceType === 0 || $scope.invoice.InvoiceType === 2) { // فروش
			if (item.SalesTitle !== "") $scope.activeIvoiceItem.Description = item.SalesTitle;
		} else if ($scope.invoice.InvoiceType === 1 || $scope.invoice.InvoiceType === 3 || $scope.invoice.InvoiceType === 4) { // خرید
			if (item.PurchasesTitle !== "") $scope.activeIvoiceItem.Description = item.PurchasesTitle;
		}

		$scope.activeIvoiceItem.UnitPrice = $scope.invoice.InvoiceType === 0 || $scope.invoice.InvoiceType === 2 ? item.SellPrice : item.BuyPrice;
		$scope.calculateInvoice();
		$scope.$apply();
		if (selectedBy === 'enter') {
			if ($scope.activeIvoiceItem.RowNumber === $scope.invoice.InvoiceItems.length)
				$scope.addRow();
			$scope.moveRowEditor($scope.actRow.RowNumber);
			//            $(':input:eq(' + ($(':input').index(this)) + ')').focus();
			//            $('#tdItem' + activeRow.RowNumber + ' input').focus();
			$('#itemInputitemSelectInput').select();
			$('#itemInputitemSelectInput').val('');
			$('#itemInputitemSelectInput').focus();
		}
		return false;
	};
	$scope.newItem = function (invoiceItem) {
		$scope.activeIvoiceItem = invoiceItem;
		$scope.alert = false;
		$scope.item = null;
		$scope.editItemModal = true;
		$scope.$apply();
	};
	$scope.deleteItem = function () {
		$scope.actRow.Item = null;
		$scope.actRow.Description = "";
		$scope.actRow.Discount = 0;
		$scope.actRow.Tax = 0;
		$scope.actRow.Sum = 0;
		$scope.actRow.TotalAmount = 0;
		$scope.actRow.UnitPrice = 0;
		$scope.actRow.Quantity = 0;
	};
	$scope.getEditedItem = function (item) {
		if (!item) return;
		$scope.items.push(item);
		$scope.editItemModal = false;
		$scope.comboItem.setSelected(item);
		$scope.itemSelect(item, "new", $scope.activeIvoiceItem);
		applyScope($scope);
	};
	$scope.contactSelect = function (contact) {
		if (contact) {
			$scope.invoice.Contact = contact;
			//			$scope.invoice.Contact.DetailAccount = {};
			//			$scope.invoice.Contact.DetailAccount.Id = item.DetailAccountId;
			$scope.invoice.ContactTitle = contact.Name;
            $scope.setEditInvoicePageTitle();
            $scope.setTitle();
			applyScope($scope);
		}
	};
	$scope.getContactBalance = function (balance) {
		if (balance > 0) return balance;
		else if (balance < 0) return balance * -1;
		else return 0;
	};
	$scope.getContactTashkhis = function (balance) {
		if (balance > 0) return "(بدهکار)";
		else if (balance < 0) return "(بستانکار)";
		else return "";
	};
	$scope.getContactBalanceClass = function (balance) {
		if (balance > 0) return "red";
		else if (balance < 0) return "green";
		else return "blue";
	};

	$scope.selectFromTree = function (invoiceItem) {
		$scope.activeIvoiceItem = invoiceItem;
		$scope.alert = false;
		$scope.item = null;
		$scope.itemSelectModal = true;
		applyScope($scope);
	};
	$scope.getSelectedItem = function (item) {
		if (!item) return;
		$scope.itemSelectModal = false;
		$scope.itemSelect(item, "new", $scope.activeIvoiceItem);
		$scope.comboItem.setSelected(item);
		if ($scope.actRow.RowNumber === $scope.invoice.InvoiceItems.length)
			$scope.addRow(1);
		$scope.moveRowEditor($scope.actRow.RowNumber);
		$scope.$apply();
	};

	$scope.calculateInvoice = function (calTax) {
		if (!$scope.invoice) return;
		$scope.invoice.Sum = 0;
		$scope.invoice.Payable = 0;
		$scope.totalTax = 0;
		$scope.totalDiscount = 0;
		$scope.totalQuantity = 0;
		var length = $scope.invoice.InvoiceItems.length;
		var taxRate = $rootScope.currentBusiness.TaxRate;
		for (var i = 0; i < length; i++) {
			var invoiceItem = $scope.invoice.InvoiceItems[i];
			invoiceItem.ItemInput = invoiceItem.Item ? invoiceItem.Item.Name : ""; // new
			invoiceItem.Quantity = parseFloat((invoiceItem.Quantity + "").replace("/", "."));
			invoiceItem.Sum = invoiceItem.UnitPrice * invoiceItem.Quantity;
			invoiceItem.Sum = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Sum * 100) / 100 : Math.round(invoiceItem.Sum);
			$scope.totalQuantity += invoiceItem.Quantity;

			var strDiscount = invoiceItem.Discount.toString();
			if (strDiscount.charAt(strDiscount.length - 1) === '%') {
				strDiscount = strDiscount.substr(0, strDiscount.length - 1);
				invoiceItem.Discount = invoiceItem.Sum * parseInt(strDiscount) / 100;
			}
			invoiceItem.Discount = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Discount * 100) / 100 : Math.round(invoiceItem.Discount);

			if (calTax) {
				if ($scope.invoice.InvoiceType === 4)
					invoiceItem.Tax = 0;
				else if ($scope.autoCalTax) {
					invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
					invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
				} else {
					invoiceItem.Tax = 0;
				}
				invoiceItem.calcTax = false;
			} else if ($scope.autoCalTax && invoiceItem.calcTax) {
				invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
				invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
				invoiceItem.calcTax = false;
			}

			//			}
			//				if ($scope.autoCalTax) {
			//				if ($scope.autoCalTax) {
			//					if (invoiceItem.calcTax) {
			//						invoiceItem.Tax = (invoiceItem.Sum - invoiceItem.Discount) * (taxRate / 100);
			//						invoiceItem.Tax = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Tax * 100) / 100 : Math.round(invoiceItem.Tax);
			//						invoiceItem.calcTax = false;
			//					}
			//				} else {
			//				}
			//			}

			invoiceItem.TotalAmount = invoiceItem.Sum - parseFloat(invoiceItem.Discount + "") + parseFloat(invoiceItem.Tax + "");
			$scope.invoice.Sum += invoiceItem.Sum;
			$scope.invoice.Payable += invoiceItem.TotalAmount;
           
			$scope.totalTax += parseFloat(invoiceItem.Tax + "");
			$scope.totalDiscount += parseFloat(invoiceItem.Discount + "");
		}
		$scope.invoice.Payable = $rootScope.isDecimalCurrency() ? Math.round($scope.invoice.Payable * 100) / 100 : Math.round($scope.invoice.Payable);
        $scope.invoice.Rest = $scope.invoice.Payable - $scope.invoice.Paid;
		applyScope($scope);
	};
	$scope.addDiscount = function () {
		var length = $scope.invoice.InvoiceItems.length;
		for (var i = 0; i < length; i++) {
			var invoiceItem = $scope.invoice.InvoiceItems[i];
			if (invoiceItem.Sum > 0) {
				invoiceItem.calcTax = true;
				invoiceItem.Discount = invoiceItem.Sum * ($scope.discount / 100);
				invoiceItem.Discount = $rootScope.isDecimalCurrency() ? Math.round(invoiceItem.Discount * 100) / 100 : Math.round(invoiceItem.Discount);
			}
		}
		$scope.calculateInvoice();
	};
	$scope.addRow = function (n) {
		if (!n) n = 1;
		for (var i = 0; i < n; i++) {
			var newInvoiceItem = {};
			angular.copy($scope.invoiceItem, newInvoiceItem);
			newInvoiceItem.RowNumber = $scope.invoice.InvoiceItems.length + 1;
			$scope.invoice.InvoiceItems.push(newInvoiceItem);
		}
		//$scope.$apply();
	};
	$scope.deleteRow = function (invoiceItem) {
		if ($scope.invoice.InvoiceItems.length === 1) return;   // if only one remaind, do not delete
		// prevent removing autocompletes from page
		if ($scope.actRow.RowNumber === invoiceItem.RowNumber) {
			if (invoiceItem.RowNumber > 1)
				$scope.moveRowEditor(invoiceItem.RowNumber - 2);
			else
				$scope.moveRowEditor(invoiceItem.RowNumber);
		}

		// remove invoice item from InvoiceItems list
		var items = $scope.invoice.InvoiceItems;
		findAndRemoveByPropertyValue(items, 'RowNumber', invoiceItem.RowNumber);
		for (var i = 0; i < items.length; i++)  // add row numbers again
			items[i].RowNumber = i + 1;
		$scope.calculateInvoice();  // recalculate invoice
		$scope.$apply();
	};
	$scope.moveUpRow = function (invoiceItem) {
		var items = $scope.invoice.InvoiceItems;
		items.moveUp(invoiceItem);
		for (var i = 0; i < items.length; i++)
			items[i].RowNumber = i + 1;
		$scope.$apply();
	};
	$scope.moveDownRow = function (invoiceItem) {
		var items = $scope.invoice.InvoiceItems;
		items.moveDown(invoiceItem);
		for (var i = 0; i < items.length; i++)
			items[i].RowNumber = i + 1;
		$scope.$apply();
	};
	$scope.getReferenceTooltip = function () {
		if (!$scope.invoice) return "شماره ارجاع";
		switch ($scope.invoice.InvoiceType) {
			case 0: return "شماره ارجاع";
			case 1: return "شماره ارجاع (شماره فاکتور خرید)";
			case 2: return "شماره ارجاع (شماره فاکتور فروش)";
			case 3: return "شماره ارجاع (شماره فاکتور خرید)";
			default: return "شماره ارجاع";
		}
	};
	$scope.enterTax = function (invoiceItem) {
		if (invoiceItem.Tax === 0)
			invoiceItem.Tax = $scope.Hesabfa.taxRate && $scope.Hesabfa.taxRate > 0 ? $scope.Hesabfa.taxRate + "%" : "0";
		applyScope($scope);
		$("#inputTax").select();
	};

	$scope.saveInvoice = function (command) {

	 
		if ($scope.calling) return;
		$scope.alert = false;
		$scope.alertMessage = "";
		$scope.alertMessages = [];

		if (!$scope.invoice) return;
		var n = $scope.invoice.Number;
		$scope.calling = true;
		if (command === "saveAndSubmitForApproval") $scope.invoice.Status = 1;
		if (command === "approve" || command === "approveAndNext" || command === "approveAndPrint") $scope.invoice.Status = 2;

		//	$scope.invoice.DisplayDate = $("#invoiceDate").val();
		//	$scope.invoice.DisplayDueDate = $("#invoiceDueDate").val();
		var rowIndex = $scope.actRow.RowNumber;


		//callws(DefaultUrl.MainWebService + "SaveInvoice", { invoice: $scope.invoice, sendSms: $scope.sendSmsByApprove })
        //    .success(function (result) {

            $.ajax({
                type:"POST",
                data: JSON.stringify($scope.invoice),
                url: "/app/api/Invoice/SaveInvoice",
                contentType:"application/json"

            }).done(function (res) {
                
                if (res.resultCode === 1) {
                    $scope.calling = false;
                    if ($scope.accessError(res.data)) { applyScope($scope); return; }
                    alertbox({ content: res.data, type: "warning" });
                    $scope.invoice.Status = $scope.currentInvoiceStatus;
                    applyScope($scope);
                    window.scrollTo(0, 0);
                    return;
                }
             
                var result = res.data;
            	var smsSent = result.smsSent ? 1 : 0;

              //var invoice = result.invoice;

            	var save = function () {
            		//var path = "";

            		//$state.go('invoices');

                    if ($scope.invoice.InvoiceType === 0)
                        $state.go('invoicesSell', { show: 'new' });
                    else if ($scope.invoice.InvoiceType === 1)
                        $state.go('invoicesPurchase', { show: 'bills' });
                    else if ($scope.invoice.InvoiceType === 2)
                        $state.go('invoicesSaleReturn', { show: 'saleReturn' });
                    else if ($scope.invoice.InvoiceType === 3)
                        $state.go('invoicesPurchaseReturn', { show: 'purchaseReturn' });

            		//if ($scope.invoice.InvoiceType === 0)
            		//	path = "/invoices/status=draft&sms={0}&invoiceId={1}&invoiceNumber={2}&invoiceReference={3}&invoiceType={4}&contactName={5}&price={6}"
					//		.formatString(smsSent, result.id, $scope.invoice.Number, $scope.invoice.Reference, $scope.invoice.InvoiceType, $scope.invoice.Contact.Name, $scope.invoice.Payable);
            		//else if ($scope.invoice.InvoiceType === 1)
            		//	path = "/invoices/show=bills";
            		//$location.path(path);
            		$scope.$apply();
            		return;
            	};
            	var saveAndSubmitForApproval = function () {

            	    $state.go('viewInvoice', { id: result.ID, status: "awaitingApproval" });
            		//$location.path("/ViewInvoice/status=awaitingApproval&sms=" + smsSent + "&id=" + result.id);
            		$scope.$apply();
            		return;
            	};
            	var approve = function () {
            	    //$location.path("/ViewInvoice");
            	  
            	    $state.go('viewInvoice', { id: result.ID, status: "approved" });
            		//$location.path("/ViewInvoice/status=approved&sms=" + smsSent + "&id=" + result.id);
            		$scope.$apply();
            		return;
            	};
            	var approveAndPrint = function () {
            	    var status = "approved";
            	    $state.go('viewInvoice', { status:status,print:1,sms:smsSent,id: result.ID });
            		//$location.path("/ViewInvoice/status=approved&print=true&sms=" + smsSent + "&id=" + result.id);
            		$scope.$apply();
            		return;
            	};

            	$scope.calling = false;
            	if (command === "saveAndContinueEdit") {
            		$scope.invoice.ID = result.ID;
            		$scope.alert = true;
            		$scope.alertType = "success";
            		$scope.alertMessage = "فاکتور {2} ذخیره شد - {0} - مبلغ کل: {1}".
                        formatString($scope.invoice.Contact.Name, $scope.money($scope.invoice.Payable), $scope.invoice.Number, $scope.invoice.InvoiceType === 0 ? "فروش" : "خرید");
            		$scope.actRow = null;
            		$scope.outRowEditor();
            		//            		$scope.invoice = $scope.invoice;
            		$scope.$apply();
            		if (rowIndex > $scope.invoice.InvoiceItems.length)
            			$scope.moveRowEditor(0);
            		else
            			$scope.moveRowEditor(rowIndex - 1);
            		return;
            	}
            	else if (command === "saveAndSubmitForApproval") {
            		saveAndSubmitForApproval();
            	}
            	else if (command === "save") {
            		if ($scope.invoice.Status === 1) saveAndSubmitForApproval();
            		else save();
            	}
            	else if (command === "approve") {
            		approve();
            	}
            	else if (command === "approveAndPrint") {
            		approveAndPrint();
            	}
            	else if (command === "saveAndNext") {
            		$scope.alert = true;
            		$scope.alertType = "success";
            		$scope.alertMessage = "فاکتور {2} ذخیره شد - {0} - مبلغ کل: {1}".
                        formatString($scope.invoice.Contact.Name, $scope.money($scope.invoice.Payable), $scope.invoice.Number, $scope.invoice.InvoiceType === 0 ? "فروش" : "خرید");

            		$scope.actRow = null;
            		$scope.outRowEditor();
            		$scope.loadInvoiceData(0);
            		$scope.contactInput = "";
            		$scope.$apply();

            		return;
            	}
            	else if (command === "approveAndNext") {
            		$scope.alert = true;
            		$scope.alertType = "success";
            		$scope.alertMessage = "فاکتور {2} تایید شد - {0} - مبلغ کل: {1}".
                        formatString($scope.invoice.Contact.Name, $scope.money($scope.invoice.Payable), $scope.invoice.Number, $scope.invoice.InvoiceType === 0 ? "فروش" : "خرید");
            		if (result.smsSent) $scope.alertMessage += " [پیامک ارسال شد]";

            		$scope.actRow = null;
            		$scope.outRowEditor();
            		$scope.loadInvoiceData(0);
            		$scope.contactInput = "";
            		$scope.$apply();

            		return;
            	}

            }).fail(function(error){
            
                $scope.calling = false;
            	if ($scope.accessError(error)) { applyScope($scope); return; }
            	alertbox({ content: error, type: "warning" });
            	$scope.invoice.Status = $scope.currentInvoiceStatus;
            	applyScope($scope);
            	window.scrollTo(0, 0);
            });
            
          
	};
	$scope.setAutoSave = function (switchOnOff) {
		if (switchOnOff) {
			$scope.Hesabfa.autoSaveTimerInvoice = window.setInterval(function () {
				$scope.saveInvoice("saveAndContinueEdit");
			}, 60000);
			$rootScope.setUISetting("editInvoiceAutoSave", "true");
		}
		else {
			window.clearInterval($scope.Hesabfa.autoSaveTimerInvoice);
			$rootScope.setUISetting("editInvoiceAutoSave", "false");
		}
	};
	$scope.setSendSms = function () {
		$scope.sendSmsByApprove = !$scope.sendSmsByApprove ? true : false;
		if ($scope.sendSmsByApprove)
			$rootScope.setUISetting("sendInvoiceSms", "true");
		else
			$rootScope.setUISetting("sendInvoiceSms", "false");
	};
	$scope.setAllowItemRepeat = function () {
		$scope.allowItemRepeat = !$scope.allowItemRepeat ? true : false;
		if ($scope.allowItemRepeat)
			$rootScope.setUISetting("allowItemRepeatInvoice", "true");
		else
			$rootScope.setUISetting("allowItemRepeatInvoice", "false");
	};
	$scope.setAutoCalTax = function () {
		$scope.autoCalTax = !$scope.autoCalTax ? true : false;
	};
	$scope.cancel = function () {
		window.history.back();
	};
	$scope.openSmsEditor = function () {
		$scope.sendSmsByApprove = !$scope.sendSmsByApprove ? true : false;
		$scope.editSmsModal = true;
		applyScope($scope);
	};
	$scope.getEditedSms = function (sms) {
		callws(DefaultUrl.MainWebService + "SaveSetting", { name: "SMS_DefaultInvoiceSMS", value: sms })
            .success(function () {
            	return;
            }).fail(function (error) {
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.openBarcodeList = function () {
		messagebox({
			title: "ورود گروهی توسط بارکد",
			buttonCount: 2,
			btn1Title: "تایید", btn1Class: "btn btn-sm btn-primary",
			btn2Title: "انصراف", btn2Class: "btn btn-sm btn-default btn-smoke",
			content: "<div>ورود گروهی توسط بارکد. هر بارکد را در یک سطر وارد کنید.</div>" +
                "<textarea id='barcodeTextArea' style='width: 100%;border: 1px solid silver';" +
                " rows='10' spellcheck='false' ng-trim='false'></textarea>",
			onBtn1Click: function () {
				var lines = $('#barcodeTextArea').val().split('\n');
				var notExist = "";
				for (var i = 0; i < lines.length; i++) {
					if (lines[i] === "") continue;
					var found = false;
					var length = $scope.items.length;
					for (var n = 0; n < length; n++) {
						if ($scope.items[n].Barcode === lines[i]) {
							$scope.itemSelect($scope.items[n], "enter", $scope.actRow);
							found = true;
						}
					}
					if (!found) notExist += lines[i] + "<br/>";
				}
				if (notExist !== "")
					alertbox({ content: "بارکدهای زیر پیدا نشد:<br/>" + notExist });
			}
		});
		$('#barcodeTextArea').val("");
		setTimeout(function () { $('#barcodeTextArea').focus(); }, 500);
	};
	$scope.$on("$destroy", function () {
		window.clearInterval($scope.Hesabfa.autoSaveTimerInvoice);
	});

	$scope.printProformaInvoice = function () {
		var settings = jQuery.extend(true, {}, $scope.invoiceSettings);
		settings.footerNote = $scope.invoice.Note + "\n" + settings.footerNote + "\n";
		settings.footerNoteDraft = $scope.invoice.Note + "\n" + settings.footerNoteDraft + "\n";

               var business ={
                Name:"ژیور",
                LegalName: "ژیور",
                Address: "",
                PostalCode: "",
                    Fax:""
                };

		printInvoice($scope.invoice, settings, $scope.totalDiscount, $scope.totalTax, null, business, $scope.getCurrency());
	};
	$scope.pdfProformaInvoice = function (asString, doAfterRead) {
		if ($scope.invoice.Status === 0 && $scope.invoice.InvoiceType === 0)
		    $scope.invoiceSettings.footerNote = $scope.footerNote + "\n" + $scope.invoice.Note;

		var business = {

		    Address: "",
	    BusinessLine: "",
	    CalendarType: 0,
	    City: "",
	    Country: "",
	    Currency: 0,
	    DocCredit: 0,
	    EconomicCode: "",
	    Email: "",
	    ExpireDisplayDate: "1398/01/07",
	    Fax: "",
	    Id: 13798,
	    IsExpire: false,
	    LegalName: "",
	    Name: "ژیور",
	    NationalCode: "",
	    OrganizationType: 0,
	    Phone: "",
	    PostalCode: "",
	    RegisterationDisplayDate: "1397/12/06",
	    RegistrationNumber: "",
	    SetupStep: 7,
	    State: "",
	    SubscriptionLevel: 0,
	    TaxRate: 9,
	    Website: "",
	    setupInProgress: false,
	    token: "cW5GSAXUjtOZ-BfMy-mQVw..",
	  
		};

		generateInvoicePDF($scope.invoice, $scope.invoiceSettings, $scope.totalDiscount, $scope.totalTax, asString, doAfterRead, null, null, null, business, $scope.getCurrency());
	};
	$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
	//

    $scope.setTitle = function () {
        var jenset = "";
        var company = "";

        if (angular.isDefined($scope.invoice.Contact.Jensiat) && $scope.invoice.Contact.Jensiat !== null) {
            if ($scope.invoice.Contact.Jensiat === 1)
                jenset = " جناب آقای ";
            else if ($scope.invoice.Contact.Jensiat === 1)
                jenset = " سرکار خانم ";
        }

        if (angular.isDefined($scope.invoice.Contact.Company) && $scope.invoice.Contact.Company !== null) {
            company = " به نمایندگی " + $scope.invoice.Contact.Company;

        };

        $scope.invoice.ContactTitle = jenset + $scope.invoice.Contact.Name + company;
    };

	function applyScope($scope) {
	    if (!$scope.$$phase) {
	        $scope.$apply();
	    }
	}

    }])
}); 