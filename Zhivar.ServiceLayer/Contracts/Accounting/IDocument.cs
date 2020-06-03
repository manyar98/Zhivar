using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Accounting;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.BaseInfo;

namespace Zhivar.ServiceLayer.Contracts.Accounting
{
    public interface IDocument
    {
        //IList<Document> GetAll();
        Task<IList<Document>> GetAllByOrganIdAsync(int organId);
        Task<IList<Document>> GetByChequeIdAsync(int chequeId);
        //Task<IList<Document>> GetAllAsync();
        //Document GetById(int id);
        Task<Document> GetByIdAsync(int id);
        Task<bool> Insert(Document document, int organId);
        Task<bool> Update(Document document);
        bool Delete(Document document);
        Task<bool> Delete(int id);
        Task<int> createNumberDocumentAsync(int organId);
        Task<decimal> CreateDocumentOpeningBalanceCash(Document document, List<TransactionVM> transactions, string docDate,int organId);

        Task<Document> GetFirstDocument(int id);
        Task<DocumentVM> GetFirstDocumentVM(int id);
        Task<OpeningBalanceStat> OpeningBalanceStatAsync(Document document, int organId);
        Task<decimal> CreateDocumentOpeningBalanceItem(Document document, List<ItemInfo> items, string docDate, int organId);
        Task<decimal> CreateDocumentOpeningBalanceReceivables(Document document, List<TransactionVM> transactions, string docDate, int organId);
        Task<decimal> CreateDocumentOpeningBalanceAssets(Document document, List<TransactionVM> transactions, string docDate, int iD);
        Task<decimal> CreateDocumentOpeningBalanceCreditor(Document document, List<TransactionVM> transactions, string docDate, int iD);
        Task<decimal> CreateDocumentOpeningBalancePayables(Document document, List<TransactionVM> transactions, string docDate, int iD);
        Task<decimal> CreateDocumentOpeningBalanceOtherLiabilities(Document document, List<TransactionVM> transactions, string docDate, int organId);
        Task<bool> SaveDocument(DocumentVM documentVM, int organId);
        Task<List<Document>> GetChequeRelatedDocuments(int chequeId);
        Task<DocumentVM> GetDocumentByIdAsync(int id);
        Task<List<Document>> GetDocumentsByDocumentIDs(List<int> lstDocId);
    }
}
