using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.ViewModel.BaseInfo
{
    public class MadarekPayvastVM
    {
        public int ID { get; set; }
        public int RecordID { get; set; }
        public int DocTypeID { get; set; }
        public int FileSize { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public bool IsDeleted { get; set; }
        public string TasvirBlobBase64 { get; set; }
        // public DrTasvirBlob DrTasvirBlob { get; set; }
        public byte[] Blob { get; set; }
        public string NoeTasvirStr { get; set; }

    }
}
