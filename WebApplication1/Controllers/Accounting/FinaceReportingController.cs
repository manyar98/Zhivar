using AutoMapper;
using OMF.Common.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.Business.Accounting;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;
using Zhivar.ViewModel.Common;

namespace Zhivar.Web.Controllers.Accounting
{
    [RoutePrefix("api/FinaceReporting")]
    public class FinaceReportingController : ApiController
    {
      
        [Route("GetBalanceSheet")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetBalanceSheet()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            var result = new BalanceSheet();

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            result.assets = new List<SummeryAccount>();

            #region دارایی جاری
            var daraeiJareiId = accounts.Where(x => x.ComplteCoding == "11").Select(x => x.ID).SingleOrDefault();
            decimal amounDaraeiJarei = 0;
            var transactionDaraeiJareiQuery = transactions.Where(x => x.AccountId == daraeiJareiId).AsQueryable();

            if (transactionDaraeiJareiQuery.Any())
            {
                var credit = transactionDaraeiJareiQuery.Sum(x => x.Credit);
                var debit = transactionDaraeiJareiQuery.Sum(x => x.Debit);
                amounDaraeiJarei = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounDaraeiJarei,
                Class = null,
                Date = null,
                Description = "دارایی های جاری",
                GroupCode = null,
                Type = "title",
            });
            #endregion
            #region صندوق
            var cash = accounts.Where(x => x.ComplteCoding == "1101").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childCashIds = accounts.Where(x => x.ParentId == cash.ID).Select(x => x.ID).ToList();
            decimal amounCash = 0;
            var cashQuery = transactions.Where(x => x.AccountId == cash.ID || childCashIds.Contains(x.AccountId)).AsQueryable();

            if (cashQuery.Any())
            {
                var credit = cashQuery.Sum(x => x.Credit);
                var debit = cashQuery.Sum(x => x.Debit);
                amounCash = debit-credit;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounCash,
                Class = null,
                Date = null,
                Description = cash.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region تنخواه
            var fund = accounts.Where(x => x.ComplteCoding == "1102").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childFundIds = accounts.Where(x => x.ParentId == fund.ID).Select(x => x.ID).ToList();
            decimal amounFund = 0;
            var fundQuery = transactions.Where(x => x.AccountId == fund.ID || childFundIds.Contains(x.AccountId)).AsQueryable();

            if (fundQuery.Any())
            {
                var credit = fundQuery.Sum(x => x.Credit);
                var debit = fundQuery.Sum(x => x.Debit);
                amounFund = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounFund,
                Class = null,
                Date = null,
                Description = fund.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region بانک ها
            var bank = accounts.Where(x => x.ComplteCoding == "1103").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childBankIds = accounts.Where(x => x.ParentId == bank.ID).Select(x => x.ID).ToList();
            decimal amounBank = 0;
            var bankQuery = transactions.Where(x => x.AccountId == bank.ID || childBankIds.Contains(x.AccountId)).AsQueryable();

            if (bankQuery.Any())
            {
                var credit = bankQuery.Sum(x => x.Credit);
                var debit = bankQuery.Sum(x => x.Debit);
                amounBank = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounBank,
                Class = null,
                Date = null,
                Description = bank.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region حساب های دریافتنی
            var debtor = accounts.Where(x => x.ComplteCoding == "1104").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childDebtorIds = accounts.Where(x => x.ParentId == debtor.ID).Select(x => x.ID).ToList();
            decimal amounDebtor = 0;
            var debtorQuery = transactions.Where(x => x.AccountId == debtor.ID || childDebtorIds.Contains(x.AccountId)).AsQueryable();

            if (debtorQuery.Any())
            {
                var credit = debtorQuery.Sum(x => x.Credit);
                var debit = debtorQuery.Sum(x => x.Debit);
                amounDebtor = debit - credit;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounDebtor,
                Class = null,
                Date = null,
                Description = debtor.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region اسناد دریافتنی
            var receivable = accounts.Where(x => x.ComplteCoding == "1105").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childReceivableIds = accounts.Where(x => x.ParentId == receivable.ID).Select(x => x.ID).ToList();
            decimal amounReceivable = 0;
            var receivableQuery = transactions.Where(x => x.AccountId == receivable.ID || childReceivableIds.Contains(x.AccountId)).AsQueryable();

            if (receivableQuery.Any())
            {
                var credit = receivableQuery.Sum(x => x.Credit);
                var debit = receivableQuery.Sum(x => x.Debit);
                amounReceivable = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounReceivable,
                Class = null,
                Date = null,
                Description = receivable.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region چک های در جریان وصول
            var inProgres = accounts.Where(x => x.ComplteCoding == "1106").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childInProgresIds = accounts.Where(x => x.ParentId == inProgres.ID).Select(x => x.ID).ToList();
            decimal amounInProgres = 0;
            var inProgresQuery = transactions.Where(x => x.AccountId == inProgres.ID || childInProgresIds.Contains(x.AccountId)).AsQueryable();

            if (inProgresQuery.Any())
            {
                var credit = inProgresQuery.Sum(x => x.Credit);
                var debit = inProgresQuery.Sum(x => x.Debit);
                amounInProgres = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounInProgres,
                Class = null,
                Date = null,
                Description = inProgres.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region پیش پرداخت ها
            var prepayment = accounts.Where(x => x.ComplteCoding == "1107").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childPrepaymentIds = accounts.Where(x => x.ParentId == prepayment.ID).Select(x => x.ID).ToList();
            decimal amounPrepayment = 0;
            var prepaymentQuery = transactions.Where(x => x.AccountId == prepayment.ID || childPrepaymentIds.Contains(x.AccountId)).AsQueryable();

            if (prepaymentQuery.Any())
            {
                var credit = prepaymentQuery.Sum(x => x.Credit);
                var debit = prepaymentQuery.Sum(x => x.Debit);
                amounPrepayment = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounPrepayment,
                Class = null,
                Date = null,
                Description = prepayment.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region موجودی کالا ها
            var inventory = accounts.Where(x => x.ComplteCoding == "1108").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childInventoryIds = accounts.Where(x => x.ParentId == inventory.ID).Select(x => x.ID).ToList();
            decimal amounInventory = 0;
            var inventoryQuery = transactions.Where(x => x.AccountId == inventory.ID || childInventoryIds.Contains(x.AccountId)).AsQueryable();

            if (inventoryQuery.Any())
            {
                var credit = inventoryQuery.Sum(x => x.Credit);
                var debit = inventoryQuery.Sum(x => x.Debit);
                amounInventory = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounInventory,
                Class = null,
                Date = null,
                Description = inventory.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region ملزومات
            var essential = accounts.Where(x => x.ComplteCoding == "1109").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childEssentialIds = accounts.Where(x => x.ParentId == essential.ID).Select(x => x.ID).ToList();
            decimal amounEssential = 0;
            var essentialQuery = transactions.Where(x => x.AccountId == essential.ID || childEssentialIds.Contains(x.AccountId)).AsQueryable();

            if (essentialQuery.Any())
            {
                var credit = essentialQuery.Sum(x => x.Credit);
                var debit = essentialQuery.Sum(x => x.Debit);
                amounEssential = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounEssential,
                Class = null,
                Date = null,
                Description = essential.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region مطالبات مشکوک الوصول
            var other = accounts.Where(x => x.ComplteCoding == "1110").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childOtherIds = accounts.Where(x => x.ParentId == other.ID).Select(x => x.ID).ToList();
            decimal amounOther = 0;
            var otherQuery = transactions.Where(x => x.AccountId == other.ID || childOtherIds.Contains(x.AccountId)).AsQueryable();

            if (otherQuery.Any())
            {
                var credit = otherQuery.Sum(x => x.Credit);
                var debit = otherQuery.Sum(x => x.Debit);
                amounOther = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounOther,
                Class = null,
                Date = null,
                Description = other.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region ارزش افزوده خرید
            var valueAddedBuy = accounts.Where(x => x.ComplteCoding == "1111").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childValueAddedBuyIds = accounts.Where(x => x.ParentId == valueAddedBuy.ID).Select(x => x.ID).ToList();
            decimal amounValueAddedBuy = 0;
            var valueAddedBuyQuery = transactions.Where(x => x.AccountId == valueAddedBuy.ID || childValueAddedBuyIds.Contains(x.AccountId)).AsQueryable();

            if (valueAddedBuyQuery.Any())
            {
                var credit = valueAddedBuyQuery.Sum(x => x.Credit);
                var debit = valueAddedBuyQuery.Sum(x => x.Debit);
                amounValueAddedBuy = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounValueAddedBuy,
                Class = null,
                Date = null,
                Description = valueAddedBuy.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region دارایی ثابت 
            var fixedAsset = accounts.Where(x => x.ComplteCoding == "12").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childFixedAssetIds = accounts.Where(x => x.ParentId == fixedAsset.ID).Select(x => x.ID).ToList();
            decimal amounFixedAsset = 0;
            var fixedAssetQuery = transactions.Where(x => x.AccountId == fixedAsset.ID || childFixedAssetIds.Contains(x.AccountId)).AsQueryable();

            if (fixedAssetQuery.Any())
            {
                var credit = fixedAssetQuery.Sum(x => x.Credit);
                var debit = fixedAssetQuery.Sum(x => x.Debit);
                amounFixedAsset = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounFixedAsset,
                Class = null,
                Date = null,
                Description = fixedAsset.Name,
                GroupCode = null,
                Type = "title",
            });
            #endregion
            #region اثاثیه اداری
            var officeFurniture = accounts.Where(x => x.ComplteCoding == "1201").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childOfficeFurnitureIds = accounts.Where(x => x.ParentId == officeFurniture.ID).Select(x => x.ID).ToList();
            decimal amounOfficeFurniture = 0;
            var officeFurnitureQuery = transactions.Where(x => x.AccountId == officeFurniture.ID || childOfficeFurnitureIds.Contains(x.AccountId)).AsQueryable();

            if (officeFurnitureQuery.Any())
            {
                var credit = officeFurnitureQuery.Sum(x => x.Credit);
                var debit = officeFurnitureQuery.Sum(x => x.Debit);
                amounOfficeFurniture = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounOfficeFurniture,
                Class = null,
                Date = null,
                Description = officeFurniture.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region ماشین آلات
            var machinery = accounts.Where(x => x.ComplteCoding == "1202").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childMachineryIds = accounts.Where(x => x.ParentId == machinery.ID).Select(x => x.ID).ToList();
            decimal amounMachinery = 0;
            var machineryQuery = transactions.Where(x => x.AccountId == machinery.ID || childMachineryIds.Contains(x.AccountId)).AsQueryable();

            if (machineryQuery.Any())
            {
                var credit = machineryQuery.Sum(x => x.Credit);
                var debit = machineryQuery.Sum(x => x.Debit);
                amounMachinery = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounMachinery,
                Class = null,
                Date = null,
                Description = machinery.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region زمین
            var land = accounts.Where(x => x.ComplteCoding == "1203").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childLandIds = accounts.Where(x => x.ParentId == land.ID).Select(x => x.ID).ToList();
            decimal amounLand = 0;
            var landQuery = transactions.Where(x => x.AccountId == land.ID || childLandIds.Contains(x.AccountId)).AsQueryable();

            if (landQuery.Any())
            {
                var credit = landQuery.Sum(x => x.Credit);
                var debit = landQuery.Sum(x => x.Debit);
                amounLand = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounLand,
                Class = null,
                Date = null,
                Description = land.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region ساختمان 
            var building = accounts.Where(x => x.ComplteCoding == "1204").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childBuildingIds = accounts.Where(x => x.ParentId == building.ID).Select(x => x.ID).ToList();
            decimal amounBuilding = 0;
            var buildingQuery = transactions.Where(x => x.AccountId == building.ID || childBuildingIds.Contains(x.AccountId)).AsQueryable();

            if (buildingQuery.Any())
            {
                var credit = buildingQuery.Sum(x => x.Credit);
                var debit = buildingQuery.Sum(x => x.Debit);
                amounBuilding = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounBuilding,
                Class = null,
                Date = null,
                Description = building.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region وسائط نقلیه
            var vehicles = accounts.Where(x => x.ComplteCoding == "1205").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childVehiclesIds = accounts.Where(x => x.ParentId == vehicles.ID).Select(x => x.ID).ToList();
            decimal amounVehicles = 0;
            var vehiclesQuery = transactions.Where(x => x.AccountId == vehicles.ID || childVehiclesIds.Contains(x.AccountId)).AsQueryable();

            if (vehiclesQuery.Any())
            {
                var credit = vehiclesQuery.Sum(x => x.Credit);
                var debit = vehiclesQuery.Sum(x => x.Debit);
                amounVehicles = debit - credit;;
            }

            result.assets.Add(new SummeryAccount()
            {
                Amount = amounVehicles,
                Class = null,
                Date = null,
                Description = vehicles.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion


            result.liabilities = new List<SummeryAccount>();

            #region بدهی های جاری
            var liabilitiesId = accounts.Where(x => x.ComplteCoding == "11").Select(x => x.ID).SingleOrDefault();
            decimal amounLiabilities = 0;
            var transactionLiabilitiesQuery = transactions.Where(x => x.AccountId == liabilitiesId).AsQueryable();

            if (transactionLiabilitiesQuery.Any())
            {
                var credit = transactionLiabilitiesQuery.Sum(x => x.Credit);
                var debit = transactionLiabilitiesQuery.Sum(x => x.Debit);
                amounLiabilities = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounLiabilities,
                Class = null,
                Date = null,
                Description = " بدهی های جاری",
                GroupCode = null,
                Type = "title",
            });
            #endregion
            #region حساب های پرداختنی
            var creditors = accounts.Where(x => x.ComplteCoding == "2101").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childCreditorsIds = accounts.Where(x => x.ParentId == creditors.ID).Select(x => x.ID).ToList();
            decimal amounCreditors = 0;
            var creditorsQuery = transactions.Where(x => x.AccountId == creditors.ID || childCreditorsIds.Contains(x.AccountId)).AsQueryable();

            if (creditorsQuery.Any())
            {
                var credit = creditorsQuery.Sum(x => x.Credit);
                var debit = creditorsQuery.Sum(x => x.Debit);
                amounCreditors = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounCreditors,
                Class = null,
                Date = null,
                Description = creditors.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region اسناد پرداختنی
            var payable = accounts.Where(x => x.ComplteCoding == "2102").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childPayableIds = accounts.Where(x => x.ParentId == payable.ID).Select(x => x.ID).ToList();
            decimal amounPayable = 0;
            var payableQuery = transactions.Where(x => x.AccountId == payable.ID || childPayableIds.Contains(x.AccountId)).AsQueryable();

            if (payableQuery.Any())
            {
                var credit = payableQuery.Sum(x => x.Credit);
                var debit = payableQuery.Sum(x => x.Debit);
                amounPayable = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounPayable,
                Class = null,
                Date = null,
                Description = payable.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region حقوق پرداختنی
            var salary = accounts.Where(x => x.ComplteCoding == "2103").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childSalaryIds = accounts.Where(x => x.ParentId == salary.ID).Select(x => x.ID).ToList();
            decimal amounSalary = 0;
            var salaryQuery = transactions.Where(x => x.AccountId == salary.ID || childSalaryIds.Contains(x.AccountId)).AsQueryable();

            if (salaryQuery.Any())
            {
                var credit = salaryQuery.Sum(x => x.Credit);
                var debit = salaryQuery.Sum(x => x.Debit);
                amounSalary = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounSalary,
                Class = null,
                Date = null,
                Description = salary.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region هزینه پرداختنی
            var cost = accounts.Where(x => x.ComplteCoding == "2104").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childCostIds = accounts.Where(x => x.ParentId == cost.ID).Select(x => x.ID).ToList();
            decimal amounCost = 0;
            var costQuery = transactions.Where(x => x.AccountId == cost.ID || childCostIds.Contains(x.AccountId)).AsQueryable();

            if (costQuery.Any())
            {
                var credit = costQuery.Sum(x => x.Credit);
                var debit = costQuery.Sum(x => x.Debit);
                amounCost = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounCost,
                Class = null,
                Date = null,
                Description = cost.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region مالیات بر درآمد پرداختنی
            var incomeTax = accounts.Where(x => x.ComplteCoding == "2105").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childIncomeTaxIds = accounts.Where(x => x.ParentId == incomeTax.ID).Select(x => x.ID).ToList();
            decimal amounIncomeTax = 0;
            var incomeTaxQuery = transactions.Where(x => x.AccountId == incomeTax.ID || childIncomeTaxIds.Contains(x.AccountId)).AsQueryable();

            if (incomeTaxQuery.Any())
            {
                var credit = incomeTaxQuery.Sum(x => x.Credit);
                var debit = incomeTaxQuery.Sum(x => x.Debit);
                amounIncomeTax = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounIncomeTax,
                Class = null,
                Date = null,
                Description = incomeTax.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region مالیات بر ارزش افزوده
            var vAT = accounts.Where(x => x.ComplteCoding == "2106").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childVATIds = accounts.Where(x => x.ParentId == vAT.ID).Select(x => x.ID).ToList();
            decimal amounVAT = 0;
            var vATQuery = transactions.Where(x => x.AccountId == vAT.ID || childVATIds.Contains(x.AccountId)).AsQueryable();

            if (vATQuery.Any())
            {
                var credit = vATQuery.Sum(x => x.Credit);
                var debit = vATQuery.Sum(x => x.Debit);
                amounVAT = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounVAT,
                Class = null,
                Date = null,
                Description = vAT.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region بدهی های غیر جاری
            var nonCurrentDebtId = accounts.Where(x => x.ComplteCoding == "22").Select(x => x.ID).SingleOrDefault();
            decimal amounNonCurrentDebt = 0;
            var transactionNonCurrentDebtQuery = transactions.Where(x => x.AccountId == nonCurrentDebtId).AsQueryable();

            if (transactionNonCurrentDebtQuery.Any())
            {
                var credit = transactionNonCurrentDebtQuery.Sum(x => x.Credit);
                var debit = transactionNonCurrentDebtQuery.Sum(x => x.Debit);
                amounNonCurrentDebt = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounNonCurrentDebt,
                Class = null,
                Date = null,
                Description = " بدهی های غیر جاری",
                GroupCode = null,
                Type = "title",
            });
            #endregion
            #region وام و تسهیلات
            var loan = accounts.Where(x => x.ComplteCoding == "2201").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childLoanIds = accounts.Where(x => x.ParentId == loan.ID).Select(x => x.ID).ToList();
            decimal amounLoan = 0;
            var loanQuery = transactions.Where(x => x.AccountId == loan.ID || childLoanIds.Contains(x.AccountId)).AsQueryable();

            if (loanQuery.Any())
            {
                var credit = loanQuery.Sum(x => x.Credit);
                var debit = loanQuery.Sum(x => x.Debit);
                amounLoan = credit - debit;
            }

            result.liabilities.Add(new SummeryAccount()
            {
                Amount = amounLoan,
                Class = null,
                Date = null,
                Description = loan.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion


            result.capitals = new List<SummeryAccount>();

            #region سرمایه
            var capitalId = accounts.Where(x => x.ComplteCoding == "31").Select(x => x.ID).SingleOrDefault();
            decimal amounCapital = 0;
            var transactionCapitalQuery = transactions.Where(x => x.AccountId == capitalId).AsQueryable();

            if (transactionCapitalQuery.Any())
            {
                var credit = transactionCapitalQuery.Sum(x => x.Credit);
                var debit = transactionCapitalQuery.Sum(x => x.Debit);
                amounCapital = credit - debit;
            }

            result.capitals.Add(new SummeryAccount()
            {
                Amount = amounCapital,
                Class = null,
                Date = null,
                Description = " سرمایه",
                GroupCode = null,
                Type = "title",
            });
            #endregion
            #region سرمایه اولیه
            var initialInvestment = accounts.Where(x => x.ComplteCoding == "3101").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childInitialInvestmentIds = accounts.Where(x => x.ParentId == initialInvestment.ID).Select(x => x.ID).ToList();
            decimal amounInitialInvestment = 0;
            var initialInvestmentQuery = transactions.Where(x => x.AccountId == initialInvestment.ID || childInitialInvestmentIds.Contains(x.AccountId)).AsQueryable();

            if (initialInvestmentQuery.Any())
            {
                var credit = initialInvestmentQuery.Sum(x => x.Credit);
                var debit = initialInvestmentQuery.Sum(x => x.Debit);
                amounInitialInvestment = credit - debit;
            }

            result.capitals.Add(new SummeryAccount()
            {
                Amount = amounInitialInvestment,
                Class = null,
                Date = null,
                Description = initialInvestment.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region کاهش یا افزایش سرمایه
            var reduceOrIncreaseCapital = accounts.Where(x => x.ComplteCoding == "3102").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childReduceOrIncreaseCapitalIds = accounts.Where(x => x.ParentId == reduceOrIncreaseCapital.ID).Select(x => x.ID).ToList();
            decimal amounReduceOrIncreaseCapital = 0;
            var reduceOrIncreaseCapitalQuery = transactions.Where(x => x.AccountId == reduceOrIncreaseCapital.ID || childReduceOrIncreaseCapitalIds.Contains(x.AccountId)).AsQueryable();

            if (reduceOrIncreaseCapitalQuery.Any())
            {
                var credit = reduceOrIncreaseCapitalQuery.Sum(x => x.Credit);
                var debit = reduceOrIncreaseCapitalQuery.Sum(x => x.Debit);
                amounReduceOrIncreaseCapital = credit - debit;
            }

            result.capitals.Add(new SummeryAccount()
            {
                Amount = amounReduceOrIncreaseCapital,
                Class = null,
                Date = null,
                Description = reduceOrIncreaseCapital.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region برداشت ها
            var removal = accounts.Where(x => x.ComplteCoding == "3103").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childRemovalIds = accounts.Where(x => x.ParentId == removal.ID).Select(x => x.ID).ToList();
            decimal amounRemoval = 0;
            var removalQuery = transactions.Where(x => x.AccountId == removal.ID || childRemovalIds.Contains(x.AccountId)).AsQueryable();

            if (removalQuery.Any())
            {
                var credit = removalQuery.Sum(x => x.Credit);
                var debit = removalQuery.Sum(x => x.Debit);
                amounRemoval = credit - debit;
            }

            result.capitals.Add(new SummeryAccount()
            {
                Amount = amounRemoval,
                Class = null,
                Date = null,
                Description = removal.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region سهم سود و زیان
            var share = accounts.Where(x => x.ComplteCoding == "3104").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childShareIds = accounts.Where(x => x.ParentId == share.ID).Select(x => x.ID).ToList();
            decimal amounShare = 0;
            var shareQuery = transactions.Where(x => x.AccountId == share.ID || childShareIds.Contains(x.AccountId)).AsQueryable();

            if (shareQuery.Any())
            {
                var credit = shareQuery.Sum(x => x.Credit);
                var debit = shareQuery.Sum(x => x.Debit);
                amounShare = credit - debit;
            }

            result.capitals.Add(new SummeryAccount()
            {
                Amount = amounShare,
                Class = null,
                Date = null,
                Description = share.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region سود انباشته
            var accumulatedProfit = accounts.Where(x => x.ComplteCoding == "3105").Select(x => new { x.ID, x.Name }).SingleOrDefault();
            List<int> childAccumulatedProfitIds = accounts.Where(x => x.ParentId == accumulatedProfit.ID).Select(x => x.ID).ToList();
            decimal amounAccumulatedProfit = 0;
            var accumulatedProfitQuery = transactions.Where(x => x.AccountId == accumulatedProfit.ID || childAccumulatedProfitIds.Contains(x.AccountId)).AsQueryable();

            if (accumulatedProfitQuery.Any())
            {
                var credit = accumulatedProfitQuery.Sum(x => x.Credit);
                var debit = accumulatedProfitQuery.Sum(x => x.Debit);
                amounAccumulatedProfit = credit - debit;
            }

            result.capitals.Add(new SummeryAccount()
            {
                Amount = amounAccumulatedProfit,
                Class = null,
                Date = null,
                Description = accumulatedProfit.Name,
                GroupCode = null,
                Type = "",
            });
            #endregion
            #region سود خالص

            decimal amountSells = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.Sell);
            decimal amountBuys = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.Buy);
            decimal amountReturnSells = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.ReturnSell);
            decimal amountReturnBuys = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.ReturnBuy);
            decimal amountInventoryEndOfTheCourse = await CalcInventoryEndOfTheCourseAsync(organId);
            decimal amountInventoryOfTheBeginning = await CalcInventoryOfTheBeginningAsync(organId);
            decimal amountOperatingIncome = await CalcAccountByCodeAsync(organId, "71");
            decimal amountNonOperatingIncome = await CalcAccountByCodeAsync(organId, "72");
            decimal amountStaffCosts = await CalcAccountByCodeAsync(organId, "81");
            decimal amountPublicSpending = await CalcAccountByCodeAsync(organId, "82");
            decimal amountDistributionAndSalesCosts = await CalcAccountByCodeAsync(organId, "83");
            var profit = amountSells - amountBuys + amountReturnBuys - amountReturnSells +
                            amountInventoryEndOfTheCourse - amountInventoryOfTheBeginning +
                            amountOperatingIncome + amountNonOperatingIncome - amountStaffCosts +
                            amountPublicSpending - amountDistributionAndSalesCosts;

         

            result.capitals.Add(new SummeryAccount()
            {
                Amount = profit,
                Class = null,
                Date = null,
                Description = "سود خالص",
                GroupCode = null,
                Type = "",
            });
            #endregion
            PersianCalendar pc = new PersianCalendar();
            DateTime thisDate = DateTime.Now;
            string dateStr = Utilities.PersianDateUtils.ToPersianDate(thisDate);

            result.assetsSum = amounDaraeiJarei + amounCash + amounFund + amounBank + amounDebtor + amounReceivable + amounInProgres + amounPrepayment + amounInventory
                + amounEssential + amounOther + amounValueAddedBuy + amounFixedAsset + amounOfficeFurniture + amounMachinery + amounLand + amounBuilding + amounVehicles;
            result.balanceSheetDate = dateStr;
            result.capitalSum = amounCapital + amounInitialInvestment + amounReduceOrIncreaseCapital + amounRemoval + amounAccumulatedProfit+ profit;
            result.liabilitiesSum = amounLiabilities + amounCreditors + amounPayable + amounSalary + amounCost + amounIncomeTax + amounVAT + amounNonCurrentDebt + amounLoan;
            result.sumLeft = result.liabilitiesSum + result.capitalSum;
            result.sumRight = result.assetsSum;

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = result });
        }

        [Route("GetIncomeStatement")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetIncomeStatement()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            decimal amountSells = await CalcInvoicesByTypeAsync(organId , ZhivarEnums.NoeFactor.Sell);
            decimal amountBuys = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.Buy);
            decimal amountReturnSells = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.ReturnSell);
            decimal amountReturnBuys = await CalcInvoicesByTypeAsync(organId, ZhivarEnums.NoeFactor.ReturnBuy);
            decimal amountInventoryEndOfTheCourse = await CalcInventoryEndOfTheCourseAsync(organId);
            decimal amountInventoryOfTheBeginning = await CalcInventoryOfTheBeginningAsync(organId);
            decimal amountOperatingIncome = await CalcAccountByCodeAsync(organId, "71");
            decimal amountNonOperatingIncome = await CalcAccountByCodeAsync(organId, "72");
            decimal amountStaffCosts = await CalcAccountByCodeAsync(organId, "81");
            decimal amountPublicSpending = await CalcAccountByCodeAsync(organId, "82");
            decimal amountDistributionAndSalesCosts = await CalcAccountByCodeAsync(organId, "83");

            var list = new List<KeyValuePair<string, List<SummeryAccount>>>() {
                new KeyValuePair<string, List<SummeryAccount>>("table",new List<SummeryAccount>() {
                    new SummeryAccount() {
                        Amount= amountSells,
                        Class = null,
                        Date = null,
                        Description = "فروش",
                        GroupCode = null,
                        Type ="plus"
                    },new SummeryAccount() {
                        Amount= amountBuys,
                        Class = null,
                        Date = null,
                        Description = "خريد",
                        GroupCode = null,
                        Type ="minus",
                    },new SummeryAccount() {
                        Amount= amountReturnBuys,
                        Class = null,
                        Date = null,
                        Description = "برگشت از خريد",
                        GroupCode = null,
                        Type = "plus",

                    },new SummeryAccount() {
                        Amount = amountReturnSells,
                        Class = null,
                        Date = null,
                        Description = "برگشت از فروش",
                        GroupCode = null,
                        Type = "minus",
                    },new SummeryAccount() {
                        Amount = amountInventoryEndOfTheCourse,
                        Class = null,
                        Date = null,
                        Description = "موجودی کالای پایان دوره",
                        GroupCode = null,
                        Type = "plus",

                    },new SummeryAccount() {
                        Amount = amountInventoryOfTheBeginning,
                        Class = null,
                        Date = null,
                        Description = "موجودی کالای ابتدای دوره",
                        GroupCode = null,
                        Type = "minus",
                    },new SummeryAccount() {
                        Amount = 0,
                        Class = null,
                        Date = null,
                        Description = "درآمدها",
                        GroupCode = null,
                        Type = "title",
                    },new SummeryAccount() {
                        Amount = amountOperatingIncome,
                        Class = null,
                        Date = null,
                        Description = "درآمد های عملیاتی",
                        GroupCode = null,
                        Type = "plus",
                    },new SummeryAccount() {
                        Amount = amountNonOperatingIncome,
                        Class = null,
                        Date = null,
                        Description = "درآمد های غیر عملیاتی",
                        GroupCode = null,
                        Type = "plus",
                    },new SummeryAccount() {
                        Amount = 0,
                        Class = null,
                        Date = null,
                        Description = "هزينه ها",
                        GroupCode = null,
                        Type = "title",
                    },new SummeryAccount() {
                        Amount = amountStaffCosts,
                        Class = null,
                        Date = null,
                        Description = "هزینه های پرسنلی",
                        GroupCode = null,
                        Type = "minus",
                    },new SummeryAccount() {
                        Amount = Math.Abs(amountPublicSpending),
                        Class = null,
                        Date = null,
                        Description = "هزینه های عمومی",
                        GroupCode = null,
                        Type = "minus",
                    },new SummeryAccount() {
                        Amount = amountDistributionAndSalesCosts,
                        Class = null,
                        Date = null,
                        Description = "هزینه های توزیع و فروش",
                        GroupCode = null,
                        Type = "minus",
                    }
                }),new KeyValuePair<string, List<SummeryAccount>>("netIncome",new List<SummeryAccount>() {
                    new SummeryAccount()
                    {
                        Amount = amountSells - amountBuys + amountReturnBuys - amountReturnSells + 
                                    amountInventoryEndOfTheCourse - amountInventoryOfTheBeginning +
                                    amountOperatingIncome + amountNonOperatingIncome - amountStaffCosts + 
                                    amountPublicSpending - amountDistributionAndSalesCosts
                    }
                })
            };






            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = list });
        }

        private async Task<decimal> CalcAccountByCodeAsync(int organId, string code)
        {
            //var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            decimal amount = 0;
            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var operatinIncomeAccount = accounts.Where(x => x.ComplteCoding == code).SingleOrDefault();

            var accountsMoienQuery = accounts.AsQueryable().Where(x => x.ParentId == operatinIncomeAccount.ID);

            var allAccountQuery = accounts.AsQueryable();

            List<int> childIds = (from account in accountsMoienQuery
                                  select account.ID).ToList();

            List<int> childChildIds = (from account in allAccountQuery
                                       join accountsMoien in accountsMoienQuery
                                       on account.ParentId equals accountsMoien.ID
                                       select account.ID).ToList();


            var selected = transactions.Where(a => a.AccountId == operatinIncomeAccount.ID || childIds.Contains(a.AccountId) || childChildIds.Contains(a.AccountId)).ToList();

            if (selected.Any())
            {
                var credit = selected.Sum(x => x.Credit);
                var debit = selected.Sum(x => x.Debit);
                amount = credit - debit;
            }

            return amount;
        }


        private async Task<decimal> CalcInventoryOfTheBeginningAsync(int organId)
        {
            //var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            DocumentRule documentRule = new DocumentRule();
            var documents = await documentRule.GetAllByOrganIdAsync(organId);

            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);

            var firstDocument = documents.Where(x => x.IsFirsDocument == true).SingleOrDefault();

            decimal amount = 0;

            if (firstDocument == null)
                return amount;

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            var accountsQuery = accounts.AsQueryable().Where(x => x.ComplteCoding == "1108");
            var itemsQuery = accounts.AsQueryable();


            List<int> childIds = (from account in accountsQuery
                                  join item in itemsQuery
                                  on account.ID equals item.ParentId
                                  select item.ID).ToList();

            var selected = firstDocument.Transactions.Where(a => childIds.Contains(a.AccountId)).ToList();

            if (selected.Any())
            {

                foreach (var transaction in selected ?? new List<Zhivar.DomainClasses.Accounting.Transaction>())
                {
                    amount += transaction.Stock * transaction.UnitPrice;
                }
            }

            return amount;
        }

        private async Task<decimal> CalcInventoryEndOfTheCourseAsync(int organId)
        {
            ItemGroupRule itemGroupRule = new ItemGroupRule();
            var itemGroups = await itemGroupRule.GetAllByOrganIdAsync(organId);

            decimal amount = 0;

            foreach (var itemGroup in itemGroups)
            {
                foreach (var item in itemGroup.Items.Where(x => x.ItemType == ZhivarEnums.NoeItem.Item)?? new List<ItemVM>())
                {
                    amount += item.Stock * item.BuyPrice; 
                }
            }

            return amount;
        }

        private async Task<decimal> CalcInvoicesByTypeAsync(int organId, ZhivarEnums.NoeFactor invoiceType)
        {
            InvoiceRule invoiceRule = new InvoiceRule();
            var invoices = await invoiceRule.GetAllByOrganIdAsync(organId);
            invoices = invoices.Where(x => x.InvoiceType == invoiceType && (x.Status == ZhivarEnums.NoeInsertFactor.WaitingToReceive || x.Status == ZhivarEnums.NoeInsertFactor.Received)).ToList();

            decimal amount = 0;

            foreach (var invoice in invoices)
            {
                foreach (var invoiceItem in invoice.InvoiceItems?? new List<InvoiceItemVM>())
                {
                    if (invoiceItem.Item.ItemType == ZhivarEnums.NoeItem.Item)
                    {
                        amount += invoiceItem.Sum;
                    }
                }
            }

            return amount;
        }

        //        [Route("GetCapitalStatement")]
        //        [HttpPost]
        //        public virtual async Task<HttpResponseMessage> GetCapitalStatement()
        //        {
        //            var userId = SecurityManager.CurrentUserContext.UserId;
        //            //  var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //            var capitalStatement = new CapitalStatement();

        //            capitalStatement.reportDate = "1397/01/01";
        //            capitalStatement.tableCapitals = new List<SummeryAccount>() {
        //                    new SummeryAccount() {
        //Amount= 800000,
        //Class= null,
        //Date= "1397/01/01",
        //Description= "سند افتتاحیه - سرمایه اولیه مونا ابراهیمی",
        //GroupCode= null,
        //Type= "Debit",
        //                    },new SummeryAccount() {
        //                       Amount= 0,
        //Class= null,
        //Date= "1397/10/15",
        //Description= "سهم سود و زیان مونا ابراهیمی",
        //GroupCode= null,
        //Type= "Debit",
        //                    },new SummeryAccount() {
        //                      Amount= 0,
        //Class= null,
        //Date= "1397/10/15",
        //Description= "سهم سود و زیان مانیار محمدی",
        //GroupCode= null,
        //Type= "Debit",

        //                    },new SummeryAccount() {
        //                      Amount= -800000,
        //Class= null,
        //Date= "1397/10/15",
        //Description= "سرمایه نهایی مونا ابراهیمی",
        //GroupCode= null,
        //Type= "Debit",


        //                    },new SummeryAccount() {
        //       Amount= 0,
        //Class= null,
        //Date= "1397/10/15",
        //Description= "سرمایه نهایی مانیار محمدی",
        //GroupCode= null,
        //Type= "Debit",
        //                    }

        //            };






        //            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = capitalStatement });
        //        }



        //[Route("GetFinanAccounts")]
        //[HttpPost]
        //public virtual async Task<HttpResponseMessage> GetFinanAccounts()
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    //  var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    var finanAccount = new FinanAccount();

        //    finanAccount.systemAccountTypes = new List<string>() {  "حساب های دریافتنی","حساب های پرداختنی",
        //        "صندوق","بانک","تنخواه گردان","اسناد دریافتنی","اسناد پرداختنی","اسناد در جریان وصول",
        //        "موجودی کالا","فروش","خرید","برگشت از فروش","برگشت از خرید","مالیات بر ارزش افزوده",
        //        "مالیات بر ارزش افزوده فروش","مالیات بر ارزش افزوده خرید","مالیات بر درآمد","فروش خدمات",
        //        "هزینه خدمات خریداری شده","سرمایه اولیه","افزایش یا کاهش سرمایه","برداشت","سهم سود و زیان",
        //        "تخفیفات نقدی خرید","تخفیفات نقدی فروش","هزینه ضایعات کالا","کنترل ضایعات کالا","حقوق","تراز افتتاحیه",
        //        "تراز اختتامیه","خلاصه سود و زیان","سود انباشته"
        //    };
        //    finanAccount.finanAccounts = new List<AccountVM>() {
        //        new AccountVM() {
        //            Balance= 0,
        //            BalanceType = 2,
        //            Code = "1",
        //            Coding = "1",
        //            GroupCode = "1",
        //            //Id = 1,
        //            ID = 1,
        //            Level = Enums.AccountType.Moen,
        //            LevelString = "گروه",
        //            Name = "دارایی ها",
        //            ParentCoding = "",
        //            SystemAccount = 1,
        //            SystemAccountName = "",
        //            credit = 0,
        //            debit = 0
        //        },
        //    };


        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = finanAccount });
        //}



        [Route("GetAccountsToExplore")]
        [HttpPost]
        public virtual async Task<HttpResponseMessage> GetAccountsToExplore()
        {
            var userId = SecurityManager.CurrentUserContext.UserId;
            //  var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));




            var res = new List<AccountVM>() {
                new AccountVM() {
                    Balance= 0,
                    BalanceType = 2,
                    Code = "1",
                    Coding = "1",
                    GroupCode = "1",
                    ID = 1,
                    Level = ZhivarEnums.AccountType.Gorohe,
                    LevelString = "گروه",
                    Name = "دارایی ها",
                    ParentCoding = "",
                    SystemAccount = 1,
                    SystemAccountName = "",
                    credit = 0,
                    debit = 0
                },new AccountVM() {
                    Balance = 10000000,
BalanceType= 0,
Code= "01",
Coding= "1101",
GroupCode= "1",
ID= 3,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "صندوق ها",
ParentCoding= "11",
SystemAccount= 7,
SystemAccountName= "صندوق",
credit= 0,
debit= 0,
              },new AccountVM()
                {

                    Balance = 0,
BalanceType= 2,
Code= "02",
Coding= "1102",
GroupCode= "1",
ID= 4,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "تنخواه گردان",
ParentCoding= "11",
SystemAccount= 6,
SystemAccountName= "تنخواه گردان",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "03",
Coding= "1103",
GroupCode= "1",
ID= 5,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "بانک ها",
ParentCoding= "11",
SystemAccount= 8,
SystemAccountName= "بانک",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 5000000,
BalanceType= 0,
Code= "04",
Coding= "1104",
GroupCode= "1",
ID= 6,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "حسابهای دریافتنی",
ParentCoding= "11",
SystemAccount= 3,
SystemAccountName= "حساب های دریافتنی",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "05",
Coding= "1105",
GroupCode= "1",
ID= 7,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "اسناد دریافتنی",
ParentCoding= "11",
SystemAccount= 9,
SystemAccountName= "اسناد دریافتنی",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "06",
Coding= "1106",
GroupCode= "1",
ID= 8,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "چک های در جریان وصول",
ParentCoding= "11",
SystemAccount= 10,
SystemAccountName= "اسناد در جریان وصول",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "07",
Coding= "1107",
GroupCode= "1",
ID= 9,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "پیش پرداخت ها",
ParentCoding= "11",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 200000,
BalanceType= 0,
Code= "08",
Coding= "1108",
GroupCode= "1",
ID= 10,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "موجودی کالا",
ParentCoding= "11",
SystemAccount= 11,
SystemAccountName= "موجودی کالا",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "09",
Coding= "1109",
GroupCode= "1",
ID= 11,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "ملزومات",
ParentCoding= "11",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "10",
Coding= "1110",
GroupCode= "1",
ID= 12,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "ذخیره مطالبات مشکوک الوصول",
ParentCoding= "11",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "11",
Coding= "1111",
GroupCode= "1",
ID= 13,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "مالیات بر ارزش افزوده خرید",
ParentCoding= "11",
SystemAccount= 33,
SystemAccountName= "مالیات بر ارزش افزوده خرید",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "01",
Coding= "1201",
GroupCode= "1",
ID= 15,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "اثاثیه اداری",
ParentCoding= "12",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "02",
Coding= "1202",
GroupCode= "1",
ID= 16,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "ماشین آلات",
ParentCoding= "12",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "03",
Coding= "1203",
GroupCode= "1",
ID= 17,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "زمین",
ParentCoding= "12",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
},new AccountVM() {
Balance= 0,
BalanceType= 2,
Code= "04",
Coding= "1204",
GroupCode= "1",
ID= 18,
Level=ZhivarEnums.AccountType.Moen,
LevelString= "معین",
Name= "ساختمان",
ParentCoding= "12",
SystemAccount= 0,
SystemAccountName= "",
credit= 0,
debit= 0,
} };

            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = res });
        }





    }
}
