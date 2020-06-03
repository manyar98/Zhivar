//drop down list with treelist, can cascade from.
//To Cascade Set: CascadeId, SelectedId, MemberID 
//Output: MemberID
define(['application', 'dataService', 'kendoUi', 'kendoGridFa'], function (app) {
    app.register.directive('treeDropDown', ['dataService', function (dataService) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                options: '=',
                change: "&"
            },
            transclude: true,
            templateUrl: "/App/template/treeDropDown.html",
            controller: function ($scope, $element, $attrs) {
                $scope.onChange = function (id) {
                    $scope.change({ id: id });
                };
            },
            link: function ($scope, $elem, $attrs) {
                var selfChange = false;

                if (angular.isUndefined($scope.options.ControlName) || $scope.options.ControlName.length < 1)
                    throw Error("ControlName is not Defined or Incorrect!");

                function guid() {
                    function s4() {
                        return Math.floor((1 + Math.random()) * 0x10000)
                          .toString(16)
                          .substring(1);
                    }
                    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
                      s4() + '-' + s4() + s4() + s4();
                }

                var ddlTerr = guid();
                var isLoaded = false;
                $scope.dropDownModel = {
                    title: "",
                    optionLabel: "لطفا انتخاب نمایید",
                    disabled: false
                };

                $elem.find(".tree-drop-down-list").attr("id", ddlTerr);

                var panelCombo = "panel" + ddlTerr;
                $elem.find(".drop-down-list-wrapper").attr("id", panelCombo);

                var kCombo = "k-combo" + ddlTerr;
                $elem.find(".k-combo-tree").attr("id", kCombo);

                var kinput = "k-input" + ddlTerr;
                $elem.find(".k-input-combo-tree").attr("id", kinput);

                var mainPanel = "mainPanel" + ddlTerr;
                $elem.find(".panel-main-tree").attr("id", mainPanel);

                if (angular.isDefined($scope.options.Disabled)) {
                    $scope.$watch('options.Disabled', function (newValue, oldValue) {
                        if (newValue == false) {
                            //to show treedropdown list
                            $("#" + mainPanel).on("click", "#" + kCombo, function () {
                                $("#" + panelCombo).slideToggle("fast");
                                var r = "#" + kCombo + " .k-state-default";
                                $(this).find(".k-dropdown-wrap").addClass("focused");
                            });
                            $scope.dropDownModel.disabled = false;
                        }
                        else if (newValue == true) {
                            $scope.dropDownModel.disabled = true;
                        }
                    });
                }

                //error optionLabel
                if (angular.isDefined($scope.options.optionLabel)) {
                    $scope.$watch('options.optionLabel', function (newValue, oldValue) {
                        if (newValue != "") {
                            $scope.dropDownModel.optionLabel = newValue;
                        }
                    });
                }

                //error message
                if (angular.isDefined($scope.options.MessageError)) {
                    $elem.find(".combo-message").attr("text-required", $scope.options.MessageError);
                    $scope.$watch('options.MessageError', function (newValue, oldValue) {
                        if (newValue != "") {
                            $elem.find(".combo-message").attr("text-required", newValue);
                        }
                    });
                }

                //required
                if (angular.isDefined($scope.options.Required)) {
                    $scope.$watch('options.Required', function (newValue, oldValue) {
                        if (newValue == true) {
                            var msg = "اجباریست";
                            if (angular.isDefined($scope.options.MessageError)) {
                                msg = $scope.options.MessageError;
                            }
                            $elem.find(".combo-message").attr("text-required", msg);
                        }
                        else {
                            $elem.find(".combo-message").removeAttr("text-required");
                        }
                    });
                }

                var dataSource = new kendo.data.TreeListDataSource({
                    serverSorting: true,
                    transport: {
                        read: {
                            type: "GET",
                            url: $scope.options.Url,
                            dataType: "json"
                        },
                        parameterMap: function (options, operation) {
                            if (options.filter == undefined || options.filter == null) {
                                var parentId = null;

                                if (options.id) {
                                    parentId = options.id;
                                }

                                if (angular.isDefined($scope.options.CascadeId)) {
                                    parentId = $scope.options.CascadeId;
                                    var id = null;
                                    if (options.id) {
                                        id = options.id;
                                    }
                                    options.filter = { filters: [{ field: "filterableId", operator: "eq", value: parentId }, { field: "id", operator: "eq", value: id }] };
                                }
                                else {
                                    options.filter = { filters: [{ field: "parentId", operator: "eq", value: parentId }, { field: "needSetHasChildren", operator: "eq", value: true }] };
                                }
                            }

                            options.OperationAccess = $scope.options.OperationAccess;
                            return options;
                        }
                    },
                    schema: {
                        data: function (result) {
                            return dataService.processResponse(result);
                        },
                        total: function (data) {
                            return dataService.getCount(data);
                        },
                        model: {
                            id: $scope.options.ID,
                            fields: {
                                Title: { type: "string", field: $scope.options.Title },
                                parentId: { type: "number", field: $scope.options.ParentId, nullable: true },
                                hasChildren: { field: "HasChildren", nullable: true },
                            },
                            expanded: false
                        }
                    },
                    batch: false,
                    serverFiltering: true,
                });

                $("#" + ddlTerr).kendoTreeList({
                    dataSource: dataSource,
                    sortable: true,
                    pageable: true,
                    height: 200,
                    columnMenu: false,
                    selectable: "single",
                    dataBound: function () {
                        setTextCombo();
                    },
                    change: function (e) {
                        selfChange = true;
                        var grid = e.sender;
                        var selectedData = grid.dataItem(grid.select());
                        $("#" + kinput).text(selectedData.Title);
                        $("#" + panelCombo).slideUp();

                        $scope.$apply(function () {
                            $scope.options.MemberID = selectedData.id;
                            $scope.dropDownModel.title = selectedData.Title;
                            $scope.onChange(selectedData.id);
                        });

                        $scope.options.MemberID = selectedData.id;
                        $scope.dropDownModel.title = selectedData.Title;
                    },
                    columns: [{
                        field: "Title",
                        title: "عنوان",
                        filterable: {
                            extra: false
                        },
                    }]
                });

                //set value for edit
                $scope.$watch('options.MemberID', function (newValue, oldValue) {
                    if (selfChange == false) {
                        if (angular.isUndefined($scope.options.CascadeId)) {
                            if (newValue > 0) {
                                getById(newValue);
                            }
                            else if (newValue == 0) {
                                $("#" + kinput).text("");
                                $scope.dropDownModel.title = $scope.options.optionLabel || " ";
                            }
                        }
                        else if (newValue == 0) {
                            $("#" + kinput).text("");
                            $scope.dropDownModel.title = $scope.options.optionLabel || " ";
                        }
                    }
                    selfChange = false;
                }, true);

                //watch for cascade
                if (angular.isDefined($scope.options.CascadeId)) {
                    $scope.$watch('options.CascadeId', function (newValue, oldValue) {
                        $("#" + kinput).text("");
                        $scope.dropDownModel.title = "";

                        if (!angular.equals({}, newValue) && newValue != -1 && newValue != oldValue) {
                            var tree = $("#" + ddlTerr).data("kendoTreeList");
                            tree.dataSource.read();
                        }
                    }, true);
                }

                //to close treeDropDown
                $('body').click(function (event) {
                    var target = $(event.target).parents("#" + mainPanel);
                    if (target.length == 0) {
                        $("#" + mainPanel).find(".k-dropdown-wrap").removeClass("focused");
                        $("#" + panelCombo).slideUp();
                    }
                });

                //set text of combo
                function setTextCombo() {
                    if (angular.isUndefined($scope.options.CascadeId)) {
                        if ($scope.options.MemberID > 0) {
                            getById($scope.options.MemberID);
                        }
                    }
                    else if (angular.isDefined($scope.options.MemberID) && $scope.options.MemberID != 0 && $scope.options.SelectedId != -1) {
                        getByIdWithCascade($scope.options.MemberID);
                        $scope.options.SelectedId = -1;
                    }
                }

                //set text of input
                function getByIdWithCascade(id) {
                    var req = {
                        operationAccess: $scope.options.OperationAccess,
                        id: id
                    };

                    if (angular.isUndefined($scope.options.GetByIdUrl) || $scope.options.GetByIdUrl.length < 10)
                        throw Error("GetByIdUrl not Defined or Incorrect in " + $scope.options.ControlName + " Control!");

                    dataService.getAllByParams($scope.options.GetByIdUrl, req).then(function (data) {
                        $("#" + kinput).text(data[$scope.options.Title]);
                        $scope.dropDownModel.title = data[$scope.options.Title];
                    });
                }

                //set text of input
                function getById(id) {
                    var filterInfo = {
                        OperationAccess: $scope.options.OperationAccess,
                        filter: {
                            Logic: "and",
                            Filters: [{ Operator: "eq", Field: "ID", Value: id }]
                        }
                    }

                    dataService.getAllByParams($scope.options.Url, filterInfo).then(function (data) {
                        if (data.length > 0) {
                            data.forEach(function (item) {
                                if (item.ID == id) {
                                    $("#" + kinput).text(item[$scope.options.Title]);
                                    $scope.dropDownModel.title = item[$scope.options.Title];
                                }
                            });
                        }
                    });
                }

            }
        }//end of return function
    }]);
});
