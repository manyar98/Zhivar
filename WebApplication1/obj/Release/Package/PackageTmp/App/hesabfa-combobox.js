function HesabfaCombobox(params) {
	this.items = params.items || [];
	this.filteredItems = [];
	this.displayItems = [];
	this.containerEle = params.containerEle;
	this.newBtn = params.newBtn || false;
	this.deleteBtn = params.deleteBtn || false;
	this.toggleBtn = params.toggleBtn || true;
	this.itemTemplate = params.itemTemplate || "<div> {*} </div>";
	this.matchBy = params.matchBy || "item";	// ex: item.Id
	this.callbackObj = params.callbackObj || null;
	this.displayProperty = params.displayProperty || "{*}";
	this.searchBy = params.searchBy || [];	// format: string|int|email
	this.searchable = params.searchable || true;
	this.displayCount = params.displayCount || 5;
	this.itemClass = params.itemClass || "";
	this.activeItemClass = params.activeItemClass || "active";
	this.inputClass = params.inputClass || "form-control input-sm hesabfa-combobox-input";
	this.hasScrollbar = params.hasScrollbar || true;
	this.divider = params.divider || false;
	this.selected = null;
	this.activeItem = null;
	this.activeItemIndex = 0;
	this.placeholders = [];
	this.eleListBox = null;
	this.eleListContainer = null;
	this.eleScrollbar = null;
	this.eleScrollbarPointer = null;
	this.scrollbar = null;
	this.minWidth = 300;

	this.onNew = params.onNew || null;
	this.onDelete = params.onDelete || null;
	this.onSelect = params.onSelect || null;

	this.keys = {
		down: 40,
		up: 38,
		tab: 9,
		enter: 13
	}

	var _this = this;
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

	function parseTemplate(item, template) {
		var matches = template.match(/{[^}]*}/igm);
		if (!matches)
			return template;
		var str = template;
		for (var i = 0; i < matches.length; i++) {
			var value = getValue(item, matches[i]);
			str = str.replace(matches[i], value);
		}
		return Hesabfa.farsiDigit(str);
	}

	this.makeList = function () {
		var elements = [];
		var itemsLength = _this.displayItems.length;
		for (var i = 0; i < itemsLength; i++) {
			var ele = _this.makeItem(_this.displayItems[i], i);
			elements.push(ele);
			if (_this.divider && i+1 < itemsLength) {
				var eleDivider = _this.makeDivider();
				elements.push(eleDivider);
			}
		}
		return elements;
	};
	this.makeItem = function (item, index) {
		var newItemHtml = parseTemplate(item, this.itemTemplate);
		var divItem = document.createElement("div");
		divItem.innerHTML = newItemHtml;
		divItem.id = "hcomboDivItem";
		divItem.tag = index;
		if (_this.matchBy === "item") {
			divItem.className = item === _this.activeItem ? _this.activeItemClass : _this.itemClass;
		} else {
			var itemValue = _this.getValueByPlaceholder(item, _this.matchBy);
			var activeItemValue = _this.getValueByPlaceholder(_this.activeItem, _this.matchBy);
			divItem.className = itemValue === activeItemValue ? _this.activeItemClass : _this.itemClass;
		}
		return divItem;
	};
	this.drawList = function (action, startPos) {
		_this.refreshElements();

		_this.activeItem = _this.filteredItems[_this.activeItemIndex];

		if (_this.displayItems.length === 0)
			_this.displayItems = _this.filteredItems.slice(0, _this.displayCount);

		if (action === "up" || action === "down") {
			if (!_this.contains(_this.displayItems, _this.activeItem)) {
				if (action === "down") {
					_this.displayItems.push(_this.activeItem);
					_this.displayItems.shift();
					_this.scrollbar.scrollByRow(1);
				}
				else if (action === "up") {
					_this.displayItems.unshift(_this.activeItem);
					_this.displayItems.pop();
					_this.scrollbar.scrollByRow(-1);
				}
			}
			if (_this.activeItemIndex === _this.filteredItems.length - 1)
				_this.scrollbar.scrollToBottom();
			else if (_this.activeItemIndex === 0)
				_this.scrollbar.scrollToTop();
		}
		else if (action === "reset") {
			_this.displayItems = _this.filteredItems.slice(0, _this.displayCount);
			if (_this.scrollbar) {
				setTimeout(function () {
					_this.scrollbar.reset(_this.filteredItems.length, _this.eleListContainer.offsetHeight);
				}, 50);
			}
		}
		else if (action === "scroll") {
			_this.displayItems = _this.filteredItems.slice(startPos, startPos + _this.displayCount);
		}

		var displayItemsElements = _this.makeList();

		while (_this.eleListContainer.firstChild)	// clear elements
			_this.eleListContainer.removeChild(_this.eleListContainer.firstChild);

		var divMeasureStringWidth = document.getElementById("divMeasureStringWidth");

		for (var i = 0; i < displayItemsElements.length; i++) // add elements
		{
			_this.eleListContainer.appendChild(displayItemsElements[i]);

			var ele2 = document.createElement("div");
			ele2.id = "";
			ele2.innerHTML = displayItemsElements[i].innerHTML;
			divMeasureStringWidth.appendChild(ele2);
		}

		// Measure 
		if (divMeasureStringWidth.clientWidth > _this.eleListBox.clientWidth && divMeasureStringWidth.clientWidth > this.minWidth) {
			var width = divMeasureStringWidth.clientWidth + 15;
			if (this.eleListBox)
				this.eleListBox.style.width = width + "px";
			if (this.eleListContainer)
				this.eleListContainer.style.width = (width - 17) + "px";
		}
		divMeasureStringWidth.innerHTML = "";

		if (_this.filteredItems.length === 0)
			_this.eleListBox.style.display = "none";
		else
			_this.eleListBox.style.display = "block";
	};
	this.makeDivider = function () {
		var divider = document.createElement("div");
		divider.className = "hesabfa-combobox-divider";
		return divider;
	};
	this.hideList = function () {
		_this.eleListBox.style.display = "none";
	};



	// helper methods
	this.getValueByPlaceholder = function (item, placeholder) {
		var properties = placeholder.split(".");
		var value = item;
		if (properties.length > 1) {
			for (var i = 1; i < properties.length; i++)
				value = value[properties[i]];
		}
		return value;
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
		for (var i = 0; i < this.items.length; i++) {
			if (contains(this.items[i], this.searchBy, value))
				filteredData.push(this.items[i]);
		}
		return filteredData;
	};
	this.contains = function (arr, item) {
		var value = _this.getValueByPlaceholder(item, _this.matchBy);
		for (var i = 0; i < arr.length; i++) {
			if (_this.matchBy === "item" && arr[i] === item)
				return true;
			else {
				var iValue = _this.getValueByPlaceholder(arr[i], _this.matchBy);
				if (iValue === value)
					return true;
			}
		}
		return false;
	};
	this.getTargetElement = function (e) {
		var targ = null;
		if (e.target) targ = e.target;
		else if (e.srcElement) targ = e.srcElement;
		if (targ && targ.nodeType === 3) // defeat Safari bug
			targ = targ.parentNode;
		return targ;
	};

	this.createElements = function () {
		var divInputGroup = document.createElement("div");
		divInputGroup.className = "input-group";

		var input = document.createElement("input");
		input.type = "text";
		input.className = this.inputClass;
		input.placeholder = "جستجو و انتخاب از لیست...";
		input.ishesabfacombobox = true;
		input.autocomplete = "off";
		if (!this.searchable)
			input.readOnly = true;
		this.eleInput = input;

		var divToggleBtn = document.createElement("div");
		divToggleBtn.className = "input-group-addon hand";
		divToggleBtn.ishesabfacombobox = true;
		divToggleBtn.onclick = this.toggleClick;
		var spanIconToggleBtn = document.createElement("span");
		spanIconToggleBtn.className = "fa fa-chevron-down";
		spanIconToggleBtn.ishesabfacombobox = true;

		var divNewBtn = document.createElement("div");
		divNewBtn.className = "input-group-addon hand";
		divNewBtn.ishesabfacombobox = true;
		divNewBtn.onclick = this.newClick;
		var spanIconNewBtn = document.createElement("span");
		spanIconNewBtn.className = "fa fa-plus";
		spanIconNewBtn.ishesabfacombobox = true;

		var divDeleteBtn = document.createElement("div");
		divDeleteBtn.className = "input-group-addon hand";
		divDeleteBtn.ishesabfacombobox = true;
		divDeleteBtn.onclick = this.deleteClick;
		var spanIconDeleteBtn = document.createElement("span");
		spanIconDeleteBtn.className = "fa fa-remove";
		spanIconDeleteBtn.ishesabfacombobox = true;

		var width = this.containerEle.parentElement.offsetWidth;
		if (width < this.minWidth)
			width = this.minWidth;

		this.eleListBox = document.createElement("div");
		this.eleListBox.className = "hesabfa-combobox-listbox noselect list-group";
		//		this.eleListBox.style.width = _this.containerEle.offsetWidth + "px";
		this.eleListBox.style.width = width + "px";
		this.eleListBox.style.display = "none";

		this.eleListContainer = document.createElement("div");
		this.eleListContainer.style.display = "inline-block";
		this.eleListContainer.style.width = (width - 17) + "px";
		//		this.eleListContainer.style.width = (_this.containerEle.offsetWidth - 17) + "px";
		this.eleListContainer.style.float = "left";
		this.eleListContainer.style.padding = "5px";
		this.eleScrollbar = document.createElement("div");
		this.eleScrollbar.style.display = "inline-block";
		this.eleScrollbar.style.width = "15px";
		this.eleScrollbar.style.float = "right";

		var divMeasureStringWidth = document.getElementById("divMeasureStringWidth");
		if (!divMeasureStringWidth) {
			divMeasureStringWidth = document.createElement("div");
			divMeasureStringWidth.id = "divMeasureStringWidth";
			document.body.appendChild(divMeasureStringWidth);
		} else
			divMeasureStringWidth.innerHTML = "";

		divInputGroup.appendChild(input);
		if (this.toggleBtn)
			divInputGroup.appendChild(divToggleBtn);
		if (this.newBtn)
			divInputGroup.appendChild(divNewBtn);
		if (this.deleteBtn)
			divInputGroup.appendChild(divDeleteBtn);

		divToggleBtn.appendChild(spanIconToggleBtn);
		divNewBtn.appendChild(spanIconNewBtn);
		divDeleteBtn.appendChild(spanIconDeleteBtn);

		//		_this.eleScrollbar.appendChild(_this.eleScrollbarPointer);
		this.eleListBox.appendChild(this.eleScrollbar);
		this.eleListBox.appendChild(this.eleListContainer);

		this.containerEle.appendChild(divInputGroup);
		this.containerEle.appendChild(this.eleListBox);

		this.bindEvents(input);

		if (this.hasScrollbar) {
			this.scrollbar = new HesabfaScrollbar({
				totalRows: this.filteredItems.length,
				displayRows: this.displayCount,
				eleScrollbar: this.eleScrollbar,
				elePanelToScroll: this.eleListContainer,
				onScroll: function (startPos) {
					_this.drawList("scroll", startPos);
				}
			});
		}
	};
	this.refreshElements = function () {
		var width = _this.containerEle.parentElement.offsetWidth;
		if (width < _this.minWidth)
			width = _this.minWidth;
		_this.eleListBox.style.width = width + "px";
		_this.eleListContainer.style.width = (width - 17) + "px";
	};
	this.bindEvents = function (element) {
		var bindKeydownEvents = function (e) {
			e.stopPropagation();
			if (e.which === _this.keys.down) {
				if (_this.filteredItems.length === 0 || _this.filteredItems.length === 1)
					return false;
				if (_this.activeItemIndex === _this.filteredItems.length - 1)
					return false;
				_this.activeItemIndex++;
				_this.drawList("down");
				return false; // stops the page from scrolling
			}
			if (e.which === _this.keys.up) {
				if (_this.filteredItems.length === 0 || _this.filteredItems.length === 1)
					return false;
				if (_this.activeItemIndex === 0)
					return false;
				_this.activeItemIndex--;
				_this.drawList("up");
				return false;
			}
			if (e.which === _this.keys.tab) {
				_this.hideList();
			}
			if (e.which === _this.keys.enter) {
				if (_this.filteredItems.length > 0)
					_this.itemSelect(_this.filteredItems[_this.activeItemIndex], "enter");
			}
			return false;
		};
		var bindOnchangeEvent = function (e) {
			e.stopPropagation();

			setTimeout(function () {

				if (e.which === _this.keys.tab || e.which === _this.keys.enter)
					return false;

				var value = e.target.value;
				if (_this.preValue == undefined)
					_this.preValue = value;
				else if (_this.preValue === value)
					return false;
				else
					_this.preValue = value;

				clearTimeout(_this.searchTimeOut);
				_this.searchTimeOut = setTimeout(function () {
					if (value === "") {
						_this.filteredItems = _this.items;
						_this.activeItemIndex = 0;
						_this.drawList("reset");
						return false;
					}
					_this.isOpen = true;
					_this.filteredItems = _this.filterData(value);
					_this.activeItemIndex = 0;
					_this.drawList("reset");
					return false;
				}, 300);
				return false;

			}, 50);

			return false;
		};
		var bindBlurEvent = function () {
			//			_this.hideList();
		};
		var clickOutsideOfListBox = function (e) {
			var target = _this.getTargetElement(e);
			if (target.ishesabfacombobox) return;
			if (target.parentElement && target.parentElement.ishesabfacombobox) return;
			_this.hideList();
		};
		var clickListBox = function (e) {
			e.stopPropagation();
			var targetEle = _this.getTargetElement(e);
			var index = targetEle.parentElement.tag;
			if (_this.displayItems.length >= index) {
				var item = _this.displayItems[index];
				_this.itemSelect(item, "click");
			}
			return false;
		}

		element.addEventListener("keydown", bindKeydownEvents);
		element.addEventListener("keydown", bindOnchangeEvent);
		element.addEventListener("blur", bindBlurEvent);
		_this.eleListBox.addEventListener("click", clickListBox);
		_this.eleListBox.addEventListener("touchend", clickListBox);
		window.addEventListener("click", clickOutsideOfListBox);
		//		element.bind('keydown', bindKeydownEvents);
		//		element.change(bindOnchangeEvent);
		//		element.blur(bindBlurEvent);
	};
	this.itemSelect = function (item, selectBy) {
		this.setSelected(item);
		this.hideList();
		if (this.onSelect)
			this.onSelect(item, selectBy, this.callbackObj);
	};
	this.toggleClick = function () {
		if (_this.eleListBox.style.display === "none") {
			_this.filteredItems = _this.items;
			_this.activeItemIndex = 0;
			_this.drawList("reset");
		}
		else
			_this.eleListBox.style.display = "none";
	};
	this.newClick = function () {
		_this.hideList();
		if (_this.onNew)
			_this.onNew();
	};
	this.deleteClick = function () {
		_this.hideList();
		_this.selected = null;
		_this.eleInput.value = "";
		if (_this.onDelete)
			_this.onDelete();
    };

    this.setDelete = function () {
        _this.hideList();
        _this.selected = null;
        _this.eleInput.value = "";
        if (_this.onDelete)
            _this.onDelete();
    };

	this.setSelected = function (item) {
		if (!item) {
			this.selected = item;
			this.eleInput.value = "";
			return;
		}
		this.selected = item;
		var value = parseTemplate(item, this.displayProperty);
		this.eleInput.value = value;
	};
	this.getSelected = function () {
		return this.selected;
	};
	this.focus = function() {
		_this.eleInput.focus();
	};

	this.createElements();
	this.setSelected(this.selected);
}