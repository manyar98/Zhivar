define(['application', 'dataService'], function (app) {
    app.register.directive('vahedSelect', ['dataService', function (service) {
    return {
        restrict: 'E',
        transclude: true,
        templateUrl: "/App/template/vahed-select.html",
        scope: {
            filter: "=",
            text: "=?",
            eid: "=",
            bind: "=",
            onSelect: "=",
            callbackobj: "=?",
            start: '='
        },
        link: function (scope, element, attrs) {
            scope.eid = attrs.eid;
            //            scope.$apply();
            scope.$watch('start', function (value) {
                if (value)
                    scope.init();
                scope.start = false;
            }, true);

            scope.init = function () {
                scope.nodeMenu = true;
                scope.inputNodeName = "";
                $(function () { $('[data-toggle="popover"]').popover(); });
                //callws(DefaultUrl.MainWebService + 'GetNodes', { filter: attrs.filter ? attrs.filter : 'none' })
                // .success(function (data) {
                service.getAll('/app/api/VahedTol/GetAllByOrganId').then(function (data) {
                 
                    scope.nodes = data;
                    //scope.$apply();
                    scope.createTree();
                });
              
                //}).fail(function (error) {
                //     window.location = '/error.html';
                // }).loginFail(function () {
                //    window.location = DefaultUrl.login;
                // });
            };
            scope.createTree = function () {
                scope.tree = createTree({
                    containerId: 'tree' + attrs.eid,
                    onClick: function (node) {
                        scope.nodeSelect(node.tag);
                    },
                    onDblClick: function (node) {
                        scope.nodeSelect(node.tag, true);
                    }
                });
                scope.tree.init();
                var nodes = scope.nodes;

                var treeNodes = [];
                scope.treeNodes = treeNodes;
                var nodeToSelect = null; //  by hamid
                while (treeNodes.length < nodes.length) {
                    for (var i = 0; i < nodes.length; i++) {
                        if (scope.isInTree(nodes[i]) == null) {
                            if (nodes[i].Parent == null) {
                                var image = '';
                                switch (nodes[i].SystemAccount) {
                                    case 1:
                                        image = 'person';
                                        break;
                                    case 2:
                                        image = 'good';
                                        break;
                                    case 3:
                                        image = 'cashbox2';
                                        break;
                                    case 4:
                                        image = 'bank';
                                        break;
                                    default:
                                        image = 'folder';
                                        break;
                                }

                                var node = scope.tree.addNode(null, nodes[i], nodes[i].Title, "", image);
                                treeNodes.push(node);
                                if (scope.bind != null && nodes[i].Id === scope.bind.Id) nodeToSelect = node;   // by hamid
                            } else {
                                var parentNode = scope.isInTree(nodes[i].Parent);
                                if (parentNode != null)
                                    var node = scope.tree.addNode(parentNode, nodes[i], nodes[i].Name, "", "folder");
                                treeNodes.push(node);
                                if (scope.bind != null && nodes[i].Id === scope.bind.Id) nodeToSelect = node;   // by hamid
                            }
                        }
                    }
                }

                if (nodeToSelect != null) scope.tree.select(nodeToSelect);
            };
            scope.nodeSelect = function (node, ok) {
                if (!node) {
                    scope.bind = null;
                    scope.text = "";
                    $("#nodeSelect" + attrs.eid).slideUp();
                    scope.isOpen = false;
                    scope.$apply();
                    return false;
                }
                scope.bind = node;
                //                scope.text = node.FamilyTree;
                scope.text = scope.getNodeFamilyTreeString(node);
                node.FamilyTree = scope.text;
                scope.$apply();
                if (scope.onSelect) scope.onSelect(node, scope.callbackobj);

                if (ok) {
                    $("#nodeSelect" + attrs.eid).slideUp();
                    scope.isOpen = false;
                }

                scope.$apply();
                return 1;
            };
            scope.showTree = function () {
                var offsetWidth = document.getElementById("nodeInput" + scope.eid).parentElement.offsetWidth;
                document.getElementById("nodeSelect" + scope.eid).style.width = offsetWidth + "px";

                if ($("#nodeSelect" + attrs.eid).is(":visible")) {
                    $("#nodeSelect" + attrs.eid).slideUp();
                    scope.isOpen = false;
                    return;
                }
                $("#nodeSelect" + attrs.eid).slideDown();
                scope.isOpen = true;

                $(document).mouseup(function (e) {
                    var container = $("#nodeSelect" + attrs.eid);
                    if (!container.is(e.target) && container.has(e.target).length === 0) {
                        container.slideUp();
                        scope.isOpen = false;
                    }
                });
            };
            scope.isInTree = function (node) {
                var tree = scope.treeNodes;
                for (var j = 0; j < tree.length; j++) {
                    if (tree[j].tag.Id == node.Id)
                        return tree[j];
                }
                return null;
            };
            scope.addNode = function () {
                //if (scope.tree.selected == null) {
                //    alertbox({ content: "ابتدا یک گروه را انتخاب کنید." });
                //    return;
                //}

                scope.node = { Id: 0, Title: "", Code: "" };
                scope.nodeMenu = false;
                scope.nodeEdit = true;
                scope.inputNodeName = "";
                applyScope(scope);
            };
            scope.editNode = function () {
                if (scope.tree.selected == null) {
                    alertbox({ content: "ابتدا یک گروه را انتخاب کنید." });
                    return;
                }

                //if (scope.tree.selected.tag.Parent == null && scope.tree.selected.tag.SystemAccount > 0) {
                //    alertbox({ content: "امکان ویرایش گروه سیستمی وجود ندارد." });
                //    return;
                //}
                scope.nodeMenu = false;
                scope.nodeEdit = true;
                scope.node = scope.tree.selected.tag;
                scope.inputNodeName = scope.node.Title;
                applyScope(scope);
            };
            scope.submitNode = function () {
                if (scope.callingSaveNode) return;
                if (scope.inputNodeName === "") {
                    alertbox({ content: "نام گروه را وارد کنید." });
                    return;
                }
                scope.node.Title = scope.inputNodeName;
                //if (scope.tree.selected.tag && scope.node.Id === 0) scope.node.Parent = scope.tree.selected.tag;

                scope.callingSaveNode = true;

                //callws(DefaultUrl.MainWebService + 'SaveNode', { node: scope.node })
                //    .success(function (node) {
                service.addEntity("/app/api/VahedTol/Add", scope.node).then(function (node) {
                    scope.callingSaveNode = false;
                    var tree = scope.tree;
                    var parent = tree.selected;
                    var find = findAndReplace(scope.nodes, node.ID, node);
                    if (find) tree.editNode(tree.selected, node, node.Title, "", "good");
                    else {
                        tree.addNode(parent, node, node.Title, "", "good");
                        scope.nodes.push(node);
                    }
                    scope.nodeMenu = true;
                    scope.nodeEdit = false;
                    scope.nodeRemove = false;
                    scope.$apply();

                });
                      
                    //}).fail(function (error) {
                    //    scope.callingSaveNode = false;
                    //    alertbox({ content: error });
                    //}).loginFail(function () {
                    //    window.location = DefaultUrl.login;
                    //});
            };
            scope.cancelEditNode = function () {
                scope.nodeMenu = true;
                scope.nodeEdit = false;
                scope.inputNodeName = "";
                scope.$apply();
            };
            scope.deleteNodeQuestion = function () {
                if (scope.tree.selected == null) {
                    alertbox({ content: "ابتدا یک گروه را انتخاب کنید." });
                    return;
                }
                //if (scope.tree.selected.tag.Parent == null && scope.tree.selected.tag.SystemAccount > 0) {
                //    alertbox({ content: "امکان حذف گروه سیستمی وجود ندارد." });
                //    return;
                //}
                scope.nodeMenu = false;
                scope.nodeRemove = true;
                scope.$apply();
            };
            scope.deleteNode = function () {
                if (scope.callingSaveNode) return;
                scope.callingSaveNode = true;
               // node: scope.tree.selected.tag;

                $.ajax({
                    type: "POST",
                    data: JSON.stringify(scope.tree.selected.tag.Id),
                    url: "/app/api/VahedTol/Delete",
                    contentType: "application/json"
                }).done(function (res) {

                    scope.callingSaveNode = false;
                    var tree = scope.tree;
                    findAndRemove(scope.nodes, tree.selected.tag);
                    tree.deleteNode(tree.selected);
                    tree.selected = null;
                    scope.bind = null;
                    scope.nodeMenu = true;
                    scope.nodeEdit = false;
                    scope.nodeRemove = false;
                    scope.$apply();
                }).fail(function (error) {
                       scope.callingSaveNode = false;
                        alertbox({ content: error });
                });

                //callws(DefaultUrl.MainWebService + 'DeleteNode', { node: scope.tree.selected.tag })
                //    .success(function (result) {
                       
                //    }).fail(function (error) {
                     
                //    }).loginFail(function () {
                //        window.location = DefaultUrl.login;
                //    });
            };
            scope.cancelDeleteNode = function () {
                scope.nodeMenu = true;
                scope.nodeRemove = false;
                scope.$apply();
            };
            scope.getNodeFamilyTreeString = function (node) {
                var family = new Array();
                var parent = node.Parent;
                while (parent != null) {
                    family.push(parent.Name);
                    parent = parent.Parent;
                }
                var familyTree = "";
                while (family.length > 0)
                    familyTree += family.pop() + " / ";
                return familyTree + node.Title;
            };
            scope.init();
        }
    };
    }]);
});