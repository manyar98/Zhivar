define(['angularAMD'], function (app) {
    app.service('validatorsService', ['$rootScope', 'messageService', function ($rootScope, messageService) {
        return {
            validatorNationalCode: function (nationalCode) {
                if (nationalCode == null || nationalCode == "") {
                    messageService.error("لطفا کد ملی را صحیح وارد نمایید!");
                    return false;
                }

                if (nationalCode.length != 10) {
                    messageService.error("طول کد ملی باید ده کاراکتر باشد!");
                    return false;
                }

                var numCode = nationalCode.replace(/[^0-9]/g, '')
                if (numCode != nationalCode) {
                    messageService.error("لطفا کد ملی را صحیح وارد نمایید!");
                    return false;
                }
                if (nationalCode == '1111111111' ||
                    nationalCode == '0000000000' ||
                    nationalCode == '2222222222' ||
                    nationalCode == '3333333333' ||
                    nationalCode == '4444444444' ||
                    nationalCode == '5555555555' ||
                    nationalCode == '6666666666' ||
                    nationalCode == '7777777777' ||
                    nationalCode == '8888888888' ||
                    nationalCode == '9999999999') {
                    messageService.error("لطفا کد ملی را صحیح وارد نمایید!");
                    return false;
                }
                var codeArry = nationalCode.split('');
                var num0 = parseInt(codeArry[0]) * 10;
                var num2 = parseInt(codeArry[1]) * 9;
                var num3 = parseInt(codeArry[2]) * 8;
                var num4 = parseInt(codeArry[3]) * 7;
                var num5 = parseInt(codeArry[4]) * 6;
                var num6 = parseInt(codeArry[5]) * 5;
                var num7 = parseInt(codeArry[6]) * 4;
                var num8 = parseInt(codeArry[7]) * 3;
                var num9 = parseInt(codeArry[8]) * 2;
                var a = parseInt(codeArry[9]);
                var b = (((((((num0 + num2) + num3) + num4) + num5) + num6) + num7) + num8) + num9;
                var c = b % 11;

                var isValid = (((c < 2) && (a == c)) || ((c >= 2) && ((11 - c) == a)));
                if (isValid) {
                    return true;
                }
                messageService.error("لطفا کد ملی را صحیح وارد نمایید!");
                return false;
            },
            validatorTime: function (time) {
                var timePattern = new RegExp(/^(([0-1][0-9])|([2][0-3])):([0-5][0-9])$/);
                if (!timePattern.test(time)) {
                    return false;
                }

                return true;
            },
            validatorGregorianDate: function (dateStr) {
                var pattern = new RegExp(/^(20[0-9]{2})-(1[0-2]|0[1-9])-(3[01]|0[1-9]|[12][0-9])$/);
                return pattern.test(dateStr);
            },
            validatorJalaliDate: function (dateStr) {
                var pattern = new RegExp(/^(1[234][0-9]{2})-(1[0-2]|0[1-9])-(3[01]|0[1-9]|[12][0-9])$/);
                return pattern.test(dateStr);
            }
        }
    }]);
});