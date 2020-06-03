using AutoMapper;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accunting;

namespace Zhivar.ViewModel.BaseInfo
{
    public class BaseAccountVM 
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
        public string Coding { get; set; }
        //public int GoroheHesabId { get; set; }
        public ZhivarEnums.AccountType Level { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<BaseAccountVM, BaseAccount>();

            configuration.CreateMap<BaseAccount, BaseAccountVM>();
        }

    }
}
