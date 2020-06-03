using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.UnitOfWork;
using OMF.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.Business.Contract;
using Zhivar.DomainClasses.Contract;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Business.Workflows
{
    public class ContractWorkflow
    {
        protected static IUnitOfWorkAsync UnitOfWork { get; } = new UnitOfWork();

        public static WFExchangeData PostModirAction(ActionMethodParams actionMethodParams)
        {
            int contractSazeId = actionMethodParams.InitialExchangeData[WfConstants.RelatedRecordIdKey].ConvertTo<int>();

            WFExchangeData ex = (WFExchangeData)actionMethodParams.WorkflowInstanceState.ExchangeData;

            Contract_SazeRule contract_SazeRule = new Contract_SazeRule(UnitOfWork);
            Contract_Saze contract_Saze = contract_SazeRule.Find(contractSazeId);

      

            if (contract_Saze == null)
                throw new OMFValidationException($"رسانه قرارداد با شناسه ثبت شده یافت نشد. شناسه: {contractSazeId}");

            ContractRule contractRule = new ContractRule(UnitOfWork);
            DomainClasses.Contract.Contract contract = contractRule.Find(contract_Saze.ContractID);


            if (actionMethodParams.ContinueInfo.ActionId == (int)WorkFlowActionType.TaeedVaKhateme)
            {

                contract_Saze.Status = Status.ConfirmationContract;
                contract_SazeRule.Update(contract_Saze);
                contract_SazeRule.SaveChanges();

                if (contract.Contract_Sazes.All(x => x.Status == Status.ConfirmationContract))
                {
                    contract.Status = Status.ConfirmationContract;
                    contractRule.Update(contract);
                    contractRule.SaveChanges();
                }
               
            }
            return ex;


        }
    }
}
