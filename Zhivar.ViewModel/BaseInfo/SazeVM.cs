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
using Zhivar.ViewModel.Contract;

namespace Zhivar.ViewModel.BaseInfo
{
    public class SazeVM 
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public decimal Tol { get; set; }
        public decimal Arz { get; set; }
        public decimal Masaht { get; set;}
        public Node NodeGoroheSaze { get; set; }
        public Node NodeNoeSaze { get; set; }
        public Node NodeNoeEjare { get; set; }
        public string Code { get; set; }
        public int OrganId { get; set; }
        public string GoroheName { get; set; }
        public string NoeSazeName { get; set; }
        public string NoeEjareName { get; set; }
        public int GoroheSazeID { get; set; }
        public int NoeSazeId { get; set; }
        public int NoeEjareID { get; set; }
        public DetailAccount DetailAccount { get; set; }
        public List<SazeImageVM> Images { get; set; }
        public List<SazeImage> ImagesCommon { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public bool? NoorDard { get; set; }
        public NoeEjare NoeEjare { get; set; }
        public bool Status { get; set; }
        public List<SazeOfContractInTime> SazeOfContractInTimes { get; set; }
    }
}
