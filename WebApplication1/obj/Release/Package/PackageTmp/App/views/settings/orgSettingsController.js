define(['application', 'digitGroup','number', 'dx', 'roweditor', 'helper','dateRange','gridHelper'], function (app) {
    app.register.controller('orgSettingsController', ['$scope','$rootScope','$stateParams', '$location',
        function ($scope,$rootScope, $stateParams, $location) {

	$scope.addClass = function (step) {
		return step === 1 ? 'btn btn-success btn-sm' : 'btn btn-default btn-smoke btn-sm';
	};
	$scope.init = function () {
		$rootScope.pageTitle("تنظیمات موسسه");
		$('#businessNav').hide();
		$scope.previousBtnShow = true;
		$scope.saveAndQuitBtnShow = true;
		$scope.nextBtnShow = true;
		$scope.setupMode = $stateParams.setupMode ? true : false;
		if ($scope.setupMode)
			$('#businessNav').hide();
		else
			$('#businessNav').show();
		$scope.alertBoxVisible = false;
		$scope.alertMessage = "";

		$scope._business = {
			name: $rootScope.currentBusiness.Name,
			legalName: $rootScope.currentBusiness.LegalName,
			organizationType: $rootScope.currentBusiness.OrganizationType,
			businessLine: $rootScope.currentBusiness.BusinessLine,
			nationalCode: $rootScope.currentBusiness.NationalCode,
			economicCode: $rootScope.currentBusiness.EconomicCode,
			registrationNumber: $rootScope.currentBusiness.RegistrationNumber,
			country: $rootScope.currentBusiness.Country,
			state: $rootScope.currentBusiness.State,
			city: $rootScope.currentBusiness.City,
			postalCode: $rootScope.currentBusiness.PostalCode,
			address: $rootScope.currentBusiness.Address,
			phone: $rootScope.currentBusiness.Phone,
			fax: $rootScope.currentBusiness.Fax,
			website: $rootScope.currentBusiness.Website,
			email: $rootScope.currentBusiness.Email
		}
		applyScope($scope);
	};
	$scope.nextSetupStep = function () {
		if ($scope.calling) return;
		$rootScope.currentBusiness.SetupStep++;
		$scope.saveOrgSettings('/finanSettings/1');
	};
	$scope.previousSetupStep = function () {
		if ($scope.calling) return;
		$rootScope.currentBusiness.SetupStep--;
		$scope.saveOrgSettings('/setupStart/1');
	};
	$scope.saveAndQuitSetup = function () {
		if ($scope.calling) return;
		$scope.saveOrgSettings('/dashboard', function () { $rootScope.currentBusiness = null });
	};
	$scope.saveOrgSettings = function (nextPath, method) {
		if ($scope.calling) return;
		$scope.alert = false;
		$scope.calling = true;
		$scope.previousBtnShow = false;
		$scope.saveAndQuitBtnShow = false;
		$scope.nextBtnShow = false;

		$rootScope.currentBusiness.Name = $scope._business.name;
		$rootScope.currentBusiness.LegalName = $scope._business.legalName;
		$rootScope.currentBusiness.OrganizationType = $scope._business.organizationType;
		$rootScope.currentBusiness.BusinessLine = $scope._business.businessLine;
		$rootScope.currentBusiness.NationalCode = $scope._business.nationalCode;
		$rootScope.currentBusiness.EconomicCode = $scope._business.economicCode;
		$rootScope.currentBusiness.RegistrationNumber = $scope._business.registrationNumber;
		$rootScope.currentBusiness.Country = $scope._business.country;
		$rootScope.currentBusiness.State = $scope._business.state;
		$rootScope.currentBusiness.City = $scope._business.city;
		$rootScope.currentBusiness.PostalCode = $scope._business.postalCode;
		$rootScope.currentBusiness.Address = $scope._business.address;
		$rootScope.currentBusiness.Phone = $scope._business.phone;
		$rootScope.currentBusiness.Fax = $scope._business.fax;
		$rootScope.currentBusiness.Website = $scope._business.website;
		$rootScope.currentBusiness.Email = $scope._business.email;

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
            	if (nextPath && nextPath === "save") {
            		$scope.calling = false;
            		$scope.alert = true;
            		$scope.alertType = "success";
            		$scope.alertMessage = "تغییرات با موفقیت ذخیره شد";
            		$scope.$apply();
            		return;
            	}
            	if (nextPath && nextPath !== "/dashboard")
            		window.location = $scope.getTokenQuerystring() + "#" + nextPath;
            	else if (nextPath)
            		$location.path(nextPath);
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.calling = false;
            	applyScope($scope);
            	if ($scope.accessError(error)) return;
            	if ($scope.setupMode) $rootScope.currentBusiness.SetupStep = 1;
            	$scope.previousBtnShow = true;
            	$scope.saveAndQuitBtnShow = true;
            	$scope.nextBtnShow = true;
            	alertbox({ title: "خطا", content: error });
            	applyScope($scope);
            }).loginFail(function () {
            	if ($scope.setupMode) $rootScope.currentBusiness.SetupStep = 1;
            	$scope.calling = false;
            	if (Hesabfa.user) $scope.inactivityLogout();
            	else window.location = DefaultUrl.login;
            });
	};
	
	$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
}])
});