define(['angularAMD'], function (app) {
    app.directive('infoWindow', ['$parse', function ($parse) {
        return {
            restrict: 'E',
            scope: {
                message: '@',
                isOpen: '=',
                mode: '@',
                accept: "&"
            },
            transclude: true,
            templateUrl: "/App/template/infoWindow.html",
            controller: function ($scope, $element, $attrs) {
                $scope.state = "close";
                $scope.modalTitle = $attrs.title;
                $scope.acceptDelete = function () {
                    $scope.accept()
                };
            },
            link: function ($scope, $elem, $attrs) {
                var window = $elem.find('[modal-confirm]');

                $scope.$watch(function () { return $scope.isOpen }, function (obj) {
                    if (obj == true) {
                        if ($scope.state == "close") {
                            $scope.state = "open";
                            window.modal({
                                backdrop: 'static',
                                keyboard: false
                            });
                        }
                    }
                    if (obj == false) {
                        if ($scope.state == "open") {
                            $scope.state = "close";
                            $('.modal-backdrop').remove();
                            window.modal('hide');
                        }
                    }
                }, true);

                if ($scope.mode && $scope.mode == "confirm") {
                    var rejectBtn = window.find('#reject');
                    rejectBtn.on('click', function (event) {
                        $scope.isOpen = false;
                        $scope.$$phase || $scope.$apply();
                        window.modal('hide');
                    });

                    var confirmBtn = window.find('#confirm');
                    confirmBtn.on('click', function (event) {
                        $scope.isOpen = false;
                        $scope.$$phase || $scope.$apply();
                        $scope.acceptDelete();
                    });
                }
            }
        };
    }]);
});