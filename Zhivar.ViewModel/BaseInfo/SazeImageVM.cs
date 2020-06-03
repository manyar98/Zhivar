using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accunting;

namespace Zhivar.ViewModel.BaseInfo
{
    public class SazeImageVM
    {
        public int ID { get; set; }
        public int? Order { get; set; }
        public int SazeId { get; set; }
        public string MimeType { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public byte[] Blob { get; set; }
        public bool IsDeleted { get; set; }
        public string TasvirBlobBase64 { get; set; }
    }


}
