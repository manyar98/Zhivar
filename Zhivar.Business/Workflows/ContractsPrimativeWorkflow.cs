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
    public class ContractsPrimativeWorkflow
    {
        protected static IUnitOfWorkAsync UnitOfWork { get; } = new UnitOfWork();

        public static WFExchangeData PostModirDakhiliAction(ActionMethodParams actionMethodParams)
        {
            try
            {
                int contractId = actionMethodParams.InitialExchangeData[WfConstants.RelatedRecordIdKey].ConvertTo<int>();

                WFExchangeData ex = (WFExchangeData)actionMethodParams.WorkflowInstanceState.ExchangeData;

                ContractRule contractRule = new ContractRule(UnitOfWork);
                DomainClasses.Contract.Contract contract = contractRule.Find(contractId);



                if (contract == null)
                    throw new OMFValidationException($" قرارداد با شناسه ثبت شده یافت نشد. شناسه: {contractId}");


                if (actionMethodParams.ContinueInfo.ActionId == (int)WorkFlowActionType.TabdelPishghrardadBeGharardad)
                {
                    using (var uow = new UnitOfWork())
                    {
                        contract.Status = Status.ConfirmationPreContract;
                        contract.ContractType = ContractType.RentTo;

                       // var contract_Sazes = uow.Repository<Contract_Saze>().Queryable().Where(x => x.ContractID == contract.ID).ToList();

                        foreach (var contract_Saze in contract.Contract_Sazes ?? new List<Contract_Saze>())
                        {
                            contract_Saze.Status = Status.ConfirmationPreContract;
                            contract_Saze.ObjectState = OMF.Common.Enums.ObjectState.Modified;
                        }

                        uow.Repository<DomainClasses.Contract.Contract>().Update(contract);


                        uow.SaveChanges();
                    }


                    InvoiceRule invoiceRule = new InvoiceRule(UnitOfWork);
                    var invoice = invoiceRule.ConvertContractToInvoice(contract.ID, NoeFactor.RentTo);

                    var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                    DocumentRule documentRule = new DocumentRule();
                    documentRule.Insert(document, invoice.OrganId);
                    documentRule.SaveChanges();

                }
                return ex;
            }
            catch (Exception ex)
            {

                throw;
            }
        


        }
    }
}
