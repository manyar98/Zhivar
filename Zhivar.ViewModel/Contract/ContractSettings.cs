using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.Contract
{
    public class ContractSettings
    {
        public bool allowApproveWithoutStock { get; set; }
        public bool autoAddTax { get; set; }
        public string bottomMargin { get; set; }
        public string businessLogo { get; set; }
        public string font { get; set; }
        public string fontSize { get; set; }
        public string footerNote { get; set; }
        public string footerNoteDraft { get; set; }
        public bool hideZeroItems { get; set; }
        public bool onlineInvoiceEnabled { get; set; }
        public string pageSize { get; set; }
        public string payReceiptTitle { get; set; }
        public string purchaseInvoiceTitle { get; set; }
        public string receiveReceiptTitle { get; set; }
        public string rowPerPage { get; set; }
        public string saleDraftInvoiceTitle { get; set; }
        public string saleInvoiceTitle { get; set; }
        public bool showAmountInWords { get; set; }
        public bool showCustomerBalance { get; set; }
        public bool showItemUnit { get; set; }
        public bool showSignaturePlace { get; set; }
        public bool showTransactions { get; set; }
        public bool showVendorInfo { get; set; }
        public string topMargin { get; set; }
        public bool updateBuyPrice { get; set; }
        public bool updateSellPrice { get; set; }
    }
}
