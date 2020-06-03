using OMF.Common.Extensions;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.UnitOfWork;
using OMF.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMF.Common.ExceptionManagement.Exceptions;
using static Zhivar.DomainClasses.ZhivarEnums;
using Zhivar.Business.Accounting;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.Business.Workflows
{
    public class InvoiceSellWorkflow
    {
        protected static IUnitOfWorkAsync UnitOfWork { get; } = new UnitOfWork();

        public static WFExchangeData PostModirAction(ActionMethodParams actionMethodParams)
        {
            try
            {
                int invoiceId = actionMethodParams.InitialExchangeData[WfConstants.RelatedRecordIdKey].ConvertTo<int>();

                WFExchangeData ex = (WFExchangeData)actionMethodParams.WorkflowInstanceState.ExchangeData;

                InvoiceRule invoiceRule = new InvoiceRule(UnitOfWork);
                Invoice invoice = invoiceRule.Find(invoiceId);

                if (invoice == null)
                    throw new OMFValidationException($"فاکتوری با شناسه ثبت شده یافت نشد. شناسه: {invoiceId}");

                if (actionMethodParams.ContinueInfo.ActionId == (int)WorkFlowActionType.TaeedVaKhateme)
                {
                    var document = invoiceRule.RegisterDocument(invoice, invoice.OrganId);

                    DocumentRule documentRule = new DocumentRule();
                    documentRule.Insert(document, invoice.OrganId);
                    documentRule.SaveChanges();

                    invoice.Status = NoeInsertFactor.WaitingToReceive;
                    invoiceRule.Update(invoice);

                    invoiceRule.SaveChanges();
                }
                return ex;
            }
            catch (Exception ex1)
            {

                throw;
            }
           


        }
  
    }
}
