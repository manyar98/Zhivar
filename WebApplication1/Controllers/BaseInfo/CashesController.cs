using AutoMapper;
using OMF.Common.Security;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Zhivar.Business.Accounting;
using Zhivar.Business.BaseInfo;
using Zhivar.Business.Common;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses;
using Zhivar.ServiceLayer.Contracts.Accounting;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.Common;
using Zhivar.ViewModel.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;


namespace Zhivar.Web.Controllers.BaseInfo
{
    [RoutePrefix("api/Cashes")]
    public partial class CashesController : ApiController
    {


        //[Route("GetAllByOrganId")]
        //public virtual async Task<HttpResponseMessage> GetAllByOrganId()
        //{
        //    //var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));


        //    var list = await _goroheSazeService.GetAllByPersonIdAsync(Convert.ToInt32(organId));
        //    var list2 = list.Select(x => new { FamilyTree = "کالاها و خدمات", Id = x.ID, Title = x.Title, Parents = ",2,", SystemAccount = 2 }).ToList();

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = list2.AsQueryable() });
        //}

        //[Route("Add")]
        //[HttpPost]
        //public virtual async Task<HttpResponseMessage> AddGoroheKala(GoroheSazeVM goroheSazeVM)
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    if (!ModelState.IsValid)
        //    {
        //    }

        //    var goroheSaze = new GoroheSaze();


        //    Mapper.Map(goroheSazeVM, goroheSaze);
        //    goroheSaze.OrganID = organId;

        //    if (goroheSazeVM.ID.HasValue && goroheSaze.ID > 0)
        //    {
        //        _goroheSazeService.Update(goroheSaze);

        //    }
        //    else
        //    {
        //        _goroheSazeService.Insert(goroheSaze);
        //    }

        //    await _unitOfWork.SaveAllChangesAsync();

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Successful, data = goroheSaze });
        //}

        //[HttpPost]
        //public virtual async Task<HttpResponseMessage> Post(GoroheSazeVM goroheSazeVM)
        //{
        //    var userId = SecurityManager.CurrentUserContext.UserId;
        //    var person = personRule.GetPersonByUserId(Convert.ToInt32(userId));

        //    if (!ModelState.IsValid)
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Exception, data = "" });


        //    var goroheSaze = new GoroheSaze();

        //    Mapper.Map(goroheSazeVM, goroheSaze);

        //    _goroheSazeService.Insert(goroheSaze);

        //    goroheSaze.OrganID = organId;

        //    await _unitOfWork.SaveAllChangesAsync();

        //    var afetrGoroheKala = new GoroheSaze();
        //    afetrGoroheKala.OrganID = goroheSaze.OrganID;
        //    afetrGoroheKala.Title = goroheSaze.Title;
        //    // گرید آی دی جدید را به این صورت دریافت می‌کند

        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Exception, data = afetrGoroheKala });




        //}

        //[HttpPut]
        //public virtual async Task<HttpResponseMessage> Update(int id, GoroheSaze goroheSaze)
        //{
        //    var item = await _goroheSazeService.GetByIdAsync(id);

        //    if (item == null)
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.NotFound, data = "" });


        //    if (!ModelState.IsValid || id != item.ID)
        //        return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Exception, data = "" });


        //    //  var GoroheSaze = new GoroheSaze();
        //    //  Mapper.Map(GoroheKalaVM, GoroheSaze);

        //    _goroheSazeService.Update(goroheSaze);

        //    await _unitOfWork.SaveAllChangesAsync();

        //    //Return HttpStatusCode.OK
        //    return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)Enums.ResultCode.Exception, data = goroheSaze });

        //}


        [HttpPost]
        [Route("GetCashesAndBanks")]
        public virtual async Task<HttpResponseMessage> GetCashesAndBanks()
        {
            var organId = Convert.ToInt32(SecurityManager.CurrentUserContext.OrganizationId);

            BankRule bankRule = new BankRule();
            var banks = await bankRule.GetAllByOrganIdAsync(organId);

            CashRule cashRule = new CashRule();
            var cashs = await cashRule.GetAllByOrganIdAsync(organId);

            var cashes = new Cashe();

            cashes.banks = new List<BankVM>();
            cashes.cashes = new List<CashVM>();

            foreach (var bank in banks)
            {
                decimal balance = await CalcBalanceBank(bank.Code, organId);
                cashes.banks.Add(new BankVM() {
                    AccountNumber = bank.AccountNumber,
                    Balance = balance,
                    Branch = bank.Branch,
                    Code = bank.Code,
                    FullName = bank.FullName,
                    ID = bank.ID,
                    Name = bank.Name,
                    OrganId = bank.OrganId,
                    //Id = bank.ID,
                    DetailAccount = new DetailAccount()
                    {
                        Accounts = new List<AccountVM>(),
                        Balance = bank.Balance,
                        Name = bank.Name,
                        Node = new Node(),
                        
                    }
                });

           
            }

            foreach (var cash in cashs)
            {
                decimal balance = await CalcBalanceCash(cash.Code, organId);

                cashes.cashes.Add(new CashVM()
                {
                    Balance = balance,
                    Code = cash.Code,
                    ID = cash.ID,
                    Name = cash.Name,
                    OrganId = cash.OrganId,
                    //DetailAccount = new DetailAccount() {
                    //    Balance = cash.Balance,
                    //    Name = cash.Name,
                    //    Node = new Node()
                    //}
                });
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { resultCode = (int)ZhivarEnums.ResultCode.Successful, data = cashes });


        }

        private async Task<decimal> CalcBalanceCash(string code, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Find(x => x.ComplteCoding == "1101" + code);

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);
           
            transactions = transactions.FindAll(x => x.AccountId == account.ID);
            decimal credit = 0;
            decimal debit = 0;
     

            foreach (var item in transactions)
            {
                credit += item.Credit;
                debit += item.Debit;
            }
            
            return debit - credit;

        }

        private async Task<decimal> CalcBalanceBank(string code, int organId)
        {
            AccountRule accountRule = new AccountRule();
            var accounts = await accountRule.GetAllByOrganIdAsync(organId);
            var account = accounts.Find(x => x.ComplteCoding == "1103" + code || x.ComplteCoding == "2102" + code || x.ComplteCoding == "1106" + code);

            TransactionRule transactionRule = new TransactionRule();
            var transactions = await transactionRule.GetAllByOrganIdAsync(organId);

            transactions = transactions.FindAll(x => x.AccountId == account.ID);
            decimal credit = 0;
            decimal debit = 0;

            foreach (var item in transactions)
            {
                credit += item.Credit;
                debit += item.Debit;
            }

            return debit-credit ;
        }
    }
}