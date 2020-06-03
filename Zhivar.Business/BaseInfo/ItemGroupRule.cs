using System;
using System.Collections.Generic;
using OMF.Business;
using OMF.Common;
using OMF.Common.Configuration;
using OMF.Common.ExceptionManagement.Exceptions;
using OMF.Common.Extensions;
using OMF.Common.Security;
using OMF.EntityFramework.Ef6;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.UnitOfWork;
using OMF.Security.Model;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCValidation = OMF.Common.Validation;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.ViewModel.BaseInfo;
using AutoMapper;

namespace Zhivar.Business.BaseInfo
{
    public partial class ItemGroupRule : BusinessRuleBase<ItemGroup>
    {
        public ItemGroupRule()
            : base()
        {

        }

        public ItemGroupRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public ItemGroupRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

        public IList<ItemGroup> GetAllByOrganId(int organId)
        {
            var itemGroupQuery = this.Queryable().Where(x => x.OrganID == organId);
            var itemQuery = this.unitOfWork.Repository<Item>().Queryable();

            var joinQuery = from itemGroup in itemGroupQuery
                            join item in itemQuery
                            on itemGroup.ID equals item.ItemGroupId into itemGroups
                            select new ItemGroup()
                            {
                                ID = itemGroup.ID,
                                Items = itemGroups.ToList(),
                                Name = itemGroup.Name,
                                OrganID = itemGroup.OrganID
                            };

           return joinQuery.ToList();
        }
        public async Task<List<ItemGroupVM>> GetAllByOrganIdAsync(int organId)
        {
            var itemGroupQuery = this.unitOfWork.Repository<ItemGroup>().Queryable().Where(x => x.OrganID == organId);
            var itemQuery = this.unitOfWork.Repository<Item>().Queryable().Where(x => x.IsSaze == false || x.IsSaze == null);
            //var itemlist =await  itemQuery.ToListAsync2();
            var joinQuery = from itemGroup in itemGroupQuery
                            join item in itemQuery
                            on itemGroup.ID equals item.ItemGroupId into itemGroups
                            select new ItemGroupVM
                            {
                                ID = itemGroup.ID,
          
                                Items = itemGroups.Select(s => new ItemVM
                                {
                                    Barcode = s.Barcode,
                                    BuyPrice = s.BuyPrice,
                                    Code = s.Code,
                                    ID = s.ID,
                                    IsGoods = s.IsGoods,
                                    IsService = s.IsService,
                                    ItemGroupId = s.ItemGroupId,
                                    ItemType = s.ItemType,
                                    MoneyStock = s.MoneyStock,
                                    Name = s.Name,
                                    OrganId = s.OrganIdItem,
                                    PurchasesTitle = s.PurchasesTitle,
                                    SalesTitle = s.SalesTitle,
                                    SellPrice = s.SellPrice,
                                    Stock = s.Stock,
                                    UnitID = s.UnitID

                                }).ToList(),
            Name = itemGroup.Name,
                                OrganID = itemGroup.OrganID
                            };

            return await joinQuery.ToListAsync2();
       
        }
        public async Task<List<ItemGroupVM>> GetAllByOrganIdForCreateCodeAsync(int organId)
        {
            var itemGroupQuery = this.unitOfWork.Repository<ItemGroup>().Queryable().Where(x => x.OrganID == organId);
            var itemQuery = this.unitOfWork.Repository<Item>().Queryable();
            //var itemlist =await  itemQuery.ToListAsync2();
            var joinQuery = from itemGroup in itemGroupQuery
                            join item in itemQuery
                            on itemGroup.ID equals item.ItemGroupId into itemGroups
                            select new ItemGroupVM
                            {
                                ID = itemGroup.ID,

                                Items = itemGroups.Select(s => new ItemVM
                                {
                                    Barcode = s.Barcode,
                                    BuyPrice = s.BuyPrice,
                                    Code = s.Code,
                                    ID = s.ID,
                                    IsGoods = s.IsGoods,
                                    IsService = s.IsService,
                                    ItemGroupId = s.ItemGroupId,
                                    ItemType = s.ItemType,
                                    MoneyStock = s.MoneyStock,
                                    Name = s.Name,
                                    OrganId = s.OrganIdItem,
                                    PurchasesTitle = s.PurchasesTitle,
                                    SalesTitle = s.SalesTitle,
                                    SellPrice = s.SellPrice,
                                    Stock = s.Stock,
                                    UnitID = s.UnitID

                                }).ToList(),
                                Name = itemGroup.Name,
                                OrganID = itemGroup.OrganID
                            };

            return await joinQuery.ToListAsync2();

        }
        public async Task<List<Item>> GetAllByOrganIdAsync2(int organId)
        {
            //var itemGroupQuery = this.unitOfWork.Repository<ItemGroup>().Queryable().Where(x => x.OrganID == organId);
            var itemQuery = this.unitOfWork.Repository<Item>().Queryable();

            //var joinQuery = from itemGroup in itemGroupQuery
            //                join item in itemQuery
            //                on itemGroup.ID equals item.ItemGroupId into itemGroups
            //                select new ItemGroup()
            //                {
            //                    ID = itemGroup.ID,
            //                    Items = itemGroups.ToList(),
            //                    Name = itemGroup.Name,
            //                    OrganID = itemGroup.OrganID
            //                };

            //return await joinQuery.ToListAsync2();
            return await itemQuery.ToListAsync2();//  new List<ItemGroup>();
        }
    }
}