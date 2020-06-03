function testApi()
{
    $(function () {
        var id = 0;

        $.ajax({
            type: "POST",
            data: JSON.stringify(id),
            url: "/app/api/FactorApi/loadInvoiceData",
            contentType: "application/json"
        }).done(function (res) {
            return res;
        //console.log('res', res);
        // Do something with the result :)
    });
});
}
function callws(url, parameters) {
	function stringify(o) {
		return JSON.stringify(o, function (key, value) { return value === "" ? "" : value; });
		//return customJSONstringify(o, function (key, value) { return value === "" ? "" : value; });
	};
	//function customJSONstringify(obj) {
	//	return JSON.stringify(obj).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/")
	//}
	function parse(str) {
		var a = url;
		return JSON.parse(str);
	};

	var token = getQueryString("buf");
	if (token)
		parameters["token"] = token;

	var xmlhttp = window.XMLHttpRequest ? new window.XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
	var response = null;
	var page = url;
	var variables = stringify(parameters);

	xmlhttp.open("POST", page, true);
	xmlhttp.setRequestHeader("Content-type", "application/json;charset=utf-8");
	//    xmlhttp.setRequestHeader("Content-length", variables.length);
	//    xmlhttp.setRequestHeader("Connection", "close");

	var r = {};
	r.success = function (fn) {
		r.successFn = fn;
		return r;
	};
	r.fail = function (fn) {
		r.failFn = fn;
		return r;
	};
	r.loginFail = function (fn) {
		r.loginFailFn = fn;
		return r;
	};
	r.doSuccuss = function (result) {
		if (this.successFn)
			this.successFn(result);
	};
	r.doFail = function (error) {
		if (this.failFn)
			this.failFn(error);
	};
	r.doLoginFailed = function () {
		if (this.loginFailFn)
			this.loginFailFn();
	};
	xmlhttp.onreadystatechange = function () {
		if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
			response = xmlhttp.responseText;
			var d = parse(response);
			if (d.d[0] == '{') {
				d = parse(d.d);
				if (d.loginFailed)
					r.doLoginFailed();
				else if (d.error)
					r.doFail(d.error);
				else if (d.success)
					r.doSuccuss(d.result);

			}
			else {
				LZMA_decompress(d.d, function (result) {
					if (result.loginFailed)
						r.doLoginFailed();
					else if (result.error)
						r.doFail(result.error);
					else if (result.success)
						r.doSuccuss(result.result);
				});
			}
		}
		else if (xmlhttp.readyState == 4) {
			r.doFail(xmlhttp.statusText);
		}
	};
	xmlhttp.send(variables);

	return r;
};
function callwssync(url, parameters) {
	function stringify(o) {
		//        return JSON.stringify(o, function (key, value) { return value === "" ? "" : value; });
		return customJSONstringify(o, function (key, value) { return value === "" ? "" : value; });
	};
	function customJSONstringify(obj) {
		return JSON.stringify(obj).replace(/\/Date/g, "\\\/Date").replace(/\)\//g, "\)\\\/");
	}
	function parse(str) {
		return JSON.parse(str);
	};

	var xmlhttp = window.XMLHttpRequest ? new window.XMLHttpRequest() : new ActiveXObject("Microsoft.XMLHTTP");
	var response;
	var page = url;
	var variables = stringify(parameters);

	xmlhttp.open("POST", page, false);
	xmlhttp.setRequestHeader("Content-type", "application/json;charset=utf-8");
	//    xmlhttp.setRequestHeader("Content-length", variables.length);
	//    xmlhttp.setRequestHeader("Connection", "close");


	xmlhttp.send(variables);
	var ret = { loginFailed: false, error: '', success: false, result: null };
	if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
		response = xmlhttp.responseText;
		var d = parse(response);
		if (d.d[0] == '{') {
			d = parse(d.d);
			ret = d;
		} else {
			alert("USE ASYNC!");
		}
	}
	else if (xmlhttp.readyState == 4) {
		ret = { loginFailed: false, error: xmlhttp.statusText, success: false, result: null };
	}

	return ret;
};
function isValueValid(type, value, range, isNumericRange) {
	var letters, i, parts;
	switch (type) {
		case "integer":
			return /^\d+$/.test(value);
		case "signed-integer":
			return /^[\-\+]?\d+$/.test(value);
		case "float":
			return /^\d*\.?\d+$/.test(value);
		case "signed-float":
			return /^[\-\+]?\d*\.?\d+$/.test(value);
		case "farsi-letter":
			letters = " آابپتثجچحخدذرزژسشصضطظعغفقکكگلمنوهیىئءيـأإؤي‌‍ةۀّ";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "farsi-letter-number":
			letters = " آابپتثجچحخدذرزژسشصضطظعغفقکكگلمنوهیىئءيأإؤي‌‍ةۀ0123456789۰۱۲۳۴۵۶۷۸۹٠١٢٣٤٥٦٧٨٩ـّ";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "farsi-letter-number-punctuation":
			letters = " آابپتثجچحخدذرزژسشصضطظعغفقکكگلمنوهیىئءيأإؤي‌‍ةۀ0123456789۰۱۲۳۴۵۶۷۸۹٠١٢٣٤٥٦٧٨٩-_()[]{}.…،؛,:ـ!؟+=*%$#@~|^/ّ";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "english-letter":
			letters = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "english-letter-number":
			letters = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "english-letter-number-punctuation":
			letters = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789`!@#$%^*()-=_+~[]{};,.…/:?|";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "valid-charachter":
			letters = " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789۰۱۲۳۴۵۶۷۸۹٠١٢٣٤٥٦٧٨٩`!@#$%^*()-=_+~[]{};,.…/:?| آابپتثجچحخدذرزژسشصضطظعغفقکكگلمنوهیىئءيأإؤي‌‍ةۀ_،؛ـ؟\r\n\tّ";
			for (i = 0; i < value.length; i++) {
				if (letters.indexOf(value[i]) == -1)
					return false;
			}
			return true;
		case "range":
			parts = range.split(',');
			if (parts.length != 2)
				return true;

			if (isNumericRange) {
				parts[0] = parts[0] == "" ? -Infinity : Number(parts[0]);
				parts[1] = parts[1] == "" ? Infinity : Number(parts[1]);
			}

			if (parts[0] !== '' && value < parts[0])
				return false;
			if (parts[1] !== '' && value > parts[1])
				return false;
			return true;
		case "mobile":
			return /^09\d{9}$/.test(value);
		case "phone":
			return /^0\d{10}$/.test(value);
		case "postal-code":
			return /^\d{10}$/.test(value);
		case "national-code":
			if (!/^\d{10}$/.test(value))
				return false;

			var c = Number(value[9]);
			var n = Number(value[0]) * 10 +
						Number(value[1]) * 9 +
						Number(value[2]) * 8 +
						Number(value[3]) * 7 +
						Number(value[4]) * 6 +
						Number(value[5]) * 5 +
						Number(value[6]) * 4 +
						Number(value[7]) * 3 +
						Number(value[8]) * 2;

			var r = n % 11;
			var b = (r == 0 && r == c) || (r == 1 && c == 1) || (r > 1 && c == 11 - r);
			if (!b)
				return false;
			return true;
		case "farsi-date":
			if (!/^\d{4}\/\d{2}\/\d{2}$/.test(value))
				return false;
			parts = value.split('/');
			var m = Number(parts[1]);
			var d = Number(parts[2]);
			if (m == 0 || d == 0)
				return false;
			if (m > 12)
				return false;
			if (m > 6 && d > 30)
				return false;
			if (d > 31)
				return false;
			return true;
		case "time":
			if (!/^\d{2}:\d{2}:\d{2}$/.test(value) && !/^\d{2}:\d{2}$/.test(value))
				return false;
			parts = value.split(':');
			var hh = Number(parts[0]);
			var mm = Number(parts[1]);
			var ss = parts.length == 3 ? Number(parts[2]) : 0;
			if (hh < 0 || mm < 0 || ss < 0 || hh > 23 || mm > 59 || ss > 59)
				return false;
			return true;
		case "password":
			if (value.length < 6) {
				return false;
			} else {
				var charCount = 0;
				var numCount = 0;
				for (i = 0; i < value.length; i++) {
					if (value[i] >= 0 && value[i] <= 9)
						numCount++;
					else
						charCount++;
				}
				if (charCount == 0 || numCount == 0)
					return false;
			}
			return true;
		case "email":
			return (/^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i.test(value));
		default:
			return true;
	}
}
function attachKeyboardFilter(type, elm, ctrl) {
	$(elm).keydown(function (e) {
		if (!e.char) {
			if (e.shiftKey && e.keyCode === 53)
				e.char = "%";
			if (e.shiftKey && e.keyCode === 54)
				e.char = "^";
			if ((!e.shiftKey && e.keyCode === 173) || e.keyCode === 109 || e.keyCode === 189)
				e.char = "-";
			if ((e.shiftKey && e.keyCode === 61) || e.keyCode === 107 || e.keyCode === 187)
				e.char = "+";
			if (e.keyCode === 46 || e.keyCode === 190)
				e.char = ".";
			if (e.keyCode === 111 || e.keyCode === 191)
				e.char = "/";
			if ((e.shiftKey && e.keyCode === 56) || e.keyCode === 106)
				e.char = "*";
			if ((e.shiftKey && e.keyCode === 57))
				e.char = "(";
			if ((e.shiftKey && e.keyCode === 58))
				e.char = ")";
		}
		// Allow: backspace, delete, tab, escape, enter and F5.
		if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 116]) !== -1 ||
			// Allow: Ctrl+A, C, V, X
            ((e.keyCode == 65 || e.keyCode == 88 || e.keyCode == 67 || e.keyCode == 86) && e.ctrlKey === true) ||
			// Allow: home, end, left, right, down, up
            (e.keyCode >= 35 && e.keyCode <= 40)) {
			// let it happen, don't do anything
			return;
		}
		var val = elm.val();
		var isNumber = (!e.shiftKey && e.keyCode >= 48 && e.keyCode <= 57) || (e.keyCode >= 96 && e.keyCode <= 105);
		var isPercent = e.char === "%";
		var isSymbol = $.inArray(e.char, ["^", "-", "+", "/", "*", "(", ")"]) !== -1;
		var insertChar = isNumber;
		var containsPercent = elm.val().indexOf("%") > -1;
		var containsSymbol = false;
		for (var i = 0; i < "+-*/()^".length; i++) {
			if (val.indexOf("+-*/()^"[i]) > -1) {
				containsSymbol = true;
				break;
			}
		}


		if (isPercent) {
			if (containsSymbol) {
				insertChar = false;
			} else {
				if (containsPercent)
					val = val.replace(/%/g, "");
				else
					val = "%" + val;
				ctrl.$setViewValue(val);
				ctrl.$render(val);
				insertChar = false;
			}
		}
		else if (containsPercent) {
			val = "%" + val.replace(/%/g, "");
			ctrl.$setViewValue(val);
			ctrl.$render(val);
			insertChar = isNumber;
		} else {
			insertChar = isNumber || isSymbol;
		}

		if (!insertChar)
			e.preventDefault();

	});
	$(elm).blur(function (e) {
		var val = elm.val();
		val = val.replace(/\,/g, '');
		val = val.replace(/\،/g, '');
		try {
			val = formatToCurrency(Parser.evaluate(val));
		} catch (e) {

		}
		ctrl.$setViewValue(val);
		ctrl.$render(val);
	});
	var beforePasteValue;
	$(elm).bind("paste", function () {
		beforePasteValue = $(elm).val();
		setTimeout(function () {
			if (ctrl.$invalid) {
				$(elm).val(beforePasteValue);
				ctrl.$setViewValue(beforePasteValue);
				ctrl.$validate();
			}
		}, 1);
	});
}
function messagebox(args) {
	if (!args)
		args = {};
	var title = args.title || "توجه";
	var content = args.content || "";
	var buttonCount = args.buttonCount || 1;
	var btn1Title = args.btn1Title || "دکمه 1";
	var btn2Title = args.btn2Title || "دکمه 2";
	var btn3Title = args.btn2Title || "دکمه 3";
	var onBtn1Click = args.onBtn1Click || function () { closeModal(); };
	var onBtn2Click = args.onBtn2Click || function () { closeModal(); };
	var onBtn3Click = args.onBtn3Click || function () { closeModal(); };
	var btn1Class = args.btn1Class || "btn-primary";
	var btn2Class = args.btn2Class || "btn-default";
	var btn3Class = args.btn3Class || "btn-default";
	var onclose = args.onclose || null;
	var type = args.type || "info";

	var modalClass = "";
	var icon = "";
	if (type === "warning") {
		modalClass = "modal-header-warning";
		btn1Class = "btn-default btn-smoke";
		title = args.title || "هشدار";
		icon = "<span class='fa fa-warning fa-lg'></span>&nbsp&nbsp";
	}
	else if (type === "error") {
		modalClass = "modal-header-error";
		btn1Class = "btn-default btn-smoke";
		title = args.title || "خطا";
		icon = "<span class='fa fa-times-circle fa-lg'></span>&nbsp&nbsp";
	}
	else if (type === "question") {
	    modalClass = "modal-header-info";
		icon = "<span class='fa fa-exclamation-circle fa-lg'></span>&nbsp&nbsp";
	}
	else {
		modalClass = "modal-header-info";
		btn1Class = "btn-default btn-smoke";
		icon = "<span class='fa fa-exclamation-circle fa-lg'></span>&nbsp&nbsp";
	}

	var html = "<div class='modal-dialog'>" +
		"<div class='modal-content'><div class='modal-header " + modalClass + "'>";
	html += "<button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
    html += "<h4 class='modal-title'>" + icon + title + "</h4></div><div class='modal-body'><p>" + content + "</p><br/></div>";
	html += "<div class='modal-footer'>";
	if (buttonCount >= 1)
		html += "<button type='button' data-buttonName='button1' class='btn " + btn1Class + " btn-md' data-dismiss='modal' style='min-width:80px; margin-left:5px;'>" + btn1Title + "</button>";
	if (buttonCount >= 2)
		html += "<button type='button' data-buttonName='button2' class='btn " + btn2Class + " btn-md' data-dismiss='modal' style='min-width:80px; margin-left:5px;'>" + btn2Title + "</button>";
	if (buttonCount >= 3)
		html += "<button type='button' data-buttonName='button3' class='btn " + btn3Class + " btn-md' data-dismiss='modal' style='min-width:80px; margin-left:5px;'>" + btn3Title + "</button>";
	html += "</div></div></div>";

	var div;
	var fireClose = true;
	function show() {
		div = document.createElement('div');
		div.className = "modal fade";
		div.innerHTML = html;
		document.body.appendChild(div);
		$(div).modal();
		$(div).on('hidden.bs.modal', function () {
			if (onclose && fireClose)
				onclose();
			div.parentNode.removeChild(div);
		});
		$(div).find("[data-buttonName='button1']").on('click', function () { fireClose = false; if (onBtn1Click) onBtn1Click(); });
		$(div).find("[data-buttonName='button2']").on('click', function () { fireClose = false; if (onBtn2Click) onBtn2Click(); });
		$(div).find("[data-buttonName='button3']").on('click', function () { fireClose = false; if (onBtn3Click) onBtn3Click(); });
		$(".modal-dialog", $(div)).draggable({ handle: ".modal-header" });
	}

	show();
	function closeModal() {
		fireClose = true;
		$(div).modal('hide');
		
	};
}
function alertbox(args) {
	if (!args)
		args = {};
	messagebox({
		title: args.title || "توجه",
		content: args.content || "",
		buttonCount: 1,
		btn1Title: args.btn1Title || "تایید",
		onBtn1Click: args.onBtn1Click,
		btn1Class: args.btn1Class || "btn-primary",
		onclose: args.onclose,
		type: args.type || null
	});
}
function questionbox(args) {
	if (!args)
		args = {};
	messagebox({
		title: args.title || "توجه",
		content: args.content || "",
		buttonCount: 2,
		btn1Title: args.btn1Title || "بله",
		btn2Title: args.btn2Title || "خیر",
		onBtn1Click: args.onBtn1Click,
		onBtn2Click: args.onBtn2Click,
		btn1Class: args.btn1Class || "btn-success",
		btn2Class: args.btn2Class || "btn-smoke",
		onclose: args.onclose,
		type: args.type || null
//		type:'question'
	});
}
function questioncancelbox(args) {
	if (!args)
		args = {};
	messagebox({
		title: args.title || "توجه",
		content: "<p style='margin-bottom:30px'>" + (args.content || "") + "</p>",
		buttonCount: 3,
		btn1Title: args.btn1Title || "بله",
		btn2Title: args.btn2Title || "خیر",
		btn3Title: args.btn3Title || "انصراف",
		onBtn1Click: args.onBtn1Click,
		onBtn2Click: args.onBtn2Click,
		onBtn3Click: args.onBtn3Click,
		btn1Class: args.btn1Class || "btn-success",
		btn2Class: args.btn2Class || "btn-danger",
		btn3Class: args.btn3Class || "btn-default",
		onclose: args.onclose
	});
}
function notification(text, delaySecond, color) {
	if (!text) return;
	delaySecond = !delaySecond ? 2000 : delaySecond * 1000;
	if (!color) color = "green";
	else if (color !== "green" && color !== "red" && color !== "blue") color = "blue";
	var div = document.createElement('div');
	div.className = "notification";
	if (color === "green") div.style.backgroundColor = 'rgba(0, 116, 0, 0.7)';
	if (color === "red") div.style.backgroundColor = 'rgba(116, 0, 0, 0.7)';
	if (color === "blue") div.style.backgroundColor = 'rgba(0, 56, 168, 0.7)';
	div.innerHTML = text;
	document.body.appendChild(div);
	$(div).fadeIn();
	setTimeout(function () {
		$(div).fadeOut();
	}, delaySecond);
};

function showModal(args) {
	if (!args)
		args = {};
	var title = args.title || "توجه";
	var content = document.getElementById(args.contentId);
	var buttonCount = args.buttonCount || 1;
	var btn1Title = args.btn1Title || "دکمه 1";
	var btn2Title = args.btn2Title || "دکمه 2";
	var btn3Title = args.btn2Title || "دکمه 3";
	var onBtn1Click = args.onBtn1Click || function () { closeModal(); };
	var onBtn2Click = args.onBtn2Click || function () { closeModal(); };
	var onBtn3Click = args.onBtn3Click || function () { closeModal(); };
	var btn1Class = args.btn1Class || "btn-primary";
	var btn2Class = args.btn2Class || "btn-default";
	var btn3Class = args.btn3Class || "btn-default";
	var onclose = args.onclose || null;
	var onshow = args.onshow || null;

	var contentParent = content.parentNode;

	var html = "<div class='modal-dialog modal-lg'><div class='modal-content'><div class='modal-header'>";
	html += "<button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>";
	html += "<h4 class='modal-title'>" + title + "</h4></div><div class='modal-body'></div>";
	html += "<div class='modal-footer'>";
	if (buttonCount >= 1)
		html += "<button type='button' data-buttonName='button1' class='btn " + btn1Class + "'>" + btn1Title + "</button>";
	if (buttonCount >= 2)
		html += "<button type='button' data-buttonName='button2' class='btn " + btn2Class + "'>" + btn2Title + "</button>";
	if (buttonCount >= 3)
		html += "<button type='button' data-buttonName='button3' class='btn " + btn3Class + "'>" + btn3Title + "</button>";
	html += "</div></div></div>";

	var dlg = {};
	dlg.close = function () {
		closeModal();
	};

	var div;
	var fireClose = true;
	function show() {
		div = document.createElement('div');
		div.className = "modal fade";
		div.innerHTML = html;
		document.body.appendChild(div);
		$(div).modal({ backdrop: 'static', keyboard: false });
		$(div).on('hidden.bs.modal', function () {
			if (onclose && fireClose)
				onclose();
			contentParent.appendChild(content);
			content.style.display = "none";
			div.parentNode.removeChild(div);
		});

		$(div).find("[data-buttonName='button1']").on('click', function () { fireClose = false; if (onBtn1Click) onBtn1Click(); });
		$(div).find("[data-buttonName='button2']").on('click', function () { fireClose = false; if (onBtn2Click) onBtn2Click(); });
		$(div).find("[data-buttonName='button3']").on('click', function () { fireClose = false; if (onBtn3Click) onBtn3Click(); });
		$(div).find(".modal-body").append(content);
		content.style.display = "";

		if (onshow)
			onshow();
	}

	show();
	function closeModal() {
		fireClose = true;
		$(div).modal('hide');
	};

	return dlg;
}

function findByObjectId(arr, obj) {
	if (!obj)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].Id == obj.Id)
			return arr[i];
	}
	return null;
}
function findById(arr, id) {
	if (!id)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].ID == id)
			return arr[i];
	}
	return null;
}
function findByID(arr,id)
{
	if (!id)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].ID == id)
			return arr[i];
	}
	return null;
}
function findIndexById(arr, id) {
	if (!id)
		return -1;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].Id == id)
			return i;
	}
	return -1;
}
function findAndReplace(arr, id, obj) {
	if (!id)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].ID == id) {
			{
				arr[i] = obj;
				return arr[i];
			}
		}
	}
	return null;
}
function findBusinessById(arr, id) {
	if (!id)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].Business.Id == id)
			return arr[i].Business;
	}
	return null;
}
function findBusinessAndReplace(arr, business) {
	if (!business)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].Business.Id == business.Id) {
			{
				arr[i].Business = business;
				return arr[i].Business;
			}
		}
	}
	return null;
}

function findAndRemove(arr, obj) {
	if (!obj)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].ID == obj.ID) {
			var index = arr.indexOf(arr[i]);
			if (index > -1) {
				arr.splice(index, 1);
				break;
			}
		}
	}
}

function findAndRemoveByPropertyValue(arr, property, value) {
	for (var i = 0; i < arr.length; i++) {
		if (arr[i][property] == value) {
			var index = arr.indexOf(arr[i]);
			if (index > -1) {
				arr.splice(index, 1);
				break;
			}
		}
	}
}


function findByProperty(arr, obj, property) {
	if (!obj)
		return null;
	for (var i = 0; i < arr.length; i++) {
		if (arr[i][property] == obj[property])
			return arr[i];
	}
	return null;
}
function findByPropertyValue(arr, property, value) {
	for (var i = 0; i < arr.length; i++) {
		if (arr[i][property] == value)
			return arr[i];
	}
	return null;
}

function getKeyValue(arr, key) {
	for (var i = 0; i < arr.length; i++) {
		if (arr[i].Key && arr[i].Key == key)
			return arr[i].Value;
	}
	return null;
}

function filterData(arr, key, value) {
	value = value.toLowerCase();
	var filteredData = [];
	for (var i = 0; i < arr.length; i++) {
		if (arr[i][key].toLowerCase().search(value) != -1)
			filteredData.push(arr[i]);
	}
	return filteredData;
}

function sort_by(field, reverse) {
	var key = function (x) {

		var value = x[field];
		if (field.indexOf('.') > -1) {
			var fields = field.split('.');
			if (fields.length == 2) {
				value = !x[fields[0]] ? '' : x[fields[0]][fields[1]];
			}
			else if (fields.length == 3) {
				value = !x[fields[0]] || !x[fields[0]][fields[1]] ? '' : x[fields[0]][fields[1]][fields[2]];
			}
		}

		if (!value) value = '';

		if (type(value) == 'number')
			return parseInt(value);
		else if (type(value) == 'string')
			return (value).toUpperCase();
		else
			return value;
	};

	reverse = !reverse ? 1 : -1;

	return function (a, b) {
		return a = key(a), b = key(b), reverse * ((a > b) - (b > a));
	};
}
function type(o) {
	var TYPES = {
		'undefined': 'undefined',
		'number': 'number',
		'boolean': 'boolean',
		'string': 'string',
		'[object Function]': 'function',
		'[object RegExp]': 'regexp',
		'[object Array]': 'array',
		'[object Date]': 'date',
		'[object Error]': 'error'
	},
    TOSTRING = Object.prototype.toString;
	return TYPES[typeof o] || TYPES[TOSTRING.call(o)] || (o ? 'object' : 'null');
};

$.contain = function (a, b) {
	if (a.toString().toLowerCase().indexOf(b.toString()) >= 0)
		return true;
	return false;
};
function getCurrentPageName() {
	var path = window.location.pathname;
	var page = path.split("/").pop();
	return page;
}

function IsEmail(email) {
	var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
	return regex.test(email);
}
function getQueryString(name) {
	name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
	var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search || location.hash);
	return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function createCookie(name, value, days) {
	var expires = "";
	if (days) {
		var date = new Date();
		date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
		expires = "; expires=" + date.toGMTString();
	}
	document.cookie = name + "=" + value + expires + "; path=/";
}
function readCookie(name) {
	var nameEQ = name + "=";
	var ca = document.cookie.split(';');
	for (var i = 0; i < ca.length; i++) {
		var c = ca[i];
		while (c.charAt(0) == ' ') c = c.substring(1, c.length);
		if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
	}
	return null;
}
function eraseCookie(name) {
	createCookie(name, "", -1);
}

function enableDocumentSelection(enable, el) {
	if (!el)
		el = document.body;
	if (enable) {
		$(el).attr('unselectable', 'off')
		 .css({
		 	'-moz-user-select': '',
		 	'-o-user-select': '',
		 	'-khtml-user-select': '',
		 	'-webkit-user-select': '',
		 	'-ms-user-select': '',
		 	'user-select': ''
		 }).off('selectstart', false);;
	} else {
		$(el).attr('unselectable', 'on')
			.css({
				'-moz-user-select': '-moz-none',
				'-o-user-select': 'none',
				'-khtml-user-select': 'none',
				'-webkit-user-select': 'none',
				'-ms-user-select': 'none',
				'user-select': 'none'
			}).on('selectstart', false);
	}
}

function formatToCurrency(x) {
	if (typeof Hesabfa != "undefined" && Hesabfa.isDecimalCurrency()) {
		return x.toFixed(2).replace(/(\d)(?=(\d{3})+\.)/g, '$1,');
	}
	if (x.toString().search(',') > -1 || x.toString().search('،') > -1)
		x = currencyToMoney(x);
	var n = x < 0;
	return Math.abs(x).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + (n ? "-" : "");
	//    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function currencyToMoney(x) {
	x = x.replace(/\,/g, '');
	x = parseInt(x, 10);
	return x;
}
function isNumber(n) {
	return !isNaN(parseFloat(n)) && isFinite(n);
}

function convert2farsiDigit(value) {
	value += "";
	var map = "۰۱۲۳۴۵۶۷۸۹";
	value = value.replace(/0/g, "۰");
	value = value.replace(/1/g, "۱");
	value = value.replace(/2/g, "۲");
	value = value.replace(/3/g, "۳");
	value = value.replace(/4/g, "۴");
	value = value.replace(/5/g, "۵");
	value = value.replace(/6/g, "۶");
	value = value.replace(/7/g, "۷");
	value = value.replace(/8/g, "۸");
	value = value.replace(/9/g, "۹");
	return value;
};
function abbreviateNumber(number, maxPlaces, forcePlaces, forceLetter) {
	number = Number(number)
	forceLetter = forceLetter || false
	if (forceLetter !== false) {
		return annotate(number, maxPlaces, forcePlaces, forceLetter)
	}
	var abbr
	if (number >= 1e12) {
		abbr = 'T'
	}
	else if (number >= 1e9) {
		abbr = 'B'
	}
	else if (number >= 1e6) {
		abbr = 'M'
	}
	else if (number >= 1e3) {
		abbr = 'K'
	}
	else {
		abbr = ''
	}
	return annotateNumber(number, maxPlaces, forcePlaces, abbr);
};
function annotateNumber(number, maxPlaces, forcePlaces, abbr) {
	// set places to false to not round
	var rounded = 0
	switch (abbr) {
		case 'T':
			rounded = number / 1e12
			break
		case 'B':
			rounded = number / 1e9
			break
		case 'M':
			rounded = number / 1e6
			break
		case 'K':
			rounded = number / 1e3
			break
		case '':
			rounded = number
			break
	}
	if (maxPlaces !== false) {
		var test = new RegExp('\\.\\d{' + (maxPlaces + 1) + ',}$')
		if (test.test(('' + rounded))) {
			rounded = rounded.toFixed(maxPlaces)
		}
	}
	if (forcePlaces !== false) {
		rounded = Number(rounded).toFixed(forcePlaces)
	}
	return rounded + abbr
};

function sleep(milliseconds) {
	var start = new Date().getTime();
	for (var i = 0; i < 1e7; i++) {
		if ((new Date().getTime() - start) > milliseconds) {
			break;
		}
	}
}
Array.prototype.moveUp = function (value, by) {
	if (!value) return;
	if (Object.prototype.toString.call(this) != '[object Array]')
		return;
	var index = this.indexOf(value),
        newPos = index - (by || 1);

	if (index === -1)
		throw new Error("Element not found in array");

	if (newPos < 0)
		newPos = 0;

	this.splice(index, 1);
	this.splice(newPos, 0, value);
};
Array.prototype.moveDown = function (value, by) {
	if (!value) return;
	if (Object.prototype.toString.call(this) != '[object Array]')
		return;
	var index = this.indexOf(value),
        newPos = index + (by || 1);

	if (index === -1)
		throw new Error("Element not found in array");

	if (newPos >= this.length)
		newPos = this.length;

	this.splice(index, 1);
	this.splice(newPos, 0, value);
};
String.prototype.formatString = function () {
	var args = arguments;
	return this.replace(/{(\d+)}/g, function (match, number) {
		return typeof args[number] != 'undefined'
          ? args[number]
          : match
		;
	});
};

function reverse(s) {
	return s.split("").reverse().join("");
}

(function ($) {
	$.fn.scrollIntoView = function (duration, easing, complete) {
		// The arguments are optional.
		// The first argment can be false for no animation or a duration.
		// The first argment could also be a map of options.
		// Refer to http://api.jquery.com/animate/.
		var opts = $.extend({},
        $.fn.scrollIntoView.defaults);

		// Get options
		if ($.type(duration) == "object") {
			$.extend(opts, duration);
		} else if ($.type(duration) == "number") {
			$.extend(opts, { duration: duration, easing: easing, complete: complete });
		} else if (duration == false) {
			opts.smooth = false;
		}

		// get enclosing offsets
		var elY = Infinity, elH = 0;
		if (this.size() == 1) ((elY = this.get(0).offsetTop) == null || (elH = elY + this.get(0).offsetHeight));
		else this.each(function (i, el) { (el.offsetTop < elY ? elY = el.offsetTop : el.offsetTop + el.offsetHeight > elH ? elH = el.offsetTop + el.offsetHeight : null) });
		elH -= elY;

		// start from the common ancester
		var pEl = this.commonAncestor().get(0);

		var wH = $(window).height();

		// go up parents until we find one that scrolls
		while (pEl) {
			var pY = pEl.scrollTop, pH = pEl.clientHeight;
			if (pH > wH) pH = wH;

			// case: if body's elements are all absolutely/fixed positioned, use window height
			if (pH == 0 && pEl.tagName == "BODY") pH = wH;

			if (
				// it wiggles?
            (pEl.scrollTop != ((pEl.scrollTop += 1) == null || pEl.scrollTop) && (pEl.scrollTop -= 1) != null) ||
            (pEl.scrollTop != ((pEl.scrollTop -= 1) == null || pEl.scrollTop) && (pEl.scrollTop += 1) != null)) {
				if (elY <= pY) scrollTo(pEl, elY); // scroll up
				else if ((elY + elH) > (pY + pH)) scrollTo(pEl, elY + elH - pH); // scroll down
				else scrollTo(pEl, undefined) // no scroll
				return;
			}

			// try next parent
			pEl = pEl.parentNode;
		}

		function scrollTo(el, scrollTo) {
			if (scrollTo === undefined) {
				if ($.isFunction(opts.complete)) opts.complete.call(el);
			} else if (opts.smooth) {
				$(el).stop().animate({ scrollTop: scrollTo }, opts);
			} else {
				el.scrollTop = scrollTo;
				if ($.isFunction(opts.complete)) opts.complete.call(el);
			}
		}
		return this;
	};

	$.fn.scrollIntoView.defaults = {
		smooth: true,
		duration: null,
		easing: $.easing && $.easing.easeOutExpo ? 'easeOutExpo' : null,
		// Note: easeOutExpo requires jquery.effects.core.js
		//       otherwise jQuery will default to use 'swing'
		complete: $.noop(),
		step: null,
		specialEasing: {} // cannot be null in jQuery 1.8.3
	};

	/*
     Returns whether the elements are in view
    */
	$.fn.isOutOfView = function (completely) {
		// completely? whether element is out of view completely
		var outOfView = true;
		this.each(function () {
			var pEl = this.parentNode, pY = pEl.scrollTop, pH = pEl.clientHeight, elY = this.offsetTop, elH = this.offsetHeight;
			if (completely ? (elY) > (pY + pH) : (elY + elH) > (pY + pH)) { }
			else if (completely ? (elY + elH) < pY : elY < pY) { }
			else outOfView = false;
		});
		return outOfView;
	};

	/*
     Returns the common ancestor of the elements.
     It was taken from http://stackoverflow.com/questions/3217147/jquery-first-parent-containing-all-children
     It has received minimal testing.
    */
	$.fn.commonAncestor = function () {
		var parents = [];
		var minlen = Infinity;

		$(this).each(function () {
			var curparents = $(this).parents();
			parents.push(curparents);
			minlen = Math.min(minlen, curparents.length);
		});

		for (var i = 0; i < parents.length; i++) {
			parents[i] = parents[i].slice(parents[i].length - minlen);
		}

		// Iterate until equality is found
		for (var i = 0; i < parents[0].length; i++) {
			var equal = true;
			for (var j in parents) {
				if (parents[j][i] != parents[0][i]) {
					equal = false;
					break;
				}
			}
			if (equal) return $(parents[0][i]);
		}
		return $([]);
	}

})(jQuery);


// https://github.com/nmrugg/LZMA-JS/
var e = function () {
	"use strict"; function r(e, r) { postMessage({ action: Nt, cbn: r, result: e }) } function t(e) { var r = []; return r[e - 1] = void 0, r } function o(e, r) { return i(e[0] + r[0], e[1] + r[1]) } function n(e, r) { return u(~~Math.max(Math.min(e[1] / At, 2147483647), -2147483648) & ~~Math.max(Math.min(r[1] / At, 2147483647), -2147483648), c(e) & c(r)) } function s(e, r) { var t, o; return e[0] == r[0] && e[1] == r[1] ? 0 : (t = 0 > e[1], o = 0 > r[1], t && !o ? -1 : !t && o ? 1 : h(e, r)[1] < 0 ? -1 : 1) } function i(e, r) { var t, o; for (r %= 0x10000000000000000, e %= 0x10000000000000000, t = r % At, o = Math.floor(e / At) * At, r = r - t + o, e = e - o + t; 0 > e;) e += At, r -= At; for (; e > 4294967295;) e -= At, r += At; for (r %= 0x10000000000000000; r > 0x7fffffff00000000;) r -= 0x10000000000000000; for (; -0x8000000000000000 > r;) r += 0x10000000000000000; return [e, r] } function _(e, r) { return e[0] == r[0] && e[1] == r[1] } function a(e) { return e >= 0 ? [e, 0] : [e + At, -At] } function c(e) { return e[0] >= 2147483648 ? ~~Math.max(Math.min(e[0] - At, 2147483647), -2147483648) : ~~Math.max(Math.min(e[0], 2147483647), -2147483648) } function u(e, r) { var t, o; return t = e * At, o = r, 0 > r && (o += At), [o, t] } function f(e) { return 30 >= e ? 1 << e : f(30) * f(e - 30) } function m(e, r) { var t, o, n, s; if (r &= 63, _(e, Gt)) return r ? Wt : e; if (0 > e[1]) throw Error("Neg"); return s = f(r), o = e[1] * s % 0x10000000000000000, n = e[0] * s, t = n - n % At, o += t, n -= t, o >= 0x8000000000000000 && (o -= 0x10000000000000000), [n, o] } function d(e, r) { var t; return r &= 63, t = f(r), i(Math.floor(e[0] / t), e[1] / t) } function p(e, r) { var t; return r &= 63, t = d(e, r), 0 > e[1] && (t = o(t, m([2, 0], 63 - r))), t } function h(e, r) { return i(e[0] - r[0], e[1] - r[1]) } function P(e, r) { return e.Mc = r, e.Lc = 0, e.Vb = r.length, e } function l(e) { return e.Lc >= e.Vb ? -1 : 255 & e.Mc[e.Lc++] } function v(e, r, t, o) { return e.Lc >= e.Vb ? -1 : (o = Math.min(o, e.Vb - e.Lc), M(e.Mc, e.Lc, r, t, o), e.Lc += o, o) } function B(e) { return e.Mc = t(32), e.Vb = 0, e } function S(e) { var r = e.Mc; return r.length = e.Vb, r } function g(e, r) { e.Mc[e.Vb++] = r << 24 >> 24 } function k(e, r, t, o) { M(r, t, e.Mc, e.Vb, o), e.Vb += o } function R(e, r, t, o, n) { var s; for (s = r; t > s; ++s) o[n++] = e.charCodeAt(s) } function M(e, r, t, o, n) { for (var s = 0; n > s; ++s) t[o + s] = e[r + s] } function D(e, r) { Ar(r, 1 << e.s), r.n = e.f, Hr(r, e.m), r.eb = 0, r.fb = 3, r.Y = 2, r.y = 3 } function b(e, r, t, o, n) { var i, _; if (s(o, Ht) < 0) throw Error("invalid length " + o); for (e.Nb = o, i = Dr({}), D(n, i), i.Gc = 1, Gr(i, t), _ = 0; 64 > _; _ += 8) g(t, 255 & c(d(o, _))); e.ac = (i.S = 0, i.kc = r, i.lc = 0, Mr(i), i.d.zb = t, Fr(i), wr(i), br(i), i._.pb = i.n + 1 - 2, Qr(i._, 1 << i.Y), i.i.pb = i.n + 1 - 2, Qr(i.i, 1 << i.Y), void (i.g = Wt), X({}, i)) } function w(e, r, t) { return e.rc = B({}), b(e, P({}, r), e.rc, a(r.length), t), e } function C(e, r, t) { var o, n, s, i, _ = "", c = []; for (n = 0; 5 > n; ++n) { if (s = l(r), -1 == s) throw Error("truncated input"); c[n] = s << 24 >> 24 } if (o = ir({}), !ar(o, c)) throw Error("corrupted input"); for (n = 0; 64 > n; n += 8) { if (s = l(r), -1 == s) throw Error("truncated input"); s = s.toString(16), 1 == s.length && (s = "0" + s), _ = s + "" + _ } /^0+$|^f+$/i.test(_) ? e.Nb = Ht : (i = parseInt(_, 16), e.Nb = i > 4294967295 ? Ht : a(i)), e.ac = nr(o, r, t, e.Nb) } function E(e, r) { return e.rc = B({}), C(e, P({}, r), e.rc), e } function L(e, r, o, n) { var s; e.Bc = r, e._b = o, s = r + o + n, (null == e.c || e.Jb != s) && (e.c = null, e.Jb = s, e.c = t(e.Jb)), e.H = e.Jb - o } function y(e, r) { return e.c[e.f + e.o + r] } function z(e, r, t, o) { var n, s; for (e.X && e.o + r + o > e.h && (o = e.h - (e.o + r)), ++t, s = e.f + e.o + r, n = 0; o > n && e.c[s + n] == e.c[s + n - t]; ++n); return n } function F(e) { return e.h - e.o } function I(e) { var r, t, o; for (o = e.f + e.o - e.Bc, o > 0 && --o, t = e.f + e.h - o, r = 0; t > r; ++r) e.c[r] = e.c[o + r]; e.f -= o } function x(e) { var r; ++e.o, e.o > e.yb && (r = e.f + e.o, r > e.H && I(e), N(e)) } function N(e) { var r, t, o; if (!e.X) for (; ;) { if (o = -e.f + e.Jb - e.h, !o) return; if (r = v(e.bc, e.c, e.f + e.h, o), -1 == r) return e.yb = e.h, t = e.f + e.yb, t > e.H && (e.yb = e.H - e.f), void (e.X = 1); e.h += r, e.h >= e.o + e._b && (e.yb = e.h - e._b) } } function O(e, r) { e.f += r, e.yb -= r, e.o -= r, e.h -= r } function A(e, r, o, n, s) { var i, _, a; 1073741567 > r && (e.Ec = 16 + (n >> 1), a = ~~((r + o + n + s) / 2) + 256, L(e, r + o, n + s, a), e.ub = n, i = r + 1, e.p != i && (e.M = t(2 * (e.p = i))), _ = 65536, e.rb && (_ = r - 1, _ |= _ >> 1, _ |= _ >> 2, _ |= _ >> 4, _ |= _ >> 8, _ >>= 1, _ |= 65535, _ > 16777216 && (_ >>= 1), e.Fc = _, ++_, _ += e.N), _ != e.sc && (e.qb = t(e.sc = _))) } function H(e, r) { var t, o, n, s, i, _, a, c, u, f, m, d, p, h, P, l, v, B, S, g, k; if (e.h >= e.o + e.ub) h = e.ub; else if (h = e.h - e.o, e.xb > h) return W(e), 0; for (v = 0, P = e.o > e.p ? e.o - e.p : 0, o = e.f + e.o, l = 1, c = 0, u = 0, e.rb ? (k = Yt[255 & e.c[o]] ^ 255 & e.c[o + 1], c = 1023 & k, k ^= (255 & e.c[o + 2]) << 8, u = 65535 & k, f = (k ^ Yt[255 & e.c[o + 3]] << 5) & e.Fc) : f = 255 & e.c[o] ^ (255 & e.c[o + 1]) << 8, n = e.qb[e.N + f] || 0, e.rb && (s = e.qb[c] || 0, i = e.qb[1024 + u] || 0, e.qb[c] = e.o, e.qb[1024 + u] = e.o, s > P && e.c[e.f + s] == e.c[o] && (r[v++] = l = 2, r[v++] = e.o - s - 1), i > P && e.c[e.f + i] == e.c[o] && (i == s && (v -= 2), r[v++] = l = 3, r[v++] = e.o - i - 1, s = i), 0 != v && s == n && (v -= 2, l = 1)), e.qb[e.N + f] = e.o, S = (e.k << 1) + 1, g = e.k << 1, d = p = e.w, 0 != e.w && n > P && e.c[e.f + n + e.w] != e.c[o + e.w] && (r[v++] = l = e.w, r[v++] = e.o - n - 1), t = e.Ec; ;) { if (P >= n || 0 == t--) { e.M[S] = e.M[g] = 0; break } if (a = e.o - n, _ = (e.k >= a ? e.k - a : e.k - a + e.p) << 1, B = e.f + n, m = p > d ? d : p, e.c[B + m] == e.c[o + m]) { for (; ++m != h && e.c[B + m] == e.c[o + m];); if (m > l && (r[v++] = l = m, r[v++] = a - 1, m == h)) { e.M[g] = e.M[_], e.M[S] = e.M[_ + 1]; break } } (255 & e.c[o + m]) > (255 & e.c[B + m]) ? (e.M[g] = n, g = _ + 1, n = e.M[g], p = m) : (e.M[S] = n, S = _, n = e.M[S], d = m) } return W(e), v } function G(e) { e.f = 0, e.o = 0, e.h = 0, e.X = 0, N(e), e.k = 0, O(e, -1) } function W(e) { var r; ++e.k >= e.p && (e.k = 0), x(e), 1073741823 == e.o && (r = e.o - e.p, T(e.M, 2 * e.p, r), T(e.qb, e.sc, r), O(e, r)) } function T(e, r, t) { var o, n; for (o = 0; r > o; ++o) n = e[o] || 0, t >= n ? n = 0 : n -= t, e[o] = n } function Y(e, r) { e.rb = r > 2, e.rb ? (e.w = 0, e.xb = 4, e.N = 66560) : (e.w = 2, e.xb = 3, e.N = 0) } function Z(e, r) { var t, o, n, s, i, _, a, c, u, f, m, d, p, h, P, l, v; do { if (e.h >= e.o + e.ub) d = e.ub; else if (d = e.h - e.o, e.xb > d) { W(e); continue } for (p = e.o > e.p ? e.o - e.p : 0, o = e.f + e.o, e.rb ? (v = Yt[255 & e.c[o]] ^ 255 & e.c[o + 1], _ = 1023 & v, e.qb[_] = e.o, v ^= (255 & e.c[o + 2]) << 8, a = 65535 & v, e.qb[1024 + a] = e.o, c = (v ^ Yt[255 & e.c[o + 3]] << 5) & e.Fc) : c = 255 & e.c[o] ^ (255 & e.c[o + 1]) << 8, n = e.qb[e.N + c], e.qb[e.N + c] = e.o, P = (e.k << 1) + 1, l = e.k << 1, f = m = e.w, t = e.Ec; ;) { if (p >= n || 0 == t--) { e.M[P] = e.M[l] = 0; break } if (i = e.o - n, s = (e.k >= i ? e.k - i : e.k - i + e.p) << 1, h = e.f + n, u = m > f ? f : m, e.c[h + u] == e.c[o + u]) { for (; ++u != d && e.c[h + u] == e.c[o + u];); if (u == d) { e.M[l] = e.M[s], e.M[P] = e.M[s + 1]; break } } (255 & e.c[o + u]) > (255 & e.c[h + u]) ? (e.M[l] = n, l = s + 1, n = e.M[l], m = u) : (e.M[P] = n, P = s, n = e.M[P], f = u) } W(e) } while (0 != --r) } function V(e, r, t) { var o = e.o - r - 1; for (0 > o && (o += e.L) ; 0 != t; --t) o >= e.L && (o = 0), e.Kb[e.o++] = e.Kb[o++], e.o >= e.L && j(e) } function $(e, r) { (null == e.Kb || e.L != r) && (e.Kb = t(r)), e.L = r, e.o = 0, e.h = 0 } function j(e) { var r = e.o - e.h; r && (k(e.bc, e.Kb, e.h, r), e.o >= e.L && (e.o = 0), e.h = e.o) } function K(e, r) { var t = e.o - r - 1; return 0 > t && (t += e.L), e.Kb[t] } function q(e, r) { e.Kb[e.o++] = r, e.o >= e.L && j(e) } function J(e) { j(e), e.bc = null } function Q(e) { return e -= 2, 4 > e ? e : 3 } function U(e) { return 4 > e ? 0 : 10 > e ? e - 3 : e - 6 } function X(e, r) { return e.cb = r, e.$ = null, e.zc = 1, e } function er(e, r) { return e.$ = r, e.cb = null, e.zc = 1, e } function rr(e) { if (!e.zc) throw Error("bad state"); return e.cb ? or(e) : tr(e), e.zc } function tr(e) { var r = sr(e.$); if (-1 == r) throw Error("corrupted input"); e.Sb = Ht, e.Pc = e.$.g, (r || s(e.$.Nc, Wt) >= 0 && s(e.$.g, e.$.Nc) >= 0) && (j(e.$.B), J(e.$.B), e.$.e.zb = null, e.zc = 0) } function or(e) { Rr(e.cb, e.cb.Ub, e.cb.vc, e.cb.Kc), e.Sb = e.cb.Ub[0], e.cb.Kc[0] && (Or(e.cb), e.zc = 0) } function nr(e, r, t, o) { return e.e.zb = r, J(e.B), e.B.bc = t, _r(e), e.W = 0, e.ib = 0, e.Jc = 0, e.Ic = 0, e.Qc = 0, e.Nc = o, e.g = Wt, e.gc = 0, er({}, e) } function sr(e) { var r, t, n, i, _, u; if (u = c(e.g) & e.Dc, vt(e.e, e.Gb, (e.W << 4) + u)) { if (vt(e.e, e.Wb, e.W)) n = 0, vt(e.e, e.Cb, e.W) ? (vt(e.e, e.Db, e.W) ? (vt(e.e, e.Eb, e.W) ? (t = e.Qc, e.Qc = e.Ic) : t = e.Ic, e.Ic = e.Jc) : t = e.Jc, e.Jc = e.ib, e.ib = t) : vt(e.e, e.tb, (e.W << 4) + u) || (e.W = 7 > e.W ? 9 : 11, n = 1), n || (n = mr(e.sb, e.e, u) + 2, e.W = 7 > e.W ? 8 : 11); else if (e.Qc = e.Ic, e.Ic = e.Jc, e.Jc = e.ib, n = 2 + mr(e.Mb, e.e, u), e.W = 7 > e.W ? 7 : 10, _ = at(e.kb[Q(n)], e.e), _ >= 4) { if (i = (_ >> 1) - 1, e.ib = (2 | 1 & _) << i, 14 > _) e.ib += ut(e.jc, e.ib - _ - 1, e.e, i); else if (e.ib += Bt(e.e, i - 4) << 4, e.ib += ct(e.Bb, e.e), 0 > e.ib) return -1 == e.ib ? 1 : -1 } else e.ib = _; if (s(a(e.ib), e.g) >= 0 || e.ib >= e.mb) return -1; V(e.B, e.ib, n), e.g = o(e.g, a(n)), e.gc = K(e.B, 0) } else r = Pr(e.jb, c(e.g), e.gc), e.gc = 7 > e.W ? vr(r, e.e) : Br(r, e.e, K(e.B, e.ib)), q(e.B, e.gc), e.W = U(e.W), e.g = o(e.g, Tt); return 0 } function ir(e) { e.B = {}, e.e = {}, e.Gb = t(192), e.Wb = t(12), e.Cb = t(12), e.Db = t(12), e.Eb = t(12), e.tb = t(192), e.kb = t(4), e.jc = t(114), e.Bb = _t({}, 4), e.Mb = dr({}), e.sb = dr({}), e.jb = {}; for (var r = 0; 4 > r; ++r) e.kb[r] = _t({}, 6); return e } function _r(e) { e.B.h = 0, e.B.o = 0, gt(e.Gb), gt(e.tb), gt(e.Wb), gt(e.Cb), gt(e.Db), gt(e.Eb), gt(e.jc), lr(e.jb); for (var r = 0; 4 > r; ++r) gt(e.kb[r].G); pr(e.Mb), pr(e.sb), gt(e.Bb.G), St(e.e) } function ar(e, r) { var t, o, n, s, i, _, a; if (5 > r.length) return 0; for (a = 255 & r[0], n = a % 9, _ = ~~(a / 9), s = _ % 5, i = ~~(_ / 5), t = 0, o = 0; 4 > o; ++o) t += (255 & r[1 + o]) << 8 * o; return t > 99999999 || !ur(e, n, s, i) ? 0 : cr(e, t) } function cr(e, r) { return 0 > r ? 0 : (e.Pb != r && (e.Pb = r, e.mb = Math.max(e.Pb, 1), $(e.B, Math.max(e.mb, 4096))), 1) } function ur(e, r, t, o) { if (r > 8 || t > 4 || o > 4) return 0; hr(e.jb, t, r); var n = 1 << o; return fr(e.Mb, n), fr(e.sb, n), e.Dc = n - 1, 1 } function fr(e, r) { for (; r > e.P; ++e.P) e.ec[e.P] = _t({}, 3), e.hc[e.P] = _t({}, 3) } function mr(e, r, t) { if (!vt(r, e.uc, 0)) return at(e.ec[t], r); var o = 8; return o += vt(r, e.uc, 1) ? 8 + at(e.tc, r) : at(e.hc[t], r) } function dr(e) { return e.uc = t(2), e.ec = t(16), e.hc = t(16), e.tc = _t({}, 8), e.P = 0, e } function pr(e) { gt(e.uc); for (var r = 0; e.P > r; ++r) gt(e.ec[r].G), gt(e.hc[r].G); gt(e.tc.G) } function hr(e, r, o) { var n, s; if (null == e.V || e.u != o || e.I != r) for (e.I = r, e.oc = (1 << r) - 1, e.u = o, s = 1 << e.u + e.I, e.V = t(s), n = 0; s > n; ++n) e.V[n] = Sr({}) } function Pr(e, r, t) { return e.V[((r & e.oc) << e.u) + ((255 & t) >>> 8 - e.u)] } function lr(e) { var r, t; for (t = 1 << e.u + e.I, r = 0; t > r; ++r) gt(e.V[r].Hb) } function vr(e, r) { var t = 1; do t = t << 1 | vt(r, e.Hb, t); while (256 > t); return t << 24 >> 24 } function Br(e, r, t) { var o, n, s = 1; do if (n = t >> 7 & 1, t <<= 1, o = vt(r, e.Hb, (1 + n << 8) + s), s = s << 1 | o, n != o) { for (; 256 > s;) s = s << 1 | vt(r, e.Hb, s); break } while (256 > s); return s << 24 >> 24 } function Sr(e) { return e.Hb = t(768), e } function gr(e, r) { var t, o, n, s; e.lb = r, n = e.a[r].r, o = e.a[r].j; do e.a[r].t && (st(e.a[n]), e.a[n].r = n - 1, e.a[r].yc && (e.a[n - 1].t = 0, e.a[n - 1].r = e.a[r].r2, e.a[n - 1].j = e.a[r].j2)), s = n, t = o, o = e.a[s].j, n = e.a[s].r, e.a[s].j = t, e.a[s].r = r, r = s; while (r > 0); return e.nb = e.a[0].j, e.q = e.a[0].r } function kr(e) { e.l = 0, e.J = 0; for (var r = 0; 4 > r; ++r) e.v[r] = 0 } function Rr(e, r, t, n) { var i, u, f, m, d, p, P, l, v, B, S, g, k, R, M; if (r[0] = Wt, t[0] = Wt, n[0] = 1, e.kc && (e.b.bc = e.kc, G(e.b), e.S = 1, e.kc = null), !e.lc) { if (e.lc = 1, R = e.g, _(e.g, Wt)) { if (!F(e.b)) return void Cr(e, c(e.g)); xr(e), k = c(e.g) & e.y, kt(e.d, e.C, (e.l << 4) + k, 0), e.l = U(e.l), f = y(e.b, -e.s), rt(Xr(e.A, c(e.g), e.J), e.d, f), e.J = f, --e.s, e.g = o(e.g, Tt) } if (!F(e.b)) return void Cr(e, c(e.g)); for (; ;) { if (P = Er(e, c(e.g)), B = e.nb, k = c(e.g) & e.y, u = (e.l << 4) + k, 1 == P && -1 == B) kt(e.d, e.C, u, 0), f = y(e.b, -e.s), M = Xr(e.A, c(e.g), e.J), 7 > e.l ? rt(M, e.d, f) : (v = y(e.b, -e.v[0] - 1 - e.s), tt(M, e.d, v, f)), e.J = f, e.l = U(e.l); else { if (kt(e.d, e.C, u, 1), 4 > B) { if (kt(e.d, e.bb, e.l, 1), B ? (kt(e.d, e.gb, e.l, 1), 1 == B ? kt(e.d, e.Ob, e.l, 0) : (kt(e.d, e.Ob, e.l, 1), kt(e.d, e.wc, e.l, B - 2))) : (kt(e.d, e.gb, e.l, 0), 1 == P ? kt(e.d, e.Z, u, 0) : kt(e.d, e.Z, u, 1)), 1 == P ? e.l = 7 > e.l ? 9 : 11 : (Kr(e.i, e.d, P - 2, k), e.l = 7 > e.l ? 8 : 11), m = e.v[B], 0 != B) { for (p = B; p >= 1; --p) e.v[p] = e.v[p - 1]; e.v[0] = m } } else { for (kt(e.d, e.bb, e.l, 0), e.l = 7 > e.l ? 7 : 10, Kr(e._, e.d, P - 2, k), B -= 4, g = Tr(B), l = Q(P), mt(e.K[l], e.d, g), g >= 4 && (d = (g >> 1) - 1, i = (2 | 1 & g) << d, S = B - i, 14 > g ? Pt(e.Lb, i - g - 1, e.d, d, S) : (Rt(e.d, S >> 4, d - 4), pt(e.U, e.d, 15 & S), ++e.Qb)), m = B, p = 3; p >= 1; --p) e.v[p] = e.v[p - 1]; e.v[0] = m, ++e.Rb } e.J = y(e.b, P - 1 - e.s) } if (e.s -= P, e.g = o(e.g, a(P)), !e.s) { if (e.Rb >= 128 && wr(e), e.Qb >= 16 && br(e), r[0] = e.g, t[0] = Dt(e.d), !F(e.b)) return void Cr(e, c(e.g)); if (s(h(e.g, R), [4096, 0]) >= 0) return e.lc = 0, void (n[0] = 0) } } } } function Mr(e) { var r, t; e.b || (r = {}, t = 4, e.T || (t = 2), Y(r, t), e.b = r), Ur(e.A, e.eb, e.fb), (e.ab != e.wb || e.Fb != e.n) && (A(e.b, e.ab, 4096, e.n, 274), e.wb = e.ab, e.Fb = e.n) } function Dr(e) { var r; for (e.v = t(4), e.a = [], e.d = {}, e.C = t(192), e.bb = t(12), e.gb = t(12), e.Ob = t(12), e.wc = t(12), e.Z = t(192), e.K = [], e.Lb = t(114), e.U = ft({}, 4), e._ = qr({}), e.i = qr({}), e.A = {}, e.m = [], e.R = [], e.hb = [], e.mc = t(16), e.x = t(4), e.O = t(4), e.Ub = [Wt], e.vc = [Wt], e.Kc = [0], e.fc = t(5), e.xc = t(128), e.vb = 0, e.T = 1, e.D = 0, e.Fb = -1, e.nb = 0, r = 0; 4096 > r; ++r) e.a[r] = {}; for (r = 0; 4 > r; ++r) e.K[r] = ft({}, 6); return e } function br(e) { for (var r = 0; 16 > r; ++r) e.mc[r] = ht(e.U, r); e.Qb = 0 } function wr(e) { var r, t, o, n, s, i, _, a; for (n = 4; 128 > n; ++n) i = Tr(n), o = (i >> 1) - 1, r = (2 | 1 & i) << o, e.xc[n] = lt(e.Lb, r - i - 1, o, n - r); for (s = 0; 4 > s; ++s) { for (t = e.K[s], _ = s << 6, i = 0; e.cc > i; ++i) e.R[_ + i] = dt(t, i); for (i = 14; e.cc > i; ++i) e.R[_ + i] += (i >> 1) - 1 - 4 << 6; for (a = 128 * s, n = 0; 4 > n; ++n) e.hb[a + n] = e.R[_ + n]; for (; 128 > n; ++n) e.hb[a + n] = e.R[_ + Tr(n)] + e.xc[n] } e.Rb = 0 } function Cr(e, r) { Nr(e), Wr(e, r & e.y), Mt(e.d) } function Er(e, r) { var t, o, n, s, i, _, a, c, u, f, m, d, p, h, P, l, v, B, S, g, k, R, M, D, b, w, C, E, L, I, x, N, O, A, H, G, W, T, Y, Z, V, $, j, K, q, J, Q, X, er, rr; if (e.lb != e.q) return p = e.a[e.q].r - e.q, e.nb = e.a[e.q].j, e.q = e.a[e.q].r, p; if (e.q = e.lb = 0, e.Q ? (d = e.vb, e.Q = 0) : d = xr(e), C = e.D, b = F(e.b) + 1, 2 > b) return e.nb = -1, 1; for (b > 273 && (b = 273), Z = 0, u = 0; 4 > u; ++u) e.x[u] = e.v[u], e.O[u] = z(e.b, -1, e.x[u], 273), e.O[u] > e.O[Z] && (Z = u); if (e.O[Z] >= e.n) return e.nb = Z, p = e.O[Z], Ir(e, p - 1), p; if (d >= e.n) return e.nb = e.m[C - 1] + 4, Ir(e, d - 1), d; if (a = y(e.b, -1), v = y(e.b, -e.v[0] - 1 - 1), 2 > d && a != v && 2 > e.O[Z]) return e.nb = -1, 1; if (e.a[0].Hc = e.l, A = r & e.y, e.a[1].z = Vt[e.C[(e.l << 4) + A] >>> 2] + nt(Xr(e.A, r, e.J), e.l >= 7, v, a), st(e.a[1]), B = Vt[2048 - e.C[(e.l << 4) + A] >>> 2], Y = B + Vt[2048 - e.bb[e.l] >>> 2], v == a && (V = Y + zr(e, e.l, A), e.a[1].z > V && (e.a[1].z = V, it(e.a[1]))), m = d >= e.O[Z] ? d : e.O[Z], 2 > m) return e.nb = e.a[1].j, 1; e.a[1].r = 0, e.a[0].Yb = e.x[0], e.a[0].Zb = e.x[1], e.a[0].$b = e.x[2], e.a[0].pc = e.x[3], f = m; do e.a[f--].z = 268435455; while (f >= 2); for (u = 0; 4 > u; ++u) if (T = e.O[u], !(2 > T)) { G = Y + yr(e, u, e.l, A); do s = G + Jr(e.i, T - 2, A), x = e.a[T], x.z > s && (x.z = s, x.r = 0, x.j = u, x.t = 0); while (--T >= 2) } if (D = B + Vt[e.bb[e.l] >>> 2], f = e.O[0] >= 2 ? e.O[0] + 1 : 2, d >= f) { for (E = 0; f > e.m[E];) E += 2; for (; c = e.m[E + 1], s = D + Lr(e, c, f, A), x = e.a[f], x.z > s && (x.z = s, x.r = 0, x.j = c + 4, x.t = 0), f != e.m[E] || (E += 2, E != C) ; ++f); } for (t = 0; ;) { if (++t, t == m) return gr(e, t); if (S = xr(e), C = e.D, S >= e.n) return e.vb = S, e.Q = 1, gr(e, t); if (++r, O = e.a[t].r, e.a[t].t ? (--O, e.a[t].yc ? (j = e.a[e.a[t].r2].Hc, j = 4 > e.a[t].j2 ? 7 > j ? 8 : 11 : 7 > j ? 7 : 10) : j = e.a[O].Hc, j = U(j)) : j = e.a[O].Hc, O == t - 1 ? j = e.a[t].j ? U(j) : 7 > j ? 9 : 11 : (e.a[t].t && e.a[t].yc ? (O = e.a[t].r2, N = e.a[t].j2, j = 7 > j ? 8 : 11) : (N = e.a[t].j, j = 4 > N ? 7 > j ? 8 : 11 : 7 > j ? 7 : 10), I = e.a[O], 4 > N ? N ? 1 == N ? (e.x[0] = I.Zb, e.x[1] = I.Yb, e.x[2] = I.$b, e.x[3] = I.pc) : 2 == N ? (e.x[0] = I.$b, e.x[1] = I.Yb, e.x[2] = I.Zb, e.x[3] = I.pc) : (e.x[0] = I.pc, e.x[1] = I.Yb, e.x[2] = I.Zb, e.x[3] = I.$b) : (e.x[0] = I.Yb, e.x[1] = I.Zb, e.x[2] = I.$b, e.x[3] = I.pc) : (e.x[0] = N - 4, e.x[1] = I.Yb, e.x[2] = I.Zb, e.x[3] = I.$b)), e.a[t].Hc = j, e.a[t].Yb = e.x[0], e.a[t].Zb = e.x[1], e.a[t].$b = e.x[2], e.a[t].pc = e.x[3], _ = e.a[t].z, a = y(e.b, -1), v = y(e.b, -e.x[0] - 1 - 1), A = r & e.y, o = _ + Vt[e.C[(j << 4) + A] >>> 2] + nt(Xr(e.A, r, y(e.b, -2)), j >= 7, v, a), R = e.a[t + 1], g = 0, R.z > o && (R.z = o, R.r = t, R.j = -1, R.t = 0, g = 1), B = _ + Vt[2048 - e.C[(j << 4) + A] >>> 2], Y = B + Vt[2048 - e.bb[j] >>> 2], v != a || t > R.r && !R.j || (V = Y + (Vt[e.gb[j] >>> 2] + Vt[e.Z[(j << 4) + A] >>> 2]), R.z >= V && (R.z = V, R.r = t, R.j = 0, R.t = 0, g = 1)), w = F(e.b) + 1, w = w > 4095 - t ? 4095 - t : w, b = w, !(2 > b)) { if (b > e.n && (b = e.n), !g && v != a && (q = Math.min(w - 1, e.n), P = z(e.b, 0, e.x[0], q), P >= 2)) { for (K = U(j), H = r + 1 & e.y, M = o + Vt[2048 - e.C[(K << 4) + H] >>> 2] + Vt[2048 - e.bb[K] >>> 2], L = t + 1 + P; L > m;) e.a[++m].z = 268435455; s = M + (J = Jr(e.i, P - 2, H), J + yr(e, 0, K, H)), x = e.a[L], x.z > s && (x.z = s, x.r = t + 1, x.j = 0, x.t = 1, x.yc = 0) } for ($ = 2, W = 0; 4 > W; ++W) if (h = z(e.b, -1, e.x[W], b), !(2 > h)) { l = h; do { for (; t + h > m;) e.a[++m].z = 268435455; s = Y + (Q = Jr(e.i, h - 2, A), Q + yr(e, W, j, A)), x = e.a[t + h], x.z > s && (x.z = s, x.r = t, x.j = W, x.t = 0) } while (--h >= 2); if (h = l, W || ($ = h + 1), w > h && (q = Math.min(w - 1 - h, e.n), P = z(e.b, h, e.x[W], q), P >= 2)) { for (K = 7 > j ? 8 : 11, H = r + h & e.y, n = Y + (X = Jr(e.i, h - 2, A), X + yr(e, W, j, A)) + Vt[e.C[(K << 4) + H] >>> 2] + nt(Xr(e.A, r + h, y(e.b, h - 1 - 1)), 1, y(e.b, h - 1 - (e.x[W] + 1)), y(e.b, h - 1)), K = U(K), H = r + h + 1 & e.y, k = n + Vt[2048 - e.C[(K << 4) + H] >>> 2], M = k + Vt[2048 - e.bb[K] >>> 2], L = h + 1 + P; t + L > m;) e.a[++m].z = 268435455; s = M + (er = Jr(e.i, P - 2, H), er + yr(e, 0, K, H)), x = e.a[t + L], x.z > s && (x.z = s, x.r = t + h + 1, x.j = 0, x.t = 1, x.yc = 1, x.r2 = t, x.j2 = W) } } if (S > b) { for (S = b, C = 0; S > e.m[C]; C += 2); e.m[C] = S, C += 2 } if (S >= $) { for (D = B + Vt[e.bb[j] >>> 2]; t + S > m;) e.a[++m].z = 268435455; for (E = 0; $ > e.m[E];) E += 2; for (h = $; ; ++h) if (i = e.m[E + 1], s = D + Lr(e, i, h, A), x = e.a[t + h], x.z > s && (x.z = s, x.r = t, x.j = i + 4, x.t = 0), h == e.m[E]) { if (w > h && (q = Math.min(w - 1 - h, e.n), P = z(e.b, h, i, q), P >= 2)) { for (K = 7 > j ? 7 : 10, H = r + h & e.y, n = s + Vt[e.C[(K << 4) + H] >>> 2] + nt(Xr(e.A, r + h, y(e.b, h - 1 - 1)), 1, y(e.b, h - (i + 1) - 1), y(e.b, h - 1)), K = U(K), H = r + h + 1 & e.y, k = n + Vt[2048 - e.C[(K << 4) + H] >>> 2], M = k + Vt[2048 - e.bb[K] >>> 2], L = h + 1 + P; t + L > m;) e.a[++m].z = 268435455; s = M + (rr = Jr(e.i, P - 2, H), rr + yr(e, 0, K, H)), x = e.a[t + L], x.z > s && (x.z = s, x.r = t + h + 1, x.j = 0, x.t = 1, x.yc = 1, x.r2 = t, x.j2 = i + 4) } if (E += 2, E == C) break } } } } } function Lr(e, r, t, o) { var n, s = Q(t); return n = 128 > r ? e.hb[128 * s + r] : e.R[(s << 6) + Yr(r)] + e.mc[15 & r], n + Jr(e._, t - 2, o) } function yr(e, r, t, o) { var n; return r ? (n = Vt[2048 - e.gb[t] >>> 2], 1 == r ? n += Vt[e.Ob[t] >>> 2] : (n += Vt[2048 - e.Ob[t] >>> 2], n += Ct(e.wc[t], r - 2))) : (n = Vt[e.gb[t] >>> 2], n += Vt[2048 - e.Z[(t << 4) + o] >>> 2]), n } function zr(e, r, t) { return Vt[e.gb[r] >>> 2] + Vt[e.Z[(r << 4) + t] >>> 2] } function Fr(e) { kr(e), bt(e.d), gt(e.C), gt(e.Z), gt(e.bb), gt(e.gb), gt(e.Ob), gt(e.wc), gt(e.Lb), et(e.A); for (var r = 0; 4 > r; ++r) gt(e.K[r].G); $r(e._, 1 << e.Y), $r(e.i, 1 << e.Y), gt(e.U.G), e.Q = 0, e.lb = 0, e.q = 0, e.s = 0 } function Ir(e, r) { r > 0 && (Z(e.b, r), e.s += r) } function xr(e) { var r = 0; return e.D = H(e.b, e.m), e.D > 0 && (r = e.m[e.D - 2], r == e.n && (r += z(e.b, r - 1, e.m[e.D - 1], 273 - r))), ++e.s, r } function Nr(e) { e.b && e.S && (e.b.bc = null, e.S = 0) } function Or(e) { Nr(e), e.d.zb = null } function Ar(e, r) { e.ab = r; for (var t = 0; r > 1 << t; ++t); e.cc = 2 * t } function Hr(e, r) { var t = e.T; e.T = r, e.b && t != e.T && (e.wb = -1, e.b = null) } function Gr(e, r) { e.fc[0] = 9 * (5 * e.Y + e.eb) + e.fb << 24 >> 24; for (var t = 0; 4 > t; ++t) e.fc[1 + t] = e.ab >> 8 * t << 24 >> 24; k(r, e.fc, 0, 5) } function Wr(e, r) { if (e.Gc) { kt(e.d, e.C, (e.l << 4) + r, 1), kt(e.d, e.bb, e.l, 0), e.l = 7 > e.l ? 7 : 10, Kr(e._, e.d, 0, r); var t = Q(2); mt(e.K[t], e.d, 63), Rt(e.d, 67108863, 26), pt(e.U, e.d, 15) } } function Tr(e) { return 2048 > e ? Zt[e] : 2097152 > e ? Zt[e >> 10] + 20 : Zt[e >> 20] + 40 } function Yr(e) { return 131072 > e ? Zt[e >> 6] + 12 : 134217728 > e ? Zt[e >> 16] + 32 : Zt[e >> 26] + 52 } function Zr(e, r, t, o) { 8 > t ? (kt(r, e.db, 0, 0), mt(e.Tb[o], r, t)) : (t -= 8, kt(r, e.db, 0, 1), 8 > t ? (kt(r, e.db, 1, 0), mt(e.Xb[o], r, t)) : (kt(r, e.db, 1, 1), mt(e.dc, r, t - 8))) } function Vr(e) { e.db = t(2), e.Tb = t(16), e.Xb = t(16), e.dc = ft({}, 8); for (var r = 0; 16 > r; ++r) e.Tb[r] = ft({}, 3), e.Xb[r] = ft({}, 3); return e } function $r(e, r) { gt(e.db); for (var t = 0; r > t; ++t) gt(e.Tb[t].G), gt(e.Xb[t].G); gt(e.dc.G) } function jr(e, r, t, o, n) { var s, i, _, a, c; for (s = Vt[e.db[0] >>> 2], i = Vt[2048 - e.db[0] >>> 2], _ = i + Vt[e.db[1] >>> 2], a = i + Vt[2048 - e.db[1] >>> 2], c = 0, c = 0; 8 > c; ++c) { if (c >= t) return; o[n + c] = s + dt(e.Tb[r], c) } for (; 16 > c; ++c) { if (c >= t) return; o[n + c] = _ + dt(e.Xb[r], c - 8) } for (; t > c; ++c) o[n + c] = a + dt(e.dc, c - 8 - 8) } function Kr(e, r, t, o) { Zr(e, r, t, o), 0 == --e.nc[o] && (jr(e, o, e.pb, e.Cc, 272 * o), e.nc[o] = e.pb) } function qr(e) { return Vr(e), e.Cc = [], e.nc = [], e } function Jr(e, r, t) { return e.Cc[272 * t + r] } function Qr(e, r) { for (var t = 0; r > t; ++t) jr(e, t, e.pb, e.Cc, 272 * t), e.nc[t] = e.pb } function Ur(e, r, o) { var n, s; if (null == e.V || e.u != o || e.I != r) for (e.I = r, e.oc = (1 << r) - 1, e.u = o, s = 1 << e.u + e.I, e.V = t(s), n = 0; s > n; ++n) e.V[n] = ot({}) } function Xr(e, r, t) { return e.V[((r & e.oc) << e.u) + ((255 & t) >>> 8 - e.u)] } function et(e) { var r, t = 1 << e.u + e.I; for (r = 0; t > r; ++r) gt(e.V[r].ob) } function rt(e, r, t) { var o, n, s = 1; for (n = 7; n >= 0; --n) o = t >> n & 1, kt(r, e.ob, s, o), s = s << 1 | o } function tt(e, r, t, o) { var n, s, i, _, a = 1, c = 1; for (s = 7; s >= 0; --s) n = o >> s & 1, _ = c, a && (i = t >> s & 1, _ += 1 + i << 8, a = i == n), kt(r, e.ob, _, n), c = c << 1 | n } function ot(e) { return e.ob = t(768), e } function nt(e, r, t, o) { var n, s, i = 1, _ = 7, a = 0; if (r) for (; _ >= 0; --_) if (s = t >> _ & 1, n = o >> _ & 1, a += Ct(e.ob[(1 + s << 8) + i], n), i = i << 1 | n, s != n) { --_; break } for (; _ >= 0; --_) n = o >> _ & 1, a += Ct(e.ob[i], n), i = i << 1 | n; return a } function st(e) { e.j = -1, e.t = 0 } function it(e) { e.j = 0, e.t = 0 } function _t(e, r) { return e.F = r, e.G = t(1 << r), e } function at(e, r) { var t, o = 1; for (t = e.F; 0 != t; --t) o = (o << 1) + vt(r, e.G, o); return o - (1 << e.F) } function ct(e, r) { var t, o, n = 1, s = 0; for (o = 0; e.F > o; ++o) t = vt(r, e.G, n), n <<= 1, n += t, s |= t << o; return s } function ut(e, r, t, o) { var n, s, i = 1, _ = 0; for (s = 0; o > s; ++s) n = vt(t, e, r + i), i <<= 1, i += n, _ |= n << s; return _ } function ft(e, r) { return e.F = r, e.G = t(1 << r), e } function mt(e, r, t) { var o, n, s = 1; for (n = e.F; 0 != n;)--n, o = t >>> n & 1, kt(r, e.G, s, o), s = s << 1 | o } function dt(e, r) { var t, o, n = 1, s = 0; for (o = e.F; 0 != o;)--o, t = r >>> o & 1, s += Ct(e.G[n], t), n = (n << 1) + t; return s } function pt(e, r, t) { var o, n, s = 1; for (n = 0; e.F > n; ++n) o = 1 & t, kt(r, e.G, s, o), s = s << 1 | o, t >>= 1 } function ht(e, r) { var t, o, n = 1, s = 0; for (o = e.F; 0 != o; --o) t = 1 & r, r >>>= 1, s += Ct(e.G[n], t), n = n << 1 | t; return s } function Pt(e, r, t, o, n) { var s, i, _ = 1; for (i = 0; o > i; ++i) s = 1 & n, kt(t, e, r + _, s), _ = _ << 1 | s, n >>= 1 } function lt(e, r, t, o) { var n, s, i = 1, _ = 0; for (s = t; 0 != s; --s) n = 1 & o, o >>>= 1, _ += Vt[(2047 & (e[r + i] - n ^ -n)) >>> 2], i = i << 1 | n; return _ } function vt(e, r, t) { var o, n = r[t]; return o = (e.E >>> 11) * n, (-2147483648 ^ o) > (-2147483648 ^ e.Ab) ? (e.E = o, r[t] = n + (2048 - n >>> 5) << 16 >> 16, -16777216 & e.E || (e.Ab = e.Ab << 8 | l(e.zb), e.E <<= 8), 0) : (e.E -= o, e.Ab -= o, r[t] = n - (n >>> 5) << 16 >> 16, -16777216 & e.E || (e.Ab = e.Ab << 8 | l(e.zb), e.E <<= 8), 1) } function Bt(e, r) { var t, o, n = 0; for (t = r; 0 != t; --t) e.E >>>= 1, o = e.Ab - e.E >>> 31, e.Ab -= e.E & o - 1, n = n << 1 | 1 - o, -16777216 & e.E || (e.Ab = e.Ab << 8 | l(e.zb), e.E <<= 8); return n } function St(e) { e.Ab = 0, e.E = -1; for (var r = 0; 5 > r; ++r) e.Ab = e.Ab << 8 | l(e.zb) } function gt(e) { for (var r = e.length - 1; r >= 0; --r) e[r] = 1024 } function kt(e, r, t, s) { var i, _ = r[t]; i = (e.E >>> 11) * _, s ? (e.Ac = o(e.Ac, n(a(i), [4294967295, 0])), e.E -= i, r[t] = _ - (_ >>> 5) << 16 >> 16) : (e.E = i, r[t] = _ + (2048 - _ >>> 5) << 16 >> 16), -16777216 & e.E || (e.E <<= 8, wt(e)) } function Rt(e, r, t) { for (var n = t - 1; n >= 0; --n) e.E >>>= 1, 1 == (r >>> n & 1) && (e.Ac = o(e.Ac, a(e.E))), -16777216 & e.E || (e.E <<= 8, wt(e)) } function Mt(e) { for (var r = 0; 5 > r; ++r) wt(e) } function Dt(e) { return o(o(a(e.Ib), e.qc), [4, 0]) } function bt(e) { e.qc = Wt, e.Ac = Wt, e.E = -1, e.Ib = 1, e.Oc = 0 } function wt(e) { var r, t = c(p(e.Ac, 32)); if (0 != t || s(e.Ac, [4278190080, 0]) < 0) { e.qc = o(e.qc, a(e.Ib)), r = e.Oc; do g(e.zb, r + t), r = 255; while (0 != --e.Ib); e.Oc = c(e.Ac) >>> 24 } ++e.Ib, e.Ac = m(n(e.Ac, [16777215, 0]), 8) } function Ct(e, r) { return Vt[(2047 & (e - r ^ -r)) >>> 2] } function Et(e) {
		var r, t, o, n, s = "", i = e.length; for (r = 0; i > r; ++r) if (t = 255 & e[r], 128 & t) if (192 == (224 & t)) {
			if (r + 1 >= e.length) return e; if (o = 255 & e[++r], 128 != (192 & o)) return e; s += String.fromCharCode((31 & t) << 6 & 65535 | 63 & o)
		} else { if (224 != (240 & t)) return e; if (r + 2 >= e.length) return e; if (o = 255 & e[++r], 128 != (192 & o)) return e; if (n = 255 & e[++r], 128 != (192 & n)) return e; s += String.fromCharCode(65535 & ((15 & t) << 12 | (63 & o) << 6 | 63 & n)) } else { if (!t) return e; s += String.fromCharCode(65535 & t) } return s
	} function Lt(e) { var r, t, o, n = [], s = 0, i = e.length; if ("object" == typeof e) return e; for (R(e, 0, i, n, 0), o = 0; i > o; ++o) r = n[o], r >= 1 && 127 >= r ? ++s : s += !r || r >= 128 && 2047 >= r ? 2 : 3; for (t = [], s = 0, o = 0; i > o; ++o) r = n[o], r >= 1 && 127 >= r ? t[s++] = r << 24 >> 24 : !r || r >= 128 && 2047 >= r ? (t[s++] = (192 | r >> 6 & 31) << 24 >> 24, t[s++] = (128 | 63 & r) << 24 >> 24) : (t[s++] = (224 | r >> 12 & 15) << 24 >> 24, t[s++] = (128 | r >> 6 & 63) << 24 >> 24, t[s++] = (128 | 63 & r) << 24 >> 24); return t } function yt(e) { return e[1] + e[0] } function zt(e, t, o, n) { function s() { for (var e, t = (new Date).getTime() ; rr(a.c.ac) ;) if (i = yt(a.c.ac.Sb) / yt(a.c.Nb), (new Date).getTime() - t > 200) return n ? n(i) : void 0 !== _ && r(i, _), Ot(s, 0), 0; n ? n(1) : void 0 !== _ && r(1, _), e = S(a.c.rc), o ? o(e) : void 0 !== _ && postMessage({ action: It, cbn: _, result: e }) } var i, _, a = {}; "function" != typeof o && (_ = o, o = n = 0), a.c = w({}, Lt(e), $t(t)), n ? n(0) : void 0 !== _ && r(0, _), Ot(s, 0) } function Ft(e, t, o) { function n() { for (var e, u = 0, f = (new Date).getTime() ; rr(c.d.ac) ;) if (++u % 1e3 == 0 && (new Date).getTime() - f > 200) return _ && (s = yt(c.d.ac.$.g) / a, o ? o(s) : void 0 !== i && r(s, i)), Ot(n, 0), 0; _ && (o ? o(1) : void 0 !== i && r(1, i)), e = Et(S(c.d.rc)), t ? t(e) : void 0 !== i && postMessage({ action: xt, cbn: i, result: e }) } var s, i, _, a, c = {}; "function" != typeof t && (i = t, t = o = 0), c.d = E({}, e), a = yt(c.d.Nb), _ = a > -1, o ? o(_ ? 0 : -1) : void 0 !== i && r(_ ? 0 : -1, i), Ot(n, 0) } var It = 1, xt = 2, Nt = 3, Ot = "function" == typeof setImmediate ? setImmediate : setTimeout, At = 4294967296, Ht = [4294967295, -At], Gt = [0, -0x8000000000000000], Wt = [0, 0], Tt = [1, 0], Yt = function () { var e, r, t, o = []; for (e = 0; 256 > e; ++e) { for (t = e, r = 0; 8 > r; ++r) 0 != (1 & t) ? t = t >>> 1 ^ -306674912 : t >>>= 1; o[e] = t } return o }(), Zt = function () { var e, r, t, o = 2, n = [0, 1]; for (t = 2; 22 > t; ++t) for (r = 1 << (t >> 1) - 1, e = 0; r > e; ++e, ++o) n[o] = t << 24 >> 24; return n }(), Vt = function () { var e, r, t, o, n = []; for (r = 8; r >= 0; --r) for (o = 1 << 9 - r - 1, e = 1 << 9 - r, t = o; e > t; ++t) n[t] = (r << 6) + (e - t << 6 >>> 9 - r - 1); return n }(), $t = function () { var e = [{ s: 16, f: 64, m: 0 }, { s: 20, f: 64, m: 0 }, { s: 19, f: 64, m: 1 }, { s: 20, f: 64, m: 1 }, { s: 21, f: 128, m: 1 }, { s: 22, f: 128, m: 1 }, { s: 23, f: 128, m: 1 }, { s: 24, f: 255, m: 1 }, { s: 25, f: 255, m: 1 }]; return function (r) { return e[r - 1] || e[6] } }(); return "undefined" == typeof onmessage || "undefined" != typeof window && void 0 !== window.document || !function () { onmessage = function (r) { r && r.ic && (r.ic.action == xt ? e.decompress(r.ic.ic, r.ic.cbn) : r.ic.action == It && e.compress(r.ic.ic, r.ic.Rc, r.ic.cbn)) } }(), { compress: zt, decompress: Ft }
}(); this.LZMA = this.LZMA_WORKER = e;


function LZMA_compress(obj, onfinish) {
	var str = JSON.stringify(obj);
	LZMA.compress(str, 1, function (result) {
		var binary = '';
		for (var i = 0; i < result.length; i++) {
			if (result[i] < 0)
				binary += String.fromCharCode(256 + result[i]);
			else
				binary += String.fromCharCode(result[i]);
		}
		var base64 = window.btoa(binary);
		//var p = str.length + " -> " + base64.length + " : " + Math.round(base64.length * 100 / str.length, 2);
		//alert(p);
		onfinish(base64);
	});
}
function LZMA_decompress(base64, onfinish) {
	var str = window.atob(base64);
	var bytes = [];
	for (var i = 0; i < str.length; i++) {
		bytes[i] = str.charCodeAt(i);
	}

	LZMA.decompress(bytes, function (result) {
		//var p = result.length + " -> " + base64.length + " : " + Math.round(base64.length * 100 / result.length, 2);
		//alert(p);
		onfinish(JSON.parse(result));
	});
}
function LZMA_decompress2(base64, onfinish) {
	var str = window.atob(base64);
	var bytes = [];
	for (var i = 0; i < str.length; i++) {
		bytes[i] = str.charCodeAt(i);
	}

	LZMA.decompress(bytes, function (result) {
		onfinish(result);
	});
}

function copyToClipboard(elem) {
	// create hidden text element, if it doesn't already exist
	var targetId = "_hiddenCopyText_";
	var isInput = elem.tagName === "INPUT" || elem.tagName === "TEXTAREA";
	var origSelectionStart, origSelectionEnd;
	if (isInput) {
		// can just use the original source element for the selection and copy
		target = elem;
		origSelectionStart = elem.selectionStart;
		origSelectionEnd = elem.selectionEnd;
	} else {
		// must use a temporary form element for the selection and copy
		target = document.getElementById(targetId);
		if (!target) {
			var target = document.createElement("textarea");
			target.style.position = "absolute";
			target.style.left = "-9999px";
			target.style.top = "0";
			target.id = targetId;
			document.body.appendChild(target);
		}
		target.textContent = elem.textContent;
	}
	// select the content
	var currentFocus = document.activeElement;
	target.focus();
	target.setSelectionRange(0, target.value.length);

	// copy the selection
	var succeed;
	try {
		succeed = document.execCommand("copy");
	} catch (e) {
		succeed = false;
	}
	// restore original focus
	if (currentFocus && typeof currentFocus.focus === "function") {
		currentFocus.focus();
	}

	if (isInput) {
		// restore prior selection
		elem.setSelectionRange(origSelectionStart, origSelectionEnd);
	} else {
		// clear temporary content
		target.textContent = "";
	}
	return succeed;
}

function createTree(args) {
	var tree = {
		container: document.getElementById(args.containerId),
		children: [],
		selected: null,
		init: function () {
			this.container.innerHTML = "";
			enableDocumentSelection(false, tree);
		},
		createNode: function (node) {
			var container = this.getChildrenContainer(node.parent);
			var parents = [];
			var parent = node.parent;
			while (parent != null) {
				parents.splice(0, 0, parent);
				parent = parent.parent;
			}
			node.element = document.createElement("li");
			node.element.tabIndex = 0;
			node.element.node = node;
			node.childrenContainer = document.createElement("ul");
			for (var i = 0; i < node.level; i++) {
				var div = document.createElement("div");
				div.className = "rg-fields-tree-line";
				node.element.appendChild(div);

				if (node.parent == null)
					$(div).addClass(node.isFirstNode ? "bb" : "bbr");
				else
					$(div).addClass((i == node.level - 1 ? "bbr" : (parents[i].isLastNode ? "bnone" : "br")));
			}
			$(node.element).append("<div class='button'></div>");
			if (node.imageCssClass)
				$(node.element).append("<div class='image " + node.imageCssClass + "'></div>");
			$(node.element).append("<div class='text " + node.cssClass + "'>" + node.caption + "</div>");

			if (node.children.length == 0) {
				$(">div.button", node.element).addClass("none");
			}
			if (this.selected == node)
				$(">div.text", node.element).addClass("selected");
			container.appendChild(node.element);
			node.element.appendChild(node.childrenContainer);

			enableDocumentSelection(false, node.element);
			enableDocumentSelection(false, node.childrenContainer);

			$(node.childrenContainer).hide();
			$(">div.button", node.element).on('click', function (e) {
				if (e.target.parentNode.node == node) {
					if (node.expanded)
						tree.collapse(node);
					else
						tree.exapnd(node);
				}
			});
			$(">div.text", node.element).on('mousedown', function (e) {
				if (e.target.parentNode.node == node) {
					tree.select(node);
					if (args.onBeginDrag)
						args.onBeginDrag(node, e);
				}
			});
			$(">div.text", node.element).on('click', function (e) {
				if (e.target.parentNode.node == node) {
					if (args.onClick)
						args.onClick(node);
				}
			});
			$(">div.text", node.element).on('dblclick', function (e) {
				if (e.target.parentNode.node == node) {
					if (args.onDblClick)
						args.onDblClick(node);
				}
			});
		},
		getChildrenContainer: function (node) {
			return node == null ? this.container : node.childrenContainer;
		},
		clearNodes: function (node) {
			var container = this.getChildrenContainer(node);
			if (container)
				container.innerHTML = "";
		},
		update: function (parent, list) {
			var i;
			for (i = 0; i < list.length; i++) {
				this.addNode(parent, list[i], false);
			}
			if (parent) {
				parent.loaded = true;
				if (list.length == 0) {
					$(">button", parent.element).addClass("none");
				}
			}

			this.clearNodes(parent);
			var arr = parent == null ? this.children : parent.children;
			for (i = 0; i < arr.length; i++) {
				this.createNode(arr[i]);
			}
		},
		exapnd: function (node) {
			if (!node)
				return;
			if (node.children.length == 0)
				return;
			$(node.childrenContainer).slideDown();
			node.expanded = true;
			$(node.element).addClass("expanded");
		},
		collapse: function (node) {
			if (!node)
				return;
			if (node.children.length == 0)
				return;
			$(node.childrenContainer).slideUp();
			node.expanded = false;
			$(node.element).removeClass("expanded");
		},
		select: function (node) {
			$(".rg-fields-tree div.text").removeClass("selected");
			if (node)
				$(">div.text", node.element).addClass("selected");
			this.selected = node;
			if (args.onSelect)
				args.onSelect(node);
		},
		addNode: function (parent, tag, title, cssClass, imageCssClass) {
			var arr = parent == null ? this.children : parent.children;
			var node = {
				parent: parent,
				caption: title,
				cssClass: cssClass,
				imageCssClass: imageCssClass,
				tag: tag,
				expanded: false,
				children: [],
				isFirstNode: arr.length == 0,
				isLastNode: true,
				level: parent == null ? 1 : parent.level + 1
			};
			if (arr.length != 0)
				arr[arr.length - 1].isLastNode = false;
			arr.push(node);

			if (parent)
				$(">div.button", parent.element).removeClass("none");
			this.createNode(node);
			return node;
		},
		editNode: function (node, tag, title, cssClass, imageCssClass) {
			$(">div.text", node.element).html(title);
			$(">div.image", node.element).removeClass(node.imageCssClass);
			$(">div.image", node.element).addClass(imageCssClass);
			$(">div.image", node.element).removeClass(node.cssClass);
			$(">div.image", node.element).addClass(cssClass);

			node.tag = tag;
			node.caption = title;
			node.imageCssClass = imageCssClass;
			node.cssClass = cssClass;
		},
		deleteNode: function (node) {
			var arr = node.parent == null ? this.children : node.parent.children;
			for (var i = 0; i < arr.length; i++) {
				if (arr[i] == node) {
					arr.splice(i, 1);

					node.element.parentNode.removeChild(node.element);
					if (node == this.selected)
						this.selected = null;
					return;
				}
			}
		},
		removeChildren: function (parent) {
			parent.children = [];
			this.clearNodes(parent);
			$(">div.button", parent.element).addClass("none");
		}
	};
	return tree;
}

function createClone(obj) {
	function stringify(o) {
		return JSON.stringify(o, function (key, value) { return value === "" ? "" : value; });
	};
	function parse(str) {
		return JSON.parse(str);
	};

	var clone = stringify(obj);
	return parse(clone);
}

function htmlEncode(value) {
	return $('<div/>').text(value).html();
}

function htmlDecode(value) {
	return $('<div/>').html(value).text();
}

var uniqueIdGenerator = {
	index: 0,
	create: function () {
		return "____unique__id_" + this.index++;
	},
	randomId: function () {
		return "" + parseInt(Math.random() * 100000000) + parseInt(Math.random() * 100000000);
	}
};



function b64toBlob(b64Data, contentType, sliceSize) {
	contentType = contentType || '';
	sliceSize = sliceSize || 512;

	var byteCharacters = atob(b64Data);
	var byteArrays = [];

	for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
		var slice = byteCharacters.slice(offset, offset + sliceSize);

		var byteNumbers = new Array(slice.length);
		for (var i = 0; i < slice.length; i++) {
			byteNumbers[i] = slice.charCodeAt(i);
		}

		var byteArray = new Uint8Array(byteNumbers);

		byteArrays.push(byteArray);
	}

	var blob = new Blob(byteArrays, { type: contentType });
	return blob;
}

// download.js
(function (root, factory) {
	if (typeof define === 'function' && define.amd) {
		// AMD. Register as an anonymous module.
		define([], factory);
	} else if (typeof exports === 'object') {
		// Node. Does not work with strict CommonJS, but
		// only CommonJS-like environments that support module.exports,
		// like Node.
		module.exports = factory();
	} else {
		// Browser globals (root is window)
		root.download = factory();
	}
}(this, function () {

	return function download(data, strFileName, strMimeType) {

		var self = window, // this script is only for browsers anyway...
			defaultMime = "application/octet-stream", // this default mime also triggers iframe downloads
			mimeType = strMimeType || defaultMime,
			payload = data,
			url = !strFileName && !strMimeType && payload,
			anchor = document.createElement("a"),
			toString = function (a) { return String(a); },
			myBlob = (self.Blob || self.MozBlob || self.WebKitBlob || toString),
			fileName = strFileName || "download",
			blob,
			reader;
		myBlob = myBlob.call ? myBlob.bind(self) : Blob;

		if (String(this) === "true") { //reverse arguments, allowing download.bind(true, "text/xml", "export.xml") to act as a callback
			payload = [payload, mimeType];
			mimeType = payload[0];
			payload = payload[1];
		}


		if (url && url.length < 2048) { // if no filename and no mime, assume a url was passed as the only argument
			fileName = url.split("/").pop().split("?")[0];
			anchor.href = url; // assign href prop to temp anchor
			if (anchor.href.indexOf(url) !== -1) { // if the browser determines that it's a potentially valid url path:
				var ajax = new XMLHttpRequest();
				ajax.open("GET", url, true);
				ajax.responseType = 'blob';
				ajax.onload = function (e) {
					download(e.target.response, fileName, defaultMime);
				};
				setTimeout(function () { ajax.send(); }, 0); // allows setting custom ajax headers using the return:
				return ajax;
			} // end if valid url?
		} // end if url?


		//go ahead and download dataURLs right away
		if (/^data\:[\w+\-]+\/[\w+\-]+[,;]/.test(payload)) {

			if (payload.length > (1024 * 1024 * 1.999) && myBlob !== toString) {
				payload = dataUrlToBlob(payload);
				mimeType = payload.type || defaultMime;
			} else {
				return navigator.msSaveBlob ?  // IE10 can't do a[download], only Blobs:
					navigator.msSaveBlob(dataUrlToBlob(payload), fileName) :
					saver(payload); // everyone else can save dataURLs un-processed
			}

		}//end if dataURL passed?

		blob = payload instanceof myBlob ?
			payload :
			new myBlob([payload], { type: mimeType });


		function dataUrlToBlob(strUrl) {
			var parts = strUrl.split(/[:;,]/),
			type = parts[1],
			decoder = parts[2] == "base64" ? atob : decodeURIComponent,
			binData = decoder(parts.pop()),
			mx = binData.length,
			i = 0,
			uiArr = new Uint8Array(mx);

			for (i; i < mx; ++i) uiArr[i] = binData.charCodeAt(i);

			return new myBlob([uiArr], { type: type });
		}

		function saver(url, winMode) {

			if ('download' in anchor) { //html5 A[download]
				anchor.href = url;
				anchor.setAttribute("download", fileName);
				anchor.className = "download-js-link";
				anchor.innerHTML = "downloading...";
				anchor.style.display = "none";
				document.body.appendChild(anchor);
				setTimeout(function () {
					anchor.click();
					document.body.removeChild(anchor);
					if (winMode === true) { setTimeout(function () { self.URL.revokeObjectURL(anchor.href); }, 250); }
				}, 66);
				return true;
			}

			// handle non-a[download] safari as best we can:
			if (/(Version)\/(\d+)\.(\d+)(?:\.(\d+))?.*Safari\//.test(navigator.userAgent)) {
				url = url.replace(/^data:([\w\/\-\+]+)/, defaultMime);
				if (!window.open(url)) { // popup blocked, offer direct download:
					if (confirm("Displaying New Document\n\nUse Save As... to download, then click back to return to this page.")) { location.href = url; }
				}
				return true;
			}

			//do iframe dataURL download (old ch+FF):
			var f = document.createElement("iframe");
			document.body.appendChild(f);

			if (!winMode) { // force a mime that will download:
				url = "data:" + url.replace(/^data:([\w\/\-\+]+)/, defaultMime);
			}
			f.src = url;
			setTimeout(function () { document.body.removeChild(f); }, 333);

		}//end saver




		if (navigator.msSaveBlob) { // IE10+ : (has Blob, but not a[download] or URL)
			return navigator.msSaveBlob(blob, fileName);
		}

		if (self.URL) { // simple fast and modern way using Blob and URL:
			saver(self.URL.createObjectURL(blob), true);
		} else {
			// handle non-Blob()+non-URL browsers:
			if (typeof blob === "string" || blob.constructor === toString) {
				try {
					return saver("data:" + mimeType + ";base64," + self.btoa(blob));
				} catch (y) {
					return saver("data:" + mimeType + "," + encodeURIComponent(blob));
				}
			}

			// Blob but not URL support:
			reader = new FileReader();
			reader.onload = function (e) {
				saver(this.result);
			};
			reader.readAsDataURL(blob);
		}
		return true;
	}; /* end download() */
}));



// date methods
function gregorian_to_jalali(gy, gm, gd) {
	var d1 = 365 * 4 + 4 / 4;
	var d3 = 365 * 33 + 32 / 4;
	var g_d_m = [0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334];
	var jy = (gy <= 1600) ? 0 : 979;
	gy -= (gy <= 1600) ? 621 : 1600;
	var gy2 = (gm > 2) ? (gy + 1) : gy;
	var days = (365 * gy) + (parseInt((gy2 + 3) / 4)) - (parseInt((gy2 + 99) / 100))
   + (parseInt((gy2 + 399) / 400)) - 80 + gd + g_d_m[gm - 1];
	jy += 33 * (parseInt(days / d3));
	days %= d3;
	jy += 4 * (parseInt(days / d1));
	days %= d1;
	jy += parseInt((days - 1) / 365);
	if (days > 365) days = (days - 1) % 365;
	jm = (days < 186) ? 1 + parseInt(days / 31) : 7 + parseInt((days - 186) / 30);
	jd = 1 + ((days < 186) ? (days % 31) : ((days - 186) % 30));
	return [jy, jm, jd];
}
function jalali_to_gregorian(shamsiDate) {
	var d1 = 365 * 4 + 4 / 4;
	var d2 = 365 * 400 + 400 / 4 - 400 / 100 + 400 / 400;
	var d4 = 365 * 100 + 100 / 4 - 100 / 100;
	var split = shamsiDate.split('/');
	var jy = parseInt(split[0]); var jm = parseInt(split[1]); var jd = parseInt(split[2]);

	gy = (jy <= 979) ? 621 : 1600;
	jy -= (jy <= 979) ? 0 : 979;
	days = (365 * jy) + ((parseInt(jy / 33)) * 8) + (parseInt(((jy % 33) + 3) / 4))
   + 78 + jd + ((jm < 7) ? (jm - 1) * 31 : ((jm - 7) * 30) + 186);
	gy += 400 * (parseInt(days / d2));
	days %= d2;
	if (days > d4) {
		gy += 100 * (parseInt(--days / d4));
		days %= d4;
		if (days >= 365) days++;
	}
	gy += 4 * (parseInt((days) / d1));
	days %= d1;
	gy += parseInt((days - 1) / 365);
	if (days > 365) days = (days - 1) % 365;
	gd = days + 1;
	sal_a = [0, 31, ((gy % 4 == 0 && gy % 100 != 0) || (gy % 400 == 0)) ? 29 : 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
	for (gm = 0; gm < 13; gm++) {
		v = sal_a[gm];
		if (gd <= v) break;
		gd -= v;
	}
	return padleft(gm, 2) + "/" + padleft(gd, 2) + "/" + gy;
}

function padleft(n, width, z) {
	z = z || '0';
	n = n + '';
	return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}


// bootstrap datepicker minified and obfuscated
eval(function (p, a, c, k, e, d) { e = function (c) { return (c < a ? '' : e(parseInt(c / a))) + ((c = c % a) > 35 ? String.fromCharCode(c + 29) : c.toString(36)) }; if (!''.replace(/^/, String)) { while (c--) { d[e(c)] = k[c] || e(c) } k = [function (e) { return d[e] }]; e = function () { return '\\w+' }; c = 1 }; while (c--) { if (k[c]) { p = p.replace(new RegExp('\\b' + e(c) + '\\b', 'g'), k[c]) } } return p }('!f($){f 1N(){k J I(I.5i.3k(I,1O))}f 5h(){c 17=J I();k 1N(17.R(),17.H(),17.Z())}c 1r=f(b,T){c 3X=3;3.b=$(b);3.B=T.B||3.b.C(\'8-B\')||"3e";3.B=3.B 1z 14?3.B:"3e";3.4c=14[3.B].4g||1G;3.P=K.44(T.P||3.b.C(\'8-P\')||\'2q/2v/2J\');3.1U=1G;3.1B=3.b.1q(\'1J\');3.15=3.b.1q(\'.8\')?3.b.q(\'.4P-2o\'):1G;3.3u=3.15&&3.b.q(\'1J\').L;a(3.15&&3.15.L===0)3.15=1G;3.43();3.22=1b;a(\'22\'1z T){3.22=T.22}u a(\'4Y\'1z 3.b.C()){3.22=3.b.C(\'8-4T-4S\')}3.x=$(K.4j).4R(3.1U?3.b:\'4U\').2o({2i:$.1i(3.2i,3),2M:$.1i(3.2M,3)});a(3.1U){3.x.1W(\'G-4V\')}u{3.x.1W(\'G-48 48-4X\')}a(3.4c){3.x.1W(\'G-4g\');3.x.q(\'.1w i, .1L i\').4W(\'47-2L-34 47-2L-3O\')}$(4A).2o(\'2M\',f(e){a($(e.Y).4a(\'.G\').L===0){3X.1o()}});3.1S=1G;a(\'1S\'1z T){3.1S=T.1S}u a(\'4Q\'1z 3.b.C()){3.1S=3.b.C(\'8-1S\')}3.1R=1b;a(\'1R\'1z T){3.1R=T.1R}u a(\'4J\'1z 3.b.C()){3.1R=3.b.C(\'8-4I-4H\')}3.1n=3.2O=0;1k(T.4K||3.b.C(\'8-4L-3B\')){l 2:l\'4O\':3.1n=3.2O=2;F;l 1:l\'h\':3.1n=3.2O=1;F}3.2C=(T.2C||3.b.C(\'8-17-4N\')||1b);3.3t=(T.3t||3.b.C(\'8-17-4M\')||1b);3.1H=((T.1H||3.b.C(\'8-4Z\')||14[3.B].1H||0)%7);3.3J=((3.1H+6)%7);3.O=-U;3.N=U;3.1h=[];3.4m(T.O||3.b.C(\'8-50\'));3.4l(T.N||3.b.C(\'8-5d\'));3.4k(T.1h||3.b.C(\'8-1I-5c-5b-1y\'));3.4r();3.3A();3.1m();3.2d();a(3.1U){3.1s()}};1r.2u={5e:1r,1j:[],43:f(){3.3r();a(3.1B){3.1j=[[3.b,{41:$.1i(3.1s,3),4i:$.1i(3.1m,3),2n:$.1i(3.2n,3)}]]}u a(3.15&&3.3u){3.1j=[[3.b.q(\'1J\'),{41:$.1i(3.1s,3),4i:$.1i(3.1m,3),2n:$.1i(3.2n,3)}],[3.15,{2i:$.1i(3.1s,3)}]]}u a(3.b.1q(\'1l\')){3.1U=1b}u{3.1j=[[3.b,{2i:$.1i(3.1s,3)}]]}1E(c i=0,2e,24;i<3.1j.L;i++){2e=3.1j[i][0];24=3.1j[i][1];2e.2o(24)}},3r:f(){1E(c i=0,2e,24;i<3.1j.L;i++){2e=3.1j[i][0];24=3.1j[i][1];2e.3w(24)}3.1j=[]},1s:f(e){3.x.1s();3.3h=3.15?3.15.2F():3.b.2F();3.1m();3.2I();$(3C).2o(\'4w\',$.1i(3.2I,3));a(e){e.49();e.2b()}3.b.1M({1T:\'1s\',8:3.8})},1o:f(e){a(3.1U)k;3.x.1o();$(3C).3w(\'4w\',3.2I);3.1n=3.2O;3.2d();a(!3.1B){$(4A).3w(\'2M\',3.1o)}a(3.22&&(3.1B&&3.b.Q()||3.3u&&3.b.q(\'1J\').Q()))3.1X();3.b.1M({1T:\'1o\',8:3.8})},4B:f(){3.3r();3.x.4B();5g 3.b.C().G},2U:f(){c d=3.Z();k J I(d.2z()+(d.4C()*4v))},Z:f(){k 3.8},5a:f(d){3.W(J I(d.2z()-(d.4C()*4v)))},W:f(d){3.8=d;3.1X()},1X:f(){c 2H=3.4u();a(!3.1B){a(3.15){3.b.q(\'1J\').Q(2H)}3.b.C(\'8\',2H)}u{3.b.Q(2H)}},4u:f(P){a(P===4G)P=3.P;k K.4f(3.8,P,3.B)},4m:f(O){3.O=O||-U;a(3.O!==-U){3.O=K.2S(3.O,3.P,3.B)}3.1m();3.23()},4l:f(N){3.N=N||U;a(3.N!==U){3.N=K.2S(3.N,3.P,3.B)}3.1m();3.23()},4k:f(1h){3.1h=1h||[];a(!$.59(3.1h)){3.1h=3.1h.45(/,\\s*/)}3.1h=$.53(3.1h,f(d){k 1K(d,10)});3.1m();3.23()},2I:f(){a(3.1U)k;c 3m=1K(3.b.52().2w(f(){k $(3).1c(\'z-3o\')!=\'51\'}).54().1c(\'z-3o\'))+10;c 2r=3.15?3.15.2r():3.b.2r();c 3h=3.15?3.15.2F(1b):3.b.2F(1b);3.x.1c({4p:2r.4p+3h,34:2r.34,3m:3m})},1m:f(){c 8,3l=1G;a(1O&&1O.L&&(2B 1O[0]===\'3S\'||1O[0]3Z I)){8=1O[0];3l=1b}u{8=3.1B?3.b.Q():3.b.C(\'8\')||3.b.q(\'1J\').Q()}3.8=K.2S(8,3.P,3.B);a(3l)3.1X();c 3i=3.o;a(3.8<3.O){3.o=J I(3.O)}u a(3.8>3.N){3.o=J I(3.N)}u{3.o=J I(3.8)}a(3i&&3i.2z()!=3.o.2z()){3.b.1M({1T:\'3d\',8:3.o})}3.2g()},4r:f(){c 3f=3.1H,V=\'<1p>\';2a(3f<3.1H+7){V+=\'<16 X="55">\'+14[3.B].4t[(3f++)%7]+\'</16>\'}V+=\'</1p>\';3.x.q(\'.G-1I 32\').3N(V)},3A:f(){c V=\'\',i=0;2a(i<12){V+=\'<1F X="A">\'+14[3.B].2j[i++]+\'</1F>\'}3.x.q(\'.G-18 1D\').V(V)},2g:f(){c d=J I(3.o),h=d.R(),A=d.H(),2E=3.O!==-U?3.O.R():-U,3Q=3.O!==-U?3.O.H():-U,2N=3.N!==U?3.N.R():U,3M=3.N!==U?3.N.H():U,3b=3.8&&3.8.1V(),17=J I();3.x.q(\'.G-1I 32 16:2Z(1)\').26(14[3.B].18[A]+\' \'+h);3.x.q(\'35 16.17\').26(14[3.B].17).58(3.2C!==1G);3.23();3.3A();c E=1N(h,A-1,28,0,0,0,0),1f=K.4y(E.R(),E.H());E.W(1f);E.W(1f-(E.25()-3.1H+7)%7);c 2f=J I(E);2f.W(2f.Z()+42);2f=2f.1V();c V=[];c 1d;2a(E.1V()<2f){a(E.25()==3.1H){V.2x(\'<1p>\')}1d=\'\';a(E.R()<h||(E.R()==h&&E.H()<A)){1d+=\' 3n\'}u a(E.R()>h||(E.R()==h&&E.H()>A)){1d+=\' J\'}a(3.3t&&E.R()==17.3G()&&E.H()==17.3g()&&E.Z()==17.2U()){1d+=\' 17\'}a(3b&&E.1V()==3b){1d+=\' 2A\'}a(E.1V()<3.O||E.1V()>3.N||$.2W(E.25(),3.1h)!==-1){1d+=\' 1y\'}V.2x(\'<1D X="1f\'+1d+\'">\'+E.Z()+\'</1D>\');a(E.25()==3.3J){V.2x(\'</1p>\')}E.W(E.Z()+1)}3.x.q(\'.G-1I 2k\').57().3N(V.3W(\'\'));c 2G=3.8&&3.8.R();c 18=3.x.q(\'.G-18\').q(\'16:2Z(1)\').26(h).4x().q(\'1F\').56(\'2A\');a(2G&&2G==h){18.2Z(3.8.H()).1W(\'2A\')}a(h<2E||h>2N){18.1W(\'1y\')}a(h==2E){18.1Y(0,3Q).1W(\'1y\')}a(h==2N){18.1Y(3M+1).1W(\'1y\')}V=\'\';h=1K(h/10,10)*10;c 4D=3.x.q(\'.G-3D\').q(\'16:2Z(1)\').26(h+\'-\'+(h+9)).4x().q(\'1D\');h-=1;1E(c i=-1;i<11;i++){V+=\'<1F X="h\'+(i==-1||i==10?\' 3n\':\'\')+(2G==h?\' 2A\':\'\')+(h<2E||h>2N?\' 1y\':\'\')+\'">\'+h+\'</1F>\';h+=1}4D.V(V)},23:f(){c d=J I(3.o),h=d.R(),A=d.H();1k(3.1n){l 0:a(3.O!==-U&&h<=3.O.R()&&A<=3.O.H()){3.x.q(\'.1w\').1c({1C:\'20\'})}u{3.x.q(\'.1w\').1c({1C:\'2p\'})}a(3.N!==U&&h>=3.N.R()&&A>=3.N.H()){3.x.q(\'.1L\').1c({1C:\'20\'})}u{3.x.q(\'.1L\').1c({1C:\'2p\'})}F;l 1:l 2:a(3.O!==-U&&h<=3.O.R()){3.x.q(\'.1w\').1c({1C:\'20\'})}u{3.x.q(\'.1w\').1c({1C:\'2p\'})}a(3.N!==U&&h>=3.N.R()){3.x.q(\'.1L\').1c({1C:\'20\'})}u{3.x.q(\'.1L\').1c({1C:\'2p\'})}F}},2i:f(e){e.49();e.2b();c Y=$(e.Y).4a(\'1F, 1D, 16\');a(Y.L==1){1k(Y[0].5f.5m()){l\'16\':1k(Y[0].4b){l\'1k\':3.2d(1);F;l\'1w\':l\'1L\':c j=K.3z[3.1n].2D*(Y[0].4b==\'1w\'?-1:1);1k(3.1n){l 0:3.o=3.1t(3.o,j);F;l 1:l 2:3.o=3.1P(3.o,j);F}3.2g();F;l\'17\':c 8=J I();8=1N(8.3G(),8.3g(),8.2U(),0,0,0);3.2d(-2);c 1v=3.2C==\'6b\'?3R:\'3B\';3.3q(8,1v);F}F;l\'1F\':a(!Y.1q(\'.1y\')){3.o.W(1);a(Y.1q(\'.A\')){c A=Y.6c().q(\'1F\').3o(Y);3.o.2X(A);3.b.1M({1T:\'6a\',8:3.o})}u{c h=1K(Y.26(),10)||0;3.o.3a(h);3.b.1M({1T:\'67\',8:3.o})}3.2d(-1);3.2g()}F;l\'1D\':a(Y.1q(\'.1f\')&&!Y.1q(\'.1y\')){c 1f=1K(Y.26(),10)||1;c h=3.o.R(),A=3.o.H();a(Y.1q(\'.3n\')){a(A===0){A=11;h-=1}u{A-=1}}u a(Y.1q(\'.J\')){a(A==11){A=0;h+=1}u{A+=1}}3.3q(1N(h,A,1f,0,0,0,0))}F}}},3q:f(8,1v){a(!1v||1v==\'8\')3.8=8;a(!1v||1v==\'3B\')3.o=8;3.2g();3.1X();3.b.1M({1T:\'3d\',8:3.8});c b;a(3.1B){b=3.b}u a(3.15){b=3.b.q(\'1J\')}a(b){b.3L();a(3.1S&&(!1v||1v==\'8\')){3.1o()}}},1t:f(8,j){a(!j)k 8;c 19=J I(8.1V()),1f=19.Z(),A=19.H(),3y=3j.68(j),1e,2h;j=j>0?1:-1;a(3y==1){2h=j==-1?f(){k 19.H()==A}:f(){k 19.H()!=1e};1e=A+j;19.2X(1e);a(1e<0||1e>11)1e=(1e+12)%12}u{1E(c i=0;i<3y;i++)19=3.1t(19,j);1e=19.H();19.W(1f);2h=f(){k 1e!=19.H()}}2a(2h()){19.W(--1f);19.2X(1e)}k 19},1P:f(8,j){k 3.1t(8,j*12)},3s:f(8){k 8>=3.O&&8<=3.N},2n:f(e){a(3.x.1q(\':69(:2p)\')){a(e.2R==27)3.1s();k}c 2Q=1G,j,1f,A,1a,1g;1k(e.2R){l 27:3.1o();e.2b();F;l 37:l 39:a(!3.1R)F;j=e.2R==37?-1:1;a(e.3I){1a=3.1P(3.8,j);1g=3.1P(3.o,j)}u a(e.3P){1a=3.1t(3.8,j);1g=3.1t(3.o,j)}u{1a=J I(3.8);1a.W(3.8.Z()+j);1g=J I(3.o);1g.W(3.o.Z()+j)}a(3.3s(1a)){3.8=1a;3.o=1g;3.1X();3.1m();e.2b();2Q=1b}F;l 38:l 40:a(!3.1R)F;j=e.2R==38?-1:1;a(e.3I){1a=3.1P(3.8,j);1g=3.1P(3.o,j)}u a(e.3P){1a=3.1t(3.8,j);1g=3.1t(3.o,j)}u{1a=J I(3.8);1a.W(3.8.Z()+j*7);1g=J I(3.o);1g.W(3.o.Z()+j*7)}a(3.3s(1a)){3.8=1a;3.o=1g;3.1X();3.1m();e.2b();2Q=1b}F;l 13:3.1o();e.2b();F;l 9:3.1o();F}a(2Q){3.b.1M({1T:\'3d\',8:3.8});c b;a(3.1B){b=3.b}u a(3.15){b=3.b.q(\'1J\')}a(b){b.3L()}}},2d:f(j){a(j){3.1n=3j.66(0,3j.65(2,3.1n+j))}3.x.q(\'>1l\').1o().2w(\'.G-\'+K.3z[3.1n].1d).1c(\'6e\',\'60\');3.23()}};$.2c.G=f(21){c 3v=5Z.3k(3R,1O);3v.46();k 3.61(f(){c $3=$(3),C=$3.C(\'G\'),T=2B 21==\'62\'&&21;a(!C){$3.C(\'G\',(C=J 1r(3,$.4n({},$.2c.G.4F,T))))}a(2B 21==\'3S\'&&2B C[21]==\'f\'){C[21].3k(C,3v)}})};$.2c.G.4F={};$.2c.G.64=1r;c 14=$.2c.G.14={3e:{1I:["4q","63","6d","6i","5j","6p","6q","4q"],4h:["4s","6r","6l","6g","6k","6j","6h","4s"],4t:["4o","6f","6m","6o","5X","5x","5w","4o"],18:["5v","5y","5z","5C","4E","5B","5A","5u","5t","5n","5Y","5l"],2j:["5k","5o","5p","5s","4E","5r","5q","5D","5E","5R","5Q","5P"],17:"5S"}};c K={3z:[{1d:\'1I\',3F:\'5T\',2D:1},{1d:\'18\',3F:\'3T\',2D:1},{1d:\'3D\',3F:\'3T\',2D:10}],4z:f(h){k(((h%4===0)&&(h%5W!==0))||(h%5V===0))},4y:f(h,A){k[31,(K.4z(h)?29:28),31,30,31,30,31,31,30,31,30,31][A]},3H:/2v?|4d?|2q?|2l?|2m(?:2m)?/g,3V:/[^-\\/:-@\\[\\5U-\\5O-`{-~\\t\\n\\r]+/g,44:f(P){c 1Z=P.5N(3.3H,\'\\0\').45(\'\\0\'),S=P.3c(3.3H);a(!1Z||!1Z.L||!S||S.L===0){5H J 5G("5F 8 P.")}k{1Z:1Z,S:S}},2S:f(8,P,B){a(8 3Z I)k 8;a(/^[\\-+]\\d+[33]([\\s,]+[\\-+]\\d+[33])*$/.2h(8)){c 3Y=/([\\-+]\\d+)([33])/,S=8.3c(/([\\-+]\\d+)([33])/g),1x,j;8=J I();1E(c i=0;i<S.L;i++){1x=3Y.5I(S[i]);j=1K(1x[1]);1k(1x[2]){l\'d\':8.W(8.Z()+j);F;l\'m\':8=1r.2u.1t.3U(1r.2u,8,j);F;l\'w\':8.W(8.Z()+j*7);F;l\'y\':8=1r.2u.1P.3U(1r.2u,8,j);F}}k 1N(8.R(),8.H(),8.Z(),0,0,0)}c S=8&&8.3c(3.3V)||[],8=J I(),2t={},2Y=[\'2J\',\'2m\',\'M\',\'2l\',\'m\',\'2q\',\'d\',\'2v\'],1A={2J:f(d,v){k d.3a(v)},2m:f(d,v){k d.3a(5J+v)},m:f(d,v){v-=1;2a(v<0)v+=12;v%=12;d.2X(v);2a(d.H()!=v)d.W(d.Z()-1);k d},d:f(d,v){k d.W(v)}},Q,2s,1x;1A[\'M\']=1A[\'2l\']=1A[\'2q\']=1A[\'m\'];1A[\'2v\']=1A[\'d\'];8=1N(8.3G(),8.3g(),8.2U(),0,0,0);c 1Q=P.S.1Y();a(S.L!=1Q.L){1Q=$(1Q).2w(f(i,p){k $.2W(p,2Y)!==-1}).5M()}a(S.L==1Q.L){1E(c i=0,2V=1Q.L;i<2V;i++){Q=1K(S[i],10);1x=1Q[i];a(4e(Q)){1k(1x){l\'2l\':2s=$(14[B].18).2w(f(){c m=3.1Y(0,S[i].L),p=S[i].1Y(0,m.L);k m==p});Q=$.2W(2s[0],14[B].18)+1;F;l\'M\':2s=$(14[B].2j).2w(f(){c m=3.1Y(0,S[i].L),p=S[i].1Y(0,m.L);k m==p});Q=$.2W(2s[0],14[B].2j)+1;F}}2t[1x]=Q}1E(c i=0,s;i<2Y.L;i++){s=2Y[i];a(s 1z 2t&&!4e(2t[s]))1A[s](8,2t[s])}}k 8},4f:f(8,P,B){c Q={d:8.Z(),D:14[B].4h[8.25()],4d:14[B].1I[8.25()],m:8.H()+1,M:14[B].2j[8.H()],2l:14[B].18[8.H()],2m:8.R().5L().5K(2),2J:8.R()};Q.2v=(Q.d<10?\'0\':\'\')+Q.d;Q.2q=(Q.m<10?\'0\':\'\')+Q.m;c 8=[],36=$.4n([],P.1Z);1E(c i=0,2V=P.S.L;i<2V;i++){a(36.L)8.2x(36.46());8.2x(Q[P.S[i]])}k 8.3W(\'\')},2T:\'<32>\'+\'<1p>\'+\'<16 X="1w"><2y X="2K 2K-2L-34" 3K-20="1b"></2y></16>\'+\'<16 3E="5" X="1k"></16>\'+\'<16 X="1L"><2y X="2K 2K-2L-3O" 3K-20="1b"></2y></16>\'+\'</1p>\'+\'</32>\',3p:\'<2k><1p><1D 3E="7"></1D></1p></2k>\',2P:\'<35><1p><16 3E="7" X="17"></16></1p></35>\'};K.4j=\'<1l X="G">\'+\'<1l X="G-1I">\'+\'<1u X=" 1u-3x">\'+K.2T+\'<2k></2k>\'+K.2P+\'</1u>\'+\'</1l>\'+\'<1l X="G-18">\'+\'<1u X="1u-3x">\'+K.2T+K.3p+K.2P+\'</1u>\'+\'</1l>\'+\'<1l X="G-3D">\'+\'<1u X="1u-3x">\'+K.2T+K.3p+K.2P+\'</1u>\'+\'</1l>\'+\'</1l>\';$.2c.G.K=K}(3C.6n);', 62, 400, '|||this|||||date||if|element|var|||function||year||dir|return|case|||viewDate||find||||else|||picker|||month|language|data||prevMonth|break|datepicker|getUTCMonth|Date|new|DPGlobal|length||endDate|startDate|format|val|getUTCFullYear|parts|options|Infinity|html|setUTCDate|class|target|getUTCDate|||||dates|component|th|today|months|new_date|newDate|true|css|clsName|new_month|day|newViewDate|daysOfWeekDisabled|proxy|_events|switch|div|update|viewMode|hide|tr|is|Datepicker|show|moveMonth|table|which|prev|part|disabled|in|setters_map|isInput|visibility|td|for|span|false|weekStart|days|input|parseInt|next|trigger|UTCDate|arguments|moveYear|fparts|keyboardNavigation|autoclose|type|isInline|valueOf|addClass|setValue|slice|separators|hidden|option|forceParse|updateNavArrows|ev|getUTCDay|text||||while|preventDefault|fn|showMode|el|nextMonth|fill|test|click|monthsShort|tbody|MM|yy|keydown|on|visible|mm|offset|filtered|parsed|prototype|dd|filter|push|li|getTime|active|typeof|todayBtn|navStep|startYear|outerHeight|currentYear|formatted|place|yyyy|fa|arrow|mousedown|endYear|startViewMode|footTemplate|dateChanged|keyCode|parseDate|headTemplate|getDate|cnt|inArray|setUTCMonth|setters_order|eq|||thead|dmwy|left|tfoot|seps||||setUTCFullYear|currentDate|match|changeDate|en|dowCnt|getMonth|height|oldViewDate|Math|apply|fromArgs|zIndex|old|index|contTemplate|_setDate|_detachEvents|dateWithinRange|todayHighlight|hasInput|args|off|condensed|mag|modes|fillMonths|view|window|years|colspan|navFnc|getFullYear|validParts|ctrlKey|weekEnd|aria|change|endMonth|append|right|shiftKey|startMonth|null|string|FullYear|call|nonpunctuation|join|that|part_re|instanceof||focus||_attachEvents|parseFormat|split|shift|icon|dropdown|stopPropagation|closest|className|isRTL|DD|isNaN|formatDate|rtl|daysShort|keyup|template|setDaysOfWeekDisabled|setEndDate|setStartDate|extend|Su|top|Sunday|fillDow|Sun|daysMin|getFormattedDate|60000|resize|end|getDaysInMonth|isLeapYear|document|remove|getTimezoneOffset|yearCont|May|defaults|undefined|navigation|keyboard|dateKeyboardNavigation|startView|start|highlight|btn|decade|add|dateAutoclose|appendTo|parse|force|body|inline|toggleClass|menu|dateForceParse|weekstart|startdate|auto|parents|map|first|dow|removeClass|empty|toggle|isArray|setDate|week|of|enddate|constructor|nodeName|delete|UTCToday|UTC|Thursday|Jan|December|toLowerCase|October|Feb|Mar|Jul|Jun|Apr|September|August|January|Sa|Fr|February|March|July|June|April|Aug|Sep|Invalid|Error|throw|exec|2000|substring|toString|toArray|replace|u9fff|Dec|Nov|Oct|Today|Month|u3400|400|100|Th|November|Array|block|each|object|Monday|Constructor|min|max|changeYear|abs|not|changeMonth|linked|parent|Tuesday|display|Mo|Wed|Sat|Wednesday|Fri|Thu|Tue|Tu|jQuery|We|Friday|Saturday|Mon'.split('|'), 0, {}))

var wordifyfa = function (num, level) {
	'use strict';
	if (num === null) return "";
	// convert negative number to positive and get wordify value
	if (num < 0) {
		num = num * -1;
		return "منفی " + wordifyfa(num, level);
	}
	if (num === 0) {
		if (level === 0) {
			return "صفر";
		} else {
			return "";
		}
	}
	var result = "",
		yekan = ["یک", "دو", "سه", "چهار", "پنج", "شش", "هفت", "هشت", "نه"],
		dahgan = ["بیست", "سی", "چهل", "پنجاه", "شصت", "هفتاد", "هشتاد", "نود"],
		sadgan = ["یکصد", "دویست", "سیصد", "چهارصد", "پانصد", "ششصد", "هفتصد", "هشتصد", "نهصد"],
		dah = ["ده", "یازده", "دوازده", "سیزده", "چهارده", "پانزده", "شانزده", "هفده", "هیجده", "نوزده"];
	if (level > 0) {
		result += " و ";
		level -= 1;
	}

	if (num < 10) {
		result += yekan[num - 1];
	} else if (num < 20) {
		result += dah[num - 10];
	} else if (num < 100) {
		result += dahgan[parseInt(num / 10, 10) - 2] + wordifyfa(num % 10, level + 1);
	} else if (num < 1000) {
		result += sadgan[parseInt(num / 100, 10) - 1] + wordifyfa(num % 100, level + 1);
	} else if (num < 1000000) {
		result += wordifyfa(parseInt(num / 1000, 10), level) + " هزار " + wordifyfa(num % 1000, level + 1);
	} else if (num < 1000000000) {
		result += wordifyfa(parseInt(num / 1000000, 10), level) + " میلیون " + wordifyfa(num % 1000000, level + 1);
	} else if (num < 1000000000000) {
		result += wordifyfa(parseInt(num / 1000000000, 10), level) + " میلیارد " + wordifyfa(num % 1000000000, level + 1);
	} else if (num < 1000000000000000) {
		result += wordifyfa(parseInt(num / 1000000000000, 10), level) + " تریلیارد " + wordifyfa(num % 1000000000000, level + 1);
	}
	return result;
};
