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
using Zhivar.Business.Accounting;
using Zhivar.Business.Contract;
using Zhivar.DomainClasses.Contract;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Business.Workflows
{
    public class ContractRentWorkflow
    {
        protected static IUnitOfWorkAsync UnitOfWork { get; } = new UnitOfWork();

        public static WFExchangeData PostModirAction(ActionMethodParams actionMethodParams)
        {
            int contractId = actionMethodParams.InitialExchangeData[WfConstants.RelatedRecordIdKey].ConvertTo<int>();

            WFExchangeData ex = (WFExchangeData)actionMethodParams.WorkflowInstanceState.ExchangeData;

            ContractRule contractRule = new ContractRule(UnitOfWork);
            DomainClasses.Contract.Contract contract = contractRule.Find(contractId);



            if (contract == null)
                throw new OMFValidationException($" قرارداد با شناسه ثبت شده یافت نشد. شناسه: {contractId}");


            if (actionMethodParams.ContinueInfo.ActionId == (int)WorkFlowActionType.Taeed)
            {

                contract.Status = Status.ConfirmationContract;
                contractRule.Update(contract);
                contractRule.SaveChanges();

                InvoiceRule invoiceRule = new InvoiceRule(UnitOfWork);
                invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentFrom);

            }
            return ex;


        }
    }
}
