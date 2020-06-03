define(['angularAMD', 'dataService'], function (app) {
    app.directive('captchaImage', ['dataService', '$rootScope', function (dataService, $rootScope) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                captchaoption: '=',
            },
            transclude: true,
            template: '<div class="captcha-wrapper"><img class="captcha-image" src="{{captchaImageSrc}}" ng-style="imgStyle" ng-click="generate()"/><a ng-click="generate()"><i class="glyphicon glyphicon-refresh"></i></a></div>',
            controller: function ($scope, $element, $attrs) {
                $scope.$on('callRegenarateCaptchaMethod', function (event) {
                    $scope.generate();
                });

                $scope.generate = function () {
                    dataService.postData('/app/api/Login/GetCaptcha').then(function (data) {
                        $scope.captchaImageSrc = "data:image/png;base64," + data;
                    });
                }
            },
            link: function ($scope, $elem, $attrs) {
                $scope.imgStyle = {
                    'height': $scope.captchaoption.height,
                    'width': $scope.captchaoption.width,
                }
            }
        };
    }]);
});