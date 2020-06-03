using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.BaseInfo;

namespace Zhivar.ViewModel.Accunting
{
    public class BankVM 
    {
        public int? ID { get; set; }
        public DetailAccount DetailAccount { get; set; }
       // public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string AccountNumber { get; set; }
        public string Branch { get; set; }
        public decimal Balance { get; set; }
        public string Code { get; set; }
        public int OrganId { get; set; }
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<BankVM, Bank>();
            configuration.CreateMap<Bank, BankVM>();
        }
    }
}
