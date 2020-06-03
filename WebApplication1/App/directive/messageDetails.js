define(['angularAMD'], function (app) {
    app.directive('messageDetails', ['$parse', function ($parse) {
        return {
            restrict: 'E',
            scope: {
                hasOpen: '=',
                accept: "&",
                item: '=',
                image: '=',
                //download:'='
            },
            transclude: true,
            templateUrl: "/App/template/messageDetails.html",
            controller: function ($scope, $element, $attrs) {
                $scope.acceptDelete = function () {
                    $scope.accept()
                };
                $scope.download = function () {
                    if ($scope.image != null) {
                        var l = document.getElementById('download');
                        l.click();
                    }
                }
            },

            link: function ($scope, $elem, $attrs) {
                var window = $elem.find('#messageDetails');
                var popup = window.kendoWindow({
                    width: "500px",
                    title: $attrs.title,
                    actions: ["Close"],
                    modal: true,
                    visible: false
                });
                $('#download').attr({ href: $scope.image });
                //$('#download').attr({ download: file.name });
           
                $scope.$watch(function () { return $scope.hasOpen }, function (obj) {
                    if (obj == true) {
                        window.data("kendoWindow").open().center();
                    }
                    if (obj == false) {
                        window.data("kendoWindow").close();
                    }
                }, true);

                var btnClc = window.find('#Dissuasion');
                btnClc.on('click', function (event) {
                    $scope.hasOpen = false;
                    $scope.$apply();
                    window.data("kendoWindow").close();
                });

            }

        };
    }]);

});