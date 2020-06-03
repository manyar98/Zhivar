define(['application', 'digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper'], function (app) {
    app.register.controller('contactsController', ['$scope','$location', '$stateParams', '$compile', '$state', '$rootScope',
        function ($scope, $location, $stateParams, $compile, $state, $rootScope) {

            $rootScope.applicationModule = "اشخاص";
            $scope.applicationDescription = "در این صفحه امکان جستجو، اضافه و ویرایش اشخاص وجود دارد.";

	var grid, linkPopup;
	$scope.init = function () {
		$rootScope.pageTitle("اشخاص");
		$('#businessNav').show();
		$scope.rptTitle = "اشخاص";
		$scope.role = "all";
		$scope.nodeId = 0;
		$scope.linkDays = 90;

		$('#tabs a').click(function (e) {
			e.preventDefault();
			$(this).tab('show');
		});
		$scope.getContacts();
		$scope.getDateNow();

        grid = dxGrid({
            columnFixing: {
                enabled: true
            },
			elementId: 'gridContainer',
			layoutKeyName: "grid-contacts",
			selection: { mode: "multiple" },
			height: Math.max(300, $(window).height() - $('#gridContainer').offset().top - 50),
			columns: [
				{ caption: 'کد', dataField: 'Code', width: 80, printWidth: 2 },
				{ caption: 'گروه', dataField: 'DetailAccount.Node.Name', printWidth: 4 },
				{
				    caption: 'نام شخص', printWidth: 8, dataField: 'Name'
//, cellTemplate: function (cellElement, cellInfo) {
						//cellElement.html("<a class='text-info txt-bold' href='editContact/" + cellInfo.data.ID + "'>" + Hesabfa.farsiDigit(cellInfo.displayValue) + "</a>");
				//	}
				},
				{ caption: 'موبایل', dataField: 'Mobile', width: 110, printWidth: 3 },
				{ caption: 'ایمیل', dataField: 'Email', width: 180, printWidth: 5 },
				{ caption: 'استان', dataField: 'State', width: 150, printWidth: 3 },
				{ caption: 'شهر', dataField: 'City', width: 150, printWidth: 3 },
				{
					caption: 'ویرایش', dataField: '', width: 70, printVisible: false, cellTemplate: function (cellElement, cellInfo) {
					    cellElement.html("<a class='text-info'> ویرایش</a>");
					}
				}
			],
			print: {
				fileName: "ContactsList.pdf",
				rowColumnWidth: 1.5,
				page: {
					landscape: false
				},
				header: {
					title1: $rootScope.currentBusiness.Name,
					title2: "لیست اشخاص"
				}
			},
			onCellClick: function (item) {
			    if (item.column.index !== -2)
			        $state.go('editContact', { id: item.data.ID });
									//window.location = "#editContact/" + item.data.Id;
			}
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
						onBtn1Click: function () { $scope.importContactsFromExcel(data); }
					});
				} else {
					alertbox({ content: 'لطفا فایل اکسل انتخاب کنید.' });
				}
			};

			reader.readAsDataURL(file);
		});

		linkPopup = $("<div/>").appendTo("body").dxPopup({
			rtlEnabled: true,
			showTitle: true,
			height: 'auto',
			width: 500,
			title: "لینک عمومی کارت حساب اشخاص",
			dragEnabled: true,
			closeOnOutsideClick: true,
			contentTemplate: function (contentElement) {
				var html = $('#publicLinkPopup').html();
				$('#publicLinkPopup').html("");
				$(html).appendTo(contentElement);
				$compile(contentElement.contents())($scope);
			}
		}).dxPopup("instance");

		$scope.startNodeSelect = true;
		$scope.ignoreEmpty = 1;
		//$.getScript("/js/app/printReports.js?ver=1.2.9.1", function () { });
	};
	$scope.getContacts = function () {
		$scope.loading = true;
             $.ajax({
            type:"POST",
            //data: JSON.stringify({ start: "", end: "" }),
            url: "/app/api/Contact/GetAllByOrganId",
            contentType: "application/json"
       }).done(function (res) {
           var data = res.data;

            	$scope.loading = false;
            	$scope.contacts = data;
            	grid.fill(data);
            	var filter = $stateParams.filter;// $scope.getRouteQuery($routeParams.params, 'filter');
            	if (filter && filter === 'customer') {
            		$('#tabs li:eq(1) a').tab('show');
            		$scope.grid.applyFilter('IsCustomer', true); $scope.grid.applySort();
            	}
            	else if (filter && filter === 'vendor') {
            		$('#tabs li:eq(2) a').tab('show');
            		$scope.grid.applyFilter('IsVendor', true); $scope.grid.applySort();
            	}

            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	if ($scope.accessError(error)) { applyScope($scope); return; }
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.selectTab = function (tab) {
		switch (tab) {
			case "all":
				$scope.rptTitle = "اشخاص";
				grid.clearFilter();
				break;
			case "customer":
				$scope.rptTitle = "مشتریان";
				grid.filter(["IsCustomer", "=", true]);
				break;
			case "vendor":
				$scope.rptTitle = "فروشندگان";
				grid.filter(["IsVendor", "=", true]);
				break;
			default:
				$scope.rptTitle = "اشخاص";
				grid.clearFilter();
				tab = "all";
				break;
		}
		$scope.role = tab;
		applyScope($scope);
	};
	$scope.changeSort = function (name) {
		$scope.sortBy = name;
		$scope.sortDesc = $scope.sortDesc !== true;
		$rootScope.setUISetting("contactsSortBy", name);
		$rootScope.setUISetting("contactsSortDesc", $scope.sortDesc ? "true" : "false");
		$scope.grid.sort(name);
	};
	$scope.addContact = function () {
		Hesabfa.pageParams = {};
		Hesabfa.pageParams.id = 0;
		//$location.path("/editContact/0");
		
		$state.go('editContact', { id: 0 });
		return;
	};
	$scope.editContact = function (contact) {
		$scope.alert = false;
		$scope.contact = contact;
		$scope.editContactModal = true;
	};
	$scope.getEditedContact = function (contact) {
		if (!contact) return;
		$scope.alert = true;
		$scope.alertType = 'success';
		//		item.Name = item.Name;
		//		item.NodeName = item.DetailAccount.Node.Name;
		var finded = findAndReplace($scope.grid.data, contact.ID, contact);
		finded.Balance = $scope.contact.Balance;
		finded.BalanceType = $scope.contact.BalanceType;
		findAndReplace($scope.grid.pageData, contact.ID, contact);
		if (finded)
			$scope.alertMessage = "تغییرات شخص ذخیره شد.";
		else {
			$scope.grid.addItem(contact);
			$scope.alertMessage = "شخص جدید ذخیره شد.";
		}
		$scope.editContactModal = false;
		$scope.$apply();
	};
	$scope.viewContact = function (contact) {
		$location.path("/contactCard/" + contact.Id);
		return;
	};
	$scope.deleteContacts = function () {
		var contacts = grid.getSelectedRowsData();
		if (contacts.length === 0) {
			alertbox({ content: "ابتدا يك يا چند شخص را انتخاب كنيد" });
			return;
		}
		questionbox({
			content: "آيا از حذف اشخاص انتخاب شده مطمئن هستيد؟",
			onBtn1Click: function () {
				if ($scope.calling) return;
				$scope.calling = true;

				$.ajax({
				    type:"POST",
				    data: JSON.stringify(contacts),
				    url: "/app/api/Contact/Delete",
				    contentType: "application/json"
				}).done(function (res) {
				    var data = res.data;
                    	$scope.calling = false;
                    	for (var i = 0; i < contacts.length; i++)
                    		findAndRemove(grid._options.dataSource, contacts[i]);
                    	grid.refresh();
                    	$scope.$apply();
                    }).fail(function (error) {
                    	$scope.calling = false;
                    	if ($scope.accessError(error)) { applyScope($scope); return; }
                    	$scope.$apply();
                    	alertbox({ content: error });
                    });
			}
		});
	};
	$scope.search = function () {
		$scope.grid.search($scope.searchValue, $scope);
	};
	$scope.nodeSelect = function (selectedNode) {
		if (!selectedNode) return;
		var data = $scope.contacts;
		var filteredData = [];
		for (var i = 0; i < data.length; i++) {
			var c = data[i];
			if (c.DetailAccount.Node.Id === selectedNode.Id)
				filteredData.push(c);
		}
		$scope.grid.data = filteredData;
		$scope.grid.init();
	};
	$scope.getBalanceType = function (contact) {
		if (contact.Balance === 0) return "-";
		return contact.Credits < contact.Liability ? "بدهکار" : "بستانکار";
	};
	$scope.calculateTotal = function () {
		var data = $scope.grid.data;
		var sumDebit = 0;
		var sumCredit = 0;
		for (var i = 0; i < data.length; i++) {
			var c = data[i];
			c.Balance = Math.abs(c.Credits - c.Liability);
			c.BalanceType = $scope.getBalanceType(c);
			if (c.BalanceType === "بدهکار")
				sumDebit += c.Balance;
			else
				sumCredit += c.Balance;
		}
		$scope.balanceSum = Math.abs(sumDebit - sumCredit);
		if ($scope.balanceSum === 0) $scope.balanceType = "-";
		else $scope.balanceType = sumCredit < sumDebit ? "بدهکار" : "بستانکار";
	};
	$scope.importContactsFromExcel = function (file) {
		if ($scope.callingImport) return;
		var option1;
		if (!$scope.ignoreEmpty) option1 = true;
		else { option1 = $scope.ignoreEmpty === 1 ? true : false }
		$scope.callingImport = true;
		callws(DefaultUrl.MainWebService + 'ImportContactsFromExcel', { excelFile: file, ignoreEmpty: option1 })
            .success(function (result) {
            	$scope.callingImport = false;
            	$('#importExcelModal').modal('hide');
            	$scope.getContacts();
            	$scope.$apply();
            	alertbox({
            		title: "ورود اطلاعات از اکسل",
            		content: "ورود اطلاعات از اکسل با موفقیت انجام شد.<br/>" +
                        result[0] + " رکورد اضافه شد." + "<br/>" +
                        result[1] + " رکورد بروزرسانی شد."
            	});
            }).fail(function (error) {
            	$scope.callingImport = false;
            	if ($scope.accessError(error)) { applyScope($scope); return; }
            	alertbox({ content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.exportContactsToExcel = function () {
		if ($scope.callingImport) return;
		$scope.callingImport = true;
		$('#loadingModal').modal('show');
		callws(DefaultUrl.MainWebService + 'ExportContactsToExcel', {})
            .success(function (data) {
            	$scope.callingImport = false;
            	$('#loadingModal').modal('hide');
            	$scope.$apply();
            	var contentType = 'application/vnd.ms-excel';
            	var blob = b64toBlob(data, contentType);
            	if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            		window.navigator.msSaveOrOpenBlob(blob, "Hesabfa_Contacts.xlsx");
            	}
            	else {
            		var objectUrl = URL.createObjectURL(blob);
            		//                    window.open(objectUrl);
            		//                    var encodedUri = encodeURI(data);
            		var link = document.createElement("a");
            		link.href = objectUrl;
            		link.download = "Hesabfa_Contacts.xlsx";
            		link.target = '_blank';
            		document.body.appendChild(link);
            		link.click();
            	}

            }).fail(function (error) {
            	$scope.callingImport = false;
            	if ($scope.accessError(error)) { applyScope($scope); return; }
            	alertbox({ content: error });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	$scope.printContacts = function () {
		grid.print();
		//		var contacts = grid.getDataSource().items();
		//		printContacts(contacts, $scope.rptTitle, $scope.balanceSum, $scope.balanceType, $scope.role, $rootScope.currentBusiness, $scope.dateNow, $scope.getCurrency());
	};
	$scope.pdfContacts = function () {
		grid.pdf();
		//		var contacts = grid.getDataSource().items();
		//		pdfContacts(contacts, $scope.rptTitle, $scope.balanceSum, $scope.balanceType, $scope.role, $rootScope.currentBusiness, $scope.dateNow, $scope.getCurrency());
	};
	$scope.showPublicLink = function () {
		var items = grid.getSelectedRowsData();
		if (items.length === 0) {
			alertbox({ content: "ابتدا يك نفر را انتخاب كنيد" });
			return;
		}
		if (items.length > 1) {
			alertbox({ content: "فقط يك نفر را انتخاب كنيد" });
			return;
		}
		$scope.contactEmail = items[0].Email;
		$scope.publicLink = "";
		linkPopup.show();
	};
	$scope.getPublicLink = function () {
		if ($scope.calling)
			return;
		var items = grid.getSelectedRowsData();
		callws(DefaultUrl.MainWebService + "GetPublicContactCardLink", { code: items[0].DetailAccount.Code, days: $scope.linkDays })
            .success(function (data) {
            	$scope.calling = false;
            	$scope.publicLink = data;
            	applyScope($scope);
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });

	};
	$scope.copyPublicLink = function () {
		$("#publicLink").focus().select();
		try {
			var successful = document.execCommand('copy');
			if (successful)
				DevExpress.ui.notify("لینک کارت حساب کپی شد.", "success", 2000);
			else
				DevExpress.ui.notify("لینک را به صورت دستی کپی کنید.", "error", 2000);
		} catch (err) {

		}
	};
	$scope.emailPublicLink = function () {
		if ($scope.calling)
			return;
		if (!$scope.contactEmail) {
			DevExpress.ui.notify("آدرس ایمیل را وارد کنید.", "error", 3000);
			return;
		}
		var items = grid.getSelectedRowsData();
		callws(DefaultUrl.MainWebService + "EmailPublicContactCardLink", { code: items[0].DetailAccount.Code, days: $scope.linkDays, email: $scope.contactEmail })
            .success(function (data) {
            	$scope.calling = false;
            	DevExpress.ui.notify("لینک کارت حساب ایمیل شد.", "success", 3000);
            	linkPopup.hide();
            	applyScope($scope);
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	DevExpress.ui.notify(error, "error", 3000);
            }).loginFail(function () {
            	window.location = DefaultUrl.login;
            });
	};
	
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});