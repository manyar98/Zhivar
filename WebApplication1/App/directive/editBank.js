define(['application'], function (app) {
    app.register.directive('editBank',['$compile', function ($compile) {

	return {
		restrict: 'E',
		transclude: true,
		templateUrl: '/App/template/edit-bank.html',
		scope: {
			bank: '=',
			onsuccess: '=',
			open: '='
		},
		link: function (scope, element, attrs) {
			scope.initEditBank = function () {
				scope.alertBoxVisible = false;
				scope.alertMessage = "";
				if (!scope.bank) {
					scope.getNewBankObject();
				} else {
					scope.getBank(scope.bank.ID);
				}
				$('#editBankModal').modal({ keyboard: false }, 'show');
				$("#editBankModal .modal-dialog").draggable({ handle: ".modal-header" });
			};
			scope.$watch('open', function (value) {
				if (value)
					scope.initEditBank();
				scope.open = false;
			}, true);
			scope.submitBank = function () {
				scope.saveBank();
			};
			scope.getNewBankObject = function () {
				scope.calling = true;
                	$.ajax({
				    type: "POST",
				   // data: JSON.stringify(id),
				    url: "/app/api/Bank/GetNewBankObject",
				    contentType: "application/json"
				}).done(function (res) {
                     	scope.calling = false;
                    	scope.bank = res.data;
                    	scope.$apply();
                    });
				
			};
				
			scope.getBank = function (id) {
			    scope.calling = true;
			    $.ajax({
			        type: "POST",
			         data: JSON.stringify(id),
			        url: "/app/api/Bank/GetBankById",
			        contentType: "application/json"
			    }).done(function (res) {
			        var bank = res.data;
                    	scope.calling = false;
                    	scope.bank = bank;
                    	scope.$apply();
                    }).fail(function (error) {
                    	window.location = '/error.html';
                    });
			};
			scope.saveBank = function () {
				scope.alertBoxVisible = false;
				scope.alertMessage = "";
				scope.calling = true;
				$.ajax({
				    type: "POST",
				    data: JSON.stringify(scope.bank ),
				    url: "/app/api/Bank/Add",
				    contentType: "application/json"
				}).done(function (res) {
				    var savedItem = res.data;
                    	scope.calling = false;
                    	scope.bank = savedItem;
                    	scope.onsuccess(savedItem);
                    	$('#editBankModal').modal('hide');
                    	scope.$apply();
                    }).fail(function (error) {
                    	scope.calling = false;
                    	scope.alertBoxVisible = true;
                    	scope.alertMessage = error;
                    	scope.$apply();
                    });
			};
		}
	};
}]);
});