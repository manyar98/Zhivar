using OMF.Business;
using OMF.Common.Extensions;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zhivar.Business.Security
{
    public class OrganizationUnitChartRule : BusinessRuleBase<OrganizationUnitChart>
    {
        List<int> allIds = new List<int>();
        public async Task<List<int>> GetChildIdsAsync(List<int> ids)
        {
            foreach (var id in ids)
            {
                var childIds = await this.Queryable()
                                         .Where(o => o.ParentId == id)
                                         .Select(o => o.ID)
                                         .ToListAsync2();
                if (childIds.Count > 0)
                {
                    allIds.AddRange(childIds);
                    await GetChildIdsAsync(childIds);
                }
            }

            return allIds;
        }
    }
}
