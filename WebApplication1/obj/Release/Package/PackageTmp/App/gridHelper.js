// Grid Helper for bootstrap and angular by Hamid.R Gharahzadeh     1394/4/19
function gridHelper(params) {
	if (!params)
		params = {};

	var _this = this;
	this.data = params.data || [];         // all data
	this.gridData = params.data || [];     // data showing in grid
	this.pageData = [];                    // data showing in current page
	this.rpp = Number(params.rpp) || 10;           // row per page
	this.pages = [1];
	this.currentPage = 1;
	this.pagePage = params.pagePage || null;
	this.rowCount = params.rowCount || null;
	this.sortAsc = true;
	this.previousFilter = '';
	this.previousSort = '';
	this.filterField = params.filterfield || '';
	this.filterKey = params.filterkey || '';
	this.sortField = params.sort || '';
	this.multiselect = params.mutilselect || true;
	this.checkAll = false;
	this.totalPage = 1;
	this.scope = params.scope || undefined;
	this.gridId = params.gridid || 'grid';
	this.showContextMenu = params.showContextMenu != undefined ? params.showContextMenu : true;
	this.searchBy = params.searchBy || [];
	this.displayCurrentPage = "";
	this.txtPageNumber = null;

	this.init = function () {
		angular.copy(_this.data, _this.gridData);
		//        _this.gridData = _this.data;
		var length = _this.gridData.length;
		for (var i = 0; i < length; i++) {
			_this.gridData[i].checked = false;
		}

		_this.applyFilterSort();
		_this.calPageCount();
		_this.setCurrentPageData();
		if (this.currentPage < 1)
			this.currentPage = 1;
		if (this.currentPage > this.pages.length)
			this.currentPage = this.pages.length;
		this.updateDisplayCurrentPage();
	};
	this.applyFilterSort = function () {
		if (_this.filterField) _this.applyFilter(_this.filterField, _this.filterKey);
		_this.sortAsc = !_this.sortAsc;
		if (_this.sortField) _this.sort(_this.sortField);
	};
	this.applySort = function () {
		_this.sortAsc = !_this.sortAsc;
		if (_this.sortField) _this.sort(_this.sortField);
	};
	this.calPageCount = function () {
		var pageCount = 1;
		if (_this.pagePage)
			pageCount = Math.ceil(_this.rowCount / _this.rpp);
		else pageCount = Math.ceil(_this.gridData.length / _this.rpp);
		_this.totalPage = pageCount;

		_this.pages = [];
		for (var i = 0; i < pageCount; i++)
			_this.pages.push(i + 1);
	};
	this.setCurrentPageData = function () {
		if (_this.pagePage) {
			_this.pageData = _this.gridData;
			return;
		}
		var a = _this.currentPage == 1 ? 0 : (_this.currentPage - 1) * _this.rpp;
		var b = _this.currentPage == 1 ? _this.rpp : _this.currentPage * _this.rpp;
		_this.pageData = _this.gridData.slice(a, _this.gridData[b - 1] ? b : _this.gridData.length);
		this.updateDisplayCurrentPage();
	};
	this.sort = function (fieldName, reverse) {
		_this.sortField = fieldName;

		if (_this.previousSort != fieldName)
			_this.sortAsc = true;
		if (reverse) _this.sortAsc = false;

		if (_this.sortAsc)
			_this.gridData.sort(sort_by(fieldName, false));
		else
			_this.gridData.sort(sort_by(fieldName, true));

		_this.setCurrentPageData();
		_this.previousSort = fieldName;
		_this.sortAsc = !_this.sortAsc;
	};
	this.sortAscend = function (fieldName) {
		_this.sortField = fieldName;
		_this.gridData.sort(sort_by(fieldName, false));

		_this.setCurrentPageData();
		_this.previousSort = fieldName;
	};
	this.removeSort = function () {
		_this.sortAsc = true;
		_this.sortField = '';

		angular.copy(_this.data, _this.gridData);
		_this.calPageCount();
		_this.setCurrentPageData();
		if (_this.scope.$apply());
	};
	this.applyFilter = function (fieldName, key, afterTimeout, scope, inside) {
		var fields = [fieldName];
		if (fieldName.search('.') > -1) fields = fieldName.split('.');

		var doFilter = function () {
			_this.filterField = fieldName;
			_this.filterKey = key;
			var filterData = [];
			var data = inside ? _this.gridData : _this.data;
			var l = data ? data.length : 0;
			for (var i = 0; i < l; i++) {
				var value = '';
				if (fields.length == 1)
					value = data[i][fieldName].toString();
				else if (fields.length == 2)
					value = !data[i][fields[0]] ? '' : data[i][fields[0]][fields[1]].toString();
				else if (fields.length == 3)
					value = !data[i][fields[0]][fields[1]] ? '' : data[i][fields[0]][fields[1]][fields[2]].toString();
				if (!value) value = '';

				// شروط عددی
				var keyStr = key.toString();
				if (keyStr.length > 3 && keyStr.substr(0, 3) === "{>}") {
					var key_ = keyStr.substr(3);
					if (parseInt(value) > parseInt(key_)) filterData.push(data[i]);
				}
				else if (keyStr.length > 3 && keyStr.substr(0, 3) === "{<}") {
					var key_ = keyStr.substr(3);
					if (parseInt(value) < parseInt(key_)) filterData.push(data[i]);
				}
				else if (keyStr.length > 3 && keyStr.substr(0, 3) === "{=}") {
					var key_ = keyStr.substr(3);
					if (parseInt(value) === parseInt(key_)) filterData.push(data[i]);
				}
				else if (keyStr.length > 4 && keyStr.substr(0, 4) === "{!=}") {
					var key_ = keyStr.substr(4);
					if (parseInt(value) !== parseInt(key_)) filterData.push(data[i]);
				}
				else if (keyStr.length > 4 && keyStr.substr(0, 4) === "{>=}") {
					var key_ = keyStr.substr(4);
					if (parseInt(value) >= parseInt(key_)) filterData.push(data[i]);
				}
				else if (keyStr.length > 4 && keyStr.substr(0, 4) === "{<=}") {
					var key_ = keyStr.substr(4);
					if (parseInt(value) <= parseInt(key_)) filterData.push(data[i]);
				} else {
					// شرط جستجو در رشته: شرط پیش فرض
					if (value.toLowerCase().search(key.toString().toLowerCase()) != -1)
						filterData.push(data[i]);
				}
			}
			_this.gridData = filterData;
			_this.calPageCount();
			if (_this.currentPage > _this.totalPage)
				_this.currentPage = (_this.totalPage > 0 ? _this.totalPage : 1);
			_this.setCurrentPageData();
		};

		if (afterTimeout) {
			if (_this.timeout)
				clearTimeout(_this.timeout);
			_this.timeout = setTimeout(function () {
				doFilter();
				if (scope) applyScope(scope);
			}, 1000);
		} else {
			if (_this.timeout)
				clearTimeout(_this.timeout);
			doFilter();
		}
	};
	this.removeFilter = function () {
		if (_this.timeout)
			clearTimeout(_this.timeout);
		_this.filterField = '';
		_this.filterKey = '';

		angular.copy(_this.data, _this.gridData);
		_this.calPageCount();
		_this.setCurrentPageData();
		if (_this.scope)
			_this.applyScope(_this.scope);
	};
	function getValue(item, field) {
		if (field.indexOf('{') === 0)
			field = field.substr(1, field.length - 2);

		if (field === "*")
			return item;

		var properties = field.split(".");
		var value = item;
		for (var i = 0; i < properties.length; i++) {
			if (typeof value == "undefined" || value == null)
				return null;
			value = value[properties[i]];
		}
		return value;
	}
	this.search = function (value, scope) {
		clearTimeout(_this.searchTimer);
		_this.searchTimer = setTimeout(function () {
			_this.gridData = _this.filterData(value);
			_this.calPageCount();
			_this.setCurrentPageData();
			_this.applyScope(scope);
		}, 1000);
	};
	this.filterData = function (value) {
		function contains(item, fields, search) {
			for (var j = 0; j < fields.length; j++) {
				if ((getValue(item, fields[j]) + "").toLowerCase().indexOf(search) > -1)
					return true;
			}
			return (item + "").toLowerCase().indexOf(search) > -1;
		}
		value = (value + "").toLowerCase();
		var filteredData = [];
		for (var i = 0; i < this.data.length; i++) {
			if (contains(this.data[i], this.searchBy, value))
				filteredData.push(this.data[i]);
		}
		return filteredData;
	};
	this.goToPage = function (number, event) {
		if (event) {
			var t = event.target;
			setTimeout(function () {
				t.select();
			}, 0);
		}

		number = isNaN(Number(number)) ? this.currentPage : Number(number);
		if (number < 1)
			number = 1;
		if (number > _this.pages.length)
			number = _this.pages.length;

		_this.currentPage = number;
		_this.displayCurrentPage = number;

		if (!event)
			this.updateDisplayCurrentPage();
		if (_this.pagePage) {
			_this.pagePage(number, _this.rpp);
			return;
		}

		_this.setCurrentPageData();
	};
	this.nextPage = function () {
		this.goToPage(this.currentPage + 1);
	};
	this.previousPage = function () {
		this.goToPage(this.currentPage - 1);
	};
	this.goToLastPage = function () {
		this.goToPage(this.pages.length);
	};
	this.changeRpp = function (rpp) {
		_this.rpp = rpp;
		_this.calPageCount();
		if (_this.currentPage > _this.totalPage)
			_this.currentPage = (_this.totalPage > 0 ? _this.totalPage : 1);

		if (_this.pagePage) {
			_this.pagePage(_this.currentPage, _this.rpp);
			return;
		}
		_this.setCurrentPageData();
	};
	this.addItem = function (item) {
		_this.data.push(item);
		_this.init();
	};
	this.removeItem = function (item) {
		for (var i = 0; i < _this.data.length; i++) {
			if (_this.data[i].Id == item.Id) {
				var index = _this.data.indexOf(_this.data[i]);
				if (index > -1) {
					_this.data.splice(index, 1);
					_this.init();
					break;
				}
			}
		}
	};
	this.removeItems = function (items) {
		for (var j = 0; j < items.length; j++) {
			for (var i = 0; i < _this.data.length; i++) {
				if (_this.data[i].Id == items[j].Id) {
					var index = _this.data.indexOf(_this.data[i]);
					if (index > -1) {
						_this.data.splice(index, 1);
						break;
					}
				}
			}
		}
		_this.init();
	};
	this.findItemRowNumber = function (item) {
		for (var i = 0; i < _this.data.length; i++) {
			if (_this.data[i].Id == item.Id) {
				var index = _this.data.indexOf(_this.data[i]);
				return index + 1;
			}
		}
	};
	this.selectAll = function () {
		for (var i = 0; i < _this.pageData.length; i++) {
			_this.pageData[i].checked = true;
		}
		if (_this.scope.$apply());
		//        _this.applyFilterSort();
	};
	this.selectNone = function () {
		for (var i = 0; i < _this.pageData.length; i++) {
			_this.pageData[i].checked = false;
		}
		if (_this.scope.$apply());
		//        _this.applyFilterSort();
	};
	this.selectAllClick = function () {
		if (_this.checkAll)
			_this.selectAll();
		else _this.selectNone();
	};
	this.selectInverse = function () {
		for (var i = 0; i < _this.gridData.length; i++) {
			_this.gridData[i].checked = !_this.gridData[i].checked;
		}
		_this.applyFilterSort();
		if (_this.scope.$apply());
	};
	this.getSelectedItems = function () {
		var checkedItems = [];
		for (var i = 0; i < _this.gridData.length; i++) {
			if (_this.gridData[i].checked)
				checkedItems.push(_this.gridData[i]);
		}
		return checkedItems;
	};
	this.resetGrid = function () {
		_this.removeFilter();
		_this.removeSort();
		_this.init();
	};
	this.createContextMenu = function () {

		function addMenuItem(text, onclick, disable) {
			var li = document.createElement("li");
			ul.appendChild(li);
			if (text == "-") {
				li.className = "divider";
				return;
			}
			var a = document.createElement("a");
			li.appendChild(a);
			if (disable)
				$(li).addClass("disabled");
			a.tabIndex = -1;
			$(a).bind("click", onclick);
			a.innerHTML = text;
		}

		var contextMenu = document.createElement("div");
		document.body.appendChild(contextMenu);
		contextMenu.className = "dropdown clearfix grid-context-menu";
		var ul = document.createElement("ul");
		contextMenu.appendChild(ul);
		ul.className = "dropdown-menu";
		ul.role = "menu";
		ul["aria-labelledby"] = "dropdownMenu";
		ul.style.display = "block";
		ul.style.position = "static";

		addMenuItem("انتخاب همه", function () { _this.selectAll(); }, !_this.multiselect);
		addMenuItem("انتخاب هیچکدام", function () { _this.selectNone(); }, !_this.multiselect);
		addMenuItem("انتخاب معکوس", function () { _this.selectInverse(); }, !_this.multiselect);
		addMenuItem("-");
		addMenuItem("ريست جدول", function () { _this.removeSort(); });

		$(document).on("click", function () {
			$(contextMenu).hide();
		});
		$('#' + _this.gridId).bind("contextmenu", function (e) {
			e.preventDefault();
		});
		$('#' + _this.gridId).bind("contextmenu", function (e) {
			e.preventDefault();

			$(contextMenu).css({
				display: "block",
				left: e.pageX - $(contextMenu).width(),
				top: e.pageY
			});
		});
	};
	this.onPageNavigationFocus = function (event) {
		this.txtPageNumber = event.target;
		this.txtPageNumber.hasFocus = true;
		this.updateDisplayCurrentPage();
		setTimeout(function () {
			_this.txtPageNumber.select();
		}, 0);
	};
	this.onPageNavigationBlur = function () {
		this.txtPageNumber.hasFocus = false;
		this.updateDisplayCurrentPage();
	};
	this.updateDisplayCurrentPage = function () {
		var count = this.rowCount || this.gridData.length;
		if (!count) {
			if (this.txtPageNumber && this.txtPageNumber.hasFocus)
				this.displayCurrentPage = "0";
			else
				this.displayCurrentPage = "خالی";
		} else {
			if (this.txtPageNumber && this.txtPageNumber.hasFocus)
				this.displayCurrentPage = this.currentPage;
			else
				this.displayCurrentPage = Hesabfa.farsiDigit(this.currentPage + " از " + this.totalPage);
		}

	};
	this.applyScope = function ($scope) {
		if (!$scope.$$phase) {
			$scope.$apply();
		}
	}
	if (this.showContextMenu)
		this.createContextMenu();
}

function dxGridPrint(args, data, exportPdf) {
	function mm(d) {
		return d * 3.9381;
	}

	for (var j = 0; j < args.cols.length; j++) {
		args.cols[j] = jQuery.extend(true, {
			caption: "",
			width: 10,
			align: "center",
			bold: false,
			format: "",
			totals: false,
			fontName: "",
			fontSize: "",
			color: "",
			rtl: true
		}, args.cols[j]);
	}
	args = jQuery.extend(true, {
	    fileName: "report.pdf",
        rowColumnWidth: 50,
		page: {
			size: "A4",
			landscape: false,
			margins: {
				left: 7,
				right: 7,
				top: 7,
				bottom: 7
			}
		},
		header: {
			title1: "",
			title2: "",
			left: "",
			center: "",
			right: ""
		},
		table: {
			header: {
				height: 10,
				fill: '#e1e1e1',
				fontName: 'Iransans',
				fontSize: '8pt',
				bold: true,
				color: "#000"
			},
			rows: {
				height: 8,
				fill1: '#fff',
				fill2: '#f1f1f1',
				fontName: 'Iransans',
				fontSize: '8pt',
				color: "#000"
			}
		}
	}, args);

	var width = 210, height = 297;

	if (args.page.size === "A4") {
		width = 210;
		height = 297;
	}
	if (args.page.size === "A5") {
		width = 148;
		height = 210;
	}
	if (args.page.landscape) {
		var w = width;
		width = height;
		height = w;
	}

	var margins = args.page.margins;

	var cols = args.cols;
	var rows = data;

	var pagerect = { l: margins.left, t: margins.top, w: width - margins.left - margins.right, h: height - margins.top - margins.bottom, r: width - margins.right, b: height - margins.bottom };


	if (cols.length === 0)
		return;

	var pdf = exportPdf ? new jsPDF("p", "mm", [width, height]) : null;
	var wnd = exportPdf ? null : window.open("");

	function calcColumnsWidths() {
		var sum = 0, i;
		for (i = 0; i < cols.length; i++) {
			var w = cols[i].width;
			if (isNaN(Number(w)))
				w = 2;
			cols[i].width = w;
			sum += w;
		}
		var r = pagerect.w / sum;
		for (i = 0; i < cols.length; i++) {
			cols[i].width *= r;
		}

	}

	calcColumnsWidths();
	function drawCell(gr, l, t, w, h, text, fontName, fontSize, bold, hAlign, vAlign, textColor, backColor, borderColor, borderWidth, borderStype, rtl) {
		gr.fillRect(mm(l), mm(t), mm(w), mm(h), backColor, "1px");
		gr.drawRect(mm(l), mm(t), mm(w), mm(h), borderColor, borderWidth, borderStype);
		gr.drawText(mm(l), mm(t), mm(w), mm(h), text, fontName, fontSize, textColor, vAlign, hAlign, bold, false, false, rtl);
	}

	function calcHeaderHeight() {
		var h = args.header.title1 ? 10 : 0;
		h += args.header.title2 ? 10 : 0;
		h += (args.header.left || args.header.center || args.header.right) ? 7 : 0;
		return h;
	}
	function calcFooterHeight(gr) {
		return 7;
	}
	function drawHeader(gr, t) {
		function calc(o) {
			if (typeof o == "undefined" || o == null)
				return "";
			if (typeof o == "function")
				return o();
			return o + "";
		}

		var t1 = calc(args.header.title1);
		var t2 = calc(args.header.title2);
		var tr = calc(args.header.right);
		var tc = calc(args.header.center);
		var tl = calc(args.header.left);
		if (t1) {
			drawCell(gr, pagerect.l, t, pagerect.w, 10, t1, "Iransans", "14pt", false, "center", "middle", "#000", "transparent", "transparent", "0px", "none", true);
			t += 10;
		}
		if (t2) {
			drawCell(gr, pagerect.l, t - 1, pagerect.w, 10, t2, "Iransans", "12pt", true, "center", "middle", "#444", "transparent", "transparent", "0px", "none", true);
			t += 10;
		}
		if (tl) {
			drawCell(gr, pagerect.l + 1, t, pagerect.w / 3, 7, tl, "Iransans", "10pt", true, "left", "middle", "#000", "transparent", "transparent", "0px", "none", true);
		}
		if (tc) {
			drawCell(gr, pagerect.l + pagerect.w / 3, t, pagerect.w / 3, 7, tc, "Iransans", "10pt", true, "center", "middle", "#000", "transparent", "transparent", "0px", "none", true);
		}
		if (tr) {
			drawCell(gr, pagerect.l + pagerect.w * 2 / 3 - 1, t, pagerect.w / 3, 7, tr, "Iransans", "10pt", true, "right", "middle", "#000", "transparent", "transparent", "0px", "none", true);
		}
		t += (args.header.left || args.header.center || args.header.right) ? 7 : 0;
		gr.drawLine(mm(pagerect.l), mm(t), mm(pagerect.r), mm(t), "#000", "1px", "solid");
		return t;
	}

	function drawFooter(gr) {
		var h = 7;
		var t = pagerect.b - h;
		var left = "صفحه " + (pageIndex + 1) + " از " + pageCount;
		var center = "www.smbt.ir";
		var right = window.todayLongDate;
		drawCell(gr, pagerect.l + 1, t, pagerect.w / 3, h, left, "Iransans", "8pt", false, "left", "middle", "#000", "transparent", "transparent", "0px", "none", true);
		drawCell(gr, pagerect.l + pagerect.w / 3, t, pagerect.w / 3, h, center, "Iransans", "10pt", true, "center", "middle", "#888", "transparent", "transparent", "0px", "none", true);
		drawCell(gr, pagerect.l + pagerect.w * 2 / 3 - 1, t, pagerect.w / 3, h, right, "Iransans", "8pt", false, "right", "middle", "#000", "transparent", "transparent", "0px", "none", true);
		gr.drawLine(mm(pagerect.l), mm(t), mm(pagerect.r), mm(t), "#000", "1px", "solid");
		return h;
	}


	function drawTableHeader(gr, t) {
		var left = pagerect.l;
		for (var i = cols.length - 1; i >= 0; i--) {
			drawCell(gr, left, t, cols[i].width, args.table.header.height, cols[i].caption, args.table.header.fontName, args.table.header.fontSize,
				args.table.header.bold, "center", "middle", args.table.header.color, args.table.header.fill, "#000", "1px", "solid", true);
			left += cols[i].width;
		}
		return t + args.table.header.height;
	}

	function drawRow(gr, t, row, alternate) {
		var left = pagerect.l;
		for (var i = cols.length - 1; i >= 0; i--) {
			var val = row[i];
			if (val === true)
				val = "✓";
			if (val === false)
				val = "";
			if (cols[i].format === "#")
				val = formatToCurrency(val + "");
			drawCell(gr, left, t, cols[i].width, args.table.rows.height, val, cols[i].fontName || args.table.rows.fontName, cols[i].fontSize || args.table.rows.fontSize,
				cols[i].bold, cols[i].align, "middle", cols[i].color || args.table.rows.color, alternate ? args.table.rows.fill1 : args.table.rows.fill2, "#000", "1px", "solid", cols[i].rtl);
			left += cols[i].width;
		}
		return t + args.table.rows.height;
	}
	function drawTotalRow(gr, t) {
		var summary = args.grid.option("summary");
		var left = pagerect.l;
		for (var i = cols.length - 1; i >= 0; i--) {
			var val = "";
			if (cols[i].gridCol) {
				for (var j = 0; j < summary.totalItems.length; j++) {
					if (summary.totalItems[j].column === cols[i].gridCol.dataField) {
						val = args.grid.getTotalSummaryValue(summary.totalItems[j].column);
						break;
					}
				}
			}
			if (cols[i].format === "#")
				val = formatToCurrency(val + "");
			drawCell(gr, left, t, cols[i].width, args.table.rows.height, val, cols[i].fontName || args.table.rows.fontName, cols[i].fontSize || args.table.rows.fontSize,
				true, cols[i].align, "middle", cols[i].color || args.table.header.color, args.table.header.fill, "#000", "1px", "solid", cols[i].rtl);
			left += cols[i].width;
		}
		return t + args.table.rows.height;
	}
	function drawTable(gr, pageIndex, t) {
		t = drawTableHeader(gr, t);
		var rpp = getRowPerPage();
		var start = rpp * pageIndex;
		var end = Math.min(rows.length, start + rpp);
		for (var i = start; i < end; i++) {
			t = drawRow(gr, t, rows[i], i % 2 === 0);
		}
		if (hasTotalRow() && pageIndex === pageCount - 1)
			drawTotalRow(gr, t);

	}

	function getTableHeight() {
		return pagerect.h - calcHeaderHeight() - calcFooterHeight();
	}
	function getRowPerPage() {
		var h = getTableHeight() - args.table.header.height;
		return parseInt(h / args.table.rows.height);
	}

	function hasTotalRow() {
		var summary = args.grid.option("summary");
		return summary.totalItems && summary.totalItems.length > 0;
	}

	function getPageCount() {
		var rowCount = rows.length;
		if (hasTotalRow())
			rowCount++;
		var rpp = getRowPerPage();
		var count = parseInt(rowCount / rpp) + 1;
		if (rowCount % rpp === 0)
			count--;
		if (count === 0)
			count = 1;
		return count;
	}

	function printPage(index) {
		var canvas;
		function prepareGraphics() {
			var scale = 2;
			canvas = document.createElement("canvas");
			document.body.appendChild(canvas);
			canvas.style.direction = "rtl";
			canvas.style.display = "none";
			canvas.width = mm(width) * scale;
			canvas.height = mm(height) * scale;
			var _gr = new graphics(canvas);
			_gr.scale(scale, scale);
			return _gr;
		}

		function deleteGraphics() {
			$(canvas).remove();
		}


		var gr = prepareGraphics();
		gr.fillRect(0, 0, mm(width), mm(height), "#fff", "1px", 0);
		gr.drawRect(mm(pagerect.l), mm(pagerect.t), mm(pagerect.w), mm(pagerect.h), "#000", "1px", "solid");

		var top = pagerect.t;
		top = drawHeader(gr, top);
		top = drawTable(gr, index, top);
		drawFooter(gr, top);

		if (exportPdf)
			pdf.addRawImageGray(gr.ctx.getImageData(0, 0, canvas.width, canvas.height), 0, 0, width, height);
		else {
			var img = wnd.document.createElement("img");
			img.width = parseInt(mm(width));
			img.height = parseInt(mm(height));
			img.src = canvas.toDataURL("image/png");
			wnd.document.body.appendChild(img);

			var div = wnd.document.createElement("div");
			$(div).css("page-break-after", "always");
			wnd.document.body.appendChild(div);

		}

		deleteGraphics();
	}


	var pageCount = getPageCount();
	var pageIndex = 0;
	var popup;
	var progressbar;
	if (exportPdf) {
		popup = $('<div>').appendTo(document.body);
		popup.dxPopup({
			width: 400,
			height: 200,
			contentTemplate: function(ce) {
				var progress = $('<div>').appendTo(ce);
				var status = $('<div class="text-center text-info" style="font-size:12pt">').appendTo(ce);
				progressbar = $(progress).dxProgressBar({
					min: 1,
					max: pageCount,
					height: 50,
					showStatus: false,
					statusFormat: function(value) {
						status.html(Hesabfa.farsiDigit("در حال ساخت صفحه " + (pageIndex + 1) + " از " + pageCount));
					}
				}).dxProgressBar("instance");

			},
			showTitle: true,
			rtlEnabled: true,
			showCloseButton: false,
			title: "در حال ساخت فایل PDF",
			visible: true,
			dragEnabled: true,
			closeOnOutsideClick: false
		}).dxPopup("instance");
	}


	function print() {
		if (progressbar) {
			progressbar.option("value", pageIndex + 1);
		}
		if (wnd)
			wnd.document.title = "صفحه " + (pageIndex + 1) + " از " + pageCount;
		setTimeout(function () {
			printPage(pageIndex);
			pageIndex++;
			if (pageIndex < pageCount)
				print();
			else {
				setTimeout(function () {
					if (exportPdf)
						pdf.save(args.fileName);
					else
						wnd.print();
					if(popup)
						popup.hide();

				}, 500);
				if (progressbar)
					progressbar.option("value", pageCount);
			}
		}, exportPdf ? 500 : 10);
	}

	print();
}
function dxCreatDefaultGridOption(args) {
	function g(v, def) {
		if (typeof v == "undefined" || v == null)
			return def;
		return v;
	}
	var multiSelect = args.selection && args.selection.mode === 'multiple';
	var cols = g(args.columns, []);
	var newCols = [];
	newCols.push({
		caption: '#',
		allowHiding: false,
		alignment: 'center',
		allowFiltering: false,
		allowEditing: false,
		allowFixing: true,
		allowGrouping: false,
		allowHeaderFiltering: false,
		allowReordering: false,
		allowResizing: false,
		allowSearch: false,
		allowSorting: false,
		allowExporting: false,
		fixed: true,
		fixedPosition: 'right',
		visibleIndex: 0,
		width: 50,
		cellTemplate: function (cellElement, cellInfo) {
			/// for virtual scrolling use the bellow code
//			var missedRowsNumber = cellInfo.component.getController('data').virtualItemsCount().begin;
//			var s = cellInfo.rowIndex + 1 + missedRowsNumber;
			//			cellElement.text(Hesabfa.farsiDigit(s));
			var d = cellInfo.component.pageIndex() * cellInfo.component.pageSize();
			cellElement.text(Hesabfa.farsiDigit(d + cellInfo.row.rowIndex + 1));

		}
	});

	var dicColumnFormat = {};
	var printExtra = {};
	for (var i = 0; i < cols.length; i++) {
		newCols.push(cols[i]);
		dicColumnFormat[cols[i].dataField] = cols[i].format;
		printExtra[cols[i].dataField] = {
			caption: cols[i].printCaption,
			format: cols[i].printFormat || '',
			align: cols[i].printAlign,
			bold: cols[i].printBold,
			width: cols[i].printWidth || 2,
			fontName: cols[i].printFontName,
			fontSize: cols[i].printFontSize,
			visible: cols[i].printVisible,
			calcValue: cols[i].printCalcValue
		};
		cols[i].format = "";
		if (!cols[i].lookup && cols[i].dataType !== 'boolean' && !cols[i].cellTemplate) {
			cols[i].cellTemplate = function (cellElement, cellInfo) {
				var val = cellInfo.displayValue || cellInfo.text;
				if (typeof (val) == "undefined" || val == null) {
					val = "";
					cellElement.html(val);
					return;
				}
				var txt = Hesabfa.farsiDigit(val);

				var format = dicColumnFormat[cellInfo.column.dataField];

				if (format) {
					var money = Hesabfa.money(val);
					val = format.replace(/#/ig, money).replace(/\*/, txt);
				}
				else
					val = txt;

				cellElement.html(val);
			}
		}
	}
	cols = newCols;
	var args2 = {
		dataSource: g(args.dataSource, []),
		columns: cols,
		rtlEnabled: g(args.rtlEnabled, true),
		allowColumnReordering: g(args.allowColumnReordering, true),
		allowColumnResizing: g(args.allowColumnResizing, true),
		wordWrapEnabled: g(args.wordWrapEnabled, true),
		//columnResizingMode: g(args.columnResizingMode, "widget"),
		showColumnLines: g(args.showColumnLines, true),

		showRowLines: g(args.showRowLines, true),
		rowAlternationEnabled: g(args.rowAlternationEnabled, true),
		showBorders: g(args.showBorders, true),
		columnAutoWidth: g(args.columnAutoWidth, true),
		showColumnHeaders: g(args.showColumnHeaders, true),
		remoteOperations: g(args.remoteOperations, {
			sorting: true,
			paging: true,
			filtering: true,
			grouping: true,
			summary: args.isServerDataSource
		}),
		grouping: g(args.grouping, {
			contextMenuEnabled: args.allowGrouping,
			autoExpandGroup: false,
			autoExpandAll: false
		}),
		groupPanel: g(args.groupPanel, {
			visible: false
		}),
		searchPanel: g(args.searchPanel, {
			visible: false,
			width: 240,
			placeholder: "جستجو..."
		}),
		hoverStateEnabled: g(args.hoverStateEnabled, true),
		columnChooser: g(args.columnChooser, {
			enabled: false
		}),
		columnFixing: g(args.columnFixing, {
			enabled: true
		}),
		paging: g(args.paging, {
			pageSize: g(args.rpp, 10)
		}),
		pager: {
			showPageSizeSelector: true,
			allowedPageSizes: [5, 10, 20],
			showInfo: true
		},
		scrolling: g(args.scrolling, {
			mode: "virtual",
			showScrollbar: "always"
		}),
		sorting: g(args.sorting, {
			mode: "multiple"
		}),
		filterRow: g(args.filterRow, {
			visible: true,
			applyFilter: "auto"
		}),
		headerFilter: g(args.headerFilter, {
			visible: true
		}),
		"export": {
			enabled: false,
			fileName: "خروجی",
			allowExportSelectedData: false
		},

		stateStoring: {
			enabled: typeof args.layoutKeyName != "undefined" && args.layoutKeyName !== '',
			type: "custom",
			storageKey: args.layoutKeyName,
			customSave: function (state) {
				delete state.selectedRowKeys;
				state.pageIndex = 0;
				for (var i = 0; i < state.columns.length; i++) {
					delete state.columns[i].filterValues;
					delete state.columns[i].filterValue;
				}
				//state.summaries = grid.summaries;
				var s = JSON.stringify(state);
				if (typeof (Storage) !== "undefined")
					localStorage.setItem(args.layoutKeyName, s);
			},
			customLoad: function () {
				if (typeof (Storage) == "undefined")
					return null;
				if (!localStorage[args.layoutKeyName])
					return null;
				var state = JSON.parse(localStorage[args.layoutKeyName]);
				//grid.summaries = state.summaries || {};
				return state;
			}
		},
		onContextMenuPreparing: function (e) {
			var grid = e.component;
			if (!e.row)
				return;
			if (e.row.rowType === "header") {
				e.items.push({
					beginGroup: true,
					text: "حذف فیلتر",
					disabled: !grid.getCombinedFilter(),
					icon: "remove",
					onItemClick: function () {
						grid.clearFilter();
					}
				});
				e.items.push({
					beginGroup: true,
					text: "انتخابگر ستون ها",
					icon: "",
					onItemClick: function () {
						grid.showColumnChooser();
					}
				});
				e.items.push({
					text: "تنظیم مجدد قالب",
					icon: "refresh",
					onItemClick: function () {
						grid.state(null);
					}
				});
				e.items.push({
					text: "ارسال به اکسل",
					beginGroup: true,
					icon: "exportxlsx",
					onItemClick: function () {
						grid.exportToExcel(false);
					}
				});
				e.items.push({
					text: "چاپ",
					beginGroup: false,
					icon: "print",
					onItemClick: function () {
						grid.print();
					}
				});
				e.items.push({
					text: "فایل PDF",
					beginGroup: false,
					icon: "exportpdf",
					onItemClick: function () {
						grid.pdf();
					}
				});
				var col = e.column;
				e.items.push({
					text: "نتیجه",
					beginGroup: true,
					icon: "percent",
					items: [
						{ text: "تعداد", onItemClick: function () { grid.addSummery("count", col); } },
						{ text: "مجموع", onItemClick: function () { grid.addSummery("sum", col); } },
						{ text: "حداقل", onItemClick: function () { grid.addSummery("min", col); } },
						{ text: "حداکثر", onItemClick: function () { grid.addSummery("max", col); } },
						{ text: "میانگین", onItemClick: function () { grid.addSummery("avg", col); } },
						{ text: "حذف", beginGroup: true, onItemClick: function () { grid.addSummery("", col); } }
					]
				});
			}
			if (e.row.rowType === "data") {
				e.items = [];
				e.items.push({
					text: "انتخاب همه",
					disabled: !multiSelect,
					onItemClick: function () {
						grid.selectAll();
					}
				});
				e.items.push({
					text: "انتخاب هیچکدام",
					disabled: !multiSelect,
					onItemClick: function () {
						grid.clearSelection();
					}
				});
				e.items.push({
					text: "ارسال به اکسل",
					beginGroup: true,
					icon: "exportxlsx",
					onItemClick: function () {
						grid.exportToExcel(false);
					}
				});
				e.items.push({
					text: "چاپ",
					beginGroup: false,
					icon: "print",
					onItemClick: function () {
						grid.print();
					}
				});
				e.items.push({
					text: "فایل PDF",
					beginGroup: false,
					icon: "exportpdf",
					onItemClick: function () {
						grid.pdf();
					}
				});
			}
		}
	};
	for (var item in args) {
		if (typeof args2[item] == "undefined")
			args2[item] = args[item];
	}
	args2.printExtra = printExtra;
	return args2;
}
function dxGrid(args) {

	var args2 = dxCreatDefaultGridOption(args);

	args2.paging = { enabled: true, pageSize: 10 };
	args2.pager = { showPageSizeSelector: true, allowedPageSizes: [5, 10, 15, 20], showInfo: false };
	args2.scrolling = null;
	args2.height = "auto";


	var el = args.elementId ? $('#' + args.elementId) : $(args.element);
	var grid = el.dxDataGrid(args2).dxDataGrid("instance");
	grid.fill = function (data) {
		grid.option("dataSource", data);
		grid.doScroll = true;
	}
	grid.addSummery = function (type, col) {
		if (!this.summaries)
			this.summaries = {};
		this.summaries[col.dataField] = type;
		grid.refreshSummary();
	};
	grid.refreshSummary = function () {
		var items = [];
		for (var i in this.summaries) {
			if (this.summaries[i])
				items.push({
					column: i,
					summaryType: this.summaries[i],
					customizeText: function (data) {
						return Hesabfa.farsiDigit(data.valueText);
					}
				});
		}
		grid.option("summary", { totalItems: items });
	};
	grid.on("contentReady", function (e) {
		grid.fixScroll();
	});
	grid.fixScroll = function () {
		if (grid.doScroll) {
			var s = grid.getScrollable();
			var l = s.scrollLeft();
			var w = s.scrollWidth();
			s.scrollBy({ left: w, top: 0 });
		}
		grid.doScroll = false;
	}
	function print(exportPdf) {
		var extra = args2.printExtra;
		var pargs = args.print || {};
		pargs.grid = grid;
		var printArgs;
		var col, i, j;
		if (!pargs.cols) {
			pargs.cols = [];
			var cols = grid.getVisibleColumns();
			for (i = 0; i < cols.length; i++) {
				col = cols[i];
				if (col.caption === "#" && !col.dataField) {
					pargs.cols.push({
						caption: "ردیف",
						width: pargs.rowColumnWidth || col.visibleWidth,
						align: "center",
						gridCol: null
					});
				} else {
					printArgs = extra[col.dataField] || {};
					if (printArgs.visible === false || !col.calculateCellValue)
						continue;
					pargs.cols.push({
						caption: printArgs.caption || col.caption,
						width: printArgs.width || col.visibleWidth,
						align: printArgs.align || (col.dataType === "boolean" ? "center" : "right"),
						format: printArgs.format,
						fontName: printArgs.fontName,
						fontSize: printArgs.fontSize,
						bold: printArgs.bold,
						gridCol: col
					});
				}
			}
		}
		grid.getFilteredRows(function (items) {
			var rows = [];
			for (i = 0; i < items.length; i++) {
				var row = [];
				for (j = 0; j < pargs.cols.length; j++) {
					col = pargs.cols[j].gridCol;
					var val;
					if (!col)
						val = (i + 1) + '';
					else {
						printArgs = extra[col.dataField] || {};
						val = printArgs.calcValue ? printArgs.calcValue(items[i]) : col.calculateCellValue(items[i]);
						if (typeof val === "undefined") val = ""; // new by hamid
					}
					row.push(val);
				}
				rows.push(row);
			}
			dxGridPrint(pargs, rows, exportPdf);
		});
	}
	grid.print = function () {
		print(false);
	}
	grid.pdf = function () {
		print(true);
	}
	grid.getFilteredRows = function (callback) {
		var filter = grid.getCombinedFilter();
		var ds = grid.getDataSource();
		var sort = grid.getDataSource().sort();
		ds.store().load({
			filter: filter,
			sort: sort
		}).done(function (filteredData) {
			callback(filteredData);
		});
	}
	return grid;
}

function createDevExpressCustomStore(gridId, webMethod, params, webservice) {
    var store = new DevExpress.data.CustomStore({
        load: function (loadOptions) {
            var grid = $("#" + gridId).dxDataGrid('instance');
            var deferred = $.Deferred(), args = {};
            if (loadOptions.sort) {
                args.orderby = loadOptions.sort[0].selector;
                if (loadOptions.sort[0].desc)
                    args.orderby += " desc";
            }
            args.orderby = args.orderby || '';
            args.skip = loadOptions.skip || 0;
            args.take = loadOptions.take || 10;
            args.filter = loadOptions.filter || {};
            args.columns = [];
            for (var i = 0; i < grid.columnCount(); i++) {
                var dataField = grid.columnOption(i, "dataField");
                if (typeof dataField === "undefined" || dataField === "")
                    continue;
                args.columns.push([grid.columnOption(i, "dataField"), grid.columnOption(i, "enumType") || '', grid.columnOption(i, "fieldType") || '']);
            }
            params.args = args;
            if (!webservice)
                webservice = DefaultUrl.MainWebService;
            callws(webservice + webMethod, params)
                .success(function(result) {
                    deferred.resolve(result.items, { totalCount: result.totalCount });
                }).fail(function(error) {
                    deferred.reject("اطلاعات قابل دریافت نمی باشد.");
                }).loginFail(function() {
                    window.location = DefaultUrl.login;
                });
            return deferred.promise();
        }
    });
    return store;
}

