define(['application','dx','dataService'], function (app) {
    app.register.directive('dateRange', ['dataService', function (service) {

	return {
		restrict: 'E',
		transclude: true,
		templateUrl: '/App/template/select-date-range.html',
		scope: {
			fromDate: "=?",
			toDate: "=?",
			onRangeChange: "&"
		},
		link: function (scope, element, attrs) {

		    $("#checked").dxCheckBox({
		        value: true
		    });
		    

		    $("#datePopup").datePopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				title: "انتخاب بازه زمانی",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "showDatePopup"
				}
			};

			scope.tabSeasons = {
				dataSource: Hesabfa.seasons,
				bindingOptions: { selectedIndex: 'selectedSeason' },
				onSelectionChanged: function () {
					var s = scope.selectedSeason * 3;
					var e = s + 2;
					scope.rangeSelector.setValue([Hesabfa.monthNames[s], Hesabfa.monthNames[e]]);
				},
				onItemClick: function () {
					var s = scope.selectedSeason * 3;
					var e = s + 2;
					scope.rangeSelector.setValue([Hesabfa.monthNames[s], Hesabfa.monthNames[e]]);
				},
			}

			var ds = [];
			for (var i = 0; i < Hesabfa.monthNames.length; i++) {
				ds.push({ arg: Hesabfa.monthNames[11 - i] });
			}
			scope.rangeSelectorOptions = {
				dataSource: ds,
				size: {
					height: 100
				},
				onValueChanged: function (e) {
					scope.txtStart = '';
					scope.txtEnd = '';
				},
				onInitialized: function (e) {
					scope.rangeSelector = e.component;
				}
			};

			function updateText() {
				function parseMonth(str) {
					if (Number(str) === str)
						return str;
					if (str.indexOf('m') !== 0)
						return -1;
					var m = parseInt(str.substr(1));
					if (isNaN(m) || m < 1 || m > 12)
						return -1;
					return m;
				}

				scope.text = "انتخاب بازه زمانی";
				scope.$parent.selectedDateRange = "";
				var start = scope.fromDate;
				var end = scope.toDate;
				if (!start || !end)
					return;

				var startMonth = parseMonth(start);
				var endMonth = parseMonth(end);
				if (startMonth > 0 && endMonth > 0) {
					if (endMonth - startMonth === 2 && startMonth % 3 === 1) {
						scope.text = "فصل " + Hesabfa.seasons[(startMonth - 1) / 3];
					} else if (endMonth - startMonth === 0) {
						scope.text = "ماه " + Hesabfa.monthNames[startMonth - 1];
					} else {
						scope.text = "از ماه " + Hesabfa.monthNames[startMonth - 1] + " تا " + Hesabfa.monthNames[endMonth - 1];
					}
				} else {
					scope.text = "از تاریخ " + scope.fromDate + " تا " + scope.toDate;
				}
				scope.$parent.selectedDateRange = scope.text;
			}
			scope.updateRange = function () {
				if (scope.txtStart && scope.txtEnd) {
					scope.fromDate = scope.txtStart;
					scope.toDate = scope.txtEnd;
				}
				else {
					var val = scope.rangeSelector.getValue();
					var startMonth = Hesabfa.monthNames.indexOf(val[1]);
					var endMonth = Hesabfa.monthNames.indexOf(val[0]);
					scope.fromDate = "m" + (startMonth + 1);
					scope.toDate = "m" + (endMonth + 1);
				}
				updateText();
				setTimeout(function () {
					scope.$apply(function () {
						if (scope.onRangeChange)
							scope.onRangeChange();
					});
				}, 0);
				scope.showDatePopup = false;
			}

			scope.$watch('fromDate', function () {
				updateText();
			});
			scope.$watch('toDate', function () {
				updateText();
			});
		}
	};
    }]);
});