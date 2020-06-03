using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Zhivar.AutoMapperContracts;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.Accunting;
using Zhivar.ViewModel.Accounting;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using Zhivar.ViewModel.Common;
using Zhivar.ViewModel.BaseInfo;
using OMF.Security.Model;
using Zhivar.ViewModel.Security;
using Zhivar.DomainClasses.Contract;
using Zhivar.ViewModel.Contract;

namespace Zhivar.Web
{
    public class AutoMapperConfig
    {

        public static void Config()
        {
            ConfigureUserMapping();
        }

        private static void ConfigureUserMapping()
        {
            Mapper.CreateMap<Bank, BankVM>();
            Mapper.CreateMap<BankVM, Bank>();

            Mapper.CreateMap<Cash, CashVM>();
            Mapper.CreateMap<CashVM, Cash>();

            Mapper.CreateMap<Account, AccountVM>();
            Mapper.CreateMap<AccountVM, Account>();

            Mapper.CreateMap<Cheque, ChequeVM>();
            Mapper.CreateMap<ChequeVM, Cheque>();

            Mapper.CreateMap<Contact, ContactVM>();
            Mapper.CreateMap<ContactVM, Contact>();

            Mapper.CreateMap<Cost, CostVM>();
            Mapper.CreateMap<CostVM, Cost>();


            Mapper.CreateMap<CostItem, CostItemVM>();
            Mapper.CreateMap<CostItemVM, CostItem>();

            Mapper.CreateMap<Cost, CostVM>();
            Mapper.CreateMap<CostVM, Cost>();

            Mapper.CreateMap<PayRecevie, PayRecevieVM>();
            Mapper.CreateMap<PayRecevieVM, PayRecevie>();

            Mapper.CreateMap<DetailPayRecevie, DetailPayRecevieVM>();
            Mapper.CreateMap<DetailPayRecevieVM, DetailPayRecevie>();

            Mapper.CreateMap<Document, DocumentVM>();
            Mapper.CreateMap<DocumentVM, Document>();

            Mapper.CreateMap<Transaction, TransactionVM>();
            Mapper.CreateMap<TransactionVM, Transaction>();

            Mapper.CreateMap<FinanYear, FinanYearVM>();
            Mapper.CreateMap<FinanYearVM, FinanYear>();


            Mapper.CreateMap<Invoice, InvoiceVM>();
            Mapper.CreateMap<InvoiceVM, Invoice>();


            Mapper.CreateMap<InvoiceItem, InvoiceItemVM>();
            Mapper.CreateMap<InvoiceItemVM, InvoiceItem>();

            Mapper.CreateMap<InvoiceItemVM, InvoiceItem>().ForMember(dest => dest.SumInvoiceItem, opt => opt.MapFrom(src => src.Sum));

            Mapper.CreateMap<InvoiceItem, InvoiceItemVM>().ForMember(dest => dest.Sum, opt => opt.MapFrom(src => src.SumInvoiceItem));

            Mapper.CreateMap<Item, ItemVM>();
            Mapper.CreateMap<ItemVM, Item>();

            Mapper.CreateMap<ItemGroup, ItemGroupVM>();
            Mapper.CreateMap<ItemGroupVM, ItemGroup>();


            Mapper.CreateMap<Shareholder, ShareholderVM>();
            Mapper.CreateMap<ShareholderVM, Shareholder>();


            Mapper.CreateMap<Role, RoleVM>();
            Mapper.CreateMap<RoleVM, Role>();

            Mapper.CreateMap<UserRole, UserRoleVM>();
            Mapper.CreateMap<UserRoleVM, UserRole>();

            Mapper.CreateMap<UserOperation, UserOperationVM>();
            Mapper.CreateMap<UserOperationVM, UserOperation>();

            Mapper.CreateMap<RoleOperation, RoleOperationVM>();
            Mapper.CreateMap<RoleOperationVM, RoleOperation>();

            Mapper.CreateMap<UserInfo, UserInfoVM>();
            Mapper.CreateMap<UserInfoVM, UserInfo>();

            Mapper.CreateMap<Operation, Controllers.Security.OperationVM>();
            Mapper.CreateMap<Controllers.Security.OperationVM, Operation>();

            Mapper.CreateMap<GoroheSaze, GoroheSazeVM>();
            Mapper.CreateMap<GoroheSazeVM, GoroheSaze>();


            Mapper.CreateMap<GoroheSaze, GoroheSazeVM>();
            Mapper.CreateMap<GoroheSazeVM, GoroheSaze>();

            Mapper.CreateMap<NoeEjare, NoeEjareVM>();
            Mapper.CreateMap<NoeEjareVM, NoeEjare>();

            Mapper.CreateMap<Saze, SazeVM>();
            Mapper.CreateMap<SazeVM, Saze>();

            Mapper.CreateMap<SazeImage, SazeImageVM>();
            Mapper.CreateMap<SazeImageVM, SazeImage>();

            Mapper.CreateMap<NoeSaze, NoeSazeVM>();
            Mapper.CreateMap<NoeSazeVM, NoeSaze>();

            Mapper.CreateMap<Contract, ContractVM>();
            Mapper.CreateMap<ContractVM, Contract>();

            Mapper.CreateMap<Contract_Saze, Contract_SazeVM>();
            Mapper.CreateMap<Contract_SazeVM, Contract_Saze>();

            Mapper.CreateMap<Contract_Saze_Bazareab, Contract_Saze_BazareabVM>();
            Mapper.CreateMap<Contract_Saze_BazareabVM, Contract_Saze_Bazareab>();

            Mapper.CreateMap<Contract_Saze_Tarah, Contract_Saze_TarahVM>();
            Mapper.CreateMap<Contract_Saze_TarahVM, Contract_Saze_Tarah>();

            Mapper.CreateMap<Contract_Saze_Chapkhane, Contract_Saze_ChapkhaneVM>();
            Mapper.CreateMap<Contract_Saze_ChapkhaneVM, Contract_Saze_Chapkhane>();

            Mapper.CreateMap<Contract_Saze_Nasab, Contract_Saze_NasabVM>();
            Mapper.CreateMap<Contract_Saze_NasabVM, Contract_Saze_Nasab>();

            Mapper.CreateMap<Contract_PayRecevies, Contract_PayRecevieVM>();
            Mapper.CreateMap<Contract_PayRecevieVM, Contract_PayRecevies>();

            Mapper.CreateMap<Contract_DetailPayRecevies, Contract_DetailPayReceviesVM>();
            Mapper.CreateMap<Contract_DetailPayReceviesVM, Contract_DetailPayRecevies>();

            Mapper.CreateMap<ContractSazeImages, ContractSazeImagesVM>();
            Mapper.CreateMap<ContractSazeImagesVM, ContractSazeImages>();

            Mapper.CreateMap<VahedTol, VahedTolVM>();
            Mapper.CreateMap<VahedTolVM, VahedTol>();


            Mapper.CreateMap<UserInfo, UsersForRule>();
            Mapper.CreateMap<UsersForRule, UserInfo>();

            Mapper.CreateMap<NoeChap, NoeChapVM>();
            Mapper.CreateMap<NoeChapVM, NoeChap>();

            Mapper.CreateMap<MadarekPayvast, MadarekPayvastVM>();
            Mapper.CreateMap<MadarekPayvastVM, MadarekPayvast>();

            Mapper.CreateMap<DocType, DocTypeVM>();
            Mapper.CreateMap<DocTypeVM, DocType>();

            Mapper.CreateMap<Personel, PersonelVM>();
            Mapper.CreateMap<PersonelVM, Personel>();

            Mapper.CreateMap<Reservation, ReservationVM>();
            Mapper.CreateMap<ReservationVM, Reservation>();

            Mapper.CreateMap<Reservation_Detail, Reservation_DetailVM>();
            Mapper.CreateMap<Reservation_DetailVM, Reservation_Detail>();

            Mapper.CreateMap<ContractStops, ContractStopsVM>();
            Mapper.CreateMap<ContractStopsVM, ContractStops>();

            Mapper.CreateMap<ContractStopDetails, ContractStopDetailsVM>();
            Mapper.CreateMap<ContractStopDetailsVM, ContractStopDetails>();

            Mapper.CreateMap<BaseColor, BaseColorVM>();
            Mapper.CreateMap<BaseColorVM, BaseColor>();

            Mapper.CreateMap<Organ_Color, Organ_ColorVM>();
            Mapper.CreateMap<Organ_ColorVM, Organ_Color>();

        }
        //     public static void Config()
        //     {
        //         var types =
        //           Assembly
        //.GetExecutingAssembly()
        //.GetReferencedAssemblies()
        //.Select(Assembly.Load)
        //.SelectMany(x => x.GetExportedTypes()).ToList();


        //         LoadStandardMappings(types);

        //         LoadCustomMappings(types);

        //     }

        //     private static void LoadStandardMappings(IEnumerable<Type> types)
        //     {
        //         var maps = (from t in types
        //                     from i in t.GetInterfaces()
        //                     where i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>) && !t.IsAbstract && !t.IsInterface
        //                     select new
        //                     {
        //                         Source = i.GetGenericArguments()[0],
        //                         Destination = t
        //                     }).ToArray();

        //         foreach (var map in maps)
        //         {
        //             Mapper.CreateMap(map.Source, map.Destination);
        //             //IoC.Container.GetInstance<IMappingEngine>().CreateMapExpression(map.Source, map.Destination);
        //         }
        //     }

        //     private static void LoadCustomMappings(IEnumerable<Type> types)
        //     {
        //         var maps = (from t in types
        //                     from i in t.GetInterfaces()
        //                     where typeof(IHaveCustomMappings).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface
        //                     select (IHaveCustomMappings)Activator.CreateInstance(t)).ToArray();

        //         foreach (var map in maps)
        //         {
        //             //Mapper.CreateMap();
        //             //map.CreateMappings(IoC.Container.GetInstance<IMappingEngine>().ConfigurationProvider);

        //         }
        //     }
    }


}