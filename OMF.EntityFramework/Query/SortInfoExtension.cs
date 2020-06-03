using OMF.Common;
using OMF.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OMF.EntityFramework.Query
{
    public static class SortInfoExtension
    {
        public static IOrderedQueryable<T> CreateOrderedQuery<T>(
          this List<SortInfo> sortInfoList,
          IQueryable<T> query)
          where T : class
        {
            bool flag = true;
            IOrderedQueryable<T> source = (IOrderedQueryable<T>)null;
            foreach (SortInfo sortInfo in sortInfoList)
            {
                string mapPropertyName = PropertyMapCollection.GetMapPropertyName<T>(sortInfo.Field);
                try
                {
                    if (flag)
                    {
                        source = !(sortInfo.Dir.ToLower() == "asc") ? query.OrderByDescending<T>(mapPropertyName) : query.OrderBy<T>(mapPropertyName);
                        flag = false;
                    }
                    else
                        source = !(sortInfo.Dir.ToLower() == "asc") ? source.ThenByDescending<T>(mapPropertyName) : source.ThenBy<T>(mapPropertyName);
                }
                catch (ArgumentException ex)
                {
                }
            }
            return source;
        }
    }
}
