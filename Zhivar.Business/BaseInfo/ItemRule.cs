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
using Zhivar.DomainClasses;
using Zhivar.Business.Accounting;

namespace Zhivar.Business.BaseInfo
{
    public partial class ItemRule : BusinessRuleBase<Item>
    {
        public ItemRule()
            : base()
        {

        }

        public ItemRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ItemRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<Item> GetAll()
        {
            return this.Queryable().ToList();
        }

        public async Task<IList<Item>> GetAllAsync()
        {
            return await this.Queryable().ToListAsync2();
        }

        public async Task CreateServiceAccountAsync(Item item, int personId)
        {
            AccountRule accountRule = new AccountRule();

            var accounts = await accountRule.GetAllByOrganIdAsync(personId);

            var accountDaramdKhadamat = accounts.Where(x => x.ComplteCoding == "7101").SingleOrDefault();

            var tempAccountDaramdKhadamat = accounts.Where(x => x.ComplteCoding == "7101" + item.Code).SingleOrDefault();
            if (tempAccountDaramdKhadamat == null)
            {
                tempAccountDaramdKhadamat = new DomainClasses.Accounting.Account();
                tempAccountDaramdKhadamat.Coding = item.Code;
                tempAccountDaramdKhadamat.ComplteCoding = "7101" + item.Code;
                tempAccountDaramdKhadamat.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountDaramdKhadamat.Name = item.Name;
                tempAccountDaramdKhadamat.OrganId = personId;
                tempAccountDaramdKhadamat.ParentId = accountDaramdKhadamat.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountDaramdKhadamat);
                await this.UnitOfWork.SaveChangesAsync();
            }
       

            var accountSirHazeneh = accounts.Where(x => x.ComplteCoding == "8204").SingleOrDefault();

            var tempAccountSirHazeneh = accounts.Where(x => x.ComplteCoding == "8204"+item.Code).SingleOrDefault();
            if (tempAccountSirHazeneh == null)
            {
                tempAccountSirHazeneh = new DomainClasses.Accounting.Account();
                tempAccountSirHazeneh.Coding = item.Code;
                tempAccountSirHazeneh.ComplteCoding = "8204" + item.Code;
                tempAccountSirHazeneh.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountSirHazeneh.Name = item.Name;
                tempAccountSirHazeneh.OrganId = personId;
                tempAccountSirHazeneh.ParentId = accountSirHazeneh.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountSirHazeneh);
                await this.UnitOfWork.SaveChangesAsync();
            }
         
        }
        public async Task CreateGoodAccountsAsync(Item item, int personId)
        {
            AccountRule accountRule = new AccountRule();

            var accounts = await accountRule.GetAllByOrganIdAsync(personId);

            var accountMojodiKala = accounts.Where(x => x.ComplteCoding == "1108").SingleOrDefault();

            var tempAccountMojodiKala = accounts.Where(x => x.ComplteCoding == "1108" + item.Code).SingleOrDefault();

            if (tempAccountMojodiKala == null)
            {
                tempAccountMojodiKala = new DomainClasses.Accounting.Account();
                tempAccountMojodiKala.Coding = item.Code;
                tempAccountMojodiKala.ComplteCoding = "1108" + item.Code;
                tempAccountMojodiKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountMojodiKala.Name = item.Name;
                tempAccountMojodiKala.OrganId = personId;
                tempAccountMojodiKala.ParentId = accountMojodiKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountMojodiKala);
                await this.UnitOfWork.SaveChangesAsync();
            }


            var accountKharidKala = accounts.Where(x => x.ComplteCoding == "5101").SingleOrDefault();

            var tempAccountKharidKala = accounts.Where(x => x.ComplteCoding == "5101" + item.Code).SingleOrDefault();

            if (tempAccountKharidKala == null)
            {
                tempAccountKharidKala = new DomainClasses.Accounting.Account();

                tempAccountKharidKala.Coding = item.Code;
                tempAccountKharidKala.ComplteCoding = "5101" + item.Code;
                tempAccountKharidKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountKharidKala.Name = item.Name;
                tempAccountKharidKala.OrganId = personId;
                tempAccountKharidKala.ParentId = accountKharidKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountKharidKala);

                await this.UnitOfWork.SaveChangesAsync();
            }


            var accountBargashtKharidKala = accounts.Where(x => x.ComplteCoding == "5102").SingleOrDefault();

            var tempAccountBargashtKharidKala = accounts.Where(x => x.ComplteCoding == "5102" + item.Code).SingleOrDefault();
            if (tempAccountBargashtKharidKala == null)
            {
                tempAccountBargashtKharidKala = new DomainClasses.Accounting.Account();
                tempAccountBargashtKharidKala.Coding = item.Code;
                tempAccountBargashtKharidKala.ComplteCoding = "5102" + item.Code;
                tempAccountBargashtKharidKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountBargashtKharidKala.Name = item.Name;
                tempAccountBargashtKharidKala.OrganId = personId;
                tempAccountBargashtKharidKala.ParentId = accountBargashtKharidKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountBargashtKharidKala);
                await this.UnitOfWork.SaveChangesAsync();
            }



            var accountForoshKala = accounts.Where(x => x.ComplteCoding == "6101").SingleOrDefault();

            var tempAccountAccountForoshKala = accounts.Where(x => x.ComplteCoding == "6101" + item.Code).SingleOrDefault();

            if (tempAccountAccountForoshKala == null)
            {
                tempAccountAccountForoshKala = new DomainClasses.Accounting.Account();
                tempAccountAccountForoshKala.Coding = item.Code;
                tempAccountAccountForoshKala.ComplteCoding = "6101" + item.Code;
                tempAccountAccountForoshKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountAccountForoshKala.Name = item.Name;
                tempAccountAccountForoshKala.OrganId = personId;
                tempAccountAccountForoshKala.ParentId = accountForoshKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountAccountForoshKala);
                await this.UnitOfWork.SaveChangesAsync();
            }
           

            var accountBargashtForoshKala = accounts.Where(x => x.ComplteCoding == "6102").SingleOrDefault();

            var tempAccountBargashtForoshKala = accounts.Where(x => x.ComplteCoding == "6102" + item.Code).SingleOrDefault();
            if (tempAccountBargashtForoshKala == null)
            {
                tempAccountBargashtForoshKala = new DomainClasses.Accounting.Account();
                tempAccountBargashtForoshKala.Coding = item.Code;
                tempAccountBargashtForoshKala.ComplteCoding = "6102" + item.Code;
                tempAccountBargashtForoshKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountBargashtForoshKala.Name = item.Name;
                tempAccountBargashtForoshKala.OrganId = personId;
                tempAccountBargashtForoshKala.ParentId = accountBargashtForoshKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountBargashtForoshKala);

                await this.UnitOfWork.SaveChangesAsync();
            }
          

        }
        public async Task DeleteGoodAccountsAsync(Item item)
        {
            AccountRule accountRule = new AccountRule();

            await accountRule.DeleteAccountByComplteCodingAsync("1108" + item.Code);
            await accountRule.DeleteAccountByComplteCodingAsync("5101" + item.Code);
            await accountRule.DeleteAccountByComplteCodingAsync("5102" + item.Code);
            await accountRule.DeleteAccountByComplteCodingAsync("6101" + item.Code);
            await accountRule.DeleteAccountByComplteCodingAsync("6102" + item.Code);

            await accountRule.SaveChangesAsync();
        }

        public async Task DeleteServiceAccountAsync(Item item)
        {
            AccountRule accountRule = new AccountRule();

            await accountRule.DeleteAccountByComplteCodingAsync("7101" + item.Code);
            await accountRule.DeleteAccountByComplteCodingAsync("8204" + item.Code);

            await accountRule.SaveChangesAsync();
        }

        public void CreateServiceAccount(Item item, int personId)
        {
            AccountRule accountRule = new AccountRule();

            var accounts = accountRule.GetAllByOrganId(personId);

            var accountDaramdKhadamat = accounts.Where(x => x.ComplteCoding == "7101").SingleOrDefault();

            var tempAccountDaramdKhadamat = accounts.Where(x => x.ComplteCoding == "7101" + item.Code).SingleOrDefault();
            if (tempAccountDaramdKhadamat == null)
            {
                tempAccountDaramdKhadamat = new DomainClasses.Accounting.Account();
                tempAccountDaramdKhadamat.Coding = item.Code;
                tempAccountDaramdKhadamat.ComplteCoding = "7101" + item.Code;
                tempAccountDaramdKhadamat.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountDaramdKhadamat.Name = item.Name;
                tempAccountDaramdKhadamat.OrganId = personId;
                tempAccountDaramdKhadamat.ParentId = accountDaramdKhadamat.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountDaramdKhadamat);
                this.UnitOfWork.SaveChanges();
            }


            var accountSirHazeneh = accounts.Where(x => x.ComplteCoding == "8204").SingleOrDefault();

            var tempAccountSirHazeneh = accounts.Where(x => x.ComplteCoding == "8204" + item.Code).SingleOrDefault();
            if (tempAccountSirHazeneh == null)
            {
                tempAccountSirHazeneh = new DomainClasses.Accounting.Account();
                tempAccountSirHazeneh.Coding = item.Code;
                tempAccountSirHazeneh.ComplteCoding = "8204" + item.Code;
                tempAccountSirHazeneh.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountSirHazeneh.Name = item.Name;
                tempAccountSirHazeneh.OrganId = personId;
                tempAccountSirHazeneh.ParentId = accountSirHazeneh.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountSirHazeneh);
                this.UnitOfWork.SaveChanges();
            }

        }
        public void CreateGoodAccounts(Item item, int personId)
        {
            AccountRule accountRule = new AccountRule();

            var accounts = accountRule.GetAllByOrganId(personId);

            var accountMojodiKala = accounts.Where(x => x.ComplteCoding == "1108").SingleOrDefault();

            var tempAccountMojodiKala = accounts.Where(x => x.ComplteCoding == "1108" + item.Code).SingleOrDefault();

            if (tempAccountMojodiKala == null)
            {
                tempAccountMojodiKala = new DomainClasses.Accounting.Account();
                tempAccountMojodiKala.Coding = item.Code;
                tempAccountMojodiKala.ComplteCoding = "1108" + item.Code;
                tempAccountMojodiKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountMojodiKala.Name = item.Name;
                tempAccountMojodiKala.OrganId = personId;
                tempAccountMojodiKala.ParentId = accountMojodiKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountMojodiKala);
                this.UnitOfWork.SaveChanges();
            }


            var accountKharidKala = accounts.Where(x => x.ComplteCoding == "5101").SingleOrDefault();

            var tempAccountKharidKala = accounts.Where(x => x.ComplteCoding == "5101" + item.Code).SingleOrDefault();

            if (tempAccountKharidKala == null)
            {
                tempAccountKharidKala = new DomainClasses.Accounting.Account();

                tempAccountKharidKala.Coding = item.Code;
                tempAccountKharidKala.ComplteCoding = "5101" + item.Code;
                tempAccountKharidKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountKharidKala.Name = item.Name;
                tempAccountKharidKala.OrganId = personId;
                tempAccountKharidKala.ParentId = accountKharidKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountKharidKala);

                this.UnitOfWork.SaveChanges();
            }


            var accountBargashtKharidKala = accounts.Where(x => x.ComplteCoding == "5102").SingleOrDefault();

            var tempAccountBargashtKharidKala = accounts.Where(x => x.ComplteCoding == "5102" + item.Code).SingleOrDefault();
            if (tempAccountBargashtKharidKala == null)
            {
                tempAccountBargashtKharidKala = new DomainClasses.Accounting.Account();
                tempAccountBargashtKharidKala.Coding = item.Code;
                tempAccountBargashtKharidKala.ComplteCoding = "5102" + item.Code;
                tempAccountBargashtKharidKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountBargashtKharidKala.Name = item.Name;
                tempAccountBargashtKharidKala.OrganId = personId;
                tempAccountBargashtKharidKala.ParentId = accountBargashtKharidKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountBargashtKharidKala);
                this.UnitOfWork.SaveChanges();
            }



            var accountForoshKala = accounts.Where(x => x.ComplteCoding == "6101").SingleOrDefault();

            var tempAccountAccountForoshKala = accounts.Where(x => x.ComplteCoding == "6101" + item.Code).SingleOrDefault();

            if (tempAccountAccountForoshKala == null)
            {
                tempAccountAccountForoshKala = new DomainClasses.Accounting.Account();
                tempAccountAccountForoshKala.Coding = item.Code;
                tempAccountAccountForoshKala.ComplteCoding = "6101" + item.Code;
                tempAccountAccountForoshKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountAccountForoshKala.Name = item.Name;
                tempAccountAccountForoshKala.OrganId = personId;
                tempAccountAccountForoshKala.ParentId = accountForoshKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountAccountForoshKala);
                this.UnitOfWork.SaveChanges();
            }


            var accountBargashtForoshKala = accounts.Where(x => x.ComplteCoding == "6102").SingleOrDefault();

            var tempAccountBargashtForoshKala = accounts.Where(x => x.ComplteCoding == "6102" + item.Code).SingleOrDefault();
            if (tempAccountBargashtForoshKala == null)
            {
                tempAccountBargashtForoshKala = new DomainClasses.Accounting.Account();
                tempAccountBargashtForoshKala.Coding = item.Code;
                tempAccountBargashtForoshKala.ComplteCoding = "6102" + item.Code;
                tempAccountBargashtForoshKala.Level = ZhivarEnums.AccountType.Tafzeli;
                tempAccountBargashtForoshKala.Name = item.Name;
                tempAccountBargashtForoshKala.OrganId = personId;
                tempAccountBargashtForoshKala.ParentId = accountBargashtForoshKala.ID;

                this.UnitOfWork.RepositoryAsync<DomainClasses.Accounting.Account>().Insert(tempAccountBargashtForoshKala);

                this.UnitOfWork.SaveChanges();
            }


        }


        public void DeleteGoodAccounts(Item item)
        {
            AccountRule accountRule = new AccountRule();

            accountRule.DeleteAccountByComplteCoding("1108" + item.Code);
            accountRule.DeleteAccountByComplteCoding("5101" + item.Code);
            accountRule.DeleteAccountByComplteCoding("5102" + item.Code);
            accountRule.DeleteAccountByComplteCoding("6101" + item.Code);
            accountRule.DeleteAccountByComplteCoding("6102" + item.Code);

            accountRule.SaveChanges();
        }

        public void DeleteServiceAccount(Item item)
        {
            AccountRule accountRule = new AccountRule();

            accountRule.DeleteAccountByComplteCoding("7101" + item.Code);
            accountRule.DeleteAccountByComplteCoding("8204" + item.Code);

            accountRule.SaveChanges();
        }

   

    }
}