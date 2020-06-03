define(['application'], function (app) {
    app.register.directive('smsEditor', [function ($compile) {
        return {
            restrict: 'E',
            transclude: true,
            templateUrl: 'sms-editor.html?ver=1.2.9.1',
            scope: {
                sms: '=',
                onsuccess: '=',
                open: '=',
                modalTitle: '=',
                customFields: '=',
                job: '='
            },
            link: function (scope, element, attrs) {
                scope.initSmsEditor = function () {
                    scope.alertBoxVisible = false;
                    scope.alertMessage = "";
                    scope.charNumber = 0;
                    scope.smsNumber = 0;
                    scope.totalChar = 70;
                    scope.remainedChar = 70;
                    scope.lang = "فارسی";
                    scope.smsText = scope.sms;

                    if (scope.customFields.indexOf("contactFields") > -1) scope.contactFields = true;
                    if (scope.customFields.indexOf("invoiceFields") > -1) scope.invoiceFields = true;
                    if (scope.customFields.indexOf("chequeFields") > -1) scope.chequeFields = true;

                    $('#editSmsModal').modal({ keyboard: false }, 'show');
                    $("#editSmsModal .modal-dialog").draggable({ handle: ".modal-header" });
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initSmsEditor();
                    scope.open = false;
                }, true);
                scope.submitSms = function () {
                    if (!scope.smsText || scope.smsText === "") {
                        scope.alertBoxVisible = true;
                        scope.alertMessage = "متن پیامک را وارد کنید.";
                        return;
                    }
                    scope.sms = scope.smsText;
                    scope.onsuccess(scope.smsText);
                    $("#editSmsModal").modal("hide");
                };
                scope.textChange = function (smsText) {
                    scope.charNumber = smsText.length;
                    if (scope.isDoubleByte(smsText)) { // شامل کاراکترهای فارسی
                        scope.smsNumber = Math.ceil(scope.charNumber / 70);
                        scope.totalChar = scope.smsNumber * 70;
                        scope.lang = "فارسی";
                    } else {    // فقط کاراکترهای انگلیسی
                        scope.smsNumber = Math.ceil(scope.charNumber / 160);
                        scope.totalChar = scope.smsNumber * 160;
                        scope.lang = "انگلیسی";
                    }
                    scope.remainedChar = scope.totalChar - scope.charNumber;
                    scope.$apply();
                };
                scope.isDoubleByte = function (str) {
                    for (var i = 0, n = str.length; i < n; i++) {
                        if (str.charCodeAt(i) > 255) {
                            return true;
                        }
                    }
                    return false;
                };
                scope.addField = function (fieldName) {
                    var position = $('#smsText').prop("selectionStart");
                    if (position == undefined) scope.smsText = fieldName;
                    else
                        scope.smsText = [scope.smsText.slice(0, position), fieldName, scope.smsText.slice(position)].join('');

                    scope.textChange(scope.smsText);
                };
            }
        };
    }]);
});