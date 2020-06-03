define(['angularAMD', 'kendoUi', 'kendoGridFa', 'dataService', 'messageService'], function (app) {
    app.directive('autoComplete', ['$timeout', '$parse', 'messageService', 'dataService',
        function ($timeout, $parse, messageService, dataService) {
            return {
                restrict: 'A',
                scope: { options: '=' },
                require: '?ngModel',
                templateUrl: '/App/template/autoComplete.html',
                controller: function ($scope, $element, $attrs) {
                },
                link: function (iScope, iElem, iAttrs, controller) {
                    if (angular.isUndefined(iScope.options) || angular.equals(iScope.options, {}))
                        throw new Error("options is undefined or incorrect!");
                    if (angular.isUndefined(iScope.options.url) && angular.isUndefined(iScope.options.dataSource))
                        throw new Error(iScope.options.controlName + ": url or data must be defined in options!");
                    if (angular.isDefined(iScope.options.url) && angular.isDefined(iScope.options.dataSource))
                        throw new Error(iScope.options.controlName + ": url or data must be defined in options!");
                    var autoCompleteDataList = [], filters = [], filterByModel = false;
                    function guidGenerate() {
                        function s4() { return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1); }
                        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
                    }
                    iScope.id = guidGenerate();
                    function setValue() {
                        var defaultModelText = "";
                        var value = autoCompleteDataList.filter(function (v) { return v[iScope.options.dataValueField] == controller.$modelValue });
                        if (value && value[0]) defaultModelText = value[0][iScope.options.dataTextField + ""];
                        iScope.autoCompleteTmpl && iScope.autoCompleteTmpl.value(defaultModelText);
                    }
                    function getDataByID(id) {
                        var item = autoCompleteDataList.filter(function (v) { return v[iScope.options.dataValueField] == id });
                        if (item && item[0]) return item[0];
                        return null;
                    }
                    function changeEnableState(state) {
                        if (typeof (state) !== "boolean")
                            throw new Error(iScope.options.controlName + ": AutoComplete enable cannot work without boolean value!");
                        var kendoAutoCompleteData = $("#" + iScope.id).data("kendoAutoComplete");
                        if (kendoAutoCompleteData) {
                            if (state == true)
                                kendoAutoCompleteData.enable(true);
                            else
                                kendoAutoCompleteData.enable(false);
                            typeof (iScope.options.changeEnableState) === "function" && iScope.options.changeEnableState(state);
                        }
                    }

                    iScope.$watch('options.url', function (newValue, oldValue) {
                        if (angular.isDefined(newValue)) {
                            iScope.dataSource = new kendo.data.DataSource({
                                transport: {
                                    read: {
                                        url: iScope.options.url,
                                        dataType: "json"
                                    },
                                    parameterMap: function (options, operation) {
                                        if (operation == "read") {
                                            if (options.filter && options.filter.filters) {
                                                if (!filterByModel)
                                                    options.filter.filters = options.filter.filters.filter(function (v) { return v.field != iScope.options.dataValueField });
                                                if (filters.length >= 1)
                                                    filters.forEach(function (filter) { options.filter.filters.push(filter); });
                                            }
                                            else if (filters.length >= 1) {
                                                options.filter = { filters: filters };
                                            }
                                        }
                                        return options;
                                    }
                                },
                                schema: {
                                    data: function (result) {
                                        autoCompleteDataList = dataService.processResponse(result);
                                        typeof (iScope.options.getAll) === "function" && iScope.options.getAll(autoCompleteDataList);
                                        if (Array.isArray(autoCompleteDataList) && autoCompleteDataList.length > 0 && filterByModel) {
                                            filterByModel = false;
                                            setValue();
                                        }
                                        return autoCompleteDataList;
                                    }
                                },
                                serverFiltering: iScope.options.serverFiltering || false
                            });
                        }
                    }, true);
                    iScope.$watch('options.dataSource', function (newValue, oldValue) {
                        if (angular.isDefined(newValue)) {
                            if (!Array.isArray(newValue))
                                throw new Error(iScope.options.controlName + ": AutoComplete datasource cannot be set to non-array object!");
                            iScope.dataSource = autoCompleteDataList = newValue;
                        }
                    }, true);
                    iScope.$watch('options.filters', function (newValue, oldValue) {
                        if (angular.isDefined(newValue)) {
                            if (!Array.isArray(iScope.options.filters))
                                throw new Error(iScope.options.controlName + ": AutoComplete filters cannot be set to non-array object!");
                            if (iScope.options.filters.length >= 1)
                                filters = iScope.options.filters;
                        }
                    }, true);
                    iScope.$watch('options.dataTextField', function (newValue, oldValue) {
                        iScope.autoCompleteOptions = {
                            dataTextField: iScope.options.dataTextField,
                            template: '<span data-key="#=' + iScope.options.dataValueField + '#">#=' + iScope.options.dataTextField + '#</span>',
                            filter: iScope.options.filter || "contains", //startswith, endswith and contains.
                            placeholder: iScope.options.placeholder,
                            enable: typeof (iScope.options.enable) == "boolean" ? iScope.options.enable : $parse(iScope.options.enable)(iScope.options),
                            value: iScope.options.value || "",
                            headerTemplate: '<div class="notFoundMessage">متأسفانه موردی یافت نشد!</div>',
                            minLength: iScope.options.minLength || 1,
                            enforceMinLength: true,
                            animation: iScope.options.withAnimation ? {
                                open: {
                                    effects: "zoom:in",
                                    duration: 300
                                },
                                close: {
                                    effects: "zoom:out",
                                    duration: 500
                                }
                            } : false,
                            select: function (e) {
                                var elm = e.item.html();
                                if (elm) {
                                    var id = $(elm).attr('data-key');
                                    if (id) {
                                        controller && controller.$setViewValue(+id);
                                        typeof (iScope.options.select) === "function" && iScope.options.select(+id, getDataByID(+id));
                                    }
                                    else {
                                        controller && controller.$setViewValue(null);
                                    }
                                }
                                else {
                                    controller && controller.$setViewValue(null);
                                }
                            },
                            dataBound: function () {
                                var $boundedList = $(this.list);
                                var containerElm = $boundedList.closest('.k-animation-container.k-rtl');
                                var popupElm = $boundedList.closest('.k-list-container.k-popup');
                                $boundedList.css({ 'font-size': '12px', 'font-weight': 'normal' });
                                $boundedList.addClass('auto-complete-list');
                                containerElm.css('left', function (i, v) { return parseFloat(v) - parseFloat(iScope.options.listRightPosition) + "px"; });
                                var noItems = this.list.find(".notFoundMessage");
                                if (!this.dataSource.view()[0]) {
                                    noItems.show();
                                    controller && controller.$setViewValue(null);
                                    this.popup.open();
                                    noItems.hide(iScope.options.hideDuration || '500');
                                    popupElm.hide(iScope.options.hideDuration || '500');
                                }
                                else {
                                    noItems.hide();
                                }
                            },
                            change: function (e) {
                                typeof (iScope.options.change) === "function" && iScope.options.change(this.value(), this.dataItem());
                            },
                            close: function (e) {
                                var widget = e.sender;
                                if (!this.dataSource.view()[0])//if ($(this.element).is(":focus"))
                                    e.preventDefault();
                            }
                        }
                        //iScope.$$phase || iScope.$apply();
                    }, true);
                    if (typeof (iScope.options.enable) == "boolean") {
                        iScope.$watch('options.enable', function (newValue, oldValue) { changeEnableState(newValue); });
                    }
                    iScope.$watch('options.required', function (newValue, oldValue) {
                        if (typeof (newValue) !== "boolean")
                            throw new Error(iScope.options.controlName + ": AutoComplete required cannot work without boolean value!");
                        if (newValue == true)
                            iElem.find("#" + iScope.id).attr("text-required", iScope.options.textRequired);
                        else
                            iElem.find("#" + iScope.id).removeAttr("text-required");
                    });
                    iScope.$watch("options.visible", function (newValue, oldValue) {
                        if (typeof (newValue) !== "boolean")
                            throw new Error(iScope.options.controlName + ": AutoComplete visibility cannot work without boolean value!");
                        if (newValue)
                            iElem.show();
                        else
                            iElem.hide();
                    });
                    if (typeof (iScope.$parent) == "object" && typeof (iScope.options.enable) == "string") {
                        iScope.$watch(function () { return $parse(iScope.options.enable)(iScope.options) }, function (newValue, oldValue) { changeEnableState(newValue); });
                    }
                    iScope.options.setTextAsync = function (txt) {
                        if (typeof txt === 'string')
                            $("#" + iScope.id).data("kendoAutoComplete").value(txt);
                        else
                            throw new Error(iScope.options.controlName + ": setTextAsync method parameter must be string!");
                    }
                    if (controller) {
                        iScope.$watch(function () { return controller.$modelValue; }, function (newValue, oldValue) {
                            if (newValue && newValue != -1 && +newValue > 0) {
                                if (autoCompleteDataList.length > 0)
                                    setValue();
                                else if ((newValue != oldValue || !filterByModel) && autoCompleteDataList.length == 0) {
                                    filterByModel = true;
                                    iScope.dataSource.filter({ field: iScope.options.dataValueField, operator: "eq", value: +newValue });
                                }
                            }
                        }, true);
                    }
                    iScope.autoCompleteClick = function () {
                        iScope.options.autoLoad && (iScope.autoCompleteTmpl.items().length == 0 || iScope.autoCompleteTmpl.value() == "") && iScope.autoCompleteTmpl.search("");
                    }
                    iScope.enableAutoComplete = function (state) {
                        if (typeof (state) !== "boolean")
                            throw new Error(iScope.options.controlName + ": enableAutoComplete method cannot work without boolean value!");
                        iScope.autoCompleteTmpl && iScope.autoCompleteTmpl.enable(state);
                    }
                    iScope.options.clearTextAsync = function () {
                        var kendoAutoCompleteData = $("#" + iScope.id).data("kendoAutoComplete");
                        if (kendoAutoCompleteData && kendoAutoCompleteData.value()) {
                            var oldText = kendoAutoCompleteData.value();
                            kendoAutoCompleteData.value("");
                            controller && controller.$setViewValue(null);
                            typeof (iScope.options.clear) === "function" && iScope.options.clear(oldText);
                        }
                    }
                }
            };
        }
    ]);
});

/*
  - dataValueField is sync with ng-model property
  - minLength, filter, serverFiltering is optional
  - if autoLoad is true, the minlength is not important!
*/