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
using Zhivar.DomainClasses.Common;
using System.Threading;
using Zhivar.ViewModel.Accunting;
using AutoMapper;
using Zhivar.DomainClasses;
using Zhivar.Utilities;
using Zhivar.ViewModel.Contract;
using Zhivar.Business.Accounting;
using Zhivar.DomainClasses.Contract;
using Zhivar.Business.BaseInfo;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.Business.Contract
{
    public partial class Contract_SazeRule : BusinessRuleBase<Contract_Saze>
    {
        public Contract_SazeRule()
            : base()
        {

        }

        public Contract_SazeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public Contract_SazeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

      
        protected override Contract_Saze FindEntity(params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.Contarct_Saze_Bazareabs == null)
            {
                this.LoadCollection<Contract_Saze_Bazareab>(entity, x => x.Contarct_Saze_Bazareabs);
            }

            if (entity.Contract_Saze_Tarahs == null)
            {
                this.LoadCollection<Contract_Saze_Tarah>(entity, x => x.Contract_Saze_Tarahs);
            }

            if (entity.Contract_Saze_Chapkhanes == null)
            {
                this.LoadCollection<Contract_Saze_Chapkhane>(entity, x => x.Contract_Saze_Chapkhanes);
            }

            if (entity.Contract_Saze_Nasabs == null)
            {
                this.LoadCollection<Contract_Saze_Nasab>(entity, x => x.Contract_Saze_Nasabs);
            }


            if (entity.ContractSazeImages == null)
            {
                this.LoadCollection<ContractSazeImages>(entity, x => x.ContractSazeImages);
            }

            return entity;
        }

        protected async override Task<Contract_Saze> FindEntityAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = base.FindEntity(keyValues);

            if (entity.Contarct_Saze_Bazareabs == null)
            {
                await this.LoadCollectionAsync<Contract_Saze_Bazareab>(entity, x => x.Contarct_Saze_Bazareabs);
            }

            if (entity.Contract_Saze_Tarahs == null)
            {
                await this.LoadCollectionAsync<Contract_Saze_Tarah>(entity, x => x.Contract_Saze_Tarahs);
            }

            if (entity.Contract_Saze_Chapkhanes == null)
            {
                await this.LoadCollectionAsync<Contract_Saze_Chapkhane>(entity, x => x.Contract_Saze_Chapkhanes);
            }

            if (entity.Contract_Saze_Nasabs == null)
            {
                await this.LoadCollectionAsync<Contract_Saze_Nasab>(entity, x => x.Contract_Saze_Nasabs);
            }

            if (entity.ContractSazeImages == null)
            {
                await this.LoadCollectionAsync<ContractSazeImages>(entity, x => x.ContractSazeImages);
            }

            return entity;
        }

        public async Task<IList<Contract_SazeVM>> GetAllSazeByContractIdAsync(int contractId)
        {
            try
            {
                var contract_Sazes = await this.unitOfWork.RepositoryAsync<Contract_Saze>().Queryable().Where(x => x.ContractID == contractId).ToListAsync2();


                List<Contract_SazeVM> contract_SazeVMs = new List<Contract_SazeVM>();

                Mapper.Map(contract_Sazes, contract_SazeVMs);

                SazeRule sazeRule = new SazeRule();
                NoeEjareRule noeEjareRule = new NoeEjareRule();

                foreach (var contract_SazeVM in contract_SazeVMs)
                {
                    contract_SazeVM.Saze = Mapper.Map<SazeVM>(await sazeRule.FindAsync(contract_SazeVM.SazeId));
                    contract_SazeVM.NoeEjare = Mapper.Map<NoeEjareVM>(await noeEjareRule.FindAsync(contract_SazeVM.NoeEjareId));
                }
                return contract_SazeVMs;
            }
            catch (Exception ex)
            {

                throw;
            }


        }

     

    }
}