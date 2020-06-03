using AutoMapper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;

namespace Zhivar.ViewModel.Accounting
{
    public class TarfeHesabVM: IHaveCustomMappings
    {
        public int? ID { get; set; }
        [Display(Name = "حد اعتبار")]
        public decimal HadeEtebar { get; set; }
        [Display(Name = "درصد تخفیف")]
        public decimal DarsadTakhfif { get; set; }
        public int? PersonID { get; set; }
        public int? OrganID { get; set; }
        public BankVM BankVM { get; set; }
        public PersonVM PersonVM { get; set; }
        public int BankID { get; set; }
       
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<TarfeHesabVM, TarfeHesab>();

            configuration.CreateMap<TarfeHesabVM, TarfeHesab>()
                    .ForMember(x => x.Person, opt => opt.MapFrom(model => model.PersonVM));

            configuration.CreateMap<TarfeHesabVM, TarfeHesab>()
        .ForMember(x => x.Bank, opt => opt.MapFrom(model => model.BankVM));

            // configuration.CreateMap<TarfeHesabVM, TarfeHesab>()
            //       .ForMember(productModel => productModel.Person.Nam,
            //         opt => opt.MapFrom(product => product.Nam))
            // .ForMember(productModel => productModel.Person.Address,
            //         opt => opt.MapFrom(product => product.Address))
            // .ForMember(productModel => productModel.Person.CodeEghtesadi,
            //         opt => opt.MapFrom(product => product.CodeEghtesadi))
            // .ForMember(productModel => productModel.Person.CodeMeli,
            //         opt => opt.MapFrom(product => product.CodeMeli))
            // .ForMember(productModel => productModel.Person.CodePosti,
            //         opt => opt.MapFrom(product => product.CodePosti))
            //.ForMember(productModel => productModel.Person.Jenseat,
            // opt => opt.MapFrom(product => product.Jenseat))
            //             .ForMember(productModel => productModel.Person.ModateEtebar,
            // opt => opt.MapFrom(product => product.ModateEtebar))
            //             .ForMember(productModel => productModel.Person.NamKhanvadegi,
            // opt => opt.MapFrom(product => product.NamKhanvadegi))
            //             .ForMember(productModel => productModel.Person.NamPedar,
            // opt => opt.MapFrom(product => product.NamPedar))
            //             .ForMember(productModel => productModel.Person.NoeShakhs,
            // opt => opt.MapFrom(product => product.NoeShakhs))
            //             .ForMember(productModel => productModel.Person.SahebEmtiaz,
            // opt => opt.MapFrom(product => product.SahebEmtiaz))
            //                    .ForMember(productModel => productModel.Person.TarikhSoudor,
            // opt => opt.MapFrom(product => product.TarikhSoudor))
            //                               .ForMember(productModel => productModel.Person.Tel,
            // opt => opt.MapFrom(product => product.Tel))
            //                                           .ForMember(productModel => productModel.Person.TypeHoghoghi,
            // opt => opt.MapFrom(product => product.TypeHoghoghi))
            //                                            .ForMember(productModel => productModel.Bank.Nam,
            // opt => opt.MapFrom(product => product.NamBank))
            //                                            .ForMember(productModel => productModel.Bank.CodeShobe,
            // opt => opt.MapFrom(product => product.CodeShobe))
            //                                            .ForMember(productModel => productModel.Bank.ShomareHesab,
            // opt => opt.MapFrom(product => product.ShomareHesab))
            //                                .ForMember(productModel => productModel.Bank.ShomareShobe,
            // opt => opt.MapFrom(product => product.ShomareShobe));

            configuration.CreateMap<TarfeHesab, TarfeHesabVM>();
            configuration.CreateMap<TarfeHesab, TarfeHesabVM>()
        .ForMember(x => x.PersonVM, opt => opt.MapFrom(model => model.Person));

            configuration.CreateMap<TarfeHesab, TarfeHesabVM>()
        .ForMember(x => x.BankVM, opt => opt.MapFrom(model => model.Bank));

            //.ForMember(productModel => productModel.Nam,
            //             opt => opt.MapFrom(product => product.Person.Nam))
            //     .ForMember(productModel => productModel.Address,
            //             opt => opt.MapFrom(product => product.Person.Address))
            //     .ForMember(productModel => productModel.CodeEghtesadi,
            //             opt => opt.MapFrom(product => product.Person.CodeEghtesadi))
            //     .ForMember(productModel => productModel.CodeMeli,
            //             opt => opt.MapFrom(product => product.Person.CodeMeli))
            //     .ForMember(productModel => productModel.CodePosti,
            //             opt => opt.MapFrom(product => product.Person.CodePosti))
            //    .ForMember(productModel => productModel.Jenseat,
            //     opt => opt.MapFrom(product => product.Person.Jenseat))
            //                 .ForMember(productModel => productModel.ModateEtebar,
            //     opt => opt.MapFrom(product => product.Person.ModateEtebar))
            //                 .ForMember(productModel => productModel.NamKhanvadegi,
            //     opt => opt.MapFrom(product => product.Person.NamKhanvadegi))
            //                 .ForMember(productModel => productModel.NamPedar,
            //     opt => opt.MapFrom(product => product.Person.NamPedar))
            //                 .ForMember(productModel => productModel.NoeShakhs,
            //     opt => opt.MapFrom(product => product.Person.NoeShakhs))
            //                 .ForMember(productModel => productModel.SahebEmtiaz,
            //     opt => opt.MapFrom(product => product.Person.SahebEmtiaz))
            //                        .ForMember(productModel => productModel.TarikhSoudor,
            //     opt => opt.MapFrom(product => product.Person.TarikhSoudor))
            //                                   .ForMember(productModel => productModel.Tel,
            //     opt => opt.MapFrom(product => product.Person.Tel))
            //                                               .ForMember(productModel => productModel.TypeHoghoghi,
            //     opt => opt.MapFrom(product => product.Person.TypeHoghoghi));
            //                                           .ForMember(productModel => productModel.NamBank,
            //opt => opt.MapFrom(product => product.Bank.Nam))
            //                                           .ForMember(productModel => productModel.CodeShobe,
            //opt => opt.MapFrom(product => product.Bank.CodeShobe))
            //                                           .ForMember(productModel => productModel.ShomareHesab,
            //opt => opt.MapFrom(product => product.Bank.ShomareHesab))
            //                               .ForMember(productModel => productModel.ShomareShobe,
            //opt => opt.MapFrom(product => product.Bank.ShomareShobe));


        }
    }
}
