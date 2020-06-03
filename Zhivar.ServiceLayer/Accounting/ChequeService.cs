using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DataLayer.Context;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Common;
using Zhivar.ServiceLayer.Contracts.Accunting;
using Zhivar.ServiceLayer.Contracts.BaseInfo;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ServiceLayer.Accunting
{
    public class ChequeService : ICheque
    {
        IUnitOfWork _uow;
        readonly IDbSet<Cheque> _cheques;
        readonly IDbSet<Contact> _contacts;
        public ChequeService(IUnitOfWork uow)
        {
            _uow = uow;
            _cheques = _uow.Set<Cheque>();
            _contacts = _uow.Set<Contact>();
        }
        public bool Delete(int id)
        {
            try
            {
                var cheque = GetById(id);
                Delete(cheque);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool Delete(Cheque cheque)
        {
            try
            {
                _cheques.Attach(cheque);
                _cheques.Remove(cheque);

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
        public IList<Cheque> GetAll()
        {
            return _cheques.ToList();
        }
        public IList<Cheque> GetAllByOrganId(int organId)
        {
            var cheques = _cheques.AsQueryable().Where(x => x.OrganId == organId).ToList();

            return cheques;
        }
        public async Task<IList<Cheque>> GetAllByOrganIdAsync(int organId)
        {
            var cheques = await _cheques.AsQueryable().Where(x => x.OrganId == organId).Include(x => x.Contact).ToListAsync();

            return cheques;

        }
        public async Task<IList<Cheque>> GetAllAsync()
        {
            return await _cheques.ToListAsync();
        }
        public Cheque GetById(int id)
        {
            return _cheques.Find(id);
        }

        public bool Insert(Cheque cheque)
        {
            try
            {
                _cheques.Add(cheque);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public bool Update(Cheque cheque)
        {
            try
            {
                _cheques.Attach(cheque);

                _uow.Entry(cheque).State = EntityState.Modified;
                //_uow.Entry(cheque).Property(p => p.CreatedOn).IsModified = false;
                //_uow.Entry(cheque).Property(p => p.CreatedBy).IsModified = false;
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Cheque> GetByIdAsync(int id)
        {
            return await _cheques.AsQueryable().Where(x => x.ID == id).Include(x => x.Contact).FirstOrDefaultAsync();
        }

        public async Task<List<ChequeToPayVM>> GetChequesToPay(int organId)
        {
            var chequesQuery = _cheques.Where(x => x.OrganId == organId && x.Type == DomainClasses.ZhivarEnums.ChequeType.Dareaftani && x.Status == DomainClasses.ZhivarEnums.ChequeStatus.Normal).Include(x => x.Contact);
           // var contactQuery = _contacts;

            var joinQuery = from cheque in chequesQuery
                            //join contact in contactQuery
                            //on cheque.ContactId equals contact.ID
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

            return await joinQuery.ToListAsync();
        }
    }
}
