define(['angularAMD'], function (app) {
    app.directive('textNumeric', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function ($scope, $elem, $attrs) {
                $elem.on('keypress', function (evt) {
                    var hybridKey = evt.shiftKey || evt.ctrlKey || evt.altKey;
                    var charCode = (evt.which) ? evt.which : evt.keyCode;

                    if (!hybridKey && evt.keyCode && evt.keyCode == 8 && evt.which && evt.which == 8)//Backspace Key
                        return true;
                    else if (evt.keyCode && evt.keyCode == 9)//Tab Key, Shift + Tab Keies
                        return true;
                    else if (!hybridKey && evt.keyCode && (evt.keyCode == 39 || evt.keyCode == 37))//Right & Left Arrow Key
                        return true;
                    else if (!hybridKey && evt.keyCode && evt.keyCode == 46)//Delete Key
                        return true;

                    if ($attrs.negativeNumeric && charCode == 45)
                        return true;

                    if (hybridKey == false && (charCode != 99 || charCode != 118)) {
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
                    var text = $(this).val();
                    if ($attrs.sep) {
                        text = text.replace(/,/g, "");
                    }
                    if ($attrs.negativeNumeric) {
                        text = +text;
                        text = (text < 0 ? -text : text).toString();
                    }
                    var transformedInput = text.replace(/[^0-9]/g, '');
                    if (transformedInput !== text) 
                        $(this).val("");
                });
            }
        };
    }]);
});