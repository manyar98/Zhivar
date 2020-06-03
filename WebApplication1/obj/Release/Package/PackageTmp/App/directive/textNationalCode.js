define(['angularAMD'], function (app) {
    app.directive('textNationalCode', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            require: 'ngModel',
            link: function ($scope, $elem, $attrs, ngModel) {

                $elem.on('keypress', function (evt) {
                    var charCode = (evt.which) ? evt.which : evt.keyCode;
                    //console.log(charCode)
                    if (evt.ctrlKey == false && charCode === 102)
                        return true;

                    if (evt.ctrlKey == false && (charCode != 99 || charCode != 118)) {
                        if ((charCode > 31 && (charCode < 48 || charCode > 57)) && (charCode != 46)) { 
                            return false;
                        }
                        if ($attrs.sep) {
                            var value = $(this).val();
                            value = value.toString().replace(/,/g, "");
                            var value = value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                            $(this).val(value)
                        }
                    }
                    return true;

                });

                if ($attrs.sep) {
                    $scope.$watch($attrs.ngModel, function (value) {
                        var value = $elem.val();
                        value = value.toString().replace(/,/g, "");
                        var value = value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
                        $elem.val(value)
                    });
                }

                $elem.on('blur', function (evt) {
                    var text = $(this).val(), firstLetter = text.substring(0, 1), validFirstLetter = "f";

                    var transformedInput = firstLetter === validFirstLetter ? validFirstLetter + text.substring(1).replace(/[^0-9]/g, '') : text.replace(/[^0-9]/g, '');
                    if (transformedInput !== text) {
                        $(this).val("");
                    }
                });
            }
        };
    }]);
});