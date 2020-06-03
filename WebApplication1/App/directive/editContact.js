define(['application','nodeSelect'], function (app) {
    app.register.directive('editContact',['$compile', function ($compile) {
	return {
		restrict: 'E',
		transclude: true,
		templateUrl: '/App/template/edit-contac.html',
		scope: {
			contact: '=',
			onsuccess: '=',
			open: '=',
            defaultnode: '=?',
            typeaccount :'=?'
		},
		link: function (scope, element, attrs) {
			scope.initContactItem = function () {
				scope.contactTypes = ["نامشخص", "حقیقی", "حقوقی"];
				scope.alertBoxVisible = false;
				scope.alertMessage = "";
				if (!scope.contact || scope.contact.Id === 0) {
					scope.getNewContactObject();
				} else {
					scope.getContact(scope.contact.Id);
                }

                scope.fileOptions = {
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
                            scope.contact.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                            scope.contact.Blob = "data:image/jpeg;base64," + scope.contact.TasvirBlobBase64;
                            scope.contact.FileName = file.name;
                            scope.contact.FileSize = file.size;
                            scope.contact.MimeType = file.rawFile.type;
                            scope.$$phase || scope.$apply();
                        }
                        reader.readAsDataURL(file.rawFile);
                    }
                }

				scope.startNodeSelect = true;
				$('#editContactModal').modal({ keyboard: false }, 'show');
				$("#editContactModal .modal-dialog").draggable({ handle: ".modal-header" });
			};
			scope.$watch('open', function (value) {
				if (value)
					scope.initContactItem();
				scope.open = false;
			}, true);
			scope.submitItem = function () {
				scope.saveContact();
			};
			scope.getNewContactObject = function () {
				if (scope.calling) return;
				scope.calling = true;

				$.ajax({
				    type: "POST",
				   // data: JSON.stringify(id),
				    url: "/app/api/Contact/GetNewContactObject",
				    contentType: "application/json"
				}).done(function (res) {

                        scope.calling = false;
                    	if (scope.defaultnode && scope.defaultnode.SystemAccount === 1)
                            contact.DetailAccount.Node = scope.defaultnode;
              
                        scope.contact = res.data;// contact;
                        scope.contact.TypeAccount = scope.typeaccount;
                    });
				//callws('GetNewContactObject', {})
			    //    .success(function (contact) {
          
				//var contact = {
				//    Address: "", City: "", Code: "000002", ContactEmail: "", ContactType: 0, Credits: 0,
				//    DetailAccount: { Code: "000002", Name: "", RelatedAccounts: ",6,22,7,", Node: {}, Accounts: [] },
				//    EconomicCode: "", Email: "", Fax: "", FirstName: "", HesabfaKey: 0, Id: 0, IsCustomer: false,
				//    IsEmployee: false, IsShareHolder: false, IsVendor: false, LastName: "", Liability: 0, Mobile: "",
				//    Name: "", NationalCode: "", Note: "", People: "", Phone: "", PostalCode: "", Rating: 0,
				//    RegistrationDate: "/Date(1545135001855)/", RegistrationNumber: "", SharePercent: 0, State: "", Website: ""
				//};
                    	
                    	//scope.$apply();
                    //}).fail(function (error) {
                    //	$('#editContactModal').modal('hide');
                    //	if (typeof error == "string" && error.indexOf("[accessDenied]") > -1) {
                    //		var errorString = error.substr("[accessDenied]".length);
                    //		alertbox({
                    //			content: Hesabfa.accessDeniedString + "</br>" + errorString, onBtn1Click: function () {
                    //				return;
                    //			}
                    //		});
                    //		return;
                    //	}
                    //	alertbox({ content: error });
                    //	return;
                    //}).loginFail(function () {
                    //	//window.location = DefaultUrl.login;
                    //});
			};
			scope.getContact = function (id) {
				scope.calling = true;
				callws('GetContactById', { id: id })
                    .success(function (contact) {
                    	scope.calling = false;
                    	scope.contact = contact;
                    	scope.$apply();
                    }).fail(function (error) {
                    	window.location = '/error.html';
                    }).loginFail(function () {
                    	//window.location = DefaultUrl.login;
                    });
			};
			scope.saveContact = function () {
			    scope.alertBoxVisible = false;
			    scope.alertMessage = "";
			    scope.calling = true;

			    $(function () {
			   
				    $.ajax({
				        type: "POST",
				        data: JSON.stringify(scope.contact),
				        url: "/app/api/Contact/AddContact",
				        contentType: "application/json"
				    }).done(function (savedItem) {
				        scope.calling = false;
				        scope.contact = savedItem.data;
				        scope.onsuccess(savedItem.data);
				        $('#editContactModal').modal('hide');
				        scope.$apply();
				    }).fail(function (error) {
				        scope.calling = false;
				        scope.alertBoxVisible = true;
				        scope.alertMessage = error;
				        scope.$apply();
				    });
				});

				//callws('SaveContact', { contact: scope.contact })
                //    .success(function (savedItem) {
                //    	scope.calling = false;
                //    	scope.contact = savedItem;
                //    	scope.onsuccess(savedItem);
                //    	$('#editContactModal').modal('hide');
                //    	scope.$apply();
                //    }).fail(function (error) {
                //    	scope.calling = false;
                //    	scope.alertBoxVisible = true;
                //    	scope.alertMessage = error;
                //    	scope.$apply();
                //    }).loginFail(function () {
                //    	//window.location = DefaultUrl.login;
                //    });
            };

            scope.readMultipleFiles = function (file) {
                var reader = new FileReader();
                reader.onload = function (e) {
                    // bind the file content
                    $('#fileShow').attr({ src: e.target.result });

                    scope.contractStop.TasvirBlobBase64 = e.target.result.replace("data:image/jpeg;base64,", "");
                    //scope.contractStop.Blob = "data:image/jpeg;base64," + scope.contractStop.TasvirBlobBase64;
                    scope.contractStop.FileName = file.name;
                    scope.contractStop.FileSize = file.size;
                    scope.contractStop.MimeType = file.rawFile.type;
                    scope.$$phase || scope.$apply();
                };
                reader.readAsDataURL(file.rawFile);
            };
            scope.onSelect = function (e) {
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
                        scope.readMultipleFiles(value);
                    }
                });
            };

            scope.closeModal = function () {
                $('#editContactModal').modal('hide');
            };
		}
	};
}]);
});