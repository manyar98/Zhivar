"use strict";
define(['angularAMD', 'angular-ui-route', 'ui-bootstrap', 'angular-sanitize', 'ncy-angular-breadcrumb', 'blockUI', 'animate', 'moment', 'momentJalaali', 'infoWindow', 'preApp', 'dataService', 'helperFactory'],
    function (angularAMD) {

        var baseUrl = "App/views/", defaultOtherwise = 'login';

        var app = angular.module("mainModule", ['ui.router', 'blockUI', 'ngSanitize', 'ui.bootstrap', 'ngAnimate', 'ncy-angular-breadcrumb'], ["$breadcrumbProvider", "blockUIConfigProvider", "$stateProvider", "$urlRouterProvider", "$locationProvider", '$provide',
            function ($breadcrumbProvider, blockUIConfigProvider, $stateProvider, $urlRouterProvider, $locationProvider, $provide) {
                $provide.constant("defaultUrlOtherwise", defaultOtherwise);
                $breadcrumbProvider.setOptions({
                    prefixStateName: 'home',
                    templateUrl: 'App/template/breadcrumb.html',
                });
                blockUIConfigProvider.blockBrowserNavigation = true;
                blockUIConfigProvider.autoBlock = false;
                blockUIConfigProvider.autoInjectBodyBlock = true;
                blockUIConfigProvider.delay = 100;
                blockUIConfigProvider.message = "";
                blockUIConfigProvider.templateUrl = 'App/template/blockUI.html';

                $urlRouterProvider.otherwise('/' + defaultOtherwise);
                $stateProvider.state('default', angularAMD.route({
                    url: '/default',
                    controllerUrl: 'App/views/main/default/defaultController.js',
                    views: {
                        'login': {
                            templateUrl: 'App/views/main/default/default.html',
                        }
                    }

                }))
                    .state('login', angularAMD.route({
                        url: '/login',
                        controllerUrl: 'App/views/main/security/login/loginController.js',
                        views: {
                            'login': {
                                templateUrl: 'App/views/main/security/login/login.html',
                            }
                        }

                    }))
                    .state('register', angularAMD.route({
                        url: '/register',
                        params: {
                            model: { value: null, dynamic: true }
                        },
                        controllerUrl: 'App/views/main/register/registerController.js',
                        views: {
                            'login': {
                                templateUrl: 'App/views/main/register/register.html',
                            }
                        }

                    }))

                    .state('finalRegisteration', angularAMD.route({
                        url: '/finalRegisteration',
                        params: {
                            model: { value: null }
                        },
                        controllerUrl: 'App/views/main/register/finalRegisterationController.js',
                        views: {
                            'login': {
                                templateUrl: 'App/views/main/register/finalRegisteration.html',
                            }
                        }

                    }))

                    .state('home', angularAMD.route({
                        url: '/home',
                        controllerUrl: 'App/views/main/home/homeController.js',
                        templateUrl: 'App/views/main/home/home.html',
                        ncyBreadcrumb: {
                            label: 'خانه'
                        }

                    }))
                    .state('dashboard', angularAMD.route({
                        url: '/dashboard',
                        controllerUrl: 'App/views/main/admin/dashboardController.js',
                        views: {
                            'dashboard': {
                                templateUrl: 'App/views/main/admin/dashboard.html',
                            }
                        },
                        ncyBreadcrumb: {
                            label: 'خانه'
                        }


                    })).state('baseInfo', angularAMD.route({
                        url: '/baseInfo',
                        controllerUrl: 'App/views/main/admin/baseInfoController.js',
                        templateUrl: 'App/views/main/admin/baseInfo.html',

                        ncyBreadcrumb: {
                            label: 'اطلاعات پایه'
                        }

                    }))
                    .state('shareholders', angularAMD.route({
                        url: '/shareholders',
                        controllerUrl: 'App/views/baseInfo/shareholders/shareholdersController.js',
                        templateUrl: 'App/views/baseInfo/shareholders/shareholders.html',

                        ncyBreadcrumb: {
                            label: 'سهامداران'

                        }
                    }))
                    .state('financial', angularAMD.route({
                        url: '/financial',
                        controllerUrl: 'App/views/main/admin/financialController.js',
                        templateUrl: 'App/views/main/admin/financial.html',

                        ncyBreadcrumb: {

                            label: 'اطلاعات مالی'
                        }

                    }))
                    .state('financialReport', angularAMD.route({
                        url: '/financialReport',
                        controllerUrl: 'App/views/main/admin/financialReportController.js',
                        templateUrl: 'App/views/main/admin/financialReport.html',

                        ncyBreadcrumb: {
                            label: 'گزارشات عملیات مالی'

                        }

                    }))
                    .state('financialList', angularAMD.route({
                        url: '/financialList',
                        controllerUrl: 'App/views/main/admin/financialListController.js',
                        templateUrl: 'App/views/main/admin/financialList.html',

                        ncyBreadcrumb: {
                            label: 'لیست عملیات مالی'

                        }

                    }))
                    .state('rack', angularAMD.route({
                        url: '/rack',
                        //params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/contract/rackController.js',
                        templateUrl: 'App/views/contract/rack.html',

                        ncyBreadcrumb: {
                            label: 'رک قراردادها'

                        }

                    }))
                
                    .state('contractProfile', angularAMD.route({
                        url: '/contractProfile',
                        //params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/contract/contractProfileController.js',
                        templateUrl: 'App/views/contract/contractProfile.html',

                        ncyBreadcrumb: {
                            label: 'نمایش قرارداد'

                        }

                    }))
                    .state('contracts', angularAMD.route({
                        url: '/contracts',
                        params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/contract/contractsController.js',
                        templateUrl: 'App/views/contract/contracts.html',

                        ncyBreadcrumb: {
                            label: 'لیست قراردادهای اجاره به طرف حساب'

                        }

                    }))
                    .state('rentContracts', angularAMD.route({
                        url: '/rentContracts',
                        params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/contract/rentContractsController.js',
                        templateUrl: 'App/views/contract/rentContracts.html',

                        ncyBreadcrumb: {
                            label: 'لیست قراردادهای اجاره از صاحب رسانه ',

                        }

                    }))
                    .state('contractsPrimative', angularAMD.route({
                        url: '/contractsPrimative',
                        params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/contract/contractsPrimativeController.js',
                        templateUrl: 'App/views/contract/contractsPrimative.html',
                        ncyBreadcrumb: {
                            label: 'لیست پیش قراردادها',

                        }

                    }))
                    .state('sazesOfContracts', angularAMD.route({
                        url: '/sazesOfContracts',
                        params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/contract/sazesOfContractsController.js',
                        templateUrl: 'App/views/contract/sazesOfContracts.html',

                        ncyBreadcrumb: {
                            label: 'لیست رسانه ها'

                        }

                    }))
                    .state('reservations', angularAMD.route({
                        url: '/reservations',
                        controllerUrl: 'App/views/contract/reservationsController.js',
                        templateUrl: 'App/views/contract/reservations.html',

                        ncyBreadcrumb: {
                            label: 'رزروها'

                        }
                    }))
                
                    .state('contract', angularAMD.route({
                        url: '/contract',
                        controllerUrl: 'App/views/contract/contractController.js',
                        templateUrl: 'App/views/contract/contract.html',

                        ncyBreadcrumb: {
                            label: 'قراردادها',

                        }
                    }))
                    .state('newContract', angularAMD.route({
                        url: '/newContract',
                        params: { id: null, displayDate: null, lstSaze: null, reservationID: null },
                        controllerUrl: 'App/views/contract/newContractController.js',
                        templateUrl: 'App/views/contract/newContract.html',

                        ncyBreadcrumb: {
                            label: 'قرارداد جدید اجاره به طرف حساب',

                        }
                    }))
                    .state('newRentContract', angularAMD.route({
                        url: '/newRentContract',
                        params: { id: null },
                        controllerUrl: 'App/views/contract/newRentContractController.js',
                        templateUrl: 'App/views/contract/newRentContract.html',

                        ncyBreadcrumb: {
                            label: 'قرارداد جدید اجاره از صاحب رسانه',

                        }
                    }))
                    .state('viewSazeOfContract', angularAMD.route({
                        url: '/viewSazeOfContract',
                        params: { id: null },
                        controllerUrl: 'App/views/contract/viewSazeOfContractController.js',
                        templateUrl: 'App/views/contract/viewSazeOfContract.html',

                        ncyBreadcrumb: {
                            label: 'مشاهده رسانه',

                        }
                    }))
                
                    .state('invoice', angularAMD.route({
                        url: '/invoice',
                        params: { id: null },
                        controllerUrl: 'App/views/invoice/invoiceController.js',
                        templateUrl: 'App/views/invoice/invoice.html',

                        ncyBreadcrumb: {
                            label: 'فاکتور',

                        }

                    }))
                
                
                    .state('invoicesRentTo', angularAMD.route({
                        url: '/invoicesRentTo',
                        params: { id: "rentTo", show: "rentTo" },
                        controllerUrl: 'App/views/invoice/invoicesController.js',
                        templateUrl: 'App/views/invoice/invoices.html',

                        ncyBreadcrumb: {
                            label: 'لیست فاکتورهای اجاره',

                        }

                    }))
                    .state('invoicesRentFrom', angularAMD.route({
                        url: '/invoicesRentFrom',
                        params: { id: "rentFrom", show: "rentFrom" },
                        controllerUrl: 'App/views/invoice/invoicesController.js',
                        templateUrl: 'App/views/invoice/invoices.html',

                        ncyBreadcrumb: {
                            label: 'لیست فاکتورهای اجاره از صاحب رسانه',

                        }

                    }))
                    .state('invoicesSell', angularAMD.route({
                        url: '/invoicesSell',
                        params: { id: "new", show: "new" },
                        controllerUrl: 'App/views/invoice/invoicesController.js',
                        templateUrl: 'App/views/invoice/invoices.html',

                        ncyBreadcrumb: {
                            label: 'لیست فاکتورهای فروش',

                        }

                    }))
                    .state('invoicesPurchase', angularAMD.route({
                        url: '/invoicesPurchase',
                        params: { id: "newBill", show: "bills" },
                        controllerUrl: 'App/views/invoice/invoicesController.js',
                        templateUrl: 'App/views/invoice/invoices.html',

                        ncyBreadcrumb: {
                            label: 'لیست فاکتورهای خرید',

                        }

                    }))
                    .state('invoicesSaleReturn', angularAMD.route({
                        url: '/invoicesSaleReturn',
                        params: { id: "saleReturn", show: "saleReturns" },
                        controllerUrl: 'App/views/invoice/invoicesController.js',
                        templateUrl: 'App/views/invoice/invoices.html',

                        ncyBreadcrumb: {
                            label: 'لیست برگشت از فروش ها',

                        }

                    }))
                    .state('invoicesPurchaseReturn', angularAMD.route({
                        url: '/invoicesPurchaseReturn',
                        params: { id: "purchaseReturn", show: "purchaseReturns" },
                        controllerUrl: 'App/views/invoice/invoicesController.js',
                        templateUrl: 'App/views/invoice/invoices.html',

                        ncyBreadcrumb: {
                            label: 'لیست برگشت از خریدها',

                        }

                    }))
                    .state('viewInvoice', angularAMD.route({
                        url: '/viewInvoice',
                        params: { id: null },
                        controllerUrl: 'App/views/invoice/viewInvoiceController.js',
                        templateUrl: 'App/views/invoice/viewInvoice.html',

                        ncyBreadcrumb: {
                            label: 'صورتحساب',
                            parent: 'invoices'
                        }

                    }))
                    .state('cost', angularAMD.route({
                        url: '/cost',
                        params: { id: null },
                        controllerUrl: 'App/views/cost/costController.js',
                        templateUrl: 'App/views/cost/cost.html',

                        ncyBreadcrumb: {
                            label: 'صورت هزینه',

                        }

                    }))
                    .state('costs', angularAMD.route({
                        url: '/costs',
                        params: { id: "new", show: null },
                        controllerUrl: 'App/views/cost/costsController.js',
                        templateUrl: 'App/views/cost/costs.html',

                        ncyBreadcrumb: {
                            label: 'لیست صورت هزینه ها',

                        }

                    }))
                    .state('viewCost', angularAMD.route({
                        url: '/viewCost',
                        params: { id: null },
                        controllerUrl: 'App/views/cost/viewCostController.js',
                        templateUrl: 'App/views/cost/viewCost.html',

                        ncyBreadcrumb: {
                            label: 'صورت هزینه',
                            parent: 'costs'
                        }

                    }))
                    .state('receive', angularAMD.route({
                        url: '/receive',
                        params: { cashId: null, bankId: null, rpId: null },
                        controllerUrl: 'App/views/receiveAndPay/receiveController.js',
                        templateUrl: 'App/views/receiveAndPay/receive.html',

                        ncyBreadcrumb: {
                            label: ' دریافت وجه/ چک',

                        }

                    }))
                    .state('pay', angularAMD.route({
                        url: '/pay',
                        params: { cashId: null, bankId: null, rpId: null },
                        controllerUrl: 'App/views/receiveAndPay/payController.js',
                        templateUrl: 'App/views/receiveAndPay/pay.html',

                        ncyBreadcrumb: {
                            label: 'پرداخت وجه/ چک',

                        }

                    }))
                    .state('receives', angularAMD.route({
                        url: '/receives',
                        params: { isReceive: 'receive' },
                        controllerUrl: 'App/views/receiveAndPay/receiveAndPayController.js',
                        templateUrl: 'App/views/receiveAndPay/receiveAndPay.html',

                        ncyBreadcrumb: {
                            label: 'لیست رسیدهای دریافت',

                        }

                    }))

                    .state('pays', angularAMD.route({
                        url: '/pays',
                        params: { isReceive: 'pay' },
                        controllerUrl: 'App/views/receiveAndPay/receiveAndPayController.js',
                        templateUrl: 'App/views/receiveAndPay/receiveAndPay.html',

                        ncyBreadcrumb: {
                            label: 'لیست رسیدهای پرداخت',

                        }

                    }))

                    .state('chequesReceivables', angularAMD.route({
                        url: '/chequesReceivables',
                        params: { show: 'receivables', number: null },
                        controllerUrl: 'App/views/cheques/chequesController.js',
                        templateUrl: 'App/views/cheques/cheques.html',

                        ncyBreadcrumb: {
                            label: 'لیست چک ها دریافتی',

                        }

                    }))
                    .state('chequesPayables', angularAMD.route({
                        url: '/chequesPayables',
                        params: { show: 'payables', number: null },
                        controllerUrl: 'App/views/cheques/chequesController.js',
                        templateUrl: 'App/views/cheques/cheques.html',

                        ncyBreadcrumb: {
                            label: 'لیست چک های پرداختی ',

                        }

                    }))
                    .state('transfers', angularAMD.route({
                        url: '/transfers',

                        controllerUrl: 'App/views/moneyTransfer/transfersController.js',
                        templateUrl: 'App/views/moneyTransfer/transfers.html',

                        ncyBreadcrumb: {
                            label: 'انتقال وجه ها',

                        }

                    }))
                    .state('moneyTransfer', angularAMD.route({
                        url: '/moneyTransfer',
                        params: { fromCash: null, fromBank: null, toCash: null, toBank: null, docId: null },
                        controllerUrl: 'App/views/moneyTransfer/moneyTransferController.js',
                        templateUrl: 'App/views/moneyTransfer/moneyTransfer.html',

                        ncyBreadcrumb: {
                            label: 'انتفال وجه',

                        }

                    }))
                    .state('editDocument', angularAMD.route({
                        url: '/editDocument',
                        params: { id: "new" },
                        controllerUrl: 'App/views/document/editDocumentController.js',
                        templateUrl: 'App/views/document/editDocument.html',

                        ncyBreadcrumb: {
                            label: 'سند حسابداری',

                        }

                    })).state('documents', angularAMD.route({
                        url: '/documents',
                        params: { id: "new" },
                        controllerUrl: 'App/views/document/documentsController.js',
                        templateUrl: 'App/views/document/documents.html',

                        ncyBreadcrumb: {
                            label: 'اسناد حسابداری',

                        }

                    })).state('accountsBalance', angularAMD.route({
                        url: '/accountsBalance',
                        controllerUrl: 'App/views/document/accountsBalanceController.js',
                        templateUrl: 'App/views/document/accountsBalance.html',

                        ncyBreadcrumb: {
                            label: 'سند افتتاحیه',

                        }

                    })).state('closeFinanYear', angularAMD.route({
                        url: '/closeFinanYear',
                        controllerUrl: 'App/views/document/closeFinanYearController.js',
                        templateUrl: 'App/views/document/closeFinanYear.html',

                        ncyBreadcrumb: {
                            label: 'سند اختتامیه',

                        }

                    }))

                    .state('chartOfAccounts', angularAMD.route({
                        url: '/chartOfAccounts',
                        params: { id: null },
                        controllerUrl: 'App/views/report/chartOfAccountsController.js',
                        templateUrl: 'App/views/report/chartOfAccounts.html',

                        ncyBreadcrumb: {
                            label: 'تراز حساب ها (تراز آزمایشی)',

                        }

                    }))
                    .state('itemByInvoice', angularAMD.route({
                        url: '/itemByInvoice',
                        params: { type: 'sales' },
                        controllerUrl: 'App/views/report/itemByInvoice/itemByInvoiceController.js',
                        templateUrl: 'App/views/report/salesByItem/itemByInvoice.html',

                        ncyBreadcrumb: {
                            label: 'فروش کالا یا خدمات به تفکیک فاکتور',

                        }

                    })).state('itemByInvoicePurchase', angularAMD.route({
                        url: '/itemByInvoicePurchase',
                        params: { type: 'purchase' },
                        controllerUrl: 'App/views/report/itemByInvoice/itemByInvoiceController.js',
                        templateUrl: 'App/views/report/salesByItem/purchase.html',

                        ncyBreadcrumb: {
                            label: 'خرید کالا به تفکیک فاکتور',

                        }

                    }))
                    .state('purchase', angularAMD.route({
                        url: '/purchase',
                        params: { type: 'purchase' },
                        controllerUrl: 'App/views/report/salesByItem/purchaseController.js',
                        templateUrl: 'App/views/report/salesByItem/purchase.html',

                        ncyBreadcrumb: {
                            label: 'خرید به تفکیک کالا',

                        }

                    })).state('salesByItem', angularAMD.route({
                        url: '/salesByItem',
                        params: { type: 'sales' },
                        controllerUrl: 'App/views/report/salesByItem/salesByItemController.js',
                        templateUrl: 'App/views/report/salesByItem/salesByItem.html',

                        ncyBreadcrumb: {
                            label: 'فروش به تفکیک کالا',

                        }



                    }))
                    .state('balanceSheet', angularAMD.route({
                        url: '/balanceSheet',
                        controllerUrl: 'App/views/report/balanceSheetController.js',
                        templateUrl: 'App/views/report/balanceSheet.html',

                        ncyBreadcrumb: {
                            label: 'ترازنامه',

                        }
                    }))
                    .state('bankTransactions', angularAMD.route({
                        url: '/bankTransactions',
                        params: { id: null },
                        controllerUrl: 'App/views/report/bankTransactionsController.js',
                        templateUrl: 'App/views/report/bankTransactions.html',

                        ncyBreadcrumb: {
                            label: 'گردش حساب بانک',

                        }

                    }))
                    .state('capitalStatement', angularAMD.route({
                        url: '/capitalStatement',
                        controllerUrl: 'App/views/report/capitalStatementController.js',
                        templateUrl: 'App/views/report/capitalStatement.html',

                        ncyBreadcrumb: {
                            label: 'صورتحساب سرمایه',

                        }

                    }))
                    .state('cashTransactions', angularAMD.route({
                        url: '/cashTransactions',
                        params: { id: null },
                        controllerUrl: 'App/views/report/cashTransactionsController.js',
                        templateUrl: 'App/views/report/cashTransactions.html',

                        ncyBreadcrumb: {
                            label: 'گردش حساب صندوق',

                        }

                    }))
                    .state('contactCard', angularAMD.route({
                        url: '/contactCard',
                        controllerUrl: 'App/views/report/contactCardController.js',
                        templateUrl: 'App/views/report/contactCard.html',

                        ncyBreadcrumb: {
                            label: 'کارت حساب اشخاص',

                        }

                    }))
                    .state('debtorsCreditors', angularAMD.route({
                        url: '/debtorsCreditors',
                        controllerUrl: 'App/views/report/debtorsCreditorsController.js',
                        templateUrl: 'App/views/report/debtorsCreditors.html',

                        ncyBreadcrumb: {
                            label: 'بدهکاران و بستانکاران',

                        }

                    }))
                    .state('exploreAccounts', angularAMD.route({
                        url: '/exploreAccounts',
                        controllerUrl: 'App/views/report/exploreAccountsController.js',
                        templateUrl: 'App/views/report/exploreAccounts.html',

                        ncyBreadcrumb: {
                            label: 'مرور حساب ها ',

                        }

                    }))
                    .state('incomeStatement', angularAMD.route({
                        url: '/incomeStatement',
                        controllerUrl: 'App/views/report/incomeStatementController.js',
                        templateUrl: 'App/views/report/incomeStatement.html',

                        ncyBreadcrumb: {
                            label: 'صورت سود و زیان',

                        }

                    }))
                    .state('inventoryRpt', angularAMD.route({
                        url: '/inventoryRpt',
                        controllerUrl: 'App/views/report/inventoryRptController.js',
                        templateUrl: 'App/views/report/inventoryRpt.html',

                        ncyBreadcrumb: {
                            label: 'موجودی کالا',

                        }

                    }))
                    .state('itemCard', angularAMD.route({
                        url: '/itemCard',
                        controllerUrl: 'App/views/report/itemCardController.js',
                        templateUrl: 'App/views/report/itemCard.html',

                        ncyBreadcrumb: {
                            label: 'کارت حساب کالا',

                        }

                    }))
                    .state('journalReport', angularAMD.route({
                        url: '/journalReport',
                        controllerUrl: 'App/views/report/journalReportController.js',
                        templateUrl: 'App/views/report/journalReport.html',

                        ncyBreadcrumb: {
                            label: 'دفتر روزنامه',

                        }

                    }))
                    .state('journalTotalAccountsReport', angularAMD.route({
                        url: '/journalTotalAccountsReport',
                        controllerUrl: 'App/views/report/journalTotalAccountsReportController.js',
                        templateUrl: 'App/views/report/journalTotalAccountsReport.html',

                        ncyBreadcrumb: {
                            label: 'دفتر روزنامه در سطح معین',

                        }

                    }))
                    //.state('journalTotalAccountsReport', angularAMD.route({
                    //        url: '/journalTotalAccountsReport',
                    //        controllerUrl: 'App/views/report/journalTotalAccountsReportController.js',
                    //        templateUrl: 'App/views/report/journalTotalAccountsReport.html',

                    //    }))
                    .state('ledgerReport', angularAMD.route({
                        url: '/ledgerReport',
                        params: { accountId: null },
                        controllerUrl: 'App/views/report/ledgerReportController.js',
                        templateUrl: 'App/views/report/ledgerReport.html',

                        ncyBreadcrumb: {
                            label: 'دفتر کل، معین و تفصیلی',

                        }

                    }))
                    .state('saleBuyTaxRpt', angularAMD.route({
                        url: '/saleBuyTaxRpt',
                        controllerUrl: 'App/views/report/saleBuyTaxRptController.js',
                        templateUrl: 'App/views/report/saleBuyTaxRpt.html',


                        ncyBreadcrumb: {
                            label: 'خرید، فروش و مالیات',

                        }

                    }))
                    .state('contacts', angularAMD.route({
                        url: '/contacts',
                        controllerUrl: 'App/views/baseInfo/contacts/contactsController.js',
                        templateUrl: 'App/views/baseInfo/contacts/contacts.html',

                        ncyBreadcrumb: {
                            label: 'اشخاص',

                        }

                    }))
                    .state('editContact', angularAMD.route({
                        url: '/editContact',
                        params: { id: 0 },
                        controllerUrl: 'App/views/baseInfo/contacts/editContactController.js',
                        templateUrl: 'App/views/baseInfo/contacts/editContact.html',

                        ncyBreadcrumb: {
                            label: 'اطلاعات شخص',
                            parent: 'contacts'
                        }


                    }))
                    .state('inventory', angularAMD.route({
                        url: '/inventory',
                        controllerUrl: 'App/views/baseInfo/inventoryController.js',
                        templateUrl: 'App/views/baseInfo/inventory.html',

                        ncyBreadcrumb: {
                            label: 'کالا یا خدمات',

                        }

                    }))
                    .state('noeChap', angularAMD.route({
                        url: '/noeChap',
                        controllerUrl: 'App/views/baseInfo/noeChap/noeChapController.js',
                        templateUrl: 'App/views/baseInfo/noeChap/noeChap.html',

                        ncyBreadcrumb: {
                            label: 'نوع چاپ',

                        }

                    }))
                    .state('personel', angularAMD.route({
                        url: '/personel',
                        controllerUrl: 'App/views/baseInfo/personel/personelController.js',
                        templateUrl: 'App/views/baseInfo/personel/personel.html',

                        ncyBreadcrumb: {
                            label: 'پرسنل',

                        }

                    }))
                    .state('updateItemPrice', angularAMD.route({
                        url: '/updateItemPrice',
                        controllerUrl: 'App/views/baseInfo/items/updateItemPriceController.js',
                        templateUrl: 'App/views/baseInfo/items/updateItemPrice.html',

                        ncyBreadcrumb: {
                            label: 'ثبت سریع قیمت کالاها و خدمات',

                        }

                    }))
                    .state('saze', angularAMD.route({
                        url: '/saze',
                        controllerUrl: 'App/views/baseInfo/saze/sazeController.js',
                        templateUrl: 'App/views/baseInfo/saze/saze.html',


                        ncyBreadcrumb: {
                            label: 'رسانه ها',

                        }
                    })).state('cashes', angularAMD.route({
                        url: '/cashes',
                        controllerUrl: 'App/views/baseInfo/cashesController.js',
                        templateUrl: 'App/views/baseInfo/cashes.html',

                        ncyBreadcrumb: {
                            label: 'صندوق',

                        }
                    }))
                    .state('settings', angularAMD.route({
                        url: '/settings',

                        controllerUrl: 'App/views/main/admin/settingsController.js',
                        templateUrl: 'App/views/main/admin/settings.html',

                        ncyBreadcrumb: {
                            label: 'تنظیمات ',

                        }

                    }))
                    .state('finanSettings', angularAMD.route({
                        url: '/finanSettings',
                        params: { setupMode: false },
                        controllerUrl: 'App/views/settings/finanSettingsController.js',
                        templateUrl: 'App/views/settings/finanSettings.html',

                        ncyBreadcrumb: {
                            label: 'تنظیمات مالی ',

                        }

                    }))
                    .state('orgSettings', angularAMD.route({
                        url: '/orgSettings',
                        params: { setupMode: false },
                        controllerUrl: 'App/views/settings/orgSettingsController.js',
                        templateUrl: 'App/views/settings/orgSettings.html',

                        ncyBreadcrumb: {
                            label: 'تنظیمات موسسه',

                        }

                    }))
                    .state('forgetPassword',
                    angularAMD.route({
                        url: '/forgetPassword/:id',
                        controllerUrl: baseUrl + 'security/forgetPassword/forgetPasswordController.js',
                        views: {
                            'login': {
                                templateUrl: baseUrl + 'security/forgetPassword/forgetPassword.html'
                            }
                        }
                    }))
                    .state('forgotPassChanging',
                    angularAMD.route({
                        url: '/forgotPassChanging',
                        controllerUrl: baseUrl + 'security/forgetPassword/forgotPassChangingController.js',
                        views: {
                            'login': {
                                templateUrl: baseUrl + 'security/forgetPassword/forgotPassChanging.html'
                            }
                        }
                    }))
                    .state('error',
                    angularAMD.route({
                        url: '/error',
                        params: { message: null, stackTrace: null },
                        templateUrl: baseUrl + 'security/error/error.html',
                        controllerUrl: baseUrl + 'security/error/errorController.js',
                    }))
                    .state('roleSearch',
                    angularAMD.route({
                        url: '/roleSearch',
                        templateUrl: baseUrl + 'security/role/roleSearch.html',
                        controllerUrl: baseUrl + 'security/role/roleSearchController.js',
                        ncyBreadcrumb: {
                            label: 'مدیریت نقش ها'
                        }
                    }))
                    .state('roleInfo',
                    angularAMD.route({
                        url: '/roleInfo/:id/:mode',
                        templateUrl: baseUrl + 'security/role/roleInfo.html',
                        controllerUrl: baseUrl + 'security/role/roleInfoController.js',
                        ncyBreadcrumb: {
                            label: 'اطلاعات نقش',
                            parent: 'roleSearch'
                        }
                    }))
                    .state('roleOperation',
                    angularAMD.route({
                        url: '/roleOperation/:id',
                        templateUrl: baseUrl + 'security/role/roleOperation.html',
                        controllerUrl: baseUrl + 'security/role/roleOperationController.js',
                        ncyBreadcrumb: {
                            label: 'انتساب دسترسی ها',
                            parent: 'roleSearch'
                        }
                    }))
                    .state('operationSearch',
                    angularAMD.route({
                        url: '/operationSearch',
                        templateUrl: baseUrl + 'security/operation/operationSearch.html',
                        controllerUrl: baseUrl + 'security/operation/operationSearchController.js',
                        ncyBreadcrumb: {
                            label: 'مدیریت دسترسی ها'
                        }
                    }))
                    .state('operationInfo',
                    angularAMD.route({
                        url: '/operationInfo/:id/:mode',
                        templateUrl: baseUrl + 'security/operation/operationInfo.html',
                        controllerUrl: baseUrl + 'security/operation/operationInfoController.js',
                        ncyBreadcrumb: {
                            label: 'اطلاعات دسترسی',
                            parent: 'operationSearch'
                        }
                    }))
                    .state('positionOperation',
                    angularAMD.route({
                        url: '/positionOperation/:id',
                        templateUrl: baseUrl + 'security/position/positionOperation.html',
                        controllerUrl: baseUrl + 'security/position/positionOperationController.js',
                        ncyBreadcrumb: {
                            label: 'انتساب دسترسی',
                            parent: 'positionSearch'
                        }
                    }))
                    .state('personnelInfo',
                    angularAMD.route({
                        url: '/personnelInfo/:id',
                        templateUrl: baseUrl + 'baseInfo/personnel/personnelInfo.html',
                        controllerUrl: baseUrl + 'baseInfo/personnel/personnelInfoController.js',
                        ncyBreadcrumb: {
                            label: 'اطلاعات کاربر',
                            parent: 'personnelSearch'
                        }
                    }))
                    .state('cache',
                    angularAMD.route({
                        url: '/cache',
                        templateUrl: baseUrl + 'security/cache/cacheInfo.html',
                        controllerUrl: baseUrl + 'security/cache/cacheInfoController.js',
                        ncyBreadcrumb: {
                            label: 'مدیریت حافظه نهان'
                        }
                    }))
                    .state('menuSearch',
                    angularAMD.route({
                        url: '/menuSearch',
                        templateUrl: baseUrl + 'security/menu/menuSearch.html',
                        controllerUrl: baseUrl + 'security/menu/menuSearchController.js',
                        ncyBreadcrumb: {
                            label: 'مدیریت منو'
                        }
                    }))
                    .state('menuInfo',
                    angularAMD.route({
                        url: '/menuInfo/:id/:mode',
                        templateUrl: baseUrl + 'security/menu/menuInfo.html',
                        controllerUrl: baseUrl + 'security/menu/menuInfoController.js',
                        ncyBreadcrumb: {
                            label: 'اطلاعات منو',
                            parent: 'menuSearch'
                        }
                    }))
                    .state('userSearch',
                    angularAMD.route({
                        url: '/userSearch',
                        templateUrl: baseUrl + 'security/user/userSearch.html',
                        controllerUrl: baseUrl + 'security/user/userSearchController.js',
                        data: {
                            gridOptions: {},
                            fromChildState: false
                        },
                        onExit: function ($rootScope, $stateParams, $state) {
                            $state.current.data.fromChildState = false;
                        },
                        ncyBreadcrumb: {
                            label: 'مدیریت کاربران'
                        }
                    }))

                    .state('userInfo',
                    angularAMD.route({
                        url: '/userInfo/:id',
                        templateUrl: baseUrl + 'security/user/userInfo.html',
                        controllerUrl: baseUrl + 'security/user/userInfoController.js',
                        ncyBreadcrumb: {
                            label: 'اطلاعات کاربر',
                            parent: 'userSearch'
                        }
                    }))
                    .state('userOperationSearch',
                    angularAMD.route({
                        url: '/userOperationSearch',
                        templateUrl: baseUrl + 'security/operation/userOperationSearch.html',
                        controllerUrl: baseUrl + 'security/operation/userOperationSearchController.js',
                        ncyBreadcrumb: {
                            label: 'جستجوی دسترسی کاربران'
                        }
                    }))
                    .state('userRole',
                    angularAMD.route({
                        url: '/userRole/:id',
                        templateUrl: baseUrl + 'security/user/userRole.html',
                        controllerUrl: baseUrl + 'security/user/userRoleController.js',
                        ncyBreadcrumb: {
                            label: 'انتساب نقش',
                            parent: 'userSearch'
                        }
                    }))
                    .state('operationUser',
                    angularAMD.route({
                        url: '/operationUser/:id',
                        templateUrl: baseUrl + 'security/user/operationUser.html',
                        controllerUrl: baseUrl + 'security/user/operationUserController.js',
                        ncyBreadcrumb: {
                            label: 'انتساب دسترسی',
                            parent: 'userSearch'
                        }
                    }))
                    .state('changeUserPassword',
                    angularAMD.route({
                        url: '/changeUserPassword',
                        templateUrl: baseUrl + 'security/changePassword/changeUserPassword.html',
                        controllerUrl: baseUrl + 'security/changePassword/changeUserPasswordController.js',
                        ncyBreadcrumb: {
                            label: 'تغییر رمز عبور کاربران'
                        }
                    }))
                    .state('changePassword',
                    angularAMD.route({
                        url: '/changePassword',
                        templateUrl: baseUrl + 'security/changePassword/changePassword.html',
                        controllerUrl: baseUrl + 'security/changePassword/changePasswordController.js',
                        ncyBreadcrumb: {
                            label: 'تغییر رمز عبور'
                        }
                    }))
                    .state('exceptionSearch',
                    angularAMD.route({
                        url: '/exceptionSearch',
                        templateUrl: baseUrl + 'infrastructure/exceptions/exceptionSearch.html',
                        controllerUrl: baseUrl + 'infrastructure/exceptions/exceptionSearchController.js',
                        ncyBreadcrumb: {
                            label: 'خطاهای سیستم'
                        }
                    }))
                    .state('exceptionDetails',
                    angularAMD.route({
                        url: '/exceptionDetails/:id',
                        templateUrl: baseUrl + 'infrastructure/exceptions/exceptionDetails.html',
                        controllerUrl: baseUrl + 'infrastructure/exceptions/exceptionDetailsController.js',

                    }))
                    .state('activitySearch',
                    angularAMD.route({
                        url: '/activitySearch',
                        templateUrl: baseUrl + 'infrastructure/activities/activitySearch.html',
                        controllerUrl: baseUrl + 'infrastructure/activities/activitySearchController.js',
                        ncyBreadcrumb: {
                            label: 'رخدادهای سیستم'
                        }
                    }))
                    .state('activityDetails',
                    angularAMD.route({
                        url: '/activityDetails/:id',
                        templateUrl: baseUrl + 'infrastructure/activities/activityDetails.html',
                        controllerUrl: baseUrl + 'infrastructure/activities/activityDetailsController.js',
                        ncyBreadcrumb: {
                            label: 'اطلاعات رخداد',
                            parent: 'activitySearch'
                        }
                    }))
                    .state('onlineUser',
                    angularAMD.route({
                        url: '/onlineUser',
                        templateUrl: baseUrl + 'security/onlineUser/onlineUserSearch.html',
                        controllerUrl: baseUrl + 'security/onlineUser/onlineUserSearchController.js',
                        ncyBreadcrumb: {
                            label: 'کاربران آنلاین'
                        }
                    }))
                    .state('smsSearch',
                    angularAMD.route({
                        url: '/smsSearch',
                        templateUrl: baseUrl + 'common/sms/smsSearch.html',
                        controllerUrl: baseUrl + 'common/sms/smsSearchController.js',
                        ncyBreadcrumb: {
                            label: 'پیامک ها'
                        }
                    }))
                    .state('cartable',
                    angularAMD.route({
                        url: '/cartable',
                        templateUrl: baseUrl + 'main/cartable/cartable.html',
                        controllerUrl: baseUrl + 'main/cartable/cartableController.js',
                        ncyBreadcrumb: {
                            label: 'کارتابل'
                        }
                    }))
                    .state('mainCartable',
                    angularAMD.route({
                        url: '/mainCartable',
                        templateUrl: baseUrl + 'main/cartable/cartable.html',
                        controllerUrl: baseUrl + 'main/cartable/cartableController.js',
                        data: {
                            gridOptions: {},
                            fromChildState: false
                        },
                        onExit: function ($rootScope, $stateParams, $state) {
                            $state.current.data.fromChildState = false;
                        },
                        ncyBreadcrumb: {
                            label: 'کارتابل',
                        }
                    }))
                    .state('diff',
                    angularAMD.route({
                        url: '/diff',
            
                        controllerUrl: 'App/views/contract/diffController.js',
                        templateUrl: 'App/views/contract/diff.html',
                        ncyBreadcrumb: {
                            label: 'تست سروش'
                        }
                    }))
                    .state('verificationForm',
                    angularAMD.route({
                        url: '/verificationForm',
                        templateUrl: baseUrl + 'main/cartable/verificationForm.html',
                        controllerUrl: baseUrl + 'main/cartable/verificationFormController.js',
                        params: {
                            message: null,
                            nestedstate: {
                                value: "verificationForm.invoiceWorkflow",
                                dynamic: true
                            },
                            nestedstate6: {
                                value: "verificationForm.payWorkflow",
                                dynamic: true
                            },
                            nestedstate7: {
                                value: "verificationForm.receiveWorkflow",
                                dynamic: true
                            },
                            nestedstate8: {
                                value: "verificationForm.moneyTransferWorkflow",
                                dynamic: true
                            },
                            nestedstate9: {
                                value: "verificationForm.costWorkflow",
                                dynamic: true
                            },
                            nestedstate171: {
                                value: "verificationForm.wfContractTarah",
                                dynamic: true
                            },
                            nestedstate172: {
                                value: "verificationForm.wfContractModir",
                                dynamic: true
                            },
                            nestedstate173: {
                                value: "verificationForm.wfContractNasbNasab",
                                dynamic: true
                            },
                            nestedstate174: {
                                value: "verificationForm.wfContractNasbModir",
                                dynamic: true
                            },
                            nestedstate175: {
                                value: "verificationForm.wfContractChapChapkhaneh",
                                dynamic: true
                            },
                            nestedstate176: {
                                value: "verificationForm.wfContractChapModir",
                                dynamic: true
                            },
                            nestedstate18: {
                                value: "verificationForm.newContract",
                                dynamic: true
                            },
                            nestedstate19: {
                                value: "verificationForm.newRentContract",
                                dynamic: true
                            },
                           
                        },
                        ncyBreadcrumb: {
                            label: 'کارتابل',
                        }
                    }))
               

                    .state('verificationForm.newRentContract', angularAMD.route({
                        url: '/wfNewRentContract/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/contract/newRentContractController.js',
                        templateUrl: 'App/views/contract/newRentContract.html',
                        ncyBreadcrumb: {
                            label: 'قرارداد جدید اجاره از طرف حساب',
                        }

                    }))

                    .state('verificationForm.newContract', angularAMD.route({
                        url: '/wfNewContract/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/contract/newContractController.js',
                        templateUrl: 'App/views/contract/newContract.html',
                        ncyBreadcrumb: {
                            label: 'قرارداد جدید اجاره به طرف حساب',
                        }

                    }))
                    .state('verificationForm.wfContractTarah', angularAMD.route({
                        url: '/wfContractTarah/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/workflow/contract/tarahi/tarah/wfContractTarahController.js',
                        templateUrl: 'App/views/workflow/contract/tarahi/tarah/wfContractTarah.html',

                    }))
                    .state('verificationForm.wfContractModir', angularAMD.route({
                        url: '/wfContractModir/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/workflow/contract/tarahi/modir/wfContractModirController.js',
                        templateUrl: 'App/views/workflow/contract/tarahi/modir/wfContractModir.html',

                    }))
                    .state('verificationForm.wfContractNasbNasab', angularAMD.route({
                        url: '/wfContractNasbNasab/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/workflow/contract/nasb/nasab/wfContractNasbNasabController.js',
                        templateUrl: 'App/views/workflow/contract/nasb/nasab/wfContractNasbNasab.html',

                    }))
                    .state('verificationForm.wfContractNasbModir', angularAMD.route({
                        url: '/wfContractNasbModir/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/workflow/contract/nasb/modir/wfContractNasbModirController.js',
                        templateUrl: 'App/views/workflow/contract/nasb/modir/wfContractNasbModir.html',

                    }))
                    .state('verificationForm.wfContractChapChapkhaneh', angularAMD.route({
                        url: '/wfContractChapChapkhaneh/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/workflow/contract/chap/chapkhaneh/wfContractChapChapkhanehController.js',
                        templateUrl: 'App/views/workflow/contract/chap/chapkhaneh/wfContractChapChapkhaneh.html',

                    }))
                    .state('verificationForm.wfContractChapModir', angularAMD.route({
                        url: '/wfContractChapModir/:id',
                        params: { exchangeData: null },
                        controllerUrl: 'App/views/workflow/contract/chap/modir/wfContractChapModirController.js',
                        templateUrl: 'App/views/workflow/contract/chap/modir/wfContractChapModir.html',

                    }))
                    .state('verificationForm.invoiceWorkflow', angularAMD.route({
                        url: '/invoiceWorkflow/:id',
                        params: { exchangeData: null },
                       controllerUrl: baseUrl + 'workflow/invoice/invoiceWorkflowController.js',
                       templateUrl: baseUrl + 'workflow/invoice/invoiceWorkflow.html',
                    }))
                    .state('verificationForm.payWorkflow', angularAMD.route({
                        url: '/payWorkflow/:id',
                        params: { exchangeData: null },
                        controllerUrl: baseUrl + 'workflow/pay/payWorkflowController.js',
                        templateUrl: baseUrl + 'workflow/pay/payWorkflow.html',
                    }))

                .state('verificationForm.receiveWorkflow', angularAMD.route({
                    url: '/receiveWorkflow/:id',
                    params: { exchangeData: null },
                    controllerUrl: baseUrl + 'workflow/receive/receiveWorkflowController.js',
                    templateUrl: baseUrl + 'workflow/receive/receiveWorkflow.html',
                }))
                    .state('verificationForm.moneyTransferWorkflow', angularAMD.route({
                        url: '/moneyTransferWorkflow/:id',
                    params: { exchangeData: null },
                    controllerUrl: baseUrl + 'workflow/moneyTransfer/moneyTransferWorkflowController.js',
                    templateUrl: baseUrl + 'workflow/moneyTransfer/moneyTransferWorkflow.html',
                }))
                    .state('verificationForm.costWorkflow', angularAMD.route({
                        url: '/costWorkflow/:id',
                    params: { exchangeData: null },
                    controllerUrl: 'App/views/cost/costController.js',
                    templateUrl: 'App/views/cost/cost.html',
                    ncyBreadcrumb: {
                        label: 'صورت هزینه',

                    }

           
                }));

                

                function templateFactoryDecorator($delegate) {
                    var fromUrl = angular.bind($delegate, $delegate.fromUrl);
                    $delegate.fromUrl = function (url, params) {
                        var cacheBuster = Date.now().toString();
                        if (url !== null && angular.isDefined(url) && angular.isString(url)) {
                            url += (url.indexOf("?") === -1 ? "?" : "&");
                            url += "v=" + cacheBuster;
                        }
                        return fromUrl(url, params);
                    };
                    return $delegate;
                }
                $provide.decorator('$templateFactory', ['$delegate', templateFactoryDecorator]);
                $locationProvider.html5Mode({ enabled: true, requireBase: false });

           
            }]);



        var indexController = function ($scope, $rootScope, $http, $window, $timeout, $state, $stateParams, blockUI, defaultUrlOtherwise, dataService) {

            $scope.switchOn = {
                value: false
            };

            $rootScope.panelShowDialog = false;
            $rootScope.visibleForLoginView = true;
            $rootScope.visibleForAnonymousUser = false;
            var checkAuthenicate = true;
            $rootScope.MenuItems = [];
            $rootScope.project = { name: "ژیور", version: '1.0.0' };

            $scope.indexInitialize = function () {
                moment.loadPersian();
                $rootScope.currentDate = moment(new Date()).format('dddd، jD jMMMM jYYYY');

                $scope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {
                    stateChangeStartFn(event, toState, toParams, fromState, fromParams);
                });

                $timeout(function () {
                    if (reloadStatus == true)
                        $(".panel-reload-message").show();
                }, 1500);
            }

            $scope.closeReloadMessage = function () {
                $(".panel-reload-message").hide();
            }

            function stateChangeStartFn(event, toState, toParams, fromState, fromParams) {
                $("body").off("keypress");
                toState.previousState = fromState;

                if (toState.name !== defaultUrlOtherwise) {
                    $timeout(function () {
                        $(".panel-loadPage").hide();
                        $(".panel-after-loadpage").show();
                    }, 2000);
                }

                if (isDevelopment == false) {
                    var checkUrl = true;
                    var nonCheckUrlStateList = [
                        { stateName: "registeration", condition: true },
                        { stateName: defaultUrlOtherwise, condition: true },
                        { stateName: "login", condition: true },
                        //{ stateName: "azmayeshYab", condition: true },
                        { stateName: "diabetesQuestionnaire", condition: true },
                        { stateName: "joziatAzmayesh", condition: true }

                    ];

                    nonCheckUrlStateList.forEach(function (v, i, a) {
                        if (v.stateName === toState.name && v.condition)
                            checkUrl = false;
                    });

                    if (checkUrl && fromState.url && fromState.url.indexOf('^') > -1) {
                        var oldState = $.ajax({ type: "GET", url: "/app/api/AuthenticateUrl/GetUrl", async: false }).responseText;
                        var newState = toState.name;
                        for (var prop in toParams) {
                            newState += "/" + toParams[prop];
                        }
                        var state2 = newState.replace(newState, '"' + newState + '"');
                        if (oldState !== state2) {
                            event.preventDefault();
                            $state.go(defaultUrlOtherwise);
                        }
                    }
                    else if (checkUrl) {
                        var stateIndex = toState.name.indexOf('.'), state = toState.name;
                        if (stateIndex !== -1)
                            state = toState.name.substr(0, stateIndex);
                        for (var prop in toParams) {
                            state += "/" + toParams[prop];
                        }
                        $("#stateChangeStart").val(state);
                    }
                }

                if (angular.isDefined(toState.views)) {
                    if (toState.views.login) {
                        checkAuthenicate = false;
                        $rootScope.visibleForLoginView = true;
                        $rootScope.visibleForAnonymousUser = false;
                    }
                    else if (toState.views.anonymousUser) {
                        checkAuthenicate = false;
                        $rootScope.visibleForLoginView = true;
                        $rootScope.visibleForAnonymousUser = true;
                    }
                }
                //else if (angular.isUndefined(toState.views)) {
                //}

                if (checkAuthenicate) {
                    $rootScope.userModel = {};
                    $rootScope.visibleForLoginView = false;
                    $rootScope.visibleForAnonymousUser = false;
                    checkIfUserAuthenicated();
                    checkAuthenicate = false;
                }
            }

            function checkIfUserAuthenicated() {
                dataService.getPagedData('/app/api/Login/GetAuthenticatedUser', {}).then(function (data) {
                    $rootScope.userModel = data;
                    $rootScope.userModelAuthenticationType();
                    dataService.checkSession(false);
                    dataService.getMenus();
                });
            };

            $scope.logOff = function () {
                dataService.logOff(function () {
                    $state.go(defaultUrlOtherwise);
                });
            };

            $rootScope.userModelAuthenticationType = function () {
                $rootScope.userModel.isOrganizationUser = $rootScope.userModel.AuthenticationType == 2;
                $rootScope.userModel.isSystemUser = $rootScope.userModel.AuthenticationType == 1;
            }

            $rootScope.Hesabfa = Hesabfa;
            //$scope.builtinReports = builtinReports;
            $scope.logout = function () {
                callws(DefaultUrl.MainWebService + 'Logout', {})
                    .success(function () {
                        window.location = DefaultUrl.login;
                    }).fail(function () {
                        window.location = DefaultUrl.errorPage;
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.initIndex = function () {
                $('#businessNav').hide();
                if (!Hesabfa.businesses)
                    $scope.getCurrentUserAndBusinessInfo();
            };
            $scope.getCurrentUserAndBusinessInfo = function (callbackMethod) {
                if (callbackMethod)
                    callbackMethod();
            };
            $scope.getCurrentUserAndBusinesses = function (callbackMethod) {
                if (callbackMethod)
                    callbackMethod();
            };
            //$scope.getCurrentBusinessId = function () {
            //	return readCookie('currentBusinessId');
            //};
            $scope.goToMyBusinessList = function () {
            };
            $scope.switchBusiness = function (business) {
                Hesabfa.done = false;
                Hesabfa.finanYear = null;
                $location.path('/dashboard/' + business.Id + '/' + business.SetupStep);
            };
            $scope.switchFinanYear = function (finanYear) {
                if (finanYear.Id === $scope.currentFinanYear.Id) return;
                $scope.currentFinanYear = finanYear;
                var businessFinanYearLink = $scope.getBusinessFinanYearLink($rootScope.currentBusiness, finanYear);
                window.location = businessFinanYearLink;
                //		$location.path(businessFinanYearLink);
                applyScope($scope);
            };
            $scope.noClick = function (e) { e.stopPropagation(); };
            $scope.money = function (value) {
                if (value === undefined) return "";
                value = Math.round(value * 100) / 100;
                return $rootScope.farsiDigit(formatToCurrency(value));
            };
            $scope.getCurrency = function (Parentheses) {
                if (!$rootScope.currentBusiness || $rootScope.currentBusiness.Currency == undefined) return "";
                var currency = Hesabfa.currencies[$rootScope.currentBusiness.Currency];
                Hesabfa.moneyUnit = currency;
                return Parentheses ? "(" + currency + ")" : currency;
            };
            $scope.getDateNow = function () {
                $scope.dateNow = $scope.todayDate;
            };
            $rootScope.farsiDigit = function (value) {
                return Hesabfa.farsiDigit(value);
            };
            $rootScope.pageTitle = function (pageTitle) {
                $rootScope.pageTitleText = pageTitle;
                document.title = pageTitle + " - " + ($rootScope.currentBusiness ? $rootScope.currentBusiness.Name : "") + " - " + Hesabfa.pagePreTitle;
            };
            $scope.getRouteQuery = function (params, param) {
                if (!params) return "";
                var splitedParams = params.split('&');
                for (var i = 0; i < splitedParams.length; i++) {
                    var splitedParam = splitedParams[i].split('=');
                    if (splitedParam[0] === param)
                        return splitedParam[1];
                }
                return null;
            };
            $scope.$on("$locationChangeStart", function (e, currentLocation, previousLocation) {
                Hesabfa.previousLocation = previousLocation;
            });
            $scope.go = function (path) {
                $location.path(path);
                return;
            };
            $scope.underConstruction = function () {
                var content = "<h4 class='text-center'>این قسمت در دست ساخت میباشد</h4>";
                content += "<h5 class='text-muted text-center'>از شکیبایی شما متشکریم</h5>";
                alertbox({ content: content });
            };
            $rootScope.accessError = function (error) {
                if (typeof error === "string" && error.indexOf("[accessDenied]") > -1) {
                    var errorString = error.substr("[accessDenied]".length);
                    alertbox({
                        content: Hesabfa.accessDeniedString + "</br>" + errorString, onBtn1Click: function () {
                            $location.path("/dashboard/0/7");
                            $scope.$apply();
                            return;
                        }
                    });
                    return true;
                }
                return false;
            };
            $rootScope.isDecimalCurrency = function () {
                return Hesabfa.isDecimalCurrency();
            };
            $scope.startInactivityTimer = function () {
                $("#pageWrap").hide();
                $scope.inactivityTimer = setInterval(function () {
                    // new: just show logout window
                    //            $scope.inactivityLogout();
                    return;
                }, 1800000);
                document.onmousemove = $scope.resetLogoutTimer;
                document.onkeypress = $scope.resetLogoutTimer;
                $scope.inactivityTimerEvent = true;
            };
            $scope.inactivityLogout = function () {
                if ($scope.callingLogout) return;

                $scope.inactivityTimerEvent = false;
                clearInterval($scope.inactivityTimer);
                $("#pageWrap").show();

                $scope.inactivityLoginPassword = "";
                $scope.alertInactivityLoginModal = false;
                $scope.$apply();
                $scope.callingLogout = true;
                callws(DefaultUrl.MainWebService + 'Logout', {})
                    .success(function () {
                        $scope.callingLogout = false;
                        $("#pageWrap").show();
                        $("#inactivityLoginModal").modal({ backdrop: false, keyboard: false });
                        $("#inactivityLoginModal").modal("show");
                    }).fail(function (error) {
                        $scope.callingLogout = false;
                        $scope.$apply();
                        alertbox({ content: error });
                    }).loginFail(function () {
                        $scope.callingLogout = false;
                        $scope.$apply();
                        $("#inactivityLoginModal").modal({ backdrop: false, keyboard: false });
                        $("#inactivityLoginModal").modal("show");
                    });
            };
            $scope.resetLogoutTimer = function () {
                if (!$scope.inactivityTimerEvent) return;
                clearInterval($scope.inactivityTimer);
                clearInterval($scope.inactivitySubTimer);
                $scope.startInactivityTimer();
                $("#pageWrap").hide();
                $scope.$apply();
            };
            $scope.inactivityLogin = function () {
                if ($scope.calling) return;
                var password = $scope.inactivityLoginPassword;
                var email = $scope.user.Email;
                $scope.calling = true;
                callws(DefaultUrl.MainWebService + 'Login', { email: email, password: password, rememberMe: false, securityCode: "" })
                    .success(function () {
                        var finanYearId = 0;
                        if (Hesabfa.finanYear)
                            finanYearId = Hesabfa.finanYear.Id;

                        var ret = callwssync(DefaultUrl.MainWebService + 'InitBusiness', { businessId: $rootScope.currentBusiness.Id, finanYearId: finanYearId });
                        if (ret.result) {
                            $scope.calling = false;
                            $("#inactivityLoginModal").modal("hide");
                            $scope.startInactivityTimer();
                            $("#pageWrap").hide();
                            $scope.$apply();
                        }
                        else if (ret.loginFailed)
                            window.location = DefaultUrl.login;
                        else if (ret.error)
                            window.location = DefaultUrl.errorPage;
                    }).fail(function (error) {
                        $scope.calling = false;
                        $scope.alertInactivityLoginModal = true;
                        $scope.alertTypeInactivityLoginModal = "danger";
                        $scope.alertMessageInactivityLoginModal = error;
                        $scope.$apply();
                    }).loginFail(function () {
                        window.location = DefaultUrl.login;
                    });
            };
            $scope.isInFavReports = function (rptName) {
                if (!$scope.favReports)
                    return false;
                return $scope.favReports.indexOf(rptName) > -1;
            };
            $scope.ToGregorian = function (date) {
                return jalali_to_gregorian(date);
            };
            $scope.ToShamsi = function (date) {
                var dateParams = date.split('/');
                var year = parseInt(dateParams[2]);
                var month = parseInt(dateParams[0]);
                var day = parseInt(dateParams[1]);
                var dateArray = gregorian_to_jalali(year, month, day);
                return dateArray[0] + "/" + padleft(dateArray[1], 2) + "/" + padleft(dateArray[2], 2);
            };
            $rootScope.setUISetting = function (key, value) {
                if (typeof (Storage) !== "undefined") localStorage.setItem(key, value);
            };
            $rootScope.getUISetting = function (key) {
                return typeof (Storage) !== "undefined" ? localStorage.getItem(key) : null;
            };
            $scope.havePermission = function (units) {
                if (!$scope.permissions)
                    return false;
                if (!$scope.permissions.initialized) {
                    var parts = $scope.permissions.split(',');
                    $scope.permissions = { initialized: true, cache: {} };
                    for (var i = 0; i < parts.length; i++) {
                        $scope.permissions[parts[i]] = true;
                    }
                }
                var cache = $scope.permissions.cache[units];
                if (cache)
                    return cache === 1;

                var hasPermission = true;
                units = units.split(',');
                for (var j = 0; j < units.length; j++) {
                    if (!$scope.permissions[units[j]])
                        hasPermission = false;
                }
                $scope.permissions.cache[units] = hasPermission ? 1 : 2;
                return hasPermission;
            };
            $scope.havePermissionMenu = function (menu) {
                if (!$scope.permissions)
                    return false;

                if (!$scope.permissonsMenu) {
                    $scope.permissonsMenu = {}
                }

                var cache = $scope.permissonsMenu[menu];
                if (cache)
                    return cache === 1;


                var hasPermission = true;
                switch (menu) {
                    case 1:
                        if ($scope.havePermission('viewContacts') || $scope.havePermission('viewItems') || $scope.havePermission('cashBank')
                            || $scope.havePermission('finanAccounts') || $scope.havePermission('openingBalance'))
                            break;
                        hasPermission = false;
                        break;
                    case 2:
                        if ($scope.havePermission('inv,sl') || $scope.havePermission('inv,bl') || $scope.havePermission('inv,saleReturn')
                            || $scope.havePermission('inv,purchaseReturn') || $scope.havePermission('receiveFromContact')
                            || $scope.havePermission('income') || $scope.havePermission('receiveFromOther') || $scope.havePermission('payToContact')
                            || $scope.havePermission('expense') || $scope.havePermission('payToOther') || $scope.havePermission('spendCheque')
                            || $scope.havePermission('moneyTransfer') || $scope.havePermission('waste') || $scope.havePermission('doc'))
                            break;
                        hasPermission = false;
                        break;
                    case 3:
                        if ($scope.havePermission('sl') || $scope.havePermission('bl') || $scope.havePermission('saleReturn')
                            || $scope.havePermission('purchaseReturn') || $scope.havePermission('viewTransactions')
                            || $scope.havePermission('cheques') || $scope.havePermission('waste') || $scope.havePermission('doc'))
                            break;
                        hasPermission = false;
                        break;
                    case 4:
                        if ($scope.havePermission('balanceSheet') || $scope.havePermission('incomeStatement') || $scope.havePermission('capitalStatement')
                            || $scope.havePermission('inventoryRpt') || $scope.havePermission('journal') || $scope.havePermission('ledger')
                            || $scope.havePermission('trialBalance') || $scope.havePermission('exploreAccounts') || $scope.havePermission('salesByItem')
                            || $scope.havePermission('itemByInvoice') || $scope.havePermission('saleBuyTax') || $scope.havePermission('usersLog'))
                            break;
                        hasPermission = false;
                        break;
                    case 5:
                        if ($scope.havePermission('finanAccounts') || $scope.havePermission('stn') || $scope.havePermission('usr') || $scope.havePermission('closeFinanYear'))
                            break;
                        hasPermission = false;
                        break;
                    default:
                        hasPermission = false;
                }
                $scope.permissonsMenu[menu] = hasPermission ? 1 : 2;
                return hasPermission;
            };
            $scope.startInactivityTimer();
            $scope.getBusinessFinanYearLink = function (business, finanYear) {
                if (!business)
                    business = $rootScope.currentBusiness;
                var token = finanYear ? finanYear.token : business.token;
                return "?buf=" + token + "#/dashboard/" + business.Id + "/" + business.SetupStep;
            };
            $scope.getTokenQuerystring = function () {
                return $scope.currentFinanYear && $scope.currentFinanYear.token ? "?buf=" + $scope.currentFinanYear.token : "?buf=" + $rootScope.currentBusiness.token;
            };
            $scope.loadBusinesses = function (callback) {
                if ($scope.businesses) {
                    if (callback)
                        callback();
                    return;
                }
                if ($scope.loadingUserBusinessInfo)
                    return;
                $scope.loadingUserBusinessInfo = true;
                var token = "";//getQueryString("buf");

                $.ajax({
                    type: "POST",
                    //data: JSON.stringify({ start: "", end: "" }),
                    url: "app/api/Bussiness/GetCurrentUserAndBusinesses",
                    contentType: "application/json"
                }).done(function (res) {
                    var result = res.data;

                    $scope.loadingUserBusinessInfo = false;
                    $scope.currentUser = result.user;
                    $scope.businesses = result.businesses;
                    $scope.todayDate = result.todayDate;
                    $scope.showCloseFinanYearAlert = result.showCloseFinanYearAlert;

                    var bi = result.businessInfo;
                    if (bi) {
                        window.calendarType = bi.business.CalendarType;
                        window.currency = Hesabfa.currencies[bi.business.Currency];
                        $rootScope.currentBusiness = bi.business;
                        //var businessInList = findBusinessById($scope.businesses, bi.business.Id);
                        //$rootScope.currentBusiness.token = businessInList.token;
                        $scope.currentFinanYear = bi.finanYear;
                        $scope.finanYears = bi.finanYears;
                        //$scope.favReports = bi.favReports.split(',');
                        $scope.permissions = bi.permissions;
                        $scope.notifications = bi.notifications;
                        if (bi.notifications) {
                            var unreadCount = 0;
                            for (var i = 0; i < bi.notifications.length; i++) {
                                if (!bi.notifications[i].IsViewed)
                                    unreadCount++;
                            }
                            if (unreadCount > 0) {
                                $("#notificationBadge").addClass("notification-badge");
                                if (unreadCount <= 10)
                                    $("#notificationBadge").addClass("notification-badge-" + unreadCount);
                                else
                                    $("#notificationBadge").addClass("notification-badge-10plus");
                            }
                            else
                                $("#notificationBadge").removeClass("notification-badge");
                        }
                    }

                    if ($scope.businesses.length === 0)
                        window.location = "#addBusiness";

                    applyScope($scope);

                    if (callback)
                        callback();
                }).fail(function (error) {
                    $scope.loadingUserBusinessInfo = false;
                    $scope.$apply();
                    if ($scope.accessError(error)) return;
                    alertbox({ content: error });
                });
            }
            $rootScope.loadCurrentBusinessAndBusinesses = function (callback) {
                $scope.loadBusinesses(function () {
                    if (callback)
                        callback();
                });
            }
            $scope.viewDocumentModal = function (id) {
                if (!id) return;
                if ($scope.docLoading) return;
                $scope.docLoading = true;
                $.ajax({
                    type: "POST",
                    data: JSON.stringify(id),
                    url: "/app/api/Document/GetDocument",
                    contentType: "application/json"
                }).done(function (res) {

                    var document = res.data;

                    $scope.docLoading = false;
                    $scope.document = document;
                    $("#viewDocumentModal").modal({ keyboard: false }, "show");
                    $("#viewDocumentModal .modal-dialog").draggable({ handle: ".modal-header" });
                    applyScope($scope);
                }).fail(function (error) {
                    $scope.docLoading = false;
                    $scope.$apply();
                    if ($scope.accessError(error)) return;
                    alertbox({ title: "خطا", content: error, type: "error" });
                });
            };
            $scope.getTotalDebit = function () {
                var total = 0;
                if (!angular.isUndefined($scope.document) && $scope.document != null && $scope.document.Transactions != null) {
                    for (var i = 0; i < $scope.document.Transactions.length; i++) {
                        var product = $scope.document.Transactions[i];
                        total += product.Debit;
                    }

                }
                return total;
            }

            $scope.getTotalCredit = function () {
                var total = 0;
                if (!angular.isUndefined($scope.document) && $scope.document != null && $scope.document.Transactions != null) {
                    for (var i = 0; i < $scope.document.Transactions.length; i++) {
                        var product = $scope.document.Transactions[i];
                        total += product.Credit;
                    }
                }
                return total;
            }
            $scope.viewNotifications = function () {
                if ($scope.notifications) {
                    var notifications = $scope.notifications;
                    var len = notifications.length;
                    var unreadCount = 0;
                    for (var i = 0; i < len; i++) {
                        if (!notifications[i].IsViewed) {
                            unreadCount++;
                            notifications[i].IsViewed = true;
                        }
                    }
                    if (unreadCount > 0) {
                        callws(DefaultUrl.MainWebService + 'SetNotificationsViewd', {}).success(function () {
                            if (unreadCount <= 10)
                                $("#notificationBadge").removeClass("notification-badge-" + unreadCount);
                            else
                                $("#notificationBadge").removeClass("notification-badge-10plus");
                            $("#notificationBadge").removeClass("notification-badge");
                        }).fail(function (error) {
                            alertbox({ content: error, type: "error", title: "خطا" });
                        }).loginFail(function () {
                            window.location = DefaultUrl.login;
                        });
                    }
                }
            };
            $scope.deleteNotification = function (message) {
                callws(DefaultUrl.MainWebService + 'DeleteNotification', { notificationId: message.Id }).success(function () {
                    findAndRemove($scope.notifications, message);
                    applyScope($scope);
                    DevExpress.ui.notify("پیام حذف شد", "success", 3000);
                }).fail(function (error) {
                    alertbox({ content: error, type: "error", title: "خطا" });
                }).loginFail(function () {
                    window.location = DefaultUrl.login;
                });
            };
            $scope.deleteAllNotifications = function () {
                callws(DefaultUrl.MainWebService + 'DeleteAllNotifications', {}).success(function () {
                    $scope.notifications = [];
                    applyScope($scope);
                    DevExpress.ui.notify("تمام پیام ها حذف شدند", "success", 3000);
                }).fail(function (error) {
                    alertbox({ content: error, type: "error", title: "خطا" });
                }).loginFail(function () {
                    window.location = DefaultUrl.login;
                });
            };

            function updateNewFeaturesBadge() {
                $scope.showOnlineInvoiceSettingBadge = false;// readCookie("showOnlineInvoiceSettingBadge") === null;
            }
            $('.mainLoadingPanel').hide();
            updateNewFeaturesBadge();

        };
        indexController.$inject = ['$scope', '$rootScope', '$http', '$window', '$timeout', '$state', '$stateParams', 'blockUI', 'defaultUrlOtherwise', 'dataService'];
        app.controller("indexController", indexController);

        angularAMD.bootstrap(app);
        return app;

    });

