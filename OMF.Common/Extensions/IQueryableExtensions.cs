using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace OMF.Common.Extensions
{
    public static class IQueryableExtensions
    {

        public static async Task<int> CountAsync2<T>(this IQueryable<T> source)
        {
            if (source is EnumerableQuery<T>)
            {
                int num = await Task.Run<int>((Func<int>)(() =>
                {
                    if (source != null)
                        return Queryable.Count<T>(source);
                    return 0;
                }));
                return num;
            }
            int num1 = await source.CountAsync<T>();
            return num1;
        }

        public static async Task<List<T>> ToListAsync2<T>(this IQueryable<T> source)
        {
            if (source is EnumerableQuery<T>)
            {
                List<T> objList = await Task.Run<List<T>>((Func<List<T>>)(() =>
                {
                    if (source != null)
                        return ((IEnumerable<T>)source).ToList<T>();
                    return new List<T>();
                }));
                return objList;
            }
            List<T> listAsync = await source.ToListAsync<T>();
            return listAsync;
        }

        public static async Task<T[]> ToArrayAsync2<T>(this IQueryable<T> source)
        {
            if (source is EnumerableQuery<T>)
            {
                T[] objArray = await Task.Run<T[]>((Func<T[]>)(() =>
                {
                    if (source != null)
                        return ((IEnumerable<T>)source).ToArray<T>();
                    return new List<T>().ToArray();
                }));
                return objArray;
            }
            T[] arrayAsync = await source.ToArrayAsync<T>();
            return arrayAsync;
        }

        public static async Task<T> FirstOrDefaultAsync2<T>(this IQueryable<T> source)
        {
            if (source is EnumerableQuery<T>)
            {
                T obj = await Task.Run<T>((Func<T>)(() => Queryable.FirstOrDefault<T>(source)));
                return obj;
            }
            T obj1 = await source.FirstOrDefaultAsync<T>();
            return obj1;
        }

        public static async Task<T> SingleOrDefaultAsync2<T>(this IQueryable<T> source)
        {
            if (source is EnumerableQuery<T>)
            {
                T obj = await Task.Run<T>((Func<T>)(() => Queryable.SingleOrDefault<T>(source)));
                return obj;
            }
            T obj1 = await source.SingleOrDefaultAsync<T>();
            return obj1;
        }

        public static async Task<bool> AnyAsync2<T>(this IQueryable<T> source)
        {
            if (source is EnumerableQuery<T>)
            {
                bool flag = await Task.Run<bool>((Func<bool>)(() =>
                {
                    if (source != null)
                        return Queryable.Any<T>(source);
                    return false;
                }));
                return flag;
            }
            bool flag1 = await source.AnyAsync<T>();
            return flag1;
        }

        public static async Task<bool> ContainsAsync2<T>(this IQueryable<T> source, T item)
        {
            if (source is EnumerableQuery<T>)
            {
                bool flag = await Task.Run<bool>((Func<bool>)(() =>
                {
                    if (source != null)
                        return source.Contains<T>(item);
                    return false;
                }));
                return flag;
            }
            bool flag1 = await source.ContainsAsync<T>(item);
            return flag1;
        }

        public static IOrderedQueryable<T> OrderBy<T>(
          this IQueryable<T> source,
          string propertyName)
        {
            return IQueryableExtensions.ApplyOrder<T>(source, propertyName, nameof(OrderBy));
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(
          this IQueryable<T> source,
          string propertyName)
        {
            return IQueryableExtensions.ApplyOrder<T>(source, propertyName, nameof(OrderByDescending));
        }

        public static IOrderedQueryable<T> ThenBy<T>(
          this IOrderedQueryable<T> source,
          string propertyName)
        {
            return IQueryableExtensions.ApplyOrder<T>((IQueryable<T>)source, propertyName, nameof(ThenBy));
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(
          this IOrderedQueryable<T> source,
          string propertyName)
        {
            return IQueryableExtensions.ApplyOrder<T>((IQueryable<T>)source, propertyName, nameof(ThenByDescending));
        }

        private static IOrderedQueryable<T> ApplyOrder<T>(
          IQueryable<T> source,
          string propertyName,
          string methodName)
        {
            Type type = typeof(T);
            ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "en");
            MemberExpression memberExpression = Expression.Property((Expression)parameterExpression, propertyName);
            LambdaExpression lambdaExpression = Expression.Lambda((Expression)memberExpression, parameterExpression);
            MethodCallExpression methodCallExpression = Expression.Call(typeof(Queryable), methodName, new Type[2]
            {
        type,
        memberExpression.Type
            }, source.Expression, (Expression)Expression.Quote((Expression)lambdaExpression));
            return source.Provider.CreateQuery<T>((Expression)methodCallExpression) as IOrderedQueryable<T>;
        }

        public static IQueryable<object> SelectBy<T>(
          this IQueryable<T> source,
          IEnumerable<string> propertyNames)
        {
            if (source == null)
                throw new ArgumentNullException("Source Object is NULL");
            ParameterExpression sourceItem = Expression.Parameter(source.ElementType, "t");
            Dictionary<string, PropertyInfo> sourceProperties = propertyNames.Where<string>((Func<string, bool>)(name => source.ElementType.GetProperty(name) != (PropertyInfo)null)).ToDictionary<string, string, PropertyInfo>((Func<string, string>)(name => name), (Func<string, PropertyInfo>)(name => source.ElementType.GetProperty(name)));
            Type dynamicType = DynamicTypeBuilder.GetDynamicType(sourceProperties.Values.ToDictionary<PropertyInfo, string, Type>((Func<PropertyInfo, string>)(f => f.Name), (Func<PropertyInfo, Type>)(f => f.PropertyType)), typeof(object), Type.EmptyTypes);
            List<MemberBinding> list = ((IEnumerable<FieldInfo>)dynamicType.GetFields()).Select<FieldInfo, MemberAssignment>((Func<FieldInfo, MemberAssignment>)(p => Expression.Bind((MemberInfo)p, (Expression)Expression.Property((Expression)sourceItem, sourceProperties[p.Name])))).OfType<MemberBinding>().ToList<MemberBinding>();
            Expression<Func<T, object>> selector = Expression.Lambda<Func<T, object>>((Expression)Expression.MemberInit(Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), (IEnumerable<MemberBinding>)list), sourceItem);
            return source.Select<T, object>(selector);
        }

        public static IEnumerable<T> SelectIncluding<T>(
          this IQueryable<T> source,
          IEnumerable<Expression<Func<T, object>>> includeExpessions)
        {
            if (source == null)
                throw new ArgumentNullException("Source Object is NULL");
            ParameterExpression parameterExpression = Expression.Parameter(source.ElementType, "t");
            IQueryableExtensions.PredicateRewriterVisitor predicateRewriterVisitor = new IQueryableExtensions.PredicateRewriterVisitor(parameterExpression);
            Dictionary<string, Tuple<Expression, Type>> dynamicFields = new Dictionary<string, Tuple<Expression, Type>>();
            int num = 0;
            foreach (Expression<Func<T, object>> includeExpession in includeExpessions)
            {
                if (includeExpession.Body.NodeType == ExpressionType.Convert || includeExpession.Body.NodeType == ExpressionType.ConvertChecked)
                {
                    UnaryExpression body = includeExpession.Body as UnaryExpression;
                    if (body != null)
                    {
                        Type type = body.Operand.Type;
                    }
                }
                Type type1 = includeExpession.Body.Type;
                dynamicFields.Add("f" + (object)num, new Tuple<Expression, Type>(predicateRewriterVisitor.ReplaceParameter(includeExpession.Body, includeExpession.Parameters[0]), type1));
                ++num;
            }
            dynamicFields.Add("sourceObject", new Tuple<Expression, Type>((Expression)parameterExpression, source.ElementType));
            Type dynamicType = DynamicTypeBuilder.GetDynamicType(dynamicFields.ToDictionary<KeyValuePair<string, Tuple<Expression, Type>>, string, Type>((Func<KeyValuePair<string, Tuple<Expression, Type>>, string>)(x => x.Key), (Func<KeyValuePair<string, Tuple<Expression, Type>>, Type>)(x => x.Value.Item2)), typeof(object), Type.EmptyTypes);
            List<MemberBinding> list = ((IEnumerable<FieldInfo>)dynamicType.GetFields()).Select<FieldInfo, MemberAssignment>((Func<FieldInfo, MemberAssignment>)(p => Expression.Bind((MemberInfo)p, dynamicFields[p.Name].Item1))).OfType<MemberBinding>().ToList<MemberBinding>();
            Expression<Func<T, object>> selector = Expression.Lambda<Func<T, object>>((Expression)Expression.MemberInit(Expression.New(dynamicType.GetConstructor(Type.EmptyTypes)), (IEnumerable<MemberBinding>)list), parameterExpression);
            return source.Select<T, object>(selector).AsEnumerable<object>().Select<object, T>((Func<object, T>)(x =>
            {
                // ISSUE: reference to a compiler-generated field
                if (IQueryableExtensions.o__13<T>.p__1 == null)
                {
                    // ISSUE: reference to a compiler-generated field
                    IQueryableExtensions.o__13<T>.p__1 = CallSite<Func<CallSite, object, T>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.ConvertExplicit, typeof(T), typeof(IQueryableExtensions)));
                }
                // ISSUE: reference to a compiler-generated field
                Func<CallSite, object, T> target = IQueryableExtensions.o__13<T>.p__1.Target;
                // ISSUE: reference to a compiler-generated field
                CallSite<Func<CallSite, object, T>> p1 = IQueryableExtensions.o__13<T>.p__1;
                // ISSUE: reference to a compiler-generated field
                if (IQueryableExtensions.o__13<T>.p__0 == null)
                {
                    // ISSUE: reference to a compiler-generated field
                    IQueryableExtensions.o__13<T>.p__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "sourceObject", typeof(IQueryableExtensions), (IEnumerable<CSharpArgumentInfo>)new CSharpArgumentInfo[1]
                    {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                    }));
                }
                // ISSUE: reference to a compiler-generated field
                // ISSUE: reference to a compiler-generated field
                object obj = IQueryableExtensions.o__13<T>.p__0.Target((CallSite)IQueryableExtensions.o__13<T>.p__0, x);
                return target((CallSite)p1, obj);
            }));
        }
        private static class o__13<T>
        {
            // Fields
            public static CallSite<Func<CallSite, object, object>> p__0;
            public static CallSite<Func<CallSite, object, T>> p__1;
        }


        private class PredicateRewriterVisitor : ExpressionVisitor
        {
            private ParameterExpression _parameterExpression;
            private ParameterExpression _parameterExpressionToReplace;

            public PredicateRewriterVisitor(ParameterExpression parameterExpression)
            {
                this._parameterExpression = parameterExpression;
            }

            public Expression ReplaceParameter(
              Expression node,
              ParameterExpression parameterExpressionToReplace)
            {
                this._parameterExpressionToReplace = parameterExpressionToReplace;
                return this.Visit(node);
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (node == this._parameterExpressionToReplace)
                    return (Expression)this._parameterExpression;
                return (Expression)node;
            }
        }



    }
}

