define(['application'], function (app) {
    app.register.directive('checkTreeList', ['$compile', '$parse', 'blockUI', 'helperFactory', function ($compile, $parse, blockUI, helperFactory) {

        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        }

        function guidGenerate() {
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
        }

        function controller($scope, $element, $attrs) {
            $scope.htmlOptions = $scope.htmlOptions || { class: "", tabindex: "", style: {} };

            this.guidGenerate = function () {
                $scope.htmlOptions.id = $scope.id = guidGenerate();
            }
        }

        function link(iScope, iElem, iAttrs, controller) {
            var temp = {
                tree: null,
                treeLoaded: false,
                isDataBound: false,
                prefixClass: "class",
                attrName: "parent-class",
                oldCheckedModels: [],
                newCheckedModels: [],
                blockTreeClass: "block-check-tree",
                treeEnable: true,
                parentIsImportantCheckValue: "parent-checked",
                fontIcon: {
                    unchecked: 'fa-square-o',
                    parentChecked: 'fa-NewMinus',
                    checked: 'fa-check-square-o'
                },
                parentField: "Checked",
                childParentsMap: [],
            }; var blockTreeElem = $("<div></div>").addClass(temp.blockTreeClass);

            controller.guidGenerate();

            iScope.$watch('kOptions', function (newValue, oldValue) {
                if (temp.treeLoaded)
                    return;

                if (!newValue || angular.equals({}, iScope.kOptions)) {
                    if (!iAttrs.kNgDelay) {
                        helperFactory.writeConsole("checkTreeList: kOptions must be defined!", "error");
                    }
                    return;
                }

                if (!iScope.kOptions.dataSource || angular.equals({}, iScope.kOptions.dataSource)) {
                    helperFactory.writeConsole("checkTreeList: kOptions.dataSource is mandatory", "error");
                    return;
                }

                if (iScope.kOptions.checkboxes) {//activityScope ID
                    if (iScope.kOptions.columns[0]) {
                        if (!iScope.kOptions.columns[0].template) {
                            iScope.kOptions.columns[0].template = "<span class='icon-checked'><i id='#=ID #' parent-id='#=parentId#' class='fa fa-square-o'></i></span> #=" + iScope.kOptions.columns[0].field + "#";
                        }
                        else {
                            if (typeof (iScope.kOptions.columns[0].template) == "function") {
                                var template = iScope.kOptions.columns[0].template;
                                iScope.kOptions.columns[0].template = function (e) { return "<span class='icon-checked'><i id='" + e.ID + "' parent-id='" + e.parentId + "' class='fa fa-square-o'></i></span> " + template(e) };
                            }
                            if (typeof (iScope.kOptions.columns[0].template) == "string")
                                iScope.kOptions.columns[0].template = "<span class='icon-checked'><i id='#=ID #' parent-id='#=parentId#' class='fa fa-square-o'></i></span> #=" + iScope.kOptions.columns[0].template + "#";
                        }
                    }
                    else {
                        iScope.kOptions.columns.unshift({ template: "<span class='icon-checked'><i id='#=ID #' parent-id='#=parentId#' class='fa fa-square-o'></i></span>", filterable: false, width: 110 });
                    }
                }

                iScope.mainTreeOptions = {
                    dataSource: iScope.kOptions.dataSource,
                    autoBind: iScope.kOptions.autoBind || true,
                    filterable: iScope.kOptions.filterable || { extra: false },
                    groupable: iScope.kOptions.groupable || false,
                    resizable: iScope.kOptions.resizable || true,
                    reorderable: iScope.kOptions.reorderable || false,
                    columnMenu: iScope.kOptions.columnMenu || false,
                    scrollable: iScope.kOptions.scrollable || true,
                    selectable: iScope.kOptions.selectable || true,
                    sortable: iScope.kOptions.sortable || { mode: "single", allowUnsort: true },
                    editable: iScope.kOptions.editable || false,
                    dataBinding: function (e) { typeof (iScope.kOptions.dataBinding) === "function" && iScope.kOptions.dataBinding.call(this, e); },
                    dataBound: function (e) {
                        var sDate = Date.now();
                        temp.isDataBound = false;
                        e.sender.element.find('button').attr('type', 'button');
                        addRootNodesAttr();
                        loadCheckListNodes();
                        typeof (iScope.kOptions.dataBound) === "function" && iScope.kOptions.dataBound.call(this, e);
                        temp.isDataBound = true;
                        helperFactory.writeConsole(Date.now() - sDate, "warn");
                    },
                    filterMenuInit: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.filterMenuInit) === "function" && iScope.kOptions.filterMenuInit.call(this, e); }
                    },
                    drop: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.drop) === "function" && iScope.kOptions.drop.call(this, e); }
                    },
                    drag: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.drag) === "function" && iScope.kOptions.drag.call(this, e); }
                    },
                    collapse: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.collapse) === "function" && iScope.kOptions.collapse.call(this, e); }
                    },
                    change: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.change) === "function" && iScope.kOptions.change.call(this, e); }
                    },
                    cancel: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.cancel) === "function" && iScope.kOptions.cancel.call(this, e); }
                    },
                    expand: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.expand) === "function" && iScope.kOptions.expand.call(this, e); }
                    },
                    columnResize: function (e) {
                        if (!temp.treeEnable) { e.preventDefault(); }
                        else { typeof (iScope.kOptions.columnResize) === "function" && iScope.kOptions.columnResize.call(this, e); }
                    },
                    columns: iScope.kOptions.columns || []
                };
                if (iScope.kOptions.toolbar) iScope.mainTreeOptions.toolbar = iScope.kOptions.toolbar;

                iElem.find("#" + iScope.id).kendoTreeList(iScope.mainTreeOptions);
                temp.treeLoaded = true;
                setCheckTreeListTemplate();
            }, true);

            iScope.$watch('kOptions.checkboxes.checkList', function (newValue, oldValue) {
                if (newValue && Array.isArray(newValue)) {
                    if (temp.isDataBound) {
                        loadCheckListNodes(newValue);
                    }
                }
            });

            iScope.$watch('kOptions.enable', changeTreeState, true);

            iElem.on('click', '.icon-checked i', checkNodeStatusByClick);

            function setCheckTreeListTemplate() {
                temp.tree = iElem.find("#" + iScope.id).data("kendoTreeList");
                if (iAttrs.checkTreeList) {//setDataSource(), ...
                    iScope.checkTreeList = temp.tree;
                    if (typeof (iScope.kOptions.checkboxes) == "object") {
                        iScope.kOptions.checkboxes.getNewCheckedModels = getNewCheckedModels;
                        iScope.kOptions.checkboxes.getAllCheckedItems = getAllCheckedItems;
                        iScope.kOptions.checkboxes.getCheckedParents = getCheckedParents;
                        iScope.kOptions.checkboxes.getCheckedChilds = getCheckedChilds;
                        iScope.kOptions.checkboxes.uncheckedAll = function () {
                            temp.newCheckedModels = [];
                            temp.oldCheckedModels = [];
                            loadCheckListNodes(temp.oldCheckedModels);
                        };
                    }
                }
                if (!angular.equals(iScope.htmlOptions.style, {}))
                    iElem.find("#" + iScope.id).css(iScope.htmlOptions.style);
            }

            function addRootNodesAttr() {
                if (!iScope.kOptions.checkboxes) return;
                var rootNodes = iScope.kOptions.dataSource.rootNodes();
                rootNodes.forEach(function (rootModel, index, array) {
                    addChildNodeAttr(rootModel, setParentAttr(rootModel.id));
                });
            }

            function addChildNodeAttr(parentModel, parentAttrValue) {
                var childs = getChildNodesByParentModel(parentModel);
                childs.forEach(function (child, index, array) {
                    var childParents = { childId: child.id, parentIds: [] };
                    iElem.find('#' + child.id).addClass(parentAttrValue);
                    setParentAttr(child.id);
                    getDataSourceParentIds(child.parentId, childParents.parentIds);
                    childParents.parentIds.forEach(function (parentId, index, array) { iElem.find('#' + child.id).addClass(temp.prefixClass + parentId); });
                    if (!treeIsFiltered()) temp.childParentsMap.push(childParents);
                    addChildNodeAttr(child, parentAttrValue);
                });
            }

            function resetTreeCheckboxes() {
                if (!iScope.kOptions.checkboxes) return;
                var checkboxElms = iElem.find(".icon-checked i");
                checkboxElms.removeClass('fa-check-square-o').removeClass('fa-NewMinus').removeClass('fa-square-o');
                checkboxElms.addClass('fa-square-o');

                var treeData = iScope.kOptions.dataSource.view();
                treeData.forEach(function (model) { model.checked = false; });
            }

            function loadCheckListNodes(checkList, checkListField, parentField) {
                if (!iScope.kOptions.checkboxes) return;

                checkListField = checkListField || iScope.kOptions.checkboxes.checkListField || "ID";
                var filters = getTreeFilters();
                if (filters !== null && (typeof (filters) == "undefined" || typeof (filters) == "object")) {
                    checkList = checkList || iScope.kOptions.checkboxes.checkList || [];
                    if (iScope.kOptions.checkboxes.parentIsImportant == false || typeof (iScope.kOptions.checkboxes.parentIsImportant) == "object") {
                        parentField = parentField || iScope.kOptions.checkboxes.parentIsImportant.field || temp.parentField;
                        checkList = checkList.concat(translateTempCheckModels(checkListField, parentField, getNewCheckedModels()));
                    }
                    else {
                        checkList = checkList.concat(translateTempCheckModels(checkListField, undefined, getNewCheckedModels()));
                    }
                }
                else {
                    checkList = [];
                    if (iScope.kOptions.checkboxes.parentIsImportant == false || typeof (iScope.kOptions.checkboxes.parentIsImportant) == "object") {
                        parentField = parentField || iScope.kOptions.checkboxes.parentIsImportant.field || temp.parentField;
                        checkList = checkList.concat(translateTempCheckModels(checkListField, parentField, getAllCheckedItems()));
                    }
                    else {
                        checkList = checkList.concat(translateTempCheckModels(checkListField, undefined, getAllCheckedItems()));
                    }
                }

                resetTreeCheckboxes();
                if (!checkList || !Array.isArray(checkList) || !checkList.length) return;

                blockUI.start();
                checkList.forEach(function (checkboxModel, i, a) {
                    var id = checkboxModel[checkListField], model;
                    if (treeIsFiltered()) {
                        model = getCheckedItemById(id);
                        if (model) {
                            if (parentField)
                                model.checked = checkboxModel[parentField] == 1 ? true : temp.parentIsImportantCheckValue;
                            else
                                model.checked = true;
                            var childLength = getChildsLengthByParentIdFromMap(id);
                            if (childLength == 0) {
                                checkChildElm(model.ID, true);
                            }
                            else {
                                var modelElm = iElem.find("#" + model.ID);
                                if (modelElm && modelElm.length && modelElm.hasClass('fa-square-o'))
                                    checkParent(model.ID, "checkParent");
                            }
                            var parentId = model.parentId, parentIds = [];
                            while (parentId) {
                                parentIds.push(parentId);
                                var parentModel = getCheckedItemById(parentId);
                                if (parentModel)
                                    parentId = parentModel.parentId;
                            }
                            checkParentsByChilds(parentIds, false);
                        }
                    }
                    else {
                        model = getDataSourceModelById(id);
                        if (model) { //$elm = $("#" + iScope.id + " table"); to speed up
                            var modelElm = iElem.find("#" + model.ID);
                            var childLength = iElem.find("." + modelElm.attr(temp.attrName)).length;
                            if (parentField)
                                model.checked = checkboxModel[parentField] == 1 ? true : temp.parentIsImportantCheckValue;
                            else
                                model.checked = true;
                            setOldCheckedModels(model);
                            if (childLength == 0) {
                                checkChildElm(model.ID, true);
                            }
                            else {
                                if (modelElm.hasClass('fa-square-o'))
                                    checkParent(model.ID, "checkParent");
                            }

                            var childElm = $(modelElm);
                            if (!childElm || !childElm.length) return;
                            checkParentsByChilds(sortParentIds(childElm), false);
                        }
                    }
                });
                blockUI.stop();
            }

            function checkNodeStatusByClick(e) {
                if (!iScope.kOptions.checkboxes || iScope.kOptions.checkboxes.editable === false) return;

                var id = $(this).attr("id"), parentAttrValue = $(this).attr(temp.attrName), $elm = $(this);
                var model = getDataSourceModelById(id), checkedModel;
                if (!model) return;

                var parentClass = "." + parentAttrValue, childLength;
                if (treeIsFiltered()) {
                    childLength = getChildsLengthByParentIdFromMap(id);
                    checkedModel = getCheckedItemById(id);
                }
                else {
                    childLength = iElem.find(parentClass).length;
                }

                if (childLength == 0) {
                    if (!checkedModel) {
                        if (model.checked == false) {
                            model.checked = true;
                            checkChildElm(model.id, true);
                        }
                        else {
                            model.checked = false;
                            checkChildElm(model.id, false);
                        }
                    }
                    else {
                        if (checkedModel.checked == false) {
                            model.checked = checkedModel.checked = true;
                            checkChildElm(model.id, true);
                        }
                        else {
                            model.checked = checkedModel.checked = false;
                            checkChildElm(model.id, false);
                        }
                    }
                }
                else {
                    var checkStatus = undefined;
                    if ($elm.hasClass('fa-square-o')) {
                        model.checked = true;
                        checkStatus = "checked";
                        checkParent(model.id, checkStatus);
                        checkChildsByParent(parentClass, checkStatus, model);
                    }
                    else if ($elm.hasClass('fa-check-square-o')) {
                        model.checked = true;
                        checkStatus = "checkParent";
                        checkParent(model.id, checkStatus);
                        checkChildsByParent(parentClass, checkStatus, model);
                    }
                    else if ($elm.hasClass('fa-NewMinus')) {
                        model.checked = false;
                        checkStatus = "unchecked";
                        checkParent(model.id, checkStatus);
                        checkChildsByParent(parentClass, checkStatus, model);
                    }
                }

                setCheckedModels(model);
                if (!$elm || !$elm.length) return;
                checkParentsByChilds(sortParentIds($elm), true);
                if (typeof (iScope.kOptions.checkboxes.check) == "function") {
                    iScope.kOptions.checkboxes.check(model, $elm);
                }
            }

            function setParentAttr(id) {
                var attrValue = temp.prefixClass + id;
                iElem.find('#' + id).attr(temp.attrName, attrValue);
                return attrValue;
            }

            function setOldCheckedModels(model) {
                var idField = getIdField();
                var newCheckedItemExist = temp.newCheckedModels.filter(function (checkedItem, index, array) { return checkedItem[idField] == model[idField] });
                if (newCheckedItemExist && newCheckedItemExist.length) return;
                var oldCheckedItemExist = temp.oldCheckedModels.filter(function (checkedItem, index, array) { return checkedItem[idField] == model[idField] });
                if (model.checked) {
                    if (!oldCheckedItemExist || !oldCheckedItemExist.length)
                        temp.oldCheckedModels.push(model);
                }
            }

            function setCheckedModels(model) {
                var idField = getIdField();
                var newCheckedItemExist = temp.newCheckedModels.filter(function (checkedItem, index, array) { return checkedItem[idField] == model[idField] });
                var oldCheckedItemExist = temp.oldCheckedModels.filter(function (checkedItem, index, array) { return checkedItem[idField] == model[idField] });
                if (model.checked) {
                    var checkListItemExist = (iScope.kOptions.checkboxes.checkList || []).filter(function (checkedItem, index, array) { if (!iScope.kOptions.checkboxes.checkListField) return false; return checkedItem[iScope.kOptions.checkboxes.checkListField] == model.ID });
                    if (checkListItemExist && checkListItemExist.length && (!oldCheckedItemExist || !oldCheckedItemExist.length))
                        temp.oldCheckedModels.push(model);
                    else if ((!newCheckedItemExist || !newCheckedItemExist.length) && (!oldCheckedItemExist || !oldCheckedItemExist.length))
                        temp.newCheckedModels.push(model);
                }
                else if (!model.checked) {
                    if (newCheckedItemExist && newCheckedItemExist.length)
                        temp.newCheckedModels = temp.newCheckedModels.filter(function (checkedItem, index, array) { return checkedItem[idField] != model[idField] });
                    if (oldCheckedItemExist && oldCheckedItemExist.length)
                        temp.oldCheckedModels = temp.oldCheckedModels.filter(function (checkedItem, index, array) { return checkedItem[idField] != model[idField] });
                }
            }

            function translateTempCheckModels(checkListField, parentField, tempModels) {
                if (!iScope.kOptions.checkboxes || !checkListField) return [];
                if (!tempModels || !Array.isArray(tempModels) || !tempModels.length) return [];

                var id = getIdField(), parentIdField = getParentIdField();
                var newTempCheckedFormat = tempModels.map(function (newCheckedItem, i, a) {
                    var temp = Object.create(Object.prototype);
                    Object.defineProperty(temp, checkListField, { value: newCheckedItem[id], configurable: false, writable: false });
                    Object.defineProperty(temp, parentIdField, { value: newCheckedItem[parentIdField], configurable: false, writable: false });
                    Object.defineProperty(temp, "checked", { value: newCheckedItem.checked, configurable: false, writable: true });
                    if (parentField)
                        Object.defineProperty(temp, parentField, {
                            get: function () {
                                return this.checked == true ? 1 : 2;
                            }
                        });
                    return temp;
                });
                return newTempCheckedFormat;
            }

            function getCheckedParents(id) {
                if (!id || isNaN(+id)) return [];
                var model = getDataSourceModelById(id);
                if (!model) return [];

                var parents = [];
                getDataSourceParentModels(model.parentId, parents);
                return parents.filter(function (parentModel) { return parentModel.checked && parentModel.checked != false; });
            }

            function getCheckedChilds(id) {
                if (!id || isNaN(+id)) return [];
                var model = getDataSourceModelById(id);
                if (!model) return [];

                var childs = getChildNodesByParentModel(model);
                return childs.filter(function (childModel) { return childModel.checked && childModel.checked != false; });
            }

            function checkChildsModel(model, checked) {
                var childs = [];
                if (treeIsFiltered()) {
                    var idField = getIdField()
                    if (checked != false) {//Message
                        setUncheckedElmsWithCheckedModels(model[idField], checked);
                    }
                    else {
                        childs = getCheckedChildsByParentId(model[idField]);
                        childs.forEach(function (childModel) {
                            childModel.checked = checked;
                            setCheckedModels(childModel);
                            checkChildsModel(childModel, checked);
                        });
                    }
                }
                else {
                    childs = getChildNodesByParentModel(model);
                    childs.forEach(function (childModel) {
                        childModel.checked = checked;
                        setCheckedModels(childModel);
                        checkChildsModel(childModel, checked);
                    });
                }
            }

            function sortParentIds(childElm) {//faster in chrome from getDataSourceParentIds
                var parentId = +childElm.attr("parent-id"), parentIds = [];
                while (parentId) {
                    parentIds.push(parentId);
                    childElm = iElem.find("#" + parentId);
                    parentId = +childElm.attr("parent-id");
                }
                return parentIds;
            }

            function checkParentsByChilds(parentIds, checkModel) {
                if (!parentIds || !Array.isArray(parentIds) || !parentIds.length) return;
                parentIds.forEach(function (parentId, i, a) {
                    var childs, childLength, checkedChildLength = 0, parentCheckedChildLength = 0;
                    if (treeIsFiltered()) {
                        childs = getChildsByParentIdFromMap(parentId);
                        childLength = childs.length;
                        childs.forEach(function (childId, i, a) {
                            var model = getDataSourceModelById(childId);
                            var checkedModel = getCheckedItemById(childId);
                            if ((model && model.checked) || (checkedModel && checkedModel.checked)) {
                                var childsByParentId = getChildsByParentIdFromMap((model || checkedModel)[getIdField()]);
                                if (!childsByParentId || !childsByParentId.length) {
                                    checkedChildLength++;
                                }
                                else if (getAllIdsIsChecked(childsByParentId)) {
                                    checkedChildLength++;
                                }
                                else {
                                    parentCheckedChildLength++;
                                }
                            }
                        });
                    }
                    else {
                        childs = iElem.find(".class" + parentId);//$("#" + iScope.id + " table").find("[parent-id=" + parentId + "]"); to speed up
                        childLength = childs.length;
                        checkedChildLength = childs.filter(".fa-check-square-o").length;
                        parentCheckedChildLength = childs.filter(".fa-NewMinus").length;
                    }

                    if (checkedChildLength == childLength)
                        checkParent(parentId, "checked", checkModel);
                    else if (checkedChildLength > 0 || parentCheckedChildLength > 0)
                        checkParent(parentId, "checkParent", checkModel);
                    else if (checkedChildLength == 0)
                        checkParent(parentId, "unchecked", checkModel);
                });
            }

            function checkChildsByParent(parentClass, status, model) {
                $elm = iElem.find(parentClass);
                switch (status) {
                    case "checked":
                        $elm.removeClass('fa-square-o').removeClass('fa-NewMinus').addClass('fa-check-square-o');
                        checkChildsModel(model, true);
                        break;
                    case "unchecked":
                        $elm.removeClass('fa-NewMinus').removeClass('fa-check-square-o').addClass('fa-square-o');
                        checkChildsModel(model, false);
                        break;
                    case "checkParent":
                        $elm.removeClass('fa-NewMinus').removeClass('fa-check-square-o').addClass('fa-square-o');
                        checkChildsModel(model, false);
                        break;
                }
            }

            function checkParentModel(parentId, checkStatus) {
                if (!parentId || isNaN(+parentId)) return;

                var model = getDataSourceModelById(parentId);
                if (!model) return;
                $elm = iElem.find("#" + model.id);
                if (checkStatus == "unchecked") {
                    model.checked = false;
                }
                else if (checkStatus == "checked") {
                    model.checked = true;
                }
                else if (checkStatus == "checkParent") {
                    if (angular.isUndefined(iScope.kOptions.checkboxes.parentIsImportant) || iScope.kOptions.checkboxes.parentIsImportant == true)
                        model.checked = true;
                    else //if (iScope.kOptions.checkboxes.parentIsImportant == false || typeof (iScope.kOptions.checkboxes.parentIsImportant) == "object")
                        model.checked = temp.parentIsImportantCheckValue;
                }

                setCheckedModels(model);
            }

            function checkParent(id, status, checkModel) {
                $elm = iElem.find("#" + id); //$("#" + iScope.id + " table").find("#" + id); to speed up
                switch (status) {
                    case "checked":
                        $elm.removeClass('fa-square-o').removeClass('fa-NewMinus').addClass('fa-check-square-o');
                        break;
                    case "unchecked":
                        $elm.removeClass('fa-NewMinus').removeClass('fa-check-square-o').addClass('fa-square-o');
                        break;
                    case "checkParent":
                        $elm.removeClass('fa-square-o').removeClass('fa-check-square-o').addClass('fa-NewMinus');
                        break;
                }
                checkModel && checkParentModel(id, status);
            }

            function checkChildElm(id, status) {
                $elm = iElem.find("#" + id); //$("#" + iScope.id + " table").find("#" + id); to speed up
                if (status) {
                    $elm.removeClass('fa-square-o').addClass('fa-check-square-o');
                }
                else {
                    $elm.removeClass('fa-check-square-o').addClass('fa-square-o');
                }
            }

            function getChildNodesByParentModel(parentModel) {
                var childs = iScope.kOptions.dataSource.childNodes(parentModel);
                if (childs && childs.length)
                    return childs;
                return [];
            }

            function getChildsByParentIdFromMap(parentId) {
                var childs = [];
                if (!parentId || isNaN(+parentId)) return childs;
                temp.childParentsMap
                    .forEach(function (childParents, i, a) {
                        var isChild = childParents.parentIds.filter(function (childParentId, i, a) { return childParentId == parentId });
                        if (isChild && isChild.length) childs.push(childParents.childId);
                    });

                return childs;
            }

            function getChildsLengthByParentIdFromMap(parentId) {
                if (!parentId || isNaN(+parentId)) return 0;
                return (getChildsByParentIdFromMap(parentId) || []).length;
            }

            function getDataSourceParentIds(parentId, parentIds) {//faster from sortParentIds in old browsers!
                var model = getDataSourceModelById(parentId);
                if (model) {
                    parentIds.push(model.id);
                    if (model.parentId)
                        getDataSourceParentIds(model.parentId, parentIds);
                }
            }

            function getDataSourceParentModels(parentId, parents) {
                var model = getDataSourceModelById(parentId);
                if (model) {
                    parents.unshift(model);
                    if (model.parentId)
                        getDataSourceParentModels(model.parentId, parents);
                }
            }

            function getDataSourceModelById(id) {
                var models = iScope.kOptions.dataSource.data().filter(function (e) { return e.id == id; });
                if (models && models.length)
                    return models[0];
                return null;
            }

            function changeTreeState(newState, oldState) {
                if (newState === true) {
                    temp.treeEnable = true;
                    unblockTree();
                }
                else if (newState === false) {
                    temp.treeEnable = false;
                    blockTree();
                }
            }

            function unblockTree() {
                var treeElm = iElem.find("#" + iScope.id);
                if (treeElm.children("." + temp.blockTreeClass).length) {
                    treeElm.children("." + temp.blockTreeClass).remove();
                }
            }

            function blockTree() {
                var treeElem = iElem.find("#" + iScope.id);
                if (!treeElem.children("." + temp.blockTreeClass).length) {
                    treeElem.append(blockTreeElem);
                }
            }

            function getTreeFilters() {
                return temp.tree.dataSource.filter();//undefined, null, object
            }

            function treeIsFiltered() {
                var filter = getTreeFilters();
                if (!filter)
                    return false;
                return true;
            }

            function getNewCheckedModels() {
                return temp.newCheckedModels;
            }

            function getAllCheckedItems() {
                return temp.newCheckedModels.concat(temp.oldCheckedModels);
            }

            function getCheckedItemById(id) {
                if (!id || isNaN(+id)) return null;
                var checkedItem = getAllCheckedItems().filter(function (checkItem, i, a) { return checkItem[getIdField()] == id });
                if (checkedItem && checkedItem.length == 1)
                    return checkedItem[0];
                return null;
            }

            function getCheckedChildsByParentId(parentId) {
                if (!parentId || isNaN(+parentId)) return [];
                var checkedItems = getAllCheckedItems().filter(function (checkItem, i, a) { return checkItem[getParentIdField()] == parentId });
                if (checkedItems && checkedItems.length)
                    return checkedItems;
                return [];
            }

            function getItemIsChecked(id) {
                if (!id || isNaN(+id)) return false;
                var checkedItems = getAllCheckedItems().filter(function (checkItem, i, a) { return checkItem[getIdField()] == id });
                if (checkedItems && checkedItems.length) return true;
                return false;
            }

            function getAllIdsIsChecked(idList) {
                if (!idList || !Array.isArray(idList) || !idList.length) return;
                var checkItemCount = 0;
                idList.forEach(function (id, i, a) {
                    if (getItemIsChecked(id))
                        checkItemCount++;
                });
                return checkItemCount == idList.length;
            }

            function setUncheckedElmsWithCheckedModels(parentId, checked) {
                var parentIdField = getParentIdField(), parentField = iScope.kOptions.checkboxes.parentIsImportant.field || temp.parentField;
                var childIds = getChildsByParentIdFromMap(parentId);
                childIds.forEach(function (childId, index, array) {
                    var temp = Object.create(Object.prototype);
                    Object.defineProperty(temp, getIdField(), { value: childId, configurable: false, writable: false });
                    Object.defineProperty(temp, parentIdField, { value: parentId, configurable: false, writable: false });
                    Object.defineProperty(temp, "checked", { value: checked, configurable: false, writable: true });
                    if (parentField)
                        Object.defineProperty(temp, parentField, {
                            get: function () {
                                return this.checked == true ? 1 : 2;
                            }
                        });
                    setCheckedModels(temp);
                });
                //because getChildsByParentIdFromMap changed
                //childIds.forEach(function (childId, index, array) { setUncheckedElmsWithCheckedModels(childId); }); 
            }

            function getIdField() {
                return iScope.kOptions.dataSource.options.schema.modelBase.idField || "ID";
            }

            function getParentIdField() {
                return iScope.kOptions.dataSource.options.schema.modelBase.parentIdField || "parentId";
            }
        }

        return {
            restrict: 'A',
            scope: {
                kOptions: '=',
                kNgDelay: '=?',
                checkTreeList: '=?',
                htmlOptions: '=?'
            },
            template: "<div id=\"{{id}}\" class=\"checkbox-tree-list {{htmlOptions.class}}\" tabindex=\"{{htmlOptions.tabindex}}\"></div>",
            controller: controller,
            link: link
        };
    }]);
});

//TreeList
//    enable: true,
//checkboxes: {
//    editable: true,
//    checkListField: 'RoleId',//checkList or for new checked item default value "ID" //TODO
//    checkList: [],
//    parentIsImportant: true, false || { field: "Checked" }
//    check: function(checkedItem, elm){}
//}
//$scope.treeHtmlOptions = {
//    tabindex: 1,
//    class: "treeHtmlOptions",
//    style: {}
//};
//methods:
//    kOptions.checkboxes.getNewCheckedModels
//    kOptions.checkboxes.getAllCheckedItems
//    kOptions.checkboxes.getCheckedParents 
//    kOptions.checkboxes.getCheckedChilds
//    kOptions.checkboxes.uncheckedAll