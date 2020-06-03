define(['application', 'combo', 'scrollbar', 'helper', 'editContact', 'dx', 'roweditor', 'gridHelper'], function (app) {
    app.register.controller('shareholdersController', ['$scope','$rootScope', '$location', '$stateParams',
        function ($scope,$rootScope, $location, $stateParams) {

	$scope.addClass = function (step) {
		return step === 4 ? 'btn btn-success btn-sm' : 'btn btn-default btn-smoke btn-sm';
	};
	$scope.init = function () {
		$rootScope.pageTitle("سهامداران");
		$('#businessNav').hide();
		$scope.previousBtnShow = true;
		$scope.saveAndQuitBtnShow = false;
		$scope.nextBtnShow = false;
		$scope.endBtnShow = true;
		$scope.setupMode = $stateParams.setupMode ? true : false;
		if ($scope.setupMode) $('#businessNav').hide();
		else $('#businessNav').show();
		$scope.getShareholders();
		$scope.getContacts();
		$scope.total = 0;
		$scope.alert = false;
		$scope.alertMessage = "";
		$scope.shareholderName = "";

		$scope.comboShareholder = new HesabfaCombobox({
			items: [],
			containerEle: document.getElementById("comboShareholderSelect"),
			toggleBtn: true,
			itemClass: "hesabfa-combobox-item",
			activeItemClass: "hesabfa-combobox-activeitem",
			itemTemplate: Hesabfa.comboContactTemplate,
			divider: true,
			matchBy: "contact.DetailAccount.Id",
			displayProperty: "{Name}",
			searchBy: ["Name", "Code"],
			onSelect: $scope.shareholderSelect,
		});

		//applyScope($scope);
	};
	$scope.nextSetupStep = function () {
		if ($scope.calling) return;
		$rootScope.currentBusiness.SetupStep++;
		$scope.saveShareholders('start');
	};
	$scope.previousSetupStep = function () {
		if ($scope.calling) return;
		$rootScope.currentBusiness.SetupStep++;
		$scope.saveShareholders('/users/1');
	};
	$scope.saveAndQuitSetup = function () {
		if ($scope.calling) return;
		$scope.saveShareholders('/dashboard', function () { $rootScope.currentBusiness = null });
	};
	$scope.saveShareholders = function (nextPath, method) {
		if ($scope.calling) return;
		if (!$scope.validate()) {
			$rootScope.currentBusiness.SetupStep = 5;
			return;
		}
		$scope.alert = false;
		$scope.calling = true;
		$scope.previousBtnShow = false;
		$scope.saveAndQuitBtnShow = false;
		$scope.nextBtnShow = false;
		applyScope($scope);

		$.ajax({
		    type: "POST",
		    data: JSON.stringify({ shareholders: $scope.shareholders, status: 'setup' }),
		    url: "/app/api/Shareholder/SaveShareholders",
		    contentType: "application/json"
		}).done(function (res) {
            
            	callws(DefaultUrl.MainWebService + 'SaveBusiness', { business: $rootScope.currentBusiness })
                    .success(function (business) {
                    	if (method) method();
                    	$scope.calling = false;
                    	$scope.previousBtnShow = true;
                    	$scope.saveAndQuitBtnShow = true;
                    	$scope.nextBtnShow = true;
                    	$scope.$parent.currentBusiness = business;
                    	var businessInList = findBusinessById($scope.businesses, business.Id);
                    	$scope.$parent.currentBusiness.token = businessInList.token;
                    	findBusinessAndReplace($scope.businesses, $scope.$parent.currentBusiness);
                    	applyScope($scope);
                    	if (nextPath && nextPath === "start") {
                    		$scope.done = true;
                    		$("#done").show();
                    		$scope.setupMode = false;
                    		applyScope($scope);
                    	}
                    	else if (nextPath && nextPath !== "/dashboard")
                    		window.location = $scope.getTokenQuerystring() + "#" + nextPath;
                    	else
                    		$location.path(nextPath);
                    }).fail(function (error) {
                    	$scope.calling = false;
                    	applyScope($scope);
                    	if ($scope.accessError(error)) return;
                    	$rootScope.currentBusiness.SetupStep = 5;
                    	$scope.previousBtnShow = true;
                    	$scope.saveAndQuitBtnShow = true;
                    	$scope.nextBtnShow = true;
                    	$scope.alertType = "danger";
                    	$scope.alert = true;
                    	$scope.alertMessage = error;
                    	applyScope($scope);
                    });
            }).fail(function (error) {
            	if ($scope.accessError(error)) return;
            	$rootScope.currentBusiness.SetupStep = 5;
            	$scope.calling = false;
            	$scope.previousBtnShow = true;
            	$scope.saveAndQuitBtnShow = true;
            	$scope.nextBtnShow = true;
            	$scope.alertType = "danger";
            	$scope.alert = true;
            	$scope.alertMessage = error;
            	$scope.$apply();
            });
	};
	$scope.submitShareholders = function () {
		if ($scope.calling) return;
		if (!$scope.validate()) return;
		$scope.calling = true;
		$.ajax({
		    type: "POST",
		    //data: { shareholderVMs: $scope.shareholders, status: 'change' },
		    data: JSON.stringify($scope.shareholders),
		    url: "/app/api/Shareholder/SaveShareholders",
		    contentType: "application/json"
		}).done(function (res) {
            	$scope.calling = false;
            	$scope.alert = true;
            	$scope.alertType = "success";
            	$scope.alertMessage = "تغییرات سهامداران با موفقیت ذخیره شد";
            	applyScope($scope);
            }).fail(function (error) {
            	$scope.calling = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	$scope.previousBtnShow = true;
            	$scope.saveAndQuitBtnShow = true;
            	$scope.nextBtnShow = true;
            	$scope.alertType = "danger";
            	$scope.alert = true;
            	$scope.alertMessage = error;
            	applyScope($scope);
            });
	};
	$scope.getShareholders = function () {
	    $scope.loading = true;

           $.ajax({
                    type: "POST",
                    //data: JSON.stringify(bank.ID),
                    url: "/app/api/Shareholder/GetAllByOrganId",
                    contentType: "application/json"
           }).done(function (res) {
               var data = res.data;
            	$scope.loading = false;
            	$scope.shareholders = data;
            	// if (data.length === 0) $scope.addRow();
            	$scope.calTotal();
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error });
            });
	};
	$scope.shareholderSelect = function (contact) {
		for (var j = 0; j < $scope.shareholders.length; j++) {
			if ($scope.shareholders[j].ID === contact.ID) {
				$scope.alert = true;
				$scope.alertType = "warning";
				$scope.alertMessage = "این شخص قبلاً در ردیف " + (j + 1) + " ثبت شده است";
				$scope.$apply();
				return;
			}
		}
		$scope.shareholders.push(contact);
		//applyScope($scope);
	};
	$scope.newContact = function () {
		$scope.alert = false;
		$scope.contact = null;
		$scope.editContactModal = true;
		//applyScope($scope);
	};
	$scope.getEditedContact = function (contact) {
		if (!contact) return;
		$scope.contacts.push(contact);
		$scope.editContactModal = false;
		$scope.shareholders.push(contact);
		//		$scope.shareholders[$scope.activeRowIndex] = contact;
		$scope.$apply();
	};
	$scope.getContacts = function () {
	    $scope.loading = true;
	    $.ajax({
	        type: "POST",
	        url: "/app/api/Contact/GetAllByOrganId",
	        contentType: "application/json"
	    }).done(function (res) {
	        var contacts = res.data;
            	$scope.loading = false;
            	$scope.contacts = contacts;
            	$scope.comboShareholder.items = contacts;
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.loading = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.addRow = function () {
		var newObj = Hesabfa.newContactObj();
		$scope.shareholders.push(newObj);
		$scope.$apply();
	};
	$scope.deleteRow = function (index) {
		$scope.shareholders.splice(index, 1);
		$scope.calTotal();
		$scope.$apply();
	};
	$scope.cancel = function () {
		window.history.back();
	};
	$scope.calTotal = function () {
		var shareholders = $scope.shareholders;
		var total = 0;
		for (var i = 0; i < shareholders.length; i++) {
			total += shareholders[i].SharePercent ? parseFloat(shareholders[i].SharePercent) : 0;
		}
		$scope.total = parseInt(total.toFixed(1));
		applyScope($scope);
	};
	$scope.validate = function () {

		if ($scope.shareholders.length === 0) {
			$scope.alert = true;
			$scope.alertType = "danger";
			$scope.alertMessage = "حداقل یک سهامدار باید تعریف کنید";
			return false;
		}

		if ($scope.total !== 100) {
			$scope.alert = true;
			$scope.alertType = "danger";
			$scope.alertMessage = "جمع درصد سهامداران باید 100 باشد";
			return false;
		}

		for (var i = 0; i < $scope.shareholders.length; i++) {
			if ($scope.shareholders[i].ID === 0) {
				$scope.alert = true;
				$scope.alertType = "danger";
				$scope.alertMessage = "سهامدار ردیف " + (i + 1) + " را مشخص کنید";
				return false;
			}
			if ($scope.shareholders[i].SharePercent === 0) {
				$scope.alert = true;
				$scope.alertType = "danger";
				$scope.alertMessage = "درصد سهام سهامدار نمیتواند صفر باشد";
				return false;
			}
			if ($scope.shareholders[i].SharePercent < 0) {
				$scope.alert = true;
				$scope.alertType = "danger";
				$scope.alertMessage = "درصد سهام سهامدار نمیتواند کمتر از صفر باشد";
				return false;
			}
			if ($scope.shareholders[i].SharePercent > 100) {
				$scope.alert = true;
				$scope.alertType = "danger";
				$scope.alertMessage = "درصد سهام سهامدار نمیتواند بیشتر از 100 باشد";
				return false;
			}
		}
		return true;
	};
	    
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
	$scope.start = function () {
		$('#businessNav').show();
		window.location = $scope.getBusinessFinanYearLink();
		return;
	};


        }])
});