define(['angularAMD', 'messageService'], function (app) {
    app.directive('buttonValidation', ['messageService', '$timeout', '$parse',
        function (messageService, $timeout, $parse) {
            return {
                restrict: 'E',
                scope: {
                    click: '&', //event
                    classStyle: '@', //class button
                    mode: '@',
                    visible: '=', //show / hide  button
                    alwaysEnabled: '=?'
                },
                replace: true,
                transclude: true,
                template: '<button type="button" ng-click="accept()" class="button-validation {{classStyle}}" ng-disabled="disabled"><ng-transclude></ng-transclude></button>',
                controller: function ($scope, $element, $attrs) {
                    $scope.disabled = false;

                    //call controller click event or show warning message
                    $scope.accept = function () {
                        if ($scope.status) {
                            if (!$scope.alwaysEnabled)
                                $scope.disabled = true;
                            $scope.click()

                            if (angular.isDefined($attrs.scroll) && $attrs.scroll == "top")
                                $('#contentPage').animate({ scrollTop: 0 }, 1500);
                        }
                        else {
                            var elmParent = $("<ul></ul>")
                            for (var i = 0; i < $scope.messages.length; i++) {
                                elmParent.append('<li>' + $scope.messages[i].message + '</li>');
                            }
                            messageService.warningMessage(elmParent);
                            $(".vTooltip").kendoTooltip({ position: "top" });
                        }
                    };
                },
                link: function ($scope, $elem, $attrs) {

                    $elem.on('mousedown', function (e) {
                        $scope.checkControls();
                    });

                    //check all controls for detect attributes
                    $scope.checkControls = function () {
                        $(".vTooltip").remove();
                        elms = $elem.closest("form").find('[text-required]');
                        elmTextlengths = $elem.closest("form").find('input[text-length]');
                        elmEmails = $elem.closest("form").find('input[text-email]');
                        elmPersianAlphas = $elem.closest("form").find('input[persian-alpha]');
                        elmWebSite = $elem.closest("form").find('input[web-site]');
                        elmWithoutSymbols = $elem.closest("form").find('input[without-symbol]');

                        elmEnglishAlphasNumeric = $elem.closest("form").find('input[english-alpha-numeric]');

                        $scope.messages = [];
                        $scope.status = true;
                        $.each(elms, function (index, item) {
                            if (!checkControlTag(item)) {
                                var div = $(item).closest('div').css('position', 'relative');
                                div.append("<i title='" + $(item).attr("text-required") + "' class='vTooltip fa fa-info-circle'></i>");
                                $scope.messages.push({ message: $(item).attr("text-required") });
                                $scope.status = false;
                            }
                        });

                        if ($scope.status) {
                            $.each(elmTextlengths, function (index, item) {
                                var len = $(item).attr('text-length');
                                if ($(item).val() != "") {
                                    if ($(item).val().length != len) {
                                        var div = $(item).closest('div').css('position', 'relative');
                                        div.append("<i title='مقدار وارد شده باید " + len + " کاراکتر باشد' class='vTooltip fa fa-info-circle'></i>");
                                        $scope.messages.push({ message: "مقدار وارد شده باید " + len + " کاراکتر باشد " });
                                        bindControl("none", item, "input")
                                        $scope.status = false;
                                    }
                                }
                            });

                            $.each(elmEmails, function (index, item) {
                                if ($(item).val() != "") {
                                    if ($(item).hasClass("warning")) {
                                        var div = $(item).closest('div').css('position', 'relative');
                                        div.append("<i title='ایمیل را به صورت صحیح وارد کنید' class='vTooltip fa fa-info-circle'></i>");
                                        $scope.messages.push({ message: "ایمیل را به صورت صحیح وارد کنید" });
                                        bindControl("none", item, "input")
                                        $scope.status = false;
                                    }
                                }
                            });

                            $.each(elmPersianAlphas, function (index, item) {
                                var val = $(item).val();
                                var result = RegExp(/[a-z|A-Z|0-9]+/g).test(val) || RegExp(/[-!$%^&*#@()_+\|~=`{}\[\]:";'<>?,.\/]/).test(val);

                                if (result) {
                                    var div = $(item).closest('div').css('position', 'relative');
                                    div.append("<i title='از کاراکترهای فارسی استفاده نمایید' class='vTooltip fa fa-info-circle'></i>");
                                    $scope.messages.push({ message: "از کاراکترهای فارسی استفاده نمایید" });
                                    bindControl("none", item, "input")
                                    $scope.status = false;
                                }
                            });

                            $.each(elmEnglishAlphasNumeric, function (index, item) {
                                var val = $(item).val();
                                var result = RegExp(/^[a-z|A-Z|0-9]*$/g).test(val);

                                if (!result) {
                                    var div = $(item).closest('div').css('position', 'relative');
                                    div.append("<i title='از اعداد و کاراکترهای انگلیسی استفاده نمایید' class='vTooltip fa fa-info-circle'></i>");
                                    $scope.messages.push({ message: "از اعداد و کاراکترهای انگلیسی استفاده نمایید" });
                                    bindControl("none", item, "input")
                                    $scope.status = false;
                                }
                            });
                            $.each(elmWebSite, function (index, item) {
                                var val = $(item).val();
                                var result = !val || RegExp(/^((http[s]?):\/)?\/?([^\/\s]+)((\/\w+)*)$/).test(val);

                                if (!result) {
                                    var div = $(item).closest('div').css('position', 'relative');
                                    div.append("<i title='آدرس معتبر نمی باشد' class='vTooltip fa fa-info-circle'></i>");
                                    $scope.messages.push({ message: "آدرس معتبر نمی باشد" });
                                    bindControl("none", item, "input")
                                    $scope.status = false;
                                }
                            });
                            $.each(elmWithoutSymbols, function (index, item) {
                                var val = $(item).val();
                                var result = RegExp(/[-!$%^&*#@()_+\|~=`{}\[\]:";'<>?,.\/]/).test(val);

                                if (result) {
                                    var div = $(item).closest('div').css('position', 'relative');
                                    div.append("<i title='معتبر نمی باشد' class='vTooltip fa fa-info-circle'></i>");
                                    $scope.messages.push({ message: "معتبر نمی باشد" });
                                    bindControl("none", item, "input")
                                    $scope.status = false;
                                }
                            });
                        }

                        $scope.$$phase || $scope.$apply();
                    }

                    $("body").off("keypress");

                    if (angular.isDefined($scope.mode)) {
                        //if open modal then close without enter, after this by press enter modal clicked. (this keypress event call 'modal enter')
                        //because this event attached for modal.
                        if ($scope.mode != "modal")
                            $elem.closest("body").on("keypress", function (e) {
                                if (e.keyCode == 13) {

                                    //ButtonValidation in tabular pages
                                    if ($scope.mode == "tab")
                                        if ($elem.closest(".tab-pane").is(":visible") == false)
                                            return false;

                                    $scope.checkControls();
                                    $scope.accept();
                                    return false;
                                }
                            });
                    }

                    function setStarLabel() {
                        setTimeout(function () {
                            var num = $("[text-required]").closest(".form-group").find(".required-star").length;
                            if (num == 0) {
                                var textRequired = $("[text-required]").closest(".form-group").find("label").prepend('<span class="required-star" style="color:red">*</span>');
                                if (textRequired.length == 0) {
                                    setTimeout(function () {
                                        textRequired = $("[text-required]").closest(".form-group").find("label").prepend('<span class="required-star" style="color:red">*</span>');
                                    }, 700);
                                }
                            }
                        }, 500);
                    }

                    setStarLabel();

                    //define valid or invalid value of control for warning and tooltip
                    function checkControlTag(elm) {
                        var status = true;
                        if (angular.isDefined($(elm).attr("k-disabled"))) {
                            var parse = $parse($(elm).attr("k-disabled"))($scope.$parent);
                            if (parse == true) {
                                return status;
                            }
                        }

                        var name = $(elm).tagName();
                        var attr = $(elm).getAttrKendo();

                        switch (name) {
                            case "select":
                                if ($("#" + $(elm).attr("id") + " option:selected").text() == "" ||
                                    $("#" + $(elm).attr("id") + " option:selected").val() == "" ||
                                    $("#" + $(elm).attr("id") + " option:selected").val() == "-1" ||
                                    $("#" + $(elm).attr("id") + " option:selected").val() == "0") {
                                    status = false;
                                    bindControl(attr, elm, name);
                                }
                                break;
                            case "input":
                                if ($(elm).val() == "") {
                                    status = false;
                                    bindControl(attr, elm, name);
                                }
                                break;
                            case "textarea":
                                if ($(elm).val() == "") {
                                    status = false;
                                    bindControl(attr, elm, name);
                                }
                                break;
                        }

                        return status;
                    }

                    // add/remove warning class
                    function bindControl(attr, elm, tag) {
                        switch (attr) {
                            case "none":
                                $(elm).addClass('warning');
                                if (tag == "input") {
                                    $(elm).on('keypress', function () {
                                        if ($(this).hasClass('warning')) {
                                            $(this).removeClass('warning');
                                        }
                                    })
                                }
                                else {
                                    $(elm).on('change', function () {
                                        if ($(this).hasClass('warning')) {
                                            $(this).removeClass('warning');
                                        }
                                    })
                                }
                                break;
                            case "combobox":
                                var datepicker = $(elm).attr("data-role")
                                if (angular.isDefined($(elm).attr("data-role")) && datepicker == "datepicker") {
                                    $(elm).parent('.k-picker-wrap').addClass('warning');
                                    $(elm).on('change', function () {
                                        var dateP = $(elm).parent('.k-picker-wrap');
                                        if (dateP.hasClass('warning')) {
                                            dateP.removeClass('warning');
                                        }
                                    });
                                }
                                else {
                                    $(elm).closest('span').find('.k-dropdown-wrap').addClass('warning');
                                    $(elm).on('change', function () {
                                        var combobox = $(elm).closest('span').find('.k-dropdown-wrap');
                                        if (combobox.hasClass('warning')) {
                                            combobox.removeClass('warning');
                                        }
                                    });
                                }
                                break;
                            case "dropdownlist":
                                $(elm).closest('.k-dropdown').find('.k-dropdown-wrap').addClass('warning');
                                $(elm).on('change', function () {
                                    var dropdownlist = $(elm).closest('.k-dropdown').find('.k-dropdown-wrap');
                                    if (dropdownlist.hasClass('warning')) {
                                        dropdownlist.removeClass('warning');
                                    }
                                })
                                break;
                            case "autocomplete":
                                $(elm).addClass('warning');
                                $(elm).on('keypress', function () {
                                    if ($(this).hasClass('warning')) {
                                        $(this).removeClass('warning');
                                    }
                                })
                                break;
                        }
                    }

                    $.fn.tagName = function () {
                        return this.prop("tagName").toLowerCase();
                    };

                    $.fn.getAttrKendo = function () {
                        var kendoAttr = "none";
                        if (this.length) {
                            $.each(this[0].attributes, function (index, attr) {
                                if (attr.value.toLowerCase() == "combobox") {
                                    kendoAttr = "combobox";
                                }
                                if (attr.value.toLowerCase() == "dropdownlist") {
                                    kendoAttr = "dropdownlist";
                                }
                                if (attr.value.toLowerCase() == "autocomplete") {
                                    kendoAttr = "autocomplete";
                                }
                            });
                        }
                        return kendoAttr;
                    };

                    // show/hide button validation on page by show attr on this, use ng-if instead of it.
                    if (angular.isDefined($attrs.visible)) {
                        $scope.$watch("visible", function (newValue, oldValue) {
                            if (newValue)
                                $elem.show();
                            else
                                $elem.hide();
                        });
                    }

                } //end of link function
            };
        }
    ]);
});
