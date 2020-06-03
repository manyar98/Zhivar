// انتخاب کالا با کمک دسته بندی درختی
define(['application', 'service'], function (app) {
    app.register.directive('itemSelectTree', [function () {
        return {
            restrict: 'E',
            templateUrl: 'item-select-tree.html?ver=1.2.9.1',
            scope: {
                data: '=',
                nodes: '=',
                bind: "=",
                onSelect: "=",
                open: '='
            },
            link: function (scope, element, attrs) {
                var tree;
                scope.init = function () {
                    scope.items = [];
                    $(function () {
                        $('[data-toggle="popover"]').popover();
                    });
                };
                scope.initSelectItem = function (elm) {
                    scope.activeItemIndex = 0;
                    scope.items = [];
                    if (!scope.data) scope.getItems();
                    else {
                        angular.copy(scope.data, scope.items);
                        scope.filteredItems = scope.data;
                    }
                    if (!scope.nodes) scope.getNodes();
                    else scope.createTree(scope.nodes);
                    $('#selectItemTreeModal').modal({ keyboard: false }, 'show');
                    scope.$apply();
                };
                scope.$watch('open', function (value) {
                    if (value)
                        scope.initSelectItem();
                    scope.open = false;
                }, true);
                scope.getItems = function () {
                    callws('GetInventoryItemsForSelectList', { type: 'all' })
                        .success(function (items) {
                            scope.data = items;
                            scope.items = [];
                            angular.copy(scope.data, scope.items);
                            scope.filteredItems = items;
                            scope.$apply();
                        }).fail(function (error) {
                            window.location = '/error.html';
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                        });
                };
                scope.getNodes = function () {
                    callws('GetNodes', { filter: 'item' })
                        .success(function (nodes) {
                            scope.nodes = nodes;
                            scope.$apply();
                            scope.createTree(nodes);
                        }).fail(function (error) {
                            window.location = '/error.html';
                        }).loginFail(function () {
                            //window.location = DefaultUrl.login;
                        });
                };
                scope.itemSelect = function (item) {
                    if (!item) {
                        scope.bind = null;
                        $('#selectItemTreeModal').modal('hide');
                        scope.$apply();
                    }
                    scope.bind = item;
                    scope.onSelect(item);
                    $('#selectItemTreeModal').modal('hide');
                    scope.$apply();
                };
                scope.createTree = function (nodes) {
                    tree = createTree({
                        containerId: 'treeItemSelect',
                        onClick: function (node) {
                            scope.selectedNode = node.tag;
                            scope.nodeSelect(node.tag);
                            scope.$apply();
                        },
                        onDblClick: function (node) {
                        }
                    });
                    tree.init();

                    var treeNodes = [];
                    scope.treeNodes = treeNodes;
                    while (treeNodes.length < nodes.length) {
                        for (var i = 0; i < nodes.length; i++) {
                            if (scope.isInTree(nodes[i]) == null) {
                                if (nodes[i].Parent == null) {
                                    var image = 'good';
                                    var node = tree.addNode(null, nodes[i], nodes[i].Name, "", image);
                                    treeNodes.push(node);
                                } else {
                                    var parentNode = scope.isInTree(nodes[i].Parent);
                                    if (parentNode != null)
                                        var node = tree.addNode(parentNode, nodes[i], nodes[i].Name, "", "good");
                                    treeNodes.push(node);
                                }
                            }
                        }
                    }


                };
                scope.isInTree = function (node) {
                    var tree = scope.treeNodes;
                    var treelength = tree.length;
                    for (var j = 0; j < treelength; j++) {
                        if (tree[j].tag.Id === node.Id)
                            return tree[j];
                    }
                    return null;
                };
                scope.nodeSelect = function (node) {
                    scope.selectedNode = node;
                    var filteredItems = [];
                    var wholeItems = scope.data;
                    var length = wholeItems.length;
                    for (var i = 0; i < length; i++) {
                        if (wholeItems[i].DetailAccount.Node.Id === node.Id)
                            filteredItems.push(wholeItems[i]);
                    }
                    scope.items = filteredItems;
                    scope.filteredItems = filteredItems;
                    scope.$apply();
                };
                scope.search = function () {
                    var key = scope.searchKey;
                    if (!key || key == "" || key == " ") {
                        scope.filteredItems = scope.data;
                        if (scope.selectedNode) scope.nodeSelect(scope.selectedNode);
                        else angular.copy(scope.data, scope.items);
                        scope.$apply();
                        return;
                    }
                    var filteredItems = [];
                    var items = scope.items;
                    if (!isNaN(parseInt(key, 10))) {
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].DetailAccount.Code.indexOf(key.toString()) > -1)
                                filteredItems.push(items[i]);
                        }
                        scope.filteredItems = filteredItems;
                        scope.$apply();
                    } else {
                        for (var i = 0; i < items.length; i++) {
                            if (items[i].Name.toLowerCase().search(key.toString().toLowerCase()) !== -1)
                                filteredItems.push(items[i]);
                        }
                        scope.filteredItems = filteredItems;
                        scope.$apply();
                    }
                };
                scope.init();
            }
        };
    }]);
});