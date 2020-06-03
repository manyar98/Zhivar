define(['application'], function (app) {
    app.register.directive('editCash',['$compile', function ($compile) {

	return {
		restrict: 'E',
		transclude: true,
		templateUrl: '/App/template/edit-cash.html',
		scope: {
			cash: '=',
			onsuccess: '=',
			open: '='
		},
		link: function (scope, element, attrs) {
			scope.initEditCash = function () {
				scope.alertBoxVisible = false;
				scope.alertMessage = "";
				if (!scope.cash) {
					scope.getNewCashObject();
				} else {
					scope.getCash(scope.cash.ID);
				}
				$('#editCashModal').modal({ keyboard: false }, 'show');
				$("#editCashModal .modal-dialog").draggable({ handle: ".modal-header" });
			};
			scope.$watch('open', function (value) {
				if (value)
					scope.initEditCash();
				scope.open = false;
			}, true);
			scope.submitCash = function () {
				scope.saveCash();
			};
			scope.getNewCashObject = function () {
			    scope.calling = true;

			    $.ajax({
			        type: "POST",
			        url: "/app/api/Cash/GetNewCashObject",
			        contentType: "application/json"
			    }).done(function (res) {
                
                        scope.calling = false;
                    	scope.cash = res.data;
                    	scope.$apply();
			    });
	
                    	
			  
			//};

				//callws('getNewCashObject', {})
                //    .success(function (cash) {
                //    	scope.calling = false;
                //    	scope.cash = cash;
                //    	scope.$apply();
                //    }).fail(function (error) {
                //    	window.location = '/error.html';
                //    }).loginFail(function () {
                //    	//window.location = DefaultUrl.login;
                //    });
			};
			scope.getCash = function (id) {
			    scope.calling = true;

			    $.ajax({
			        type: "POST",
			        data: JSON.stringify(id),
			        url: "/app/api/Cash/GetCashById",
			        contentType: "application/json"
			    }).done(function (res) {
			        var cash = res.data;
                    	scope.calling = false;
                    	scope.cash = cash;
                    	scope.$apply();
                    }).fail(function (error) {
                    	window.location = '/error.html';
                    });
			};
			scope.saveCash = function () {
				scope.alertBoxVisible = false;
				scope.alertMessage = "";
				scope.calling = true;
				$.ajax({
				    type: "POST",
				    data: JSON.stringify(scope.cash),
				    url: "/app/api/Cash/Add",
				    contentType: "application/json"
				}).done(function (res) {
				    var savedItem = res.data;
                    	scope.calling = false;
                    	scope.cash = savedItem;
                    	scope.onsuccess(savedItem);
                    	$('#editCashModal').modal('hide');
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