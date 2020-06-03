function HesabfaScrollbar(params) {
	this.totalRows = params.totalRows;
	this.displayRows = params.displayRows;
	this.eleScrollbar = params.eleScrollbar;
	this.eleThumb = null;
	this.elePanelToScroll = params.elePanelToScroll;
	this.eleScrollbarClass = params.eleThumbClass || "hesabfa-scrollbar";
	this.eleThumbClass = params.eleThumbClass || "hesabfa-scrollbar-thumb";
	this.onScroll = params.onScroll || null;

	this.scrollbarHeight = 0;
	this.thumbHeight = 0;
	this.thumbHeightPlus = 0;
	this.itemHeight = 0;
	this.thumbTop = 0;

	var _this = this;

	this.scrollByRow = function (nRow) {
		_this.eleThumb.style.position = 'relative';
		//		var thumbTop = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;
		this.thumbTop += (nRow * _this.itemHeight);
		_this.eleThumb.style.top = this.thumbTop + "px";
	};
	this.scrollToTop = function () {
		_this.eleThumb.style.position = 'relative';
		_this.eleThumb.style.top = "0px";
	};
	this.scrollToBottom = function () {
		_this.eleThumb.style.position = 'relative';
		_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
	};
	this.calculateDisplayStart = function () {
		//		var top = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;
		var top = this.thumbTop;
		var distanceToTop = top / _this.itemHeight;
		var startRow = Math.round(distanceToTop);
		return startRow;
	};

	this.calculateHeightOfThumb = function () {
		if (_this.totalRows <= _this.displayRows) {
			_this.thumbHeight = _this.scrollbarHeight;
			if (_this.eleThumb)
				_this.eleThumb.style.display = "none";
		} else {
			_this.thumbHeight = _this.scrollbarHeight / (_this.totalRows / _this.displayRows);
			if (_this.eleThumb)
				_this.eleThumb.style.display = "block";
		}
		if (_this.thumbHeight < 10) {	// new
			_this.thumbHeightPlus = 10 - _this.thumbHeight;
			_this.thumbHeight = 10;
		}
	};
	this.calculateItemHeight = function () {
		_this.itemHeight = (_this.scrollbarHeight - _this.thumbHeight) / (_this.totalRows - _this.displayRows);
	};

	this.createThumbElement = function () {
		var divThumb = document.createElement("div");
		divThumb.style.height = _this.thumbHeight + "px";
		divThumb.style.position = "static";
		divThumb.style.top = "0px";
		divThumb.className = _this.eleThumbClass;
		_this.eleThumb = divThumb;

		_this.eleScrollbar.className = _this.eleScrollbarClass;
		_this.eleScrollbar.appendChild(divThumb);
	};
	this.bindDragEvent = function () {
		_this.eleThumb.onmousedown = function (e) {
			_this.thumbOnMousedownFired = true;
			e = e || window.event;
			var start = 0, diff = 0;
			if (e.pageY) start = e.pageY;
			else if (e.clientY) start = e.clientY;

			//			_this.currentPosition = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;
			_this.currentPosition = _this.thumbTop;
			_this.eleThumb.style.position = 'relative';
			document.body.onmousemove = function (e) {
				e = e || window.event;
				var end = 0;
				if (e.pageY) end = e.pageY;
				else if (e.clientY) end = e.clientY;

				diff = end - start;
				if (_this.currentPosition)
					diff += _this.currentPosition;

				_this.eleThumb.style.top = diff + "px";
				_this.thumbTop = diff;

				//				var thumbTop = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;
				if (_this.thumbTop <= 0) {
					_this.eleThumb.style.top = "0px";
					_this.thumbTop = 0;
				}
				else if (_this.thumbTop + _this.thumbHeight >= _this.scrollbarHeight) {
					_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
					_this.thumbTop = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;
				}
				var displayStart = _this.calculateDisplayStart();

				if (_this.onScroll)
					_this.onScroll(displayStart);
			};
			document.body.onmouseup = function () {
				//				_this.eleThumb.style.position = 'static';
				_this.currentPosition = diff;
				document.body.onmousemove = document.body.onmouseup = null;
				_this.thumbOnMousedownFired = false;
			};
		}
		_this.eleScrollbar.onclick = function (e) {
			if (_this.totalRows <= _this.displayRows)
				return false;

			_this.eleThumb.style.position = 'relative';
			var thumbOffset = _this.offset(_this.eleThumb);
			if (e.clientY >= thumbOffset.top && e.clientY <= (thumbOffset.top + _this.thumbHeight))
				return false;
			//			var thumbTop = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;

			if (e.clientY < thumbOffset.top) {
				// top clicked
				_this.thumbTop += (_this.displayRows * -1) * _this.itemHeight;
				_this.eleThumb.style.top = _this.thumbTop + "px";

				if (_this.thumbTop <= 0) {
					_this.eleThumb.style.top = "0px";
					_this.thumbTop = 0;
				}
				else if (_this.thumbTop + _this.thumbHeight >= _this.scrollbarHeight) {
					_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
					_this.thumbTop = (_this.scrollbarHeight - _this.thumbHeight);
				}
			} else {
				// bottom clicked
				_this.thumbTop += _this.displayRows * _this.itemHeight;
				_this.eleThumb.style.top = _this.thumbTop + "px";

				if (_this.thumbTop <= 0) {
					_this.eleThumb.style.top = "0px";
					_this.thumbTop = 0;
				}
				else if (_this.thumbTop + _this.thumbHeight >= _this.scrollbarHeight) {
					_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
					_this.thumbTop = (_this.scrollbarHeight - _this.thumbHeight);
				}
			}
			var displayStart = _this.calculateDisplayStart();
			if (_this.onScroll)
				_this.onScroll(displayStart);
			return false;
		};
		_this.eleScrollbar.onmousedown = function (e) {
			if (_this.thumbOnMousedownFired) return;
			_this.scrollbarMouseDownTimeout = setTimeout(function () {
				_this.scrollbarMouseDownInterval = setInterval(function () {
					if (_this.totalRows <= _this.displayRows)
						return false;

					_this.eleThumb.style.position = 'relative';
					var thumbOffset = _this.offset(_this.eleThumb);
					if (e.clientY >= thumbOffset.top && e.clientY <= (thumbOffset.top + _this.thumbHeight))
						return false;
					//			var thumbTop = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;

					if (e.clientY < thumbOffset.top) {
						// top clicked
						_this.thumbTop += (_this.displayRows * -1) * _this.itemHeight;
						_this.eleThumb.style.top = _this.thumbTop + "px";

						if (_this.thumbTop <= 0) {
							_this.eleThumb.style.top = "0px";
							_this.thumbTop = 0;
						}
						else if (_this.thumbTop + _this.thumbHeight >= _this.scrollbarHeight) {
							_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
							_this.thumbTop = (_this.scrollbarHeight - _this.thumbHeight);
						}
					} else {
						// bottom clicked
						_this.thumbTop += _this.displayRows * _this.itemHeight;
						_this.eleThumb.style.top = _this.thumbTop + "px";

						if (_this.thumbTop <= 0) {
							_this.eleThumb.style.top = "0px";
							_this.thumbTop = 0;
						}
						else if (_this.thumbTop + _this.thumbHeight >= _this.scrollbarHeight) {
							_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
							_this.thumbTop = (_this.scrollbarHeight - _this.thumbHeight);
						}
					}
					var displayStart = _this.calculateDisplayStart();
					if (_this.onScroll)
						_this.onScroll(displayStart);
					return false;
				}, 50);
			}, 500);
		};
		_this.eleScrollbar.onmouseup = function (e) {
			if (_this.scrollbarMouseDownTimeout)
				clearTimeout(_this.scrollbarMouseDownTimeout);
			if (_this.scrollbarMouseDownInterval)
				clearInterval(_this.scrollbarMouseDownInterval);
		};
	};
	this.bindWheelEvent = function () {
		if (_this.elePanelToScroll.addEventListener) {
			// IE9, Chrome, Safari, Opera
			_this.elePanelToScroll.addEventListener("mousewheel", onwheelhandler, false);
			// Firefox
			_this.elePanelToScroll.addEventListener("DOMMouseScroll", onwheelhandler, false);
		}
			// IE 6/7/8
		else {
			_this.elePanelToScroll.attachEvent("onmousewheel", onwheelhandler);
		}

		var startY, moving = false;
		$(_this.elePanelToScroll).on("touchstart mousedown", function (e) {
			startY = parseInt((e.type.toLowerCase() === 'mousedown') ? e.pageY : e.originalEvent.changedTouches[0].clientY);
			moving = true;
			e.preventDefault();
		});
		$(_this.elePanelToScroll).on("touchmove mousemove", function (e) {
			e.preventDefault();
			if (moving) {
				var touching = e.type.toLowerCase() === 'touchmove';
				var change = touching ? 4 : 1;
				var y = parseInt(!touching ? e.pageY : e.originalEvent.changedTouches[0].clientY);
				if (Math.abs(startY - y) > 10 * change) {
					onwheelhandler(e, startY < y ? change: -change);
					startY = y;
				}
			}
		});
		$(window).on("mouseup touchend", function (e) {
			moving = false;
		});

		function onwheelhandler(e, custom_delta) {
			e.preventDefault();
			e.stopPropagation();

			if (_this.totalRows <= _this.displayRows)
				return false;
			_this.eleThumb.style.position = 'relative';

			// cross-browser wheel delta
			e = window.event || e; // old IE support
			var delta = Math.max(-1, Math.min(1, (e.wheelDelta || -e.detail)));
			if (custom_delta)
				delta = custom_delta;
			//			var thumbTop = _this.eleThumb.offsetTop - _this.eleThumb.parentElement.offsetTop;

			_this.thumbTop += (delta * -1) * _this.itemHeight;
			_this.eleThumb.style.top = _this.thumbTop + "px";

			if (_this.thumbTop <= 0) {
				_this.eleThumb.style.top = "0px";
				_this.thumbTop = 0;
			}
			else if (_this.thumbTop + _this.thumbHeight >= _this.scrollbarHeight) {
				_this.eleThumb.style.top = (_this.scrollbarHeight - _this.thumbHeight) + "px";
				_this.thumbTop = (_this.scrollbarHeight - _this.thumbHeight);
			}

			var displayStart = _this.calculateDisplayStart();
			if (_this.onScroll)
				_this.onScroll(displayStart);

			return false;
		};
	};

	// helper methods
	this.offset = function (el) {
		var rect = el.getBoundingClientRect(),
			scrollLeft = window.pageXOffset || document.documentElement.scrollLeft,
			scrollTop = window.pageYOffset || document.documentElement.scrollTop;
		return { top: rect.top + scrollTop, left: rect.left + scrollLeft }
	};

	// initialize
	this.initialize = function () {
		_this.scrollbarHeight = _this.eleScrollbar.offsetHeight;
		_this.calculateHeightOfThumb();
		_this.calculateItemHeight();
		_this.createThumbElement();
		_this.bindDragEvent();
		_this.bindWheelEvent();
	};
	this.reset = function (rowsCount, scrollbarHeight) {
		_this.totalRows = rowsCount;
		_this.eleScrollbar.style.height = scrollbarHeight + "px";
		_this.scrollbarHeight = scrollbarHeight;
		_this.calculateHeightOfThumb();
		_this.calculateItemHeight();
		_this.eleThumb.style.height = _this.thumbHeight + "px";
		_this.eleThumb.style.position = "static";
		_this.eleThumb.style.top = "0px";
		_this.thumbTop = 0;
	};
	_this.initialize();
}