define(['angularAMD'], function (app) {
    app.directive('modalShow', ['$parse', '$rootScope', '$state',
        function ($parse, $rootScope, $state) {
            return {
                restrict: 'EA',
                scope: {
                    showDialog: "=",
                    redirect: "&",
                    newmodel: "&"
                },
                transclude: true,
                templateUrl: "/App/template/popupRedirect.html",
                controller: function ($scope, $element, $attrs) {

                },
                link: function ($scope, $elem, $attrs) {
                    var popup = null;

                    //hide/show new record button popup
                    if (angular.isDefined($attrs.hideNewRecord)) {
                        $scope.hideNewRecord = $attrs.hideNewRecord;
                    }
                    else {
                        $scope.hideNewRecord = false;
                    }

                    $scope.toggleModal = function (visible) {
                        if (popup == null) {
                            popup = $elem.kendoWindow({
                                width: "50%",
                                title: $attrs.title,
                                actions: [],
                                modal: true,
                                visible: false,
                            });
                        }

                        if (visible == true) {
                            $elem.find(".isshow").show();
                            popup.data("kendoWindow").open().center();
                        }
                        else if (visible == false) {
                            $scope.acceptPage();
                        }
                    }

                    $scope.redirectPage = function () {
                        $scope.acceptPage();
                        $scope.redirect();
                    }

                    $scope.newPage = function () {
                        $scope.acceptPage();
                        $scope.newmodel();
                    }

                    //close modal
                    $scope.acceptPage = function () {
                        $rootScope.panelShowDialog = false;
                        popup.data("kendoWindow").close();
                    }

                    $scope.$watch("showDialog", function (newValue, oldValue) {
                        $scope.toggleModal(newValue);
                    }, true);


                }
            };
        }
    ]);
});