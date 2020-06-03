using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Accounting;

namespace Zhivar.Business.Accounting
{
    public partial class ChequeRule : BusinessRuleBase<Cheque>
    {
        public ChequeRule()
            : base()
        {

        }

        public ChequeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ChequeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Cheque> GetAllByOrganId(int organId)
        {
            var cheques = this.Queryable().Where(x => x.OrganId == organId).ToList();

            return cheques;
        }
        public async Task<IList<ChequeVM>> GetAllByOrganIdAsync(int organId)
        {
            var chequeQuery =  this.Queryable().Where(x => x.OrganId == organId);
            var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();

            var joinQuery = from cheque in chequeQuery
                            join contact in contactQuery
                            on cheque.ContactId equals contact.ID
                            select new ChequeVM
                            {
                                Date = cheque.Date,
                                ContactId = contact.ID,
                                Contact = contact,
                                ID = cheque.ID,
                                Amount = cheque.Amount,
                                BankBranch = cheque.BankBranch,
                                BankName = cheque.BankName,
                                ChequeNumber = cheque.ChequeNumber,
                                DepositBank = cheque.DepositBank,
                                DepositBankId = cheque.DepositBankId,
                                DisplayDate = cheque.DisplayDate,
                                OrganId = cheque.OrganId,
                                ReceiptDate = cheque.ReceiptDate,
                                Status = cheque.Status,
                                Type = cheque.Type
                            };
            return await joinQuery.ToListAsync2();

        }

        public async Task<List<ChequeToPayVM>> GetChequesToPay(int organId)
        {
            var chequesQuery = this.Queryable().Where(x => x.OrganId == organId && x.Type == DomainClasses.ZhivarEnums.ChequeType.Dareaftani && x.Status == DomainClasses.ZhivarEnums.ChequeStatus.Normal);
            var contactQuery = this.unitOfWork.RepositoryAsync<Contact>().Queryable();
            // var contactQuery = _contacts;

            var joinQuery = from cheque in chequesQuery
                                join contact in contactQuery
                                on cheque.ContactId equals contact.ID
                            select new ChequeToPayVM
                            {
                                Amount = cheque.Amount,
                                BankBranch = cheque.BankBranch,
                                BankName = cheque.BankName,
                                ChequeNumber = cheque.ChequeNumber,
                                Contact = cheque.Contact,
                                DepositBank = cheque.DepositBank,
                                Status = cheque.Status,
                                //ReceiptDate = Utilities.PersianDateUtils.ToPersianDate( cheque.ReceiptDate),
                                DisplayDate = cheque.DisplayDate,
                                Id = cheque.ID,


                            };

            return await joinQuery.ToListAsync2();
        }

    }
}