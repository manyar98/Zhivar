var dashboardWidgetColors = [
	{ value: "gray", text: "خاکستری" },
	{ value: "white", text: "سفید" },
	{ value: "black", text: "سیاه" },
	{ value: "red", text: "قرمز" },
	{ value: "blue", text: "آبی" },
	{ value: "green", text: "سبز" },
	{ value: "yellow", text: "زرد" },
	{ value: "orange", text: "نارنجی" },
	{ value: "pink", text: "صورتی" },
	{ value: "purple", text: "بنفش" }
];

app.directive('empty', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/empty.html',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};

			var popupElement;

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;


				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});
		}
	};
});

app.directive('widgetDueReceivedCheques', function () {
	return {
		restrict: 'E',
		transclude: true,
		templateUrl: '/dashboard/widgets/dashboard/widget-due-cheques.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};
			config = jQuery.extend(true, {
				type: 0,
				period: 0
			}, config);
			var popupElement;
			$scope.notAccess = false;

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};
			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.type = config.type;
			$scope.period = config.period;

			var periods = [
				{
					value: 0, text: "سررسید امروز"
				},
				{
					value: 3, text: "سه روز مانده به سررسید"
				},
				{ value: 7, text: "یک هفته مانده به سررسید" }
			];

			$scope.selectDueChequePeriod = {
				rtlEnabled: true,
				items: periods,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "period"
				}
			};
			$scope.selectDueChequeType = {
				rtlEnabled: true,
				items: [{
					value: 0, text: "چک های دریافتی"
				}, {
					value: 1, text: "چک های پرداختی"
				}],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "type"
				}
			};

			$scope.title = config.title;
			$scope.color = config.color;
			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;
				config.type = $scope.type;
				config.period = $scope.period;

				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
				$scope.getOverdueCheques();
			};
			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			};
			$scope.config = function () {
				$scope.visiblePopup = true;
			};
			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});

			$scope.getOverdueCheques = function () {
				callws(DefaultUrl.MainWebService + 'GetDueCheques', {
					type: $scope.type, days: $scope.period
				})
				.success(function (data) {
					if (data == null) {
						$scope.notAccess = true;
						$scope.$apply();
						return;
					}
					$scope.cheques = data;
					$scope.totalAmount = 0;
					for (var i = 0; i < $scope.cheques.length; i++) {
						$scope.totalAmount += $scope.cheques[i].Amount;
					}
					$scope.count = $scope.cheques.length;
					$scope.$apply();
				}).fail(function (error) {
					applyScope($scope);
					alertbox({
						content: error, type: "error"
					});
				}).loginFail(function () {
					window.location = DefaultUrl.login;
				});
			};
			$scope.getBankLogoClass = function (bankName, size) {
				if (!bankName) return "";
				var pre = "";
				if (size)
					pre = "ibl" + size + " ";
				if (bankName.includes("تجارت")) return pre + 'ibl-tejarat';
				else if (bankName.includes("ملت")) return pre + 'ibl-mellat';
				else if (bankName.includes("توسعه صادرات")) return pre + 'ibl-edbi';
				else if (bankName.includes("صادرات")) return pre + 'ibl-bsi';
				else if (bankName.includes("ملی") || bankName.includes("ملي")) return pre + 'ibl-bmi';
				else if (bankName.includes("سپه")) return pre + 'ibl-sepah';
				else if (bankName.includes("کشاورزی")) return pre + 'ibl-bki';
				else if (bankName.includes("پارسیان")) return pre + 'ibl-parsian';
				else if (bankName.includes("مسکن")) return pre + 'ibl-maskan';
				else if (bankName.includes("رفاه")) return pre + 'ibl-rb';
				else if (bankName.includes("اقتصاد نوین")) return pre + 'ibl-en';
				else if (bankName.includes("انصار")) return pre + 'ibl-ansar';
				else if (bankName.includes("پاسارگاد")) return pre + 'ibl-bpi';
				else if (bankName.includes("سامان")) return pre + 'ibl-sb';
				else if (bankName.includes("سینا")) return pre + 'ibl-sina';
				else if (bankName.includes("پست")) return pre + 'ibl-post';
				else if (bankName.includes("قوامین")) return pre + 'ibl-ghbi';
				else if (bankName.includes("توسعه تعاون")) return pre + 'ibl-tt';
				else if (bankName.includes("شهر")) return pre + 'ibl-shahr';
				else if (bankName.includes("آینده")) return pre + 'ibl-ba';
				else if (bankName.includes("سرمایه")) return pre + 'ibl-sarmayeh';
				else if (bankName.includes("دی")) return pre + 'ibl-day';
				else if (bankName.includes("حکمت")) return pre + 'ibl-hi';
				else if (bankName.includes("کارآفرین")) return pre + 'ibl-kar';
				else if (bankName.includes("صنعت")) return pre + 'ibl-bim';
				else if (bankName.includes("خاورمیانه")) return pre + 'ibl-me';
				else if (bankName.includes("ونزوئلا")) return pre + 'ibl-ivbb';
				else return "";
			};

			$scope.getOverdueCheques();
			$scope.currency = window.currency;
			$scope.totalAmount = 0;
		}
	};
});

app.directive('widgetTopBestSellerItemsChart', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-top-best-seller-items-chart.html?ver=1.2.9.1',
		replace: true,
		scope: {
		},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};
			config = jQuery.extend(true, {
				itemCount: 5,
				orderByAmount: true,
				timeSpan: 3
			}, config);

			var popupElement;
			$scope.notAccess = false;

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.itemCount = config.itemCount;
			$scope.orderByAmount = config.orderByAmount;
			$scope.timeSpan = config.timeSpan;

			$scope.selectItemCount = {
				rtlEnabled: true,
				items: [{
					value: 5, text: "پنج کالای پر فروش"
				}, {
					value: 10, text: "ده کالای پر فروش"
				}],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "itemCount"
				}
			};
			$scope.selectOrderByAmount = {
				rtlEnabled: true,
				items: [{
					value: true, text: "بر اساس مبلغ فروش"
				}, { value: false, text: "بر اساس تعداد فروش" }],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "orderByAmount"
				}
			};
			$scope.selectTime = {
				rtlEnabled: true,
				items: [{ value: 0, text: "امروز" }, {
					value: 1, text: "هفته اخیر"
				}, {
					value: 2, text: "یک ماه اخیر"
				}, {
					value: 3, text: "کل دوره مالی"
				}],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "timeSpan"
				}
			};

			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;
				config.itemCount = $scope.itemCount;
				config.orderByAmount = $scope.orderByAmount;
				config.timeSpan = $scope.timeSpan;

				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
				$scope.getChartData();
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});

			var chart1 = $(".chartPieToSellerItems", $element).dxPieChart({
				dataSource: [],
				height: 250,
				commonPaneSettings: {
					backgroundColor: '#f1f1f1'
				},
				commonSeriesSettings: {
					argumentField: "name",
					valueField: "amount",
					type: "pie"
				},
				series: [
				{
					argumentField: "name",
					valueField: "displayValue",
					name: "مبلغ",
					label: {
						visible: true,
						connector: {
							visible: true,
							width: 1
						},
						customizeText: function (arg) {
							return Hesabfa.money(arg.valueText) + " (" + Hesabfa.farsiDigit(arg.percentText) + ")";
						}
					}
				}
				],
				tooltip: {
					enabled: true
				}
			}).dxPieChart("instance");

			$scope.getChartData = function () {
				callws(DefaultUrl.MainWebService + 'GetTopBestSellersItems', { count: $scope.itemCount, orderByAmount: $scope.orderByAmount, timeSpan: $scope.timeSpan })
					.success(function (data) {
						if (data == null) {
							$scope.notAccess = true;
							$scope.$apply();
							return;
						}
						$scope.data = data;

						for (var i = 0; i < data.length; i++) {
							if ($scope.orderByAmount)
								data[i].displayValue = data[i].amount;
							else
								data[i].displayValue = data[i].stock;
						}
						chart1.option('dataSource', data);
						$scope.$apply();
					}).fail(function (error) {
						applyScope($scope);
						alertbox({
							content: error, type: "error"
						});
					}).loginFail(function () {
						window.location = DefaultUrl.login;
					});
			};
			$scope.getChartData();
		}
	};
});

app.directive('widgetSaleBuyChart', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-sale-buy-chart.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};
			config = jQuery.extend(true, {
				months: 0
			}, config);

			var popupElement;
			$scope.notAccess = false;
			$scope.months = config.months;

			var chart = $('.chart1', $element).dxChart({
				dataSource: [],
				height: 300,
				commonPaneSettings: {
					backgroundColor: '#f1f1f1'
				},
				commonSeriesSettings: {
					argumentField: "Month",
					type: "bar"
				},
				argumentAxis: {
					label: {
						overlappingBehavior: "rotate",
						rotationAngle: -30
					},
					grid: { visible: true }

				},
				series: [
					{ valueField: "sumSale", name: "فروش" },
					{ valueField: "sumPurchase", name: "خرید" }
				],
				legend: {
					verticalAlignment: "bottom",
					horizontalAlignment: "center",
					margin: 0,
					itemTextPosition: "left"
				},
				tooltip: {
					enabled: true,
					shared: true,
					customizeTooltip: function (info) {
						return {
							html: "<div><div class='tooltip-header'>" +
							info.argumentText + "</div>" +
							"<div class='tooltip-body'><div class='series-name'>" +
							info.points[0].seriesName +
							": </div><div class='value-text'>" +
							Hesabfa.money(info.points[0].valueText) +
							"</div><div class='series-name'>" +
							info.points[1].seriesName +
							": </div><div class='value-text'>" +
							Hesabfa.money(info.points[1].valueText) +
							"</div></div></div>"
						};
					}
				}
			}).dxChart("instance");

			function loadList() {
				callws(DefaultUrl.MainWebService + "GetPurchaseAndSaleChart", { months: $scope.months }).success(function (result) {
					if (result == null) {
						$scope.notAccess = true;
						$scope.$apply();
						return;
					}

					chart.option("dataSource", result);
					$scope.$apply();
				}).fail(function (error) {
					$scope.saving = false;
					$scope.$apply();
					alertbox({ content: error, type: "error" });
				}).loginFail(function () {
					window.location = DefaultUrl.login;
				});
			}



			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;

				config.months = $scope.months;
				loadList();

				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});


			$scope.cmbOptions = {
				rtlEnabled: true,
				items: [{ value: 0, text: '۶ ماه گذشته' }, { value: 1, text: '۱۲ ماه گذشته' }],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "months"
				}
			};

			loadList();
		}
	};
});

app.directive('widgetReports', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-reports.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};

			var popupElement;
			$scope.reports = $scope.$parent.builtinReports;
			$scope.havePermissionMenu = $scope.$parent.havePermissionMenu;
			$scope.isInFavReports = $scope.$parent.isInFavReports;
			$scope.havePermission = $scope.$parent.havePermission;
			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;


				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});
		}
	};
});

app.directive('widgetCashBank', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-cash-bank.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};

			config = jQuery.extend(true, {
				type: 0
			}, config);


			$scope.notAccess = false;
			$scope.selected = config.type;

			$scope.currency = window.currency;

			function loadList() {
				callws(DefaultUrl.MainWebService + "GetCashBankList", { cashes: $scope.selected !== 1, banks: $scope.selected !== 2 }).success(function (result) {
					if (result == null) {
						$scope.notAccess = true;
						$scope.$apply();
						return;
					}

					$scope.list = result;
					$scope.$apply();
				}).fail(function (error) {
					$scope.saving = false;
					$scope.$apply();
					if ($scope.accessError(error)) {
						return;
					}
					alertbox({ content: error, type: "error" });
				}).loginFail(function () {
					window.location = DefaultUrl.login;
				});
			}

			var popupElement;

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;

				config.type = $scope.selected;
				loadList();

				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});

			$scope.cmbOptions = {
				rtlEnabled: true,
				items: [{ value: 0, text: 'نمایش بانک ها و صندوق ها' }, { value: 1, text: 'نمایش بانک ها' }, { value: 2, text: 'نمایش صندوق ها' }],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "selected"
				}
			};
			$scope.getBankLogoClass = function (bankName, size) {
				return Hesabfa.getBankLogoClass(bankName, size);
			};

			loadList();
		}
	};
});

app.directive('widgetPrices', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-prices.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};

			var popupElement;

			var iframe = $('iframe', $element);
			iframe.on("load", function () {
				iframe[0].height = iframe[0].contentWindow.document.body.scrollHeight + "px";
			});
			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;

				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
			});
		}
	};
});

app.directive('widgetQuickAccess', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-quick-access.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};
			if (!config.list)
				config.list = [];
			$scope.links = [
				{ id: 1, text: "ثبت فاکتور فروش", url: "#editInvoice/new" },
				{ id: 2, text: "ثبت فاکتور خرید", url: "#editInvoice/newBill" },
				{ id: 3, text: "ثبت برگشت از فروش", url: "#editInvoice/saleReturn" },
				{ id: 4, text: "ثبت برگشت از خرید", url: "#editInvoice/purchaseReturn" },
				{ id: 5, text: "لیست فاکتورهای فروش", url: "#invoices" },
				{ id: 6, text: "لیست فاکتورهای خرید", url: "#invoices/show=bills" },
				{ id: 7, text: "ثبت سند", url: "#editDocument/new" },
				{ id: 8, text: "لیست اسناد", url: "#documents" },
				{ id: 9, text: "دریافت وجه", url: "#receive" },
				{ id: 10, text: "پرداخت وجه", url: "#pay" },
				{ id: 11, text: "لیست رسیدهای دریافت", url: "#receiveAndPay/receive" },
				{ id: 12, text: "لیست رسیدهای پرداخت", url: "#receiveAndPay/pay" },
				{ id: 13, text: "لیست چک های دریافتی", url: "#cheques/show=receivables" },
				{ id: 14, text: "لیست چک های پرداختی", url: "#cheques/show=payables" }
			];

			var popupElement;

			for (var i = 0; i < $scope.links.length; i++) {
				$scope.links[i].selected = false;
				for (var j = 0; j < config.list.length; j++) {
					if (config.list[j] === $scope.links[i].id) {
						$scope.links[i].selected = true;
						break;
					}
				}
			}

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
					$(".links-sortable", popupElement).sortable({
						axis: "y"
					}).disableSelection();
				}
			};
			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.showSelected = function () {
				$scope.selected = [];
				for (var i = 0; i < config.list.length; i++) {
					for (var j = 0; j < $scope.links.length; j++) {
						if ($scope.links[j].id === config.list[i]) {
							$scope.selected.push($scope.links[j]);
							break;
						}
					}
				}
			}
			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;
				config.list = [];
				$('.links-sortable>div', popupElement).each(function (index, el) {
					var id = Number($(el).attr("data-tag"));
					var checked = $('input', el).val();
					if (checked === "true")
						config.list.push(id);
				});
				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
				$scope.showSelected();
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
				$scope.$apply();
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
			});

			$scope.showSelected();
		}
	};
});

app.directive('widgetGallery', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-gallery.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};

			var popupElement;

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};
			var widgets = [
				{ value: "widget-quick-access", text: "دسترسی سریع" },
				{ value: "widget-prices", text: "آخرین قیمت ارز" },
				{ value: "widget-due-received-cheques", text: "سررسید چک ها" },
				{ value: "widget-cash-bank", text: "بانک ها و صندوق ها" },
				{ value: "widget-reports", text: "گزارش ها" },
				{ value: "widget-top-best-seller-items-chart", text: "نمودار پرفروشترین کالاها" },
				{ value: "widget-sale-buy-chart", text: "نمودار خرید و فروش" },
				{ value: "widget-last-docs", text: "آخرین اسناد ثبت شده" },
			];
			$scope.cmbSelectOptions = {
				rtlEnabled: true,
				items: widgets,
				displayExpr: "text",
				bindingOptions: {
					value: "selected"
				}
			};
			$scope.selected = widgets[0];

			$scope.change = function () {
				config.selected = $scope.selected;
				$scope.$parent.selectWidget(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});
		}
	};
});

app.directive('widgetLastDocs', function ($compile) {
	return {
		restrict: 'E',
		transclude: false,
		templateUrl: '/dashboard/widgets/dashboard/widget-last-docs.html?ver=1.2.9.1',
		replace: true,
		scope: {},
		link: function ($scope, $element, attrs) {
			var config = attrs.config ? JSON.parse(attrs.config) : {};

			config = jQuery.extend(true, {
				count: 5
			}, config);

			$scope.notAccess = false;
			$scope.selectedCount = config.count;
			$scope.currency = window.currency;

			function loadList() {
				callws(DefaultUrl.MainWebService + "GetLastDocs", { optionCount: config.count }).success(function (result) {
					if (result == null) {
						$scope.notAccess = true;
						$scope.$apply();
						return;
					}

					$scope.lastDocs = result;
					$scope.$apply();
				}).fail(function (error) {
					$scope.saving = false;
					$scope.$apply();
					if ($scope.accessError(error)) {
						return;
					}
					alertbox({ content: error, type: "error" });
				}).loginFail(function () {
					window.location = DefaultUrl.login;
				});
			}

			var popupElement;

			$scope.configPopup = {
				rtlEnabled: true,
				showTitle: true,
				height: 'auto',
				width: 400,
				title: "تنظیمات",
				dragEnabled: true,
				closeOnOutsideClick: true,
				bindingOptions: {
					visible: "visiblePopup"
				},
				onContentReady: function (e) {
					popupElement = e.component.content();
				}
			};

			$scope.cmbColorOptions = {
				rtlEnabled: true,
				items: dashboardWidgetColors,
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "color"
				}
			};
			$scope.title = config.title;
			$scope.color = config.color;

			$scope.change = function () {
				config.title = $scope.title;
				config.color = $scope.color;

				config.count = $scope.selectedCount;
				loadList();

				$scope.$parent.widgetConfigChange(config);
				$scope.visiblePopup = false;
			}

			$scope.remove = function () {
				$scope.$parent.removeWidget(config.id);
			}

			$scope.config = function () {
				$scope.visiblePopup = true;
			}

			$scope.$on('showConfig', function (event, data) {
				if (data.id !== config.id)
					return;
				$scope.config();
				$scope.$apply();
			});

			$scope.cmbDocCount = {
				rtlEnabled: true,
				items: [{ value: 5, text: '5 سند آخر' }, { value: 10, text: '10 سند آخر' }],
				displayExpr: "text",
				valueExpr: "value",
				bindingOptions: {
					value: "selectedCount"
				}
			};

			loadList();
		}
	};
});

function dashboardWidget($scope, $compile, args) {
	args = jQuery.extend(true, {
		col: 0,
		index: 0,
		name: "",
		config: { title: "بدون عنوان", color: "gray" }
	}, args);

	this.col = args.col;
	this.index = args.index;
	this.name = args.name;
	this.config = args.config;
	var THIS = this;
	var el;
	function calcHtml() {
		THIS.config.id = parseInt(Math.random() * 100000000) + parseInt(Math.random() * 100000000);
		var config = JSON.stringify(THIS.config);
		return "<" + THIS.name + " config='" + config + "'></" + THIS.name + ">";
	}
	this.render = function (container, atPlace) {

		var html = "<div class='dashboard-widget " + this.config.color + "'>";
		html += "<div class='widget-header'>";
		html += "<div class='widget-header-div'>";
		html += "<div class='widget-header-back'></div>";
		html += "<div class='widget-header-title'>" + this.config.title + "</div>";
		html += "<div class='widget-header-buttons'>";
		html += "<div class='widget-header-button move'><span class='fa fa-arrows-alt'></span></div>";
		html += "<div class='widget-header-button config'><span class='fa fa-cogs'></span></div>";
		html += "</div>";
		html += "</div>";
		html += "</div>";
		html += "<div class='widget-body'>";
		html += "<div class='widget-body-back'></div>";
		html += "<div class='widget-body-content'>";
		html += calcHtml();
		html += "</div>";
		html += "</div>";
		html += "</div>";

		if (atPlace) {
			if (this.index === 0)
				el = $(html).prependTo(container);
			else {
				el = $(html).appendTo(container);
				$(container).children().eq(this.index).before($(container).children().last());
			}
		}
		else
			el = $(html).appendTo(container);
		el[0].widget = this;

		$compile(el.contents())($scope);

		$('.config', el).on("click", function () {
			$scope.$broadcast('showConfig', { id: THIS.config.id });
		});
	}

	this.configChanged = function () {
		$(".widget-header-title", el).html(this.config.title);
		$(el).removeClass("gray white black red blue green yellow orange pink purple");
		$(el).addClass(this.config.color);
	}
}

app.controller('dashboardController', ['$scope', '$compile', '$location', '$routeParams', function ($scope, $compile, $location, $routeParams) {
	$scope.save = function () {
		if ($scope.saving)
			return;
		var widgets = [];
		$scope.saving = true;
		$('.dashboard-widget').each(function (i, el) {
			var colIndex = Number($(el).parent().attr("column-index"));
			var index = $(el).index();
			el.widget.col = colIndex;
			el.widget.index = index;
			widgets.push(el.widget);
		});
		var w = JSON.stringify(widgets);
		callws(DefaultUrl.MainWebService + "SaveDashboard", { dash: w }).success(function (result) {
			$scope.saving = false;
			$scope.$apply();
		}).fail(function (error) {
			$scope.saving = false;
			$scope.$apply();
			if ($scope.accessError(error)) { return; }
			alertbox({ content: error, type: "error" });
		}).loginFail(function () {
			window.location = DefaultUrl.login;
		});
	}
	$scope.addWidget = function (args, save, atPlace) {
		var container = $(".app-dashboard-column")[args.col];
		var w = new dashboardWidget($scope, $compile, args);
		w.render(container, atPlace);
		if (save)
			$scope.save();
	}

	$scope.newWidget = function () {
		var container;
		var minHeight = 1000000;
		$(".app-dashboard-column").each(function (c, el) {
			if ($(el).height() < minHeight) {
				minHeight = $(el).height();
				container = $(el);
			}
		});

		var index = Number($(container).attr("column-index"));
		$scope.addWidget({ col: index, name: "widget-gallery", config: { title: 'انتخاب ابزار' } }, true);
	}

	$scope.init = function () {
		$scope.setupRoutes = ['setupStart', 'orgSettings', 'finanSettings', 'users', 'shareholders'];
		if ($rootScope.currentBusiness && $rootScope.currentBusiness.SetupStep < 7) {
			if ($rootScope.currentBusiness.SetupStep < 5) {
				$location.path('/' + $scope.setupRoutes[$routeParams.setupStep] + ($routeParams.setupStep >= 0 ? '/1' : ''));
				$scope.$apply();
				return;
			} else {
				$location.path('/' + $scope.setupRoutes[4] + ($routeParams.setupStep > 0 ? '/1' : ''));
				$scope.$apply();
				return;
			}
		}


		$(".app-dashboard-column").sortable({
			revert: true,
			connectWith: ".app-dashboard-column",
			placeholder: "dashboard-widget-placeholder",
			start: function (event, ui) {
				ui.placeholder.outerHeight(ui.item.outerHeight());
				ui.placeholder.outerWidth(ui.item.outerWidth());
			},
			stop: function (event, ui) {
				$scope.save();
			},
			handle: '.move'
		}).disableSelection();


		callws(DefaultUrl.MainWebService + "GetDashboard", {}).success(function (result) {
			$scope.loading = false;
			result = JSON.parse(result);
			var cols = {};
			for (var i = 0; i < result.length; i++) {
				var w = result[i];
				if (!cols[w.col])
					cols[w.col] = [];
				cols[w.col][w.index] = w;
			}
			for (var col in cols) {
				for (var j = 0; j < cols[col].length; j++) {
					$scope.addWidget(cols[col][j], false);
				}
			}
			$scope.$apply();
		}).fail(function (error) {
			$scope.loading = false;
			applyScope($scope);
			if ($scope.accessError(error)) { return; }
			alertbox({ content: error, type: "error" });
		}).loginFail(function () {
			window.location = DefaultUrl.login;
		});
	};

	$scope.widgetConfigChange = function (config) {
		$('.dashboard-widget').each(function (i, el) {
			if (el.widget.config.id === config.id) {
				el.widget.config = config;
				el.widget.configChanged();
				$scope.save();
				return;
			}
		});
	};

	$scope.selectWidget = function (config) {
		$('.dashboard-widget').each(function (i, el) {
			if (el.widget.config.id === config.id) {
				var col = el.widget.col;
				var index = el.widget.index;
				$(el).remove();
				$scope.addWidget({ col: col, index: index, name: config.selected.value, config: { title: config.selected.text } }, true, true);
				return;
			}
		});
	};

	$scope.removeWidget = function (id) {
		$('.dashboard-widget').each(function (i, el) {
			if (el.widget.config.id === id) {
				$(el).remove();
				$scope.save();
				return;
			}
		});
	};

	$rootScope.loadCurrentBusinessAndBusinesses($scope.init);
}
]);
