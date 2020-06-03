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
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses;
using static Zhivar.DomainClasses.ZhivarEnums;

namespace Zhivar.Business.Accounting
{
    public partial class MapItemSazeRule : BusinessRuleBase<MapItemSaze>
    {
        public MapItemSazeRule()
            : base()
        {

        }

        public MapItemSazeRule(IUnitOfWorkAsync uow)
            : base(uow)
        {

        }

        public MapItemSazeRule(bool useForAnonymousUser)
            : base()
        {
            UseForAnonymousUser = useForAnonymousUser;
        }

      

        public async Task<int> GetSazeIdByItemId(int itemId)
        {
            var result = await this.Queryable().Where(x => x.ItemID == itemId && x.Type == MapItemSazeType.SubGroup).SingleOrDefaultAsync2();

            if (result == null)
                return 0;

            return result.SazeID;
        }
        public async Task<int> GetItemIdBySazeIdAsync(int sazeId)
        {
            var result = await this.Queryable().Where(x => x.SazeID == sazeId && x.Type == MapItemSazeType.SubGroup).SingleOrDefaultAsync2();

            if (result == null)
                return 0;

            return result.ItemID;
        }

        public int GetItemIdBySazeId(int sazeId)
        {
            var result = this.Queryable().Where(x => x.SazeID == sazeId && x.Type == MapItemSazeType.SubGroup).SingleOrDefault();

            if (result == null)
                return 0;

            return result.ItemID;
        }
        public async Task<int> GetSazeGroupIdByItemGroupId(int itemGroupId)
        {
            var result = await this.Queryable().Where(x => x.ItemID == itemGroupId && x.Type == MapItemSazeType.Group).SingleOrDefaultAsync2();

            if (result == null)
                return 0;

            return result.SazeID;
        }
        public async Task<int> GetItemGroupIdBySazeGroupIdAsync(int sazeGroupId)
        {
            var result = await this.Queryable().Where(x => x.SazeID == sazeGroupId && x.Type == MapItemSazeType.Group).SingleOrDefaultAsync2();

            if (result == null)
                return 0;

            return result.ItemID;
        }
        public int GetItemGroupIdBySazeGroupId(int sazeGroupId)
        {
            var result = this.Queryable().Where(x => x.SazeID == sazeGroupId && x.Type == MapItemSazeType.Group).SingleOrDefault();

            if (result == null)
                return 0;

            return result.ItemID;
        }
    }
}