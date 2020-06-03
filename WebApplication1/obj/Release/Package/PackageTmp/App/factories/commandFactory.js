define(['angularAMD'], function (app) {
    "use strict";
    app.factory('commandFactory', ['helperFactory', function (helperFactory) {
        function validateCommandContext(commandContext) {
            if (!commandContext)
                helperFactory.writeConsole("The createColumn function parameter is undefined.", "error");
            if (!commandContext.controlSelector)
                helperFactory.writeConsole("The createColumn controlSelector must be defined.", "error");
            if (!Array.isArray(commandContext.commands))
                helperFactory.writeConsole("The createColumn commands must be array.", "error");
            if (commandContext.type !== "kendo" && commandContext.type !== "dropdown" && commandContext.type !== "icon")
                helperFactory.writeConsole("The type is incorrect, defined type: kendo, dropdown, icon.", "error");
            var map = {};
            commandContext.commands.forEach(function (cmd) {
                if (typeof (cmd) != "object")
                    helperFactory.writeConsole("The type of  all command in commands must be object.", "error");
                if (!cmd.name || typeof (cmd.name) != "string")
                    helperFactory.writeConsole("The command name in commands must be string and cannot be falsy values.", "error");
                if (!map[cmd.name])
                    map[cmd.name] = true;
                else
                    helperFactory.writeConsole("The command name in commands must be unique. command name: " + cmd.name, "error");
            });
        }

        function validateToolbarContext(toolbarContext) {
            if (!toolbarContext)
                helperFactory.writeConsole("The createToolbar function parameter is undefined.", "error");
            if (!Array.isArray(toolbarContext.toolbars))
                helperFactory.writeConsole("The createToolbar toolbars must be array.", "error");
            if (!toolbarContext.controlSelector)
                helperFactory.writeConsole("The createToolbar controlSelector must be defined.", "error");
            if (toolbarContext.type !== "kendoTreeList" && toolbarContext.type !== "kendoGrid")
                helperFactory.writeConsole("The type is incorrect, defined type: kendo, dropdown, icon.", "error");
            var map = {};
            toolbarContext.toolbars.forEach(function (tool) {
                if (typeof (tool) != "object")
                    helperFactory.writeConsole("The type of  all toolbar in toolbars must be object.", "error");
                if (!tool.name || typeof (tool.name) != "string")
                    helperFactory.writeConsole("The toolbar name in toolbars must be string and cannot be falsy values.", "error");
                if (!map[tool.name])
                    map[tool.name] = true;
                else
                    helperFactory.writeConsole("The toolbar name in toolbars must be unique. toolbar name: " + tool.name, "error");
            });
        }

        function createKendoCommand(commands, controlSelector) {
            var kendoCommands = [];
            commands.forEach(function (cmd) {
                var kendoCommand = {};
                if (cmd.text)
                    kendoCommand.text = cmd.text;
                if (cmd.name)
                    kendoCommand.name = cmd.name;
                if (cmd.click)
                    kendoCommand.click = cmd.click;
                if (cmd.className)
                    kendoCommand.className = cmd.className;
                if (cmd.iconClass)
                    kendoCommand.iconClass = cmd.iconClass;
                if (cmd.imageClass)
                    kendoCommand.imageClass = cmd.imageClass;
                if (cmd.template)
                    kendoCommand.template = cmd.template;
                if (cmd.visible)
                    kendoCommand.visible = cmd.visible;
                kendoCommands.push(kendoCommand);
            });
            return kendoCommands;
        }

        function createSplitDropdownCommand(commands, btnClassName, controlSelector) {
            var mainCommand = commands.filter(function (v) { return v.isMain; })[0],
                otherCommands = commands.filter(function (v) { return !v.isMain; }),
                btnGroupTemplate = "", actionButton = "", dropdownToggleButton = "", ulTemplate = "";
            if (mainCommand) {
                actionButton += "<button type=\"button\" data-name=\"" + mainCommand.name + "\" class=\"" + (btnClassName || "") + " " + (mainCommand.mainClassName || "") + " btn-with-toggle\">";
                if (mainCommand.template)
                    actionButton += mainCommand.template;
                else
                    actionButton += mainCommand.text;
                actionButton += "</button>";
                var eventContent = otherCommands && otherCommands.length ? ".k-grid-content table td .btn-group > button[data-name=" + mainCommand.name + "]" : ".k-grid-content table td button[data-name=" + mainCommand.name + "]";
                var $ctrlElm = $(controlSelector);
                $ctrlElm.on("click", eventContent, function (e) {
                    mainCommand.click.call($ctrlElm.data("kendoGrid"), e);
                });
            }
            if (otherCommands && otherCommands.length) {
                dropdownToggleButton = "<button type=\"button\" class=\"" + (btnClassName || "") + " dropdown-toggle\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\"><span class=\"caret\"></span><span class=\"sr-only\">سایر</span></button>";
                otherCommands.forEach(function (cmd) {
                    if (cmd.hasSeparator)
                        ulTemplate += "<li role=\"separator\" class=\"divider\"></li>";
                    if (cmd.template)
                        ulTemplate += "<li class=\"" + (cmd.listClassName || "") + "\"><a type=\"button\" data-name=\"" + cmd.name + "\" class=\"" + (cmd.anchorClassName || "") + "\">" + cmd.template + "</a></li>";
                    else
                        ulTemplate += "<li class=\"" + (cmd.listClassName || "") + "\"><a type=\"button\" data-name=\"" + cmd.name + "\" class=\"" + (cmd.anchorClassName || "") + "\">" + cmd.text + "</a></li>";
                    var $ctrlElm = $(controlSelector);
                    $ctrlElm.on("click", ".k-grid-content table td .btn-group > ul li a[data-name=" + cmd.name + "]", function (e) {
                        cmd.click.call($ctrlElm.data("kendoGrid"), e);
                    });
                });
            }
            if (ulTemplate) {
                ulTemplate = "<ul class=\"dropdown-menu\">" + ulTemplate + "</ul>";
                btnGroupTemplate = "<div class=\"btn-group\">" + actionButton + dropdownToggleButton + ulTemplate + "</div>";
            }
            else
                btnGroupTemplate = actionButton;
            return [{ template: btnGroupTemplate, name: ulTemplate ? "btnGroup" : mainCommand.name }];
        }

        function createDropdownCommand(commands, btnClassName, controlSelector) {
            var hasMainCommand = commands.filter(function (v) { return v.isMain; });
            var mainCommand = hasMainCommand && hasMainCommand.length ? hasMainCommand[0] : { name: "mainCommand", text: "عملیات", mainClassName: "" },
                otherCommands = commands.filter(function (v) { return !v.isMain; }), btnGroupTemplate = "", actionButton = "", ulTemplate = "";
            actionButton += "<button data-name=\"" + mainCommand.name + "\" class=\"" + btnClassName + " " + mainCommand.mainClassName + " dropdown-toggle\" type=\"button\" id=\"dropdownMenu1\" data-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"true\">";
            if (mainCommand.template)
                actionButton += mainCommand.template;
            else
                actionButton += mainCommand.text;
            actionButton += "<span class=\"caret\"></span></button>";

            if (otherCommands && otherCommands.length) {
                otherCommands.forEach(function (cmd) {
                    if (cmd.hasSeparator)
                        ulTemplate += "<li role=\"separator\" class=\"divider\"></li>";
                    if (cmd.template)
                        ulTemplate += "<li class=\"" + (cmd.listClassName || "") + "\"><a type=\"button\" data-name=\"" + cmd.name + "\" class=\"" + (cmd.anchorClassName || "") + "\">" + cmd.template + "</a></li>";
                    else
                        ulTemplate += "<li class=\"" + (cmd.listClassName || "") + "\"><a type=\"button\" data-name=\"" + cmd.name + "\" class=\"" + (cmd.anchorClassName || "") + "\">" + cmd.text + "</a></li>";
                    var $ctrlElm = $(controlSelector);
                    $ctrlElm.on("click", ".k-grid-content table td .dropdown > ul li a[data-name=" + cmd.name + "]", function (e) {
                        cmd.click.call($ctrlElm.data("kendoGrid"), e);
                    });
                });
            }
            if (ulTemplate) {
                ulTemplate = "<ul class=\"dropdown-menu\" aria-labelledby=\"dropdownMenu1\">" + ulTemplate + "</ul>";
                btnGroupTemplate = "<div class=\"dropdown\">" + actionButton + ulTemplate + "</div>";
            }
            else
                btnGroupTemplate = actionButton;
            return [{ template: btnGroupTemplate, name: ulTemplate ? "dropdown" : mainCommand.name }];
        }

        function createIconCommand(commands, controlSelector) {
            var kendoCommands = [];
            commands.forEach(function (cmd) {
                var kendoCommand = {},
                    $templateWrapper = $("<div></div>"),
                    $a = $('<a/>', { type: "button", "data-name": cmd.name, "class": cmd.className, title: cmd.text }),
                    $span = $('<span></span>', { "class": cmd.iconClass });
                $templateWrapper.html($a.html($span));
                if (cmd.visible)
                    kendoCommand.visible = cmd.visible;
                kendoCommand.template = $templateWrapper.html();
                var $ctrlElm = $(controlSelector);
                $ctrlElm.on("click", ".k-grid-content table td a[data-name=" + cmd.name + "]", function (e) {
                    cmd.click.call($ctrlElm.data("kendoGrid"), e);
                });
                kendoCommands.push(kendoCommand);
            });
            return kendoCommands;
        }

        function createColumn(commandContext) {
            validateCommandContext(commandContext);
            if (!commandContext.commands || !commandContext.commands.length)
                return [];
            if (commandContext.type == "kendo") {
                return createKendoCommand(commandContext.commands, commandContext.controlSelector);
            }
            else if (commandContext.type == "dropdown") {
                commandContext.btnClassName = commandContext.btnClassName || "btn btn-primary";
                var isSplitDD = commandContext.commands.filter(function (v) { return v.isMain; });
                if (isSplitDD && isSplitDD.length && isSplitDD[0].click)
                    return createSplitDropdownCommand(commandContext.commands, commandContext.btnClassName, commandContext.controlSelector);
                return createDropdownCommand(commandContext.commands, commandContext.btnClassName, commandContext.controlSelector);
            }
            else if (commandContext.type == "icon") {
                return createIconCommand(commandContext.commands, commandContext.controlSelector);
            }
        }

        function createKendoGridToolbar(toolbars, controlSelector) {
            var kendoGridToolbars = [];
            toolbars.forEach(function (tool) {
                var kendoToolbar = {};
                if (tool.text)
                    kendoToolbar.text = tool.text;
                if (tool.name)
                    kendoToolbar.name = tool.name;
                if (tool.iconClass)
                    kendoToolbar.iconClass = tool.iconClass;
                if (tool.template)
                    kendoToolbar.template = tool.template;
                if (tool.click) {
                    var $ctrlElm = $(controlSelector);
                    $ctrlElm.on("click", ".k-grid-toolbar > .k-grid-" + tool.name, function (e) {
                        tool.click.call($ctrlElm.data("kendoGrid"), e);
                    });
                }
                kendoGridToolbars.push(kendoToolbar);
            });
            return kendoGridToolbars;
        }

        function createKendoTreeListToolbar(toolbars, controlSelector) {
            var kendoTreeListToolbars = [];
            toolbars.forEach(function (tool) {
                var kendoToolbar = {};
                if (tool.text)
                    kendoToolbar.text = tool.text;
                if (tool.name)
                    kendoToolbar.name = tool.name;
                if (tool.click)
                    kendoToolbar.click = tool.click;
                if (tool.imageClass)
                    kendoToolbar.imageClass = tool.imageClass;
                kendoTreeListToolbars.push(kendoToolbar);
            });
            return kendoTreeListToolbars;
        }

        function createToolbar(toolbarContext) {
            validateToolbarContext(toolbarContext);
            if (!toolbarContext.toolbars || !toolbarContext.toolbars.length)
                return [];
            if (toolbarContext.type == "kendoGrid") {
                return createKendoGridToolbar(toolbarContext.toolbars, toolbarContext.controlSelector);
            }
            else if (toolbarContext.type == "kendoTreeList") {
                return createKendoTreeListToolbar(toolbarContext.toolbars, toolbarContext.controlSelector);
            }
        }

        return {
            createColumn: createColumn,
            createToolbar: createToolbar
        }
    }]);
});

//var x = {
//    controlSelector: "",
//    type: "", //"kendo", "dropdown", "icon"
//    btnClassName: "",//only type: dropdown
//    commands: [
//        { text: "", name: "", click: "", className: "", imageClass: "" },
//        { text: "", name: "", click: , mainClassName: "", listClassName: "", anchorClassName: "",
//          template: "", isMain: false, hasSeparator: false },
//        { text: "", name: "", iconClass: "", className: "", click:  }
//    ]
//};

//var z = {
//    controlSelector: "",
//    type: "", //"kendoTreeList", "kendoGrid"
//    toolbars: [
//        {  name: "", text: "", click: "", iconClass: "", template: "" }, //"kendoGrid"
//        {  name: "", text: "", click: "", imageClass: "" } //"kendoTreeList"
//    ]
//}