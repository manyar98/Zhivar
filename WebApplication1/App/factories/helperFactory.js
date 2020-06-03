define(['angularAMD'], function (app) {
    "use strict";
    app.factory('helperFactory', ['$state', '$stateParams', '$log', function ($state, $stateParams, $log) {
        function queryString(name) {
            var url = window.location.href;
            name = name.replace(/[\[\]]/g, "\\$&");
            var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
                results = regex.exec(url);
            if (!results) return null;
            if (!results[2]) return '';
            return decodeURIComponent(results[2].replace(/\+/g, " "));
        }

        function enumToValueText(e) {
            var result = [];
            if (e && Array.isArray(e)) result = e.map(function (item) { return { value: item.Key, text: item.Value } });
            return result;
        }

        function convertArray(sourceArray, convertionDelegate) {
            var result = [];
            for (var i in sourceArray)
                result.push(convertionDelegate(sourceArray[i]));
            return result;
        }

        function writeConsole(msg, type) {
            if (isDevelopment) {
                type == "warn" && $log.warn(msg);
                type == "log" && $log.log(msg);
                type == "error" && $log.error(msg);
                type == "info" && $log.info(msg);
            }
        }

        function considerModalBackdrop(obj) {
            var $scope = obj.$scope,
                $uibModalInstance = obj.$uibModalInstance,
                data = obj.data,
                dataSource = obj.dataSource,
                modelField = obj.modelField;

            if (!modelField)
                writeConsole("considerModalBackdrop: modelField parameter is null or undefined", "warn");
            var closeData = undefined;
            if (typeof (data) == "function")
                closeData = data();
            else
                closeData = data;

            $scope.$on('modal.closing', function (event, reason, closed) {
                if (reason == "backdrop click" || reason == "escape key press") {
                    event.preventDefault();
                    $uibModalInstance.close(closeData);
                }
            });

            $uibModalInstance.result.then(function (e) {
                if ((angular.isDefined($scope.actionExecuted) && $scope.actionExecuted) || angular.isUndefined($scope.actionExecuted)) {
                    if ($state.current.data && $state.current.data.selectedId) {
                        if (modelField && dataSource) {
                            $state.current.data.selectedId = $scope[modelField].ID || $scope[modelField][dataSource.options.schema.modelBase.idField];
                            if ($state.current.data.selectedParentId)
                                $state.current.data.selectedParentId = $scope[modelField].ParentId || $scope[modelField][dataSource.options.schema.modelBase.parentIdField];
                        }
                        else if (modelField) {
                            $state.current.data.selectedId = $scope[modelField].ID;
                            if ($state.current.data.selectedParentId)
                                $state.current.data.selectedParentId = $scope[modelField].ParentId;
                        }
                    }

                    if (dataSource) dataSource.read();
                }
            });
        }

        function changeKendoTreeListItem(e) {
            var selectedRow = this.select();
            if (selectedRow && selectedRow.length) {
                var selectedData = this.dataItem(selectedRow);
                $state.current.data.selectedId = selectedData[this.dataSource.options.schema.modelBase.idField];
                $state.current.data.selectedParentId = selectedData[this.dataSource.options.schema.modelBase.parentIdField];
            }
            else {
                $state.current.data.selectedId = -1;
                $state.current.data.selectedParentId = -1;
            }
        }

        function selectKendoTreeListItem(paramObj) {
            if (typeof (paramObj) == "undefined" || paramObj == null)
                writeConsole("selectKendoTreeListItem: kendoTreeList item selection parameter, cannot be null/undefined", "error");

            if (typeof (paramObj.treeList) == "undefined" || paramObj.treeList == null || !paramObj.treeList)
                writeConsole("selectKendoTreeListItem: kendoTreeList item selection parameter, 'treeList' field cannot be null/undefined/empty", "error");

            if (!paramObj.stateData && (!$state.current.data || !$state.current.data.selectedId))
                writeConsole("selectKendoTreeListItem: $state.current.data.selectedId or stateData is mandatory", "error");

            var tempParam = {
                parentKeyField: paramObj.parentKeyField || paramObj.treeList.dataSource.options.schema.modelBase.parentIdField,
                keyField: paramObj.keyField || paramObj.treeList.dataSource.options.schema.modelBase.idField,
                treeList: paramObj.treeList,
                stateData: paramObj.stateData || $state.current.data
            };

            var selectedId = tempParam.stateData.selectedId, selectedParentId = tempParam.stateData.selectedParentId || -1,
                dataItems = [], treeCtrl = tempParam.treeList;
            var allData = treeCtrl.dataSource.view();
            while (selectedId > 0) {
                var targetDataItem = allData.filter(function (e) { return e[tempParam.keyField] == selectedId; });
                if (selectedId != tempParam.stateData.selectedId)
                    selectedParentId = -1;
                if (targetDataItem.length == 0) {
                    selectedId = selectedParentId;
                    continue;
                }
                dataItems.push(targetDataItem[0]);
                selectedId = targetDataItem[0][tempParam.parentKeyField];
            }
            for (var i = dataItems.length - 1; i >= 0; i--) {
                var dataUid = dataItems[i].uid;
                var rowElement = treeCtrl.table.find('tr[data-uid=' + dataUid + ']');
                if (rowElement.length == 0)
                    writeConsole('target row not found !', "error");
                treeCtrl.expand(rowElement);
            }

            if (dataItems.length > 0) {
                var lastRow = treeCtrl.table.find('tr[data-uid=' + dataItems[0].uid + ']');
                treeCtrl.select(lastRow);
            }
        }

        function changeKendoTreeListHasChildrenItem(e) {
            var selectedRow = this.select();
            $state.current.data.selectedParentIds = [];
            if (selectedRow && selectedRow.length) {
                var allData = this.dataSource.view(), selectedData = this.dataItem(selectedRow);
                var parentIdField = this.dataSource.options.schema.modelBase.parentIdField;
                $state.current.data.selectedId = selectedData[this.dataSource.options.schema.modelBase.idField];
                $state.current.data.selectedParentId = selectedData[parentIdField];
                var parentIdValue = selectedData[parentIdField] || -1;
                while (parentIdValue > 0) {
                    $state.current.data.selectedParentIds.unshift(parentIdValue);
                    selectedData = allData.filter(function (v, i, a) { return v.ID == parentIdValue });
                    parentIdValue = selectedData[0][parentIdField] || -1;
                }
            }
            else {
                $state.current.data.selectedId = -1;
                $state.current.data.selectedParentId = -1;
                $state.current.data.selectedParentIds = [];
            }
        }

        function selectKendoTreeListHasChildrenItem(paramObj) {
            if (typeof (paramObj) == "undefined" || paramObj == null)
                writeConsole("selectKendoTreeItem: kendoTreeList item selection parameter, cannot be null/undefined", "error");

            if (typeof (paramObj.treeList) == "undefined" || paramObj.treeList == null || !paramObj.treeList)
                writeConsole("selectKendoTreeItem: kendoTreeList item selection parameter, 'treeList' field cannot be null/undefined/empty", "error");

            if (!paramObj.stateData && (!$state.current.data || !$state.current.data.selectedId))
                writeConsole("selectKendoTreeItem: $state.current.data.selectedId or stateData is mandatory", "error");

            var tempParam = {
                parentKeyField: paramObj.parentKeyField || paramObj.treeList.dataSource.options.schema.modelBase.parentIdField,
                keyField: paramObj.keyField || paramObj.treeList.dataSource.options.schema.modelBase.idField,
                treeList: paramObj.treeList,
                stateData: paramObj.stateData || $state.current.data
            };

            var selectedId = tempParam.stateData.selectedId, selectedParentId = tempParam.stateData.selectedParentId || -1,
                selectedParentIds = tempParam.stateData.selectedParentIds || [], treeListCtrl = tempParam.treeList;

            selectedParentIds.forEach(function (id) {
                var data = treeListCtrl.dataSource.view().filter(function (v) { return v.ID == id });
                if (data && data.length) {
                    var rowElement = treeListCtrl.table.find('tr[data-uid=' + data[0].uid + ']');
                    if (!rowElement.length)
                        writeConsole('target row not found !', "error");
                    treeListCtrl.expand(rowElement);
                }
            });

            var data = treeListCtrl.dataSource.view().filter(function (v) { return v.ID == selectedId });
            if (data && data.length) {
                var rowElement = treeListCtrl.table.find('tr[data-uid=' + data[0].uid + ']');
                if (!rowElement.length)
                    writeConsole('selected row not found !', "error");
                treeListCtrl.select(rowElement);
            }
        }

        function toggleClass(elem, cls1, cls2) {
            elem = $(elem);
            if (elem.hasClass(cls1)) {
                elem.removeClass(cls1);
                elem.addClass(cls2);
                return cls2;
            }
            else if (elem.hasClass(cls2)) {
                elem.removeClass(cls2);
                elem.addClass(cls1);
                return cls1;
            }
            else {
                elem.addClass(cls1);
                return cls1;
            }
        }

        return {
            queryString: queryString,
            enumToValueText: enumToValueText,
            convertArray: convertArray,
            writeConsole: writeConsole,
            considerModalBackdrop: considerModalBackdrop,
            changeKendoTreeListItem: changeKendoTreeListItem,
            selectKendoTreeListItem: selectKendoTreeListItem,
            changeKendoTreeListHasChildrenItem: changeKendoTreeListHasChildrenItem,
            selectKendoTreeListHasChildrenItem: selectKendoTreeListHasChildrenItem,
            toggleClass: toggleClass
        }
    }]);
});