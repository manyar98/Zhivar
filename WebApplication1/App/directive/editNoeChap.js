define(['application'], function (app) {
    app.register.directive('editNoeChap', ['$compile', function ($compile) {

        return {
            restrict: 'E',
            transclude: true,
            templateUrl: '/App/template/edit-noe-chap.html',
            scope: {
                noechap: '=',
                onsuccess: '=',
                open: '='
            },
            link: function (scope, element, attrs) {
                scope.initEditNoeChap = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    if (!scope.noechap) {
                        scope.getNewNoeChapObject();
                    } else {
                        scope.getNoeChap(scope.noechap.ID);
                    }
                    $('#editNoeChapModal').modal({ keyboard: false }, 'show');
                    $("#editNoeChapModal .modal-dialog").draggable({ handle: ".modal-header" });
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initEditNoeChap();
                    scope.open = false;
                }, true);
                scope.submitNoeChap = function () {
                    scope.saveNoeChap();
                };
                scope.getNewNoeChapObject = function () {
                    scope.calling = true;

                    $.ajax({
                        type: "POST",
                        url: "/app/api/NoeChap/GetNewNoeChapObject",
                        contentType: "application/json"
                    }).done(function (res) {

                        scope.calling = false;
                        scope.noechap = res.data;
                        scope.$apply();
                    });



                    //};

                    //callws('getNewNoeChapObject', {})
                    //    .success(function (noechap) {
                    //    	scope.calling = false;
                    //    	scope.noechap = noechap;
                    //    	scope.$apply();
                    //    }).fail(function (error) {
                    //    	window.location = '/error.html';
                    //    }).loginFail(function () {
                    //    	//window.location = DefaultUrl.login;
                    //    });
                };
                scope.getNoeChap = function (id) {
                    scope.calling = true;

                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(id),
                        url: "/app/api/NoeChap/GetNoeChapById",
                        contentType: "application/json"
                    }).done(function (res) {
                        var noechap = res.data;
                        scope.calling = false;
                        scope.noechap = noechap;
                        scope.$apply();
                    }).fail(function (error) {
                        window.location = '/error.html';
                    });
                };
                scope.saveNoeChap = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.calling = true;
                    $.ajax({
                        type: "POST",
                        data: JSON.stringify(scope.noechap),
                        url: "/app/api/NoeChap/Add",
                        contentType: "application/json"
                    }).done(function (res) {
                        var savedItem = res.data;
                        scope.calling = false;
                        scope.noechap = savedItem;
                        scope.onsuccess(savedItem);
                        $('#editNoeChapModal').modal('hide');
                        scope.$apply();
                    }).fail(function (error) {
                        scope.calling = false;
                        scope.alertBoxVisible = true;
                        scope.alertMessage = error;
                        scope.$apply();
                    });
                };

                scope.closeModal = function () {
                    $('#editNoeChapModal').modal('hide');
                };
            }
        };
    }]);
});