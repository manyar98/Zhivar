define(['application'], function (app) {
    app.register.directive('sendComment', [function ($compile) {
        return {
            restrict: 'E',
            transclude: true,
            templateUrl: 'send-comment.html?ver=1.2.9.1',
            scope: {
                onsuccess: '=',
                open: '='
            },
            link: function (scope, element, attrs) {
                scope.init = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    $('#titleSendComment').focus();
                    $('#sendCommentModal').modal({ keyboard: false }, 'show');
                };

                scope.$watch('open', function (value) {
                    if (value) scope.init();
                    scope.open = false;
                }, true);
                scope.submit = function () {
                    scope.calling = true;
                    callws('SendComment', { title: scope.title, text: scope.text })
                        .success(function (account) {
                            scope.calling = false;
                            scope.alertBoxVisible = false;
                            scope.onsuccess();
                            $('#sendCommentModal').modal('hide');
                            scope.$apply();
                        }).fail(function (error) {
                            scope.calling = false;
                            scope.alertBoxVisible = true;
                            scope.alertMessage = error;
                            scope.$apply();
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                            scope.$apply();
                        });
                };
            }
        };
    }]);
});