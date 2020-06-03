using OMF.Common;
using OMF.Common.Extensions;
using OMF.EntityFramework.Ef6;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace OMF.EntityFramework.Query
{
    public class FilterInfo
    {
        public FilterInfo()
        {
            this.Filters = new List<FilterData>();
        }

        public string Logic { get; set; }

        public List<FilterData> Filters { get; set; }

        public Expression<Func<TEntity, bool>> TranslateFilter<TEntity>(
          bool processPersianChars = true)
          where TEntity : class
        {
            QueryObject<TEntity> queryObject = new QueryObject<TEntity>();
            ParameterExpression parameterExpression = Expression.Parameter(typeof(TEntity), "entity");
            foreach (FilterData filter in this.Filters)
            {
                if (filter.Filters != null && filter.Filters.Any<FilterData>())
                {
                    Expression<Func<TEntity, bool>> query = new FilterInfo()
                    {
                        Logic = filter.Logic,
                        Filters = filter.Filters
                    }.TranslateFilter<TEntity>(true);
                    if (string.IsNullOrWhiteSpace(this.Logic))
                        queryObject.And(query);
                    else if (this.Logic.ToLower() == "and")
                        queryObject.And(query);
                    else
                        queryObject.Or(query);
                }
                else if (processPersianChars && filter.Value != null && (filter.Value.Contains("ي") || filter.Value.Contains("ك") || filter.Value.Contains("ی") || filter.Value.Contains("ک")))
                {
                    FilterInfo filterInfo = new FilterInfo()
                    {
                        Logic = "or"
                    };
                    filterInfo.Filters.Add(filter);
                    string str = "";
                    if (filter.Value.Contains("ي") || filter.Value.Contains("ك"))
                        str = filter.Value.Replace("ي", "ی").Replace("ك", "ک");
                    else if (filter.Value.Contains("ی") || filter.Value.Contains("ک"))
                        str = filter.Value.Replace("ی", "ي").Replace("ک", "ك");
                    filterInfo.Filters.Add(new FilterData()
                    {
                        Field = filter.Field,
                        Logic = filter.Logic,
                        Operator = filter.Operator,
                        Value = str
                    });
                    Expression<Func<TEntity, bool>> query = filterInfo.TranslateFilter<TEntity>(false);
                    if (string.IsNullOrWhiteSpace(this.Logic))
                        queryObject.And(query);
                    else if (this.Logic.ToLower() == "and")
                        queryObject.And(query);
                    else
                        queryObject.Or(query);
                }
                else
                {
                    string mapPropertyName = PropertyMapCollection.GetMapPropertyName<TEntity>(filter.Field);
                    Expression bodyExpression = FilterInfo.CreateBodyExpression(parameterExpression, mapPropertyName, filter.Operator, filter.Value);
                    if (bodyExpression != null)
                    {
                        Expression<Func<TEntity, bool>> query = Expression.Lambda<Func<TEntity, bool>>(bodyExpression, parameterExpression);
                        if (string.IsNullOrWhiteSpace(this.Logic))
                            queryObject.And(query);
                        else if (this.Logic.ToLower() == "and")
                            queryObject.And(query);
                        else
                            queryObject.Or(query);
                    }
                }
            }
            return queryObject.Query();
        }

        private static Expression CreateBodyExpression(
          ParameterExpression param,
          string propertyName,
          string operation,
          string valueStr)
        {
            Expression expression1 = (Expression)null;
            string[] strArray = propertyName.Split('.');
            Expression expression2;
            try
            {
                if (strArray.Length > 1)
                {
                    expression2 = (Expression)Expression.Property((Expression)param, strArray[0]);
                    for (int index = 1; index < strArray.Length; ++index)
                        expression2 = (Expression)Expression.Property(expression2, strArray[index]);
                }
                else
                    expression2 = (Expression)Expression.Property((Expression)param, propertyName);
            }
            catch (ArgumentException ex)
            {
                return (Expression)null;
            }
            object obj = (object)null;
            if (!string.IsNullOrWhiteSpace(valueStr))
                obj = valueStr.ConvertTo(expression2.Type);
            if (obj is string)
            {
                Expression expression3 = (Expression)Expression.Call((Expression)null, typeof(DbFunctions).GetMethod("AsUnicode", BindingFlags.Static | BindingFlags.Public), (Expression)Expression.Constant(obj, expression2.Type));
                MethodInfo method = typeof(string).GetMethod("CompareTo", new Type[1]
                {
          typeof (string)
                });
                switch (operation.ToLower())
                {
                    case "contains":
                        expression1 = (Expression)Expression.Call(expression2, "Contains", (Type[])null, expression3);
                        break;
                    case "doesnotcontain":
                        expression1 = (Expression)Expression.Not((Expression)Expression.Call(expression2, "Contains", (Type[])null, expression3));
                        break;
                    case "endswith":
                        expression1 = (Expression)Expression.Call(expression2, "EndsWith", (Type[])null, expression3);
                        break;
                    case "eq":
                        expression1 = (Expression)Expression.Equal((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "gt":
                        expression1 = (Expression)Expression.GreaterThan((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "gte":
                        expression1 = (Expression)Expression.GreaterThanOrEqual((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "less":
                        expression1 = (Expression)Expression.LessThan((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "lt":
                        expression1 = (Expression)Expression.LessThan((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "lte":
                        expression1 = (Expression)Expression.LessThanOrEqual((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "neq":
                        expression1 = (Expression)Expression.NotEqual((Expression)Expression.Call(expression2, method, expression3), (Expression)Expression.Constant((object)0, typeof(int)));
                        break;
                    case "startswith":
                        expression1 = (Expression)Expression.Call(expression2, "StartsWith", (Type[])null, expression3);
                        break;
                }
            }
            else
            {
                Expression right = (Expression)Expression.Constant(obj, expression2.Type);
                switch (operation.ToLower())
                {
                    case "eq":
                        expression1 = (Expression)Expression.Equal(expression2, right);
                        break;
                    case "gt":
                        expression1 = (Expression)Expression.GreaterThan(expression2, right);
                        break;
                    case "gte":
                        expression1 = (Expression)Expression.GreaterThanOrEqual(expression2, right);
                        break;
                    case "less":
                        expression1 = (Expression)Expression.LessThan(expression2, right);
                        break;
                    case "lt":
                        expression1 = (Expression)Expression.LessThan(expression2, right);
                        break;
                    case "lte":
                        expression1 = (Expression)Expression.LessThanOrEqual(expression2, right);
                        break;
                    case "neq":
                        expression1 = (Expression)Expression.NotEqual(expression2, right);
                        break;
                }
            }
            return expression1;
        }
    }
}
