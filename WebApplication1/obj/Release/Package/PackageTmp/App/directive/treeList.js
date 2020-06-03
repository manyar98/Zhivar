//treelist with checkbox
define(['application'], function (app) {
    app.register.directive('treeList', ['$compile', '$parse', 'blockUI', 'helperFactory', function ($compile, $parse, blockUI, helperFactory) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                kOptions: '=',
                kNgDelay: '='
            },
            template: "<div class=\"checkbox-tree-list\"></div>",
            controller: function ($scope, $element, $attrs) {
            },
            link: function ($scope, $elem, $attrs) {
                $elem.attr('id', $attrs.id || 'treeList');
                if ($attrs.tabindex)
                    $elem.attr('tabindex', $attrs.tabindex);

                //checkBox: have checkbox column
                var option = null, checkBox = false, isLoaded = false, isDataBound = false;

                $scope.$watch('kOptions', function (newValue, oldValue) {
                    if (!newValue && $attrs.kNgDelay) {
                        return;
                    }
                    if (!newValue) {
                        helperFactory.writeConsole("treeList: kOptions is mandatory", "error");
                        return;
                    }
                    if (!$scope.kOptions.dataSource || angular.equals({}, $scope.kOptions.dataSource)) {
                        return;
                    }

                    //add checkbox to column
                    if ($scope.kOptions.checkbox) {
                        checkBox = true;
                        if ($scope.kOptions.columns[0])
                            $scope.kOptions.columns[0].template = "<span class='icon-checked'><i id='#=ID #' pid='#=parentId #' class='fa fa-square-o'></i></span> #=" + $scope.kOptions.columns[0].field + "#";
                        else
                            $scope.kOptions.columns.unshift({ template: "<span class='icon-checked'><i id='#=ID #' pid='#=parentId #' class='fa fa-square-o'></i> </span>", filterable: false, width: 110 });
                    }

                    var withOutCombo = true, collapseAndExpand = true, oldDataBound = $scope.kOptions.dataBound;

                    $scope.kOptions.collapse = function (e) {
                        collapseAndExpand = false;//false value for unchange on dataBound  
                    }

                    $scope.kOptions.expand = function (e) {
                        collapseAndExpand = false;
                    }

                    $scope.kOptions.dataBound = function (e) {
                        //for don't change state
                        $($elem).find('button').attr('type', 'button');

                        if (checkBox == true && isLoaded == true && collapseAndExpand) {
                            addRootNodesAttr();
                            loadCheckListNodes();
                            isDataBound = true;
                            withOutCombo = false;
                        }
                        collapseAndExpand = true;
                        if (typeof (oldDataBound) == "function") {
                            oldDataBound.call(this, e);
                        }
                    }

                    if (!isLoaded) {
                        helperFactory.writeConsole("b", "info");
                        $elem.kendoTreeList($scope.kOptions);
                        isLoaded = true;
                    }
                    else if (checkBox == false) {
                        helperFactory.writeConsole("c", "info");
                        var tree = $elem.data("kendoTreeList");
                        tree.setDataSource($scope.kOptions.dataSource);
                        isDataBound = true;
                    }
                    else if (withOutCombo && isDataBound) {
                        helperFactory.writeConsole("d", "info");
                        loadCheckListNodes();
                    }
                });

                //set attr for root element: nameparent and class
                function addRootNodesAttr() {
                    isDataBound = false;
                    var rootNodes = $scope.kOptions.dataSource.rootNodes();

                    $.each(rootNodes, function (index, rootModel) {
                        var attr = "class" + rootModel.id
                        $elem.find('#' + rootModel.id).attr('nameParent', attr);
                        addChildNodeAttr(rootModel.id, attr);
                    });
                }

                //set attr for root element: nameparent and class
                function addChildNodeAttr(id, attr) {
                    var childs = getChildListByParentId(id);
                    childs.forEach(function (child, index, array) {
                        $elem.find('#' + child.id).addClass(attr);
                        var cla = "class" + child.id
                        $elem.find('#' + child.id).attr('nameParent', cla);
                        var parentIds = [];
                        getParentIds(child.parentId, parentIds);
                        $.each(parentIds, function (index, parentId) {
                            var parentClass = "class" + parentId;
                            $elem.find('#' + child.id).addClass(parentClass);
                        });
                        addChildNodeAttr(child.id, attr);
                    });
                }

                //set attr for root element: nameparent and class
                function getParentIds(parentId, parentIds) {
                    var model = getModelById(parentId);
                    if (model) {
                        parentIds.push(model.id);
                        if (model.parentId)
                            getParentIds(model.parentId, parentIds);
                    }
                }

                function loadCheckListNodes() {
                    blockUI.start();
                    elementId = $elem.attr('id');
                    //unchecked all checkbox
                    $("#" + elementId + " .icon-checked i").removeClass('fa-check-square-o');
                    $("#" + elementId + " .icon-checked i").removeClass('fa-NewMinus');
                    $("#" + elementId + " .icon-checked i").removeClass('fa-square-o');
                    $("#" + elementId + " .icon-checked i").addClass('fa-square-o');

                    var treeData = $scope.kOptions.dataSource.data();
                    treeData.forEach(function (item) { item.checked = false; });

                    var sDate = Date.now();
                    var checkboxField = $scope.kOptions.checkbox.field, checkboxList = $scope.kOptions.checkbox.list;
                    if (checkboxList && checkboxList.length) {
                        $.each(checkboxList, function (index, checkboxModel) {
                            var model = getModelById(checkboxModel[checkboxField]);
                            if (model) {
                                var modelElm = $elem.find("#" + checkboxModel[checkboxField]);
                                var id = modelElm.attr("id");
                                var nParent = modelElm.attr("nameParent");
                                var childLength = $elem.find("." + nParent).length;
                                model.checked = true;
                                if (childLength == 0) {
                                    checkChild(id, true);
                                }
                                else {
                                    if (modelElm.hasClass('fa-square-o'))
                                        checkParent(model.ID, "checkParent");
                                }
                                checkParentByChilds($(modelElm));
                            }
                        });
                    }
                    helperFactory.writeConsole(Date.now() - sDate, "info");
                    blockUI.stop();
                }

                $elem.on('blur', 'input[data-bind]', function (e) {
                    var obj = $scope.kOptions.dataSource.getByUid($(this).closest("tr").attr("data-uid"));
                    obj[$(this).attr("data-bind")] = $(this).val();
                });

                $elem.on('click', '.icon-checked i', function (e) {
                    var id = $(this).attr("id"), parentId = $(this).attr("pid"), nameParent = $(this).attr("nameParent");
                    checkNodeStatus(id, parentId, nameParent, $(this));
                });

                function checkNodeStatus(id, parentId, nameParent, e) {
                    var cls = "." + nameParent;
                    var len = $elem.find(cls).length;
                    var model = getModelById(id);
                    if (len == 0) {
                        if (e.hasClass('fa-square-o')) {
                            checkChild(id, true);
                            if (model)
                                model.checked = true;
                        }
                        else {
                            checkChild(id, false);
                            if (model)
                                model.checked = false;
                        }
                    }
                    else {
                        if (e.hasClass('fa-square-o')) {
                            checkParent(id, "checked");
                            checkChildsByParent(cls, "check");
                            if (model)
                                model.checked = true;
                            checkedEntityChild(id, true);
                        }
                        else if (e.hasClass('fa-check-square-o')) {
                            checkParent(id, "checkParent");
                            checkChildsByParent(cls, "pCheck");
                            if (model)
                                model.checked = true;
                            checkedEntityChild(id, false);
                        }
                        else if (e.hasClass('fa-NewMinus')) {
                            checkParent(id, "unchecked");
                            checkChildsByParent(cls, "unCheck");
                            if (model)
                                model.checked = false;
                            checkedEntityChild(id, false);
                        }
                    }
                    checkParentByChilds(e);
                    checkParentModel(parentId);
                }

                function checkedEntityChild(id, status) {
                    var result = getChildListByParentId(id);
                    result.forEach(function (item) {
                        item.checked = status;
                        checkedEntityChild(item.id, status)
                    });
                }

                function checkParentModel(parentId) {
                    var model = getModelById(parentId);
                    if (!model)
                        return;

                    if ($elem.find("#" + model.id).hasClass('fa-square-o')) {
                        model.checked = false;
                    }
                    else if ($elem.find("#" + model.id).hasClass('fa-check-square-o')) {
                        model.checked = true;
                    }
                    else if ($elem.find("#" + model.id).hasClass('fa-NewMinus')) {
                        model.checked = true;
                    }
                    if (model.parentId) {
                        checkParentModel(model.parentId);
                    }
                }

                function checkParentByChilds(e) {
                    //    var clss = e.attr("class").replace(/class/g, '').split(" ");
                    //    var numerics = [];
                    //    clss.forEach(function (item) {
                    //        if ($.isNumeric(item)) {
                    //            numerics.push(item);
                    //            numerics.sort().reverse();
                    //        }
                    //    });

                    //    //for check parent to checked this for 3 modes
                    //    numerics.forEach(function (item) {
                    //        var elmChild = $elem.find(".class" + item);
                    //        var childLength = elmChild.length;
                    //        var checkedChildLength = elmChild.filter(".fa-check-square-o").length + elmChild.filter(".fa-NewMinus").length;
                    //        if (checkedChildLength == childLength) {
                    //            checkParent(item, "checked");
                    //        }
                    //        else if (checkedChildLength > 0) {
                    //            checkParent(item, "checkParent");
                    //        }
                    //        else if (checkedChildLength == 0) {
                    //            checkParent(item, "unchecked");
                    //        }
                    //    });
                }

                //get row entity
                function getModelById(id) {
                    var models = $scope.kOptions.dataSource.data().filter(function (e) { return e.id == id; });
                    if (models && models.length)
                        return models[0];
                    return null;
                }

                //get parent's child
                function getChildListByParentId(id) {
                    return $.grep($scope.kOptions.dataSource.data(), function (e) { return e.parentId == id; });
                }

                function checkParent(id, status) {
                    switch (status) {
                        case "checked":
                            $elem.find("#" + id).removeClass('fa-square-o');
                            $elem.find("#" + id).removeClass('fa-NewMinus');
                            $elem.find("#" + id).addClass('fa-check-square-o');
                            break;
                        case "unchecked":
                            $elem.find("#" + id).removeClass('fa-NewMinus');
                            $elem.find("#" + id).removeClass('fa-check-square-o');
                            $elem.find("#" + id).addClass('fa-square-o');
                            break;
                        case "checkParent"://filled checkbox
                            $elem.find("#" + id).addClass('fa-NewMinus');
                            $elem.find("#" + id).removeClass('fa-square-o');
                            $elem.find("#" + id).removeClass('fa-check-square-o');
                            break;
                    }
                }

                //to check row checkbox
                function checkChild(id, status) {
                    if (status) {
                        $elem.find("#" + id).removeClass('fa-square-o');
                        $elem.find("#" + id).addClass('fa-check-square-o');
                    }
                    else {
                        $elem.find("#" + id).removeClass('fa-check-square-o');
                        $elem.find("#" + id).addClass('fa-square-o');
                    }
                }

                function checkChildsByParent(class_style, status) {
                    switch (status) {
                        case "check":
                            $elem.find(class_style).removeClass('fa-square-o');
                            $elem.find(class_style).removeClass('fa-NewMinus');
                            $elem.find(class_style).addClass('fa-check-square-o');
                            break;
                        case "unCheck":
                            $elem.find(class_style).removeClass('fa-NewMinus');
                            $elem.find(class_style).removeClass('fa-check-square-o');
                            $elem.find(class_style).addClass('fa-square-o');
                            break;
                        case "pCheck":
                            $elem.find(class_style).removeClass('fa-NewMinus');
                            $elem.find(class_style).addClass('fa-square-o');
                            $elem.find(class_style).removeClass('fa-check-square-o');
                            break;
                    }
                }
            }

        };
    }]);
});