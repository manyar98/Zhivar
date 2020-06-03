function rowEditor(params) {
	this.controls = params.controls;
	this.itemArray = params.itemArray;
	this.activeRow = null;
	this.inputText = "";
	this.elementsContainerId = params.elementsContainerId || "hideme";
	this.onMove = params.onMove || null;
	this.isHide = true;
	var _this = this;

	this.move = function (index,controlIndex) {
		if (_this.itemArray.length === 0) {
			_this.hide();
			alert(_this.itemArray);
			return;
		}
		if (_this.activeRow && _this.activeRow.rowIndex === index && !_this.isHide) return;
		if (_this.itemArray[index].disabled) return; // new
		_this.activeRow = _this.itemArray[index];
		_this.isHide = false;
		for (var i = 0; i < _this.controls.length; i++) {
			var control = _this.controls[i];
			var ele = document.getElementById(control.id);
			$(ele).hide();
		}

		setTimeout(function () {
			for (var i = 0; i < _this.controls.length; i++) {
				var control = _this.controls[i];
				var ele = document.getElementById(control.id);
				document.getElementById(control.element + index).appendChild(ele);
				$(ele).show();
			}
			if (_this.onMove)
				_this.onMove();
			_this.focus(controlIndex);
		}, 0);
	};
	this.moveUp = function (actRow, event) {
		if (_this.activeRow.rowIndex > 1) {
			event.preventDefault();
			_this.move(actRow.rowIndex - 2);
			event.target.select();
		}
	};
	this.moveDown = function (actRow, event) {
		if (actRow.rowIndex < itemArray.length) {
			event.preventDefault();
			_this.move(actRow.rowIndex);
			event.target.select();
		}
	};
	this.hide = function () {
		_this.isHide = true;
		for (var i = 0; i < _this.controls.length; i++) {
			var control = _this.controls[i];
			$("#" + control.id).prependTo("#" + _this.elementsContainerId);
		}
	};
	this.itemDeleted = function (index) {
		if (_this.itemArray.length === 0) { }
		else if (_this.itemArray.length === 1) {
			_this.move(0);
		}
		else if (_this.activeRow.rowIndex === index) {
			if (index > _this.itemArray.length - 1)
				_this.move(index - 1);
			else
				_this.move(index);
		} else {
			_this.move(_this.activeRow.rowIndex);
		}
	};
	this.focus = function(controlIndex) {
		if (controlIndex != undefined) {
			if (_this.controls[controlIndex].onfocus)
				_this.controls[controlIndex].onfocus();
			else if (_this.controls[controlIndex].focus) {
				var inputElement = document.getElementById(_this.controls[controlIndex].focus);
				inputElement.focus();
				if (inputElement.value === "0")
					inputElement.select();
			}
		}
	};
	//	if (_this.itemArray.length > 0)
	//		this.move(activeRowIndex);
	return this;
};
function rowEditorRow(controlId, elementId) {
	return { id: controlId, element: elementId };
};
