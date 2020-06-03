define(['application', 'digitGroup', 'number', 'dx', 'roweditor', 'helper', 'gridHelper'], function (app) {
    app.register.controller('editContactController', ['$scope', '$location', '$stateParams', '$compile', '$state', '$rootScope','messageService',
        function ($scope, $location, $stateParams, $compile, $state, $rootScope, messageService) {

            $rootScope.applicationModule = "اشخاص";
            $scope.applicationDescription = "در این صفحه امکان اضافه و ویرایش اشخاص وجود دارد.";

	$scope.init = function () {

		$scope.alertBoxVisible = false;
		$scope.alertMessage = "";

		var id = $stateParams.id;
		if (!id) {
			$scope.getNewContactObject();
			$rootScope.pageTitle("شخص جدید");
		}
		else
			$scope.getContact(id);

		$scope.startNodeSelect = true;
		applyScope($scope);
	};
	$scope.getContact = function (id) {
		if ($scope.calling) return;
		$scope.calling = true;
            $.ajax({
            type:"POST",
            data: JSON.stringify(id),
            url:"/app/api/Contact/GetContactById",
            contentType: "application/json"
            }).done(function (res) {
                var contact = res.data;
		//callws(DefaultUrl.MainWebService + 'GetContactById', { id: id })
       //    .success(function (contact) {



            	$scope.calling = false;
            	$rootScope.pageTitle("ویرایش " + contact.Name);
                $scope.contact = contact;

                $('#fileShow2').attr({ src: $scope.contact.TasvirBlobBase64 });

                var element = document.getElementById("test");
                element.style.backgroundColor = contact.Color;

            	$scope.$apply();
            	$("#inputName").focus();
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
	};
	$scope.getNewContactObject = function () {
		if ($scope.calling) return;
		$scope.calling = true;

		$.ajax({
		    type:"POST",
		    //data: JSON.stringify({ start: "", end: "" }),
		    url: "/app/api/Contact/GetNewContactObject",
		    contentType: "application/json"
		}).done(function (res) {
		    var contact = res.data;
            	$scope.calling = false;
                $scope.contact = contact;

                var element = document.getElementById("test");
                element.style.backgroundColor = contact.Color;
         
            	$scope.$apply();
            	$("#inputName").focus();
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, type: "error" });
            });
    };

    $scope.fileOptions = {
        localization: {
            select: "انتخاب لوگو"
        },
        multiple: false,
        showFileList: false,
        enabled: true,
        select: function (files) {
            var file = files.files[0];
            var ok = file.extension.toLowerCase() === ".jpg" || file.extension.toLowerCase() === ".jpeg";
            if (!ok) {
                messageService.error("فقط تصویر با فرمت jpg  قابل قبول است.");
                return;
            }
            else if (file.size > 204800) {
                messageService.error("حداکثر سایز مجاز برای تصویر 200kb میباشد.");
                return;
            }

            var reader = new FileReader();
            reader.onload = function (e) {
                $scope.contact.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                $scope.contact.Blob = "data:image/jpeg;base64," + $scope.contact.TasvirBlobBase64;
                $scope.contact.FileName = file.name;
                $scope.contact.FileSize = file.size;
                $scope.contact.MimeType = file.rawFile.type;
                $scope.$$phase || $scope.$apply();
            }
            reader.readAsDataURL(file.rawFile);
        }
    }
	$scope.saveContact = function (next) {
		if ($scope.calling) return;
		$scope.calling = true;

		$.ajax({
		    type:"POST",
		    data: JSON.stringify($scope.contact),
		    url: "/app/api/Contact/AddContact",
		    contentType: "application/json"
		}).done(function (res) {
		    var contact = res.data;
            	$scope.calling = false;
            	if (!next)
            	    $state.go('contacts');
            	else {
            		$scope.contact = null;
            		$scope.getNewContactObject();
            		$rootScope.pageTitle("شخص جدید");
            	}
            	$scope.$apply();
            }).fail(function (error) {
            	$scope.calling = false;
            	$scope.$apply();
            	if ($scope.accessError(error)) return;
            	alertbox({ content: error, title: "هشدار", type: "warning" });
            });
	};
	$scope.cancel = function () {
		if ($scope.contact.Id === 0 || $scope.contact.Id === '0')
			$location.path("/contacts");
		else {
			$location.path("/contactCard/" + $scope.contact.Id);
		}
		return;
	};

    $scope.readMultipleFiles = function (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            // bind the file content
            $('#fileShow').attr({ src: e.target.result });

            $scope.contact.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
            //$scope.contact.Blob = "data:image/jpeg;base64," + $scope.contact.TasvirBlobBase64;
            $scope.contact.FileName = file.name;
            $scope.contact.FileSize = file.size;
            $scope.contact.MimeType = file.rawFile.type;
            $scope.$$phase || $scope.$apply();
        };
        reader.readAsDataURL(file.rawFile);
    };
    $scope.onSelect = function (e) {
        $.each(e.files, function (index, value) {
            var ok = value.extension === ".jpg"
                || value.extension === ".JPEG"
                || value.extension === ".jpg"
                || value.extension === ".jpeg";

            if (!ok) {
                e.preventDefault();
                messageService.error("فقط تصویر با فرمت jpg  قابل قبول است.");
            }
            else if (value.size > 204800) {
                e.preventDefault();
                messageService.error("حداکثر سایز مجاز برای تصویر 200KB میباشد.");
            }
            else {
                $scope.readMultipleFiles(value);
            }
        });
    };
            $rootScope.loadCurrentBusinessAndBusinesses($scope.init);
        }])
});