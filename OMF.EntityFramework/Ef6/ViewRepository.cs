using LinqKit;
using OMF.Common;
using OMF.Common.Cache;
using OMF.Common.Extensions;
using OMF.EntityFramework.DataContext;
using OMF.EntityFramework.Query;
using OMF.EntityFramework.Repositories;
using OMF.EntityFramework.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace OMF.EntityFramework.Ef6
{
    public class ViewRepository<TEntity> : IViewRepositoryAsync<TEntity>, IViewRepository<TEntity>
      where TEntity : class, IViewEntity
    {
        protected IDataContextAsync _context;
        protected DbSet<TEntity> _dbSet;

        public ViewRepository(IUnitOfWorkAsync unitOfWork)
        {
            this._context = unitOfWork.DataContext;
            DbContext context1 = this._context as DbContext;
            if (context1 != null)
            {
                this._dbSet = context1.Set<TEntity>();
            }
            else
            {
                FakeDbContext context2 = this._context as FakeDbContext;
                if (context2 != null)
                    this._dbSet = context2.Set<TEntity>();
            }
        }

        public virtual TEntity Find(params object[] keyValues)
        {
            TEntity entity;
            if (((IEnumerable<Type>)typeof(TEntity).GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.FullName == typeof(ICacheable).FullName)))
            {
                List<TEntity> source = CacheManager.GetData(typeof(TEntity).FullName) as List<TEntity>;
                if (source == null)
                {
                    source = this._dbSet.AsNoTracking().ToList<TEntity>();
                    CacheManager.Add(typeof(TEntity).FullName, (object)source);
                }
                entity = source.FirstOrDefault<TEntity>((Func<TEntity, bool>)(e => ((IEnumerable<object>)e.GetID()).Select<object, string>((Func<object, string>)(id => id.ToString())).SequenceEqual<string>(((IEnumerable<object>)keyValues).Select<object, string>((Func<object, string>)(id => id.ToString())))));
            }
            else
                entity = this._dbSet.Find(keyValues);
            if ((object)entity is IAggregateRoot || (object)entity is IAutoReferenceLoad)
            {
                DbContext context = this._context as DbContext;
                if (context == null)
                    return entity;
                if (context.Entry<TEntity>(entity).State == EntityState.Detached)
                    this._dbSet.Attach(entity);
                foreach (PropertyInfo property in entity.GetType().GetProperties())
                {
                    if (!((IEnumerable<ParameterInfo>)property.GetIndexParameters()).Any<ParameterInfo>())
                    {
                        if ((object)entity is IAutoReferenceLoad && ((IEnumerable<Type>)property.PropertyType.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.FullName == typeof(IObjectState).FullName)))
                        {
                            DbReferenceEntry dbReferenceEntry = context.Entry<TEntity>(entity).Reference(property.Name);
                            if (!dbReferenceEntry.IsLoaded)
                            {
                                dbReferenceEntry.Load();
                                if (dbReferenceEntry.CurrentValue is ILogicalDeletable && ((ILogicalDeletable)dbReferenceEntry.CurrentValue).IsDeleted)
                                    dbReferenceEntry.CurrentValue = (object)null;
                            }
                        }
                        else if ((object)entity is IAggregateRoot && ((IEnumerable<Type>)property.PropertyType.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.Name == typeof(ICollection<>).Name)) && ((IEnumerable<Type>)property.PropertyType.GetGenericArguments()).Any<Type>((Func<Type, bool>)(type => ((IEnumerable<Type>)type.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t == typeof(IObjectState))))))
                        {
                            DbCollectionEntry dbCollectionEntry = context.Entry<TEntity>(entity).Collection(property.Name);
                            if (!dbCollectionEntry.IsLoaded)
                            {
                                if (((IEnumerable<Type>)property.PropertyType.GetGenericArguments()).Any<Type>((Func<Type, bool>)(type => ((IEnumerable<Type>)type.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t == typeof(ILogicalDeletable))))))
                                {
                                    IQueryable queryable = dbCollectionEntry.Query();
                                    Type genericArgument = property.PropertyType.GetGenericArguments()[0];
                                    ParameterExpression parameterExpression = Expression.Parameter(genericArgument, "en");
                                    LambdaExpression lambdaExpression = Expression.Lambda((Expression)Expression.Equal((Expression)Expression.Property((Expression)parameterExpression, "IsDeleted"), (Expression)Expression.Constant((object)false, typeof(bool))), parameterExpression);
                                    MethodCallExpression methodCallExpression = Expression.Call(typeof(Queryable), "Where", new Type[1]
                                    {
                    genericArgument
                                    }, queryable.Expression, (Expression)Expression.Quote((Expression)lambdaExpression));
                                    queryable.Provider.CreateQuery((Expression)methodCallExpression).Load();
                                }
                                else
                                    dbCollectionEntry.Load();
                            }
                        }
                    }
                }
            }
            return entity;
        }

        public virtual IQueryable<TEntity> SelectQuery(
          string query,
          params object[] parameters)
        {
            return this._dbSet.SqlQuery(query, parameters).AsQueryable<TEntity>().AsNoTracking<TEntity>();
        }

        public IQueryable<TEntity> Queryable(
          bool containsDeletedData = false,
          bool fromCache = true,
          List<Expression<Func<TEntity, object>>> includes = null)
        {
            Type[] interfaces = typeof(TEntity).GetInterfaces();
            Expression<Func<TEntity, bool>> predicate = (Expression<Func<TEntity, bool>>)null;
            if (!containsDeletedData && ((IEnumerable<Type>)interfaces).Any<Type>((Func<Type, bool>)(t => t.Name == typeof(ILogicalDeletable).Name)))
                predicate = new FilterInfo()
                {
                    Filters = new List<FilterData>()
          {
            new FilterData()
            {
              Field = "IsDeleted",
              Operator = "eq",
              Value = false.ToString()
            }
          },
                    Logic = "and"
                }.TranslateFilter<TEntity>(true);
            if (fromCache && ((IEnumerable<Type>)interfaces).Any<Type>((Func<Type, bool>)(t => t.Name == typeof(ICacheable).Name)))
            {
                List<TEntity> source = CacheManager.GetData(typeof(TEntity).FullName) as List<TEntity>;
                if (source == null)
                {
                    source = this._dbSet.AsNoTracking().AsExpandable<TEntity>().ToList<TEntity>();
             
                    CacheManager.Add(typeof(TEntity).FullName, (object)source);
                }
                if (predicate != null)
                    return source.Where<TEntity>(predicate.Compile()).AsQueryable<TEntity>();
                return source.AsQueryable<TEntity>();
            }
            IQueryable<TEntity> queryable = this._dbSet.AsNoTracking().AsExpandable<TEntity>();
            if (predicate != null)
                queryable = queryable.Where<TEntity>(predicate);
            if (includes != null)
                queryable = includes.Aggregate<Expression<Func<TEntity, object>>, IQueryable<TEntity>>(queryable, (Func<IQueryable<TEntity>, Expression<Func<TEntity, object>>, IQueryable<TEntity>>)((current, include) => current.Include<TEntity, object>(include)));
            return queryable;
        }

        public void LoadReference<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity
        {
            DbContext context = this._context as DbContext;
            if (context == null)
                throw new InvalidOperationException("'LoadReference' method couldn't be called when dataContext is not DbContext");
            DbEntityEntry<TEntity> dbEntityEntry = context.Entry<TEntity>(entity);
            if (dbEntityEntry.State == EntityState.Added || dbEntityEntry.State == EntityState.Detached)
                dbEntityEntry.State = EntityState.Unchanged;
            DbReferenceEntry<TEntity, TProperty> dbReferenceEntry = context.Entry<TEntity>(entity).Reference<TProperty>(navigationProperty);
            if (!dbReferenceEntry.IsLoaded)
                dbReferenceEntry.Load();
            if (!forceToDetach)
                return;
            TProperty currentValue = dbReferenceEntry.CurrentValue;
            dbEntityEntry.State = EntityState.Detached;
            (navigationProperty.GetMember() as PropertyInfo).SetValue((object)entity, (object)currentValue);
            if ((object)currentValue != null)
                context.Entry<TProperty>(currentValue).State = EntityState.Detached;
        }

        public void LoadCollection<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            this.LoadCollection<TElement>(entity, navigationProperty, (Expression<Func<TElement, bool>>)null, forceToDetach);
        }

        public void LoadCollection<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            DbContext context = this._context as DbContext;
            if (context == null)
                throw new InvalidOperationException("'LoadCollection' method couldn't be called when dataContext is not DbContext");
            DbEntityEntry<TEntity> dbEntityEntry = context.Entry<TEntity>(entity);
            if (dbEntityEntry.State == EntityState.Added || dbEntityEntry.State == EntityState.Detached)
                dbEntityEntry.State = EntityState.Unchanged;
            DbCollectionEntry<TEntity, TElement> dbCollectionEntry = context.Entry<TEntity>(entity).Collection<TElement>(navigationProperty);
            if (((IEnumerable<Type>)typeof(TElement).GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t == typeof(ILogicalDeletable))))
            {
                ParameterExpression parameterExpression = predicate == null ? Expression.Parameter(typeof(TElement), "en") : predicate.Parameters[0];
                Expression<Func<TElement, bool>> query = Expression.Lambda<Func<TElement, bool>>((Expression)Expression.Equal((Expression)Expression.Property((Expression)parameterExpression, "IsDeleted"), (Expression)Expression.Constant((object)false, typeof(bool))), parameterExpression);
                if (predicate == null)
                {
                    predicate = query;
                }
                else
                {
                    QueryObject<TElement> queryObject = new QueryObject<TElement>();
                    queryObject.And(predicate);
                    queryObject.And(query);
                    predicate = queryObject.Query();
                }
            }
            if (!dbCollectionEntry.IsLoaded)
            {
                if (predicate != null)
                  //  dbCollectionEntry.Query().Where<TElement>(predicate).Load();
                dbCollectionEntry.Query().AsExpandable<TElement>().Where<TElement>(predicate).Load();
                else
                    dbCollectionEntry.Query().Load();
            }
            if (forceToDetach)
            {
                List<TElement> elementList = new List<TElement>();
                if (dbCollectionEntry.CurrentValue != null)
                    elementList = dbCollectionEntry.CurrentValue.ToList<TElement>();
                dbEntityEntry.State = EntityState.Detached;
                (navigationProperty.GetMember() as PropertyInfo).SetValue((object)entity, (object)elementList);
                foreach (TElement entity1 in elementList)
                    context.Entry<TElement>(entity1).State = EntityState.Detached;
            }
            if (dbCollectionEntry.CurrentValue != null)
                return;
            dbCollectionEntry.CurrentValue = (ICollection<TElement>)new List<TElement>();
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues)
        {
            TEntity async = await this.FindAsync(CancellationToken.None, keyValues);
            return async;
        }

        public virtual async Task<TEntity> FindAsync(
          CancellationToken cancellationToken,
          params object[] keyValues)
        {
            TEntity entity1;
            if (((IEnumerable<Type>)typeof(TEntity).GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.FullName == typeof(ICacheable).FullName)))
            {
                List<TEntity> cacheData = CacheManager.GetData(typeof(TEntity).FullName) as List<TEntity>;
                if (cacheData == null)
                {
                    cacheData = this._dbSet.ToList<TEntity>();
                    CacheManager.Add(typeof(TEntity).FullName, (object)cacheData);
                }
                entity1 = cacheData.FirstOrDefault<TEntity>((Func<TEntity, bool>)(e => ((IEnumerable<object>)e.GetID()).Select<object, string>((Func<object, string>)(id => id.ToString())).SequenceEqual<string>(((IEnumerable<object>)keyValues).Select<object, string>((Func<object, string>)(id => id.ToString())))));
                cacheData = (List<TEntity>)null;
            }
            else
            {
                TEntity entity2 = await this._dbSet.FindAsync(cancellationToken, keyValues);
                entity1 = entity2;
                entity2 = default(TEntity);
            }
            if ((object)entity1 is IAggregateRoot || (object)entity1 is IAutoReferenceLoad)
            {
                DbContext db = this._context as DbContext;
                if (db == null)
                    return entity1;
                if (db.Entry<TEntity>(entity1).State == EntityState.Detached)
                    this._dbSet.Attach(entity1);
                PropertyInfo[] propertyInfoArray = entity1.GetType().GetProperties();
                for (int index = 0; index < propertyInfoArray.Length; ++index)
                {
                    PropertyInfo prop = propertyInfoArray[index];
                    if (!((IEnumerable<ParameterInfo>)prop.GetIndexParameters()).Any<ParameterInfo>())
                    {
                        if ((object)entity1 is IAutoReferenceLoad && ((IEnumerable<Type>)prop.PropertyType.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.FullName == typeof(IObjectState).FullName)))
                        {
                            DbReferenceEntry reference = db.Entry<TEntity>(entity1).Reference(prop.Name);
                            if (!reference.IsLoaded)
                            {
                                await reference.LoadAsync();
                                if (reference.CurrentValue is ILogicalDeletable)
                                {
                                    ILogicalDeletable deletableEntity = (ILogicalDeletable)reference.CurrentValue;
                                    if (deletableEntity.IsDeleted)
                                        reference.CurrentValue = (object)null;
                                    deletableEntity = (ILogicalDeletable)null;
                                }
                            }
                            reference = (DbReferenceEntry)null;
                        }
                        else if ((object)entity1 is IAggregateRoot && ((IEnumerable<Type>)prop.PropertyType.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t.Name == typeof(ICollection<>).Name)) && ((IEnumerable<Type>)prop.PropertyType.GetGenericArguments()).Any<Type>((Func<Type, bool>)(type => ((IEnumerable<Type>)type.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t == typeof(IObjectState))))))
                        {
                            DbCollectionEntry collectionEntry = db.Entry<TEntity>(entity1).Collection(prop.Name);
                            if (!collectionEntry.IsLoaded)
                            {
                                if (((IEnumerable<Type>)prop.PropertyType.GetGenericArguments()).Any<Type>((Func<Type, bool>)(type => ((IEnumerable<Type>)type.GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t == typeof(ILogicalDeletable))))))
                                {
                                    IQueryable query = collectionEntry.Query();
                                    Type entityType = prop.PropertyType.GetGenericArguments()[0];
                                    ParameterExpression param = Expression.Parameter(entityType, "en");
                                    Expression propertyExpression = (Expression)Expression.Property((Expression)param, "IsDeleted");
                                    Expression constantExpresssion = (Expression)Expression.Constant((object)false, typeof(bool));
                                    Expression predicateExpression = (Expression)Expression.Equal(propertyExpression, constantExpresssion);
                                    LambdaExpression whereExpression = Expression.Lambda(predicateExpression, param);
                                    MethodCallExpression whereMethodCallExpression = Expression.Call(typeof(Queryable), "Where", new Type[1]
                                    {
                    entityType
                                    }, query.Expression, (Expression)Expression.Quote((Expression)whereExpression));
                                    IQueryable resultQuery = query.Provider.CreateQuery((Expression)whereMethodCallExpression);
                                    await resultQuery.LoadAsync();
                                    query = (IQueryable)null;
                                    entityType = (Type)null;
                                    param = (ParameterExpression)null;
                                    propertyExpression = (Expression)null;
                                    constantExpresssion = (Expression)null;
                                    predicateExpression = (Expression)null;
                                    whereExpression = (LambdaExpression)null;
                                    whereMethodCallExpression = (MethodCallExpression)null;
                                    resultQuery = (IQueryable)null;
                                }
                                else
                                    await collectionEntry.LoadAsync();
                            }
                            collectionEntry = (DbCollectionEntry)null;
                        }
                        prop = (PropertyInfo)null;
                    }
                }
                propertyInfoArray = (PropertyInfo[])null;
                db = (DbContext)null;
            }
            return entity1;
        }

        public async Task LoadReferenceAsync<TProperty>(
          TEntity entity,
          Expression<Func<TEntity, TProperty>> navigationProperty,
          bool forceToDetach = false)
          where TProperty : class, IEntity
        {
            DbContext db = this._context as DbContext;
            if (db == null)
                throw new InvalidOperationException("'LoadReference' method couldn't be called when dataContext is not DbContext");
            DbEntityEntry<TEntity> entry = db.Entry<TEntity>(entity);
            if (entry.State == EntityState.Added || entry.State == EntityState.Detached)
                entry.State = EntityState.Unchanged;
            DbReferenceEntry<TEntity, TProperty> reference = db.Entry<TEntity>(entity).Reference<TProperty>(navigationProperty);
            if (!reference.IsLoaded)
                await reference.LoadAsync();
            if (!forceToDetach)
                return;
            TProperty result = reference.CurrentValue;
            entry.State = EntityState.Detached;
            PropertyInfo propInfo = navigationProperty.GetMember() as PropertyInfo;
            propInfo.SetValue((object)entity, (object)result);
            if ((object)result != null)
                db.Entry<TProperty>(result).State = EntityState.Detached;
            result = default(TProperty);
            propInfo = (PropertyInfo)null;
        }

        public async Task LoadCollectionAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            await this.LoadCollectionAsync<TElement>(entity, navigationProperty, (Expression<Func<TElement, bool>>)null, forceToDetach);
        }

        public async Task LoadCollectionAsync<TElement>(
          TEntity entity,
          Expression<Func<TEntity, ICollection<TElement>>> navigationProperty,
          Expression<Func<TElement, bool>> predicate,
          bool forceToDetach = false)
          where TElement : class, IEntity
        {
            DbContext db = this._context as DbContext;
            if (db == null)
                throw new InvalidOperationException("'LoadReference' method couldn't be called when dataContext is not DbContext");
            DbEntityEntry<TEntity> entry = db.Entry<TEntity>(entity);
            if (entry.State == EntityState.Added || entry.State == EntityState.Detached)
                entry.State = EntityState.Unchanged;
            DbCollectionEntry<TEntity, TElement> collectionEntry = db.Entry<TEntity>(entity).Collection<TElement>(navigationProperty);
            if (((IEnumerable<Type>)typeof(TElement).GetInterfaces()).Any<Type>((Func<Type, bool>)(t => t == typeof(ILogicalDeletable))))
            {
                ParameterExpression param = predicate == null ? Expression.Parameter(typeof(TElement), "en") : predicate.Parameters[0];
                Expression propertyExpression = (Expression)Expression.Property((Expression)param, "IsDeleted");
                Expression constantExpresssion = (Expression)Expression.Constant((object)false, typeof(bool));
                Expression predicateExpression = (Expression)Expression.Equal(propertyExpression, constantExpresssion);
                Expression<Func<TElement, bool>> isDeletedExpression = Expression.Lambda<Func<TElement, bool>>(predicateExpression, param);
                if (predicate == null)
                {
                    predicate = isDeletedExpression;
                }
                else
                {
                    QueryObject<TElement> queryObject = new QueryObject<TElement>();
                    queryObject.And(predicate);
                    queryObject.And(isDeletedExpression);
                    predicate = queryObject.Query();
                    queryObject = (QueryObject<TElement>)null;
                }
                param = (ParameterExpression)null;
                propertyExpression = (Expression)null;
                constantExpresssion = (Expression)null;
                predicateExpression = (Expression)null;
                isDeletedExpression = (Expression<Func<TElement, bool>>)null;
            }
            if (!collectionEntry.IsLoaded)
            {
                if (predicate != null)
                   // await collectionEntry.Query().Where<TElement>(predicate).LoadAsync();
                await collectionEntry.Query().AsExpandable<TElement>().Where<TElement>(predicate).LoadAsync();
                else
                    await collectionEntry.Query().LoadAsync();
            }
            if (forceToDetach)
            {
                List<TElement> list = new List<TElement>();
                if (collectionEntry.CurrentValue != null)
                    list = collectionEntry.CurrentValue.ToList<TElement>();
                entry.State = EntityState.Detached;
                PropertyInfo propInfo = navigationProperty.GetMember() as PropertyInfo;
                propInfo.SetValue((object)entity, (object)list);
                foreach (TElement element in list)
                {
                    TElement item = element;
                    db.Entry<TElement>(item).State = EntityState.Detached;
                    item = default(TElement);
                }
                list = (List<TElement>)null;
                propInfo = (PropertyInfo)null;
            }
            if (collectionEntry.CurrentValue != null)
                return;
            collectionEntry.CurrentValue = (ICollection<TElement>)new List<TElement>();
        }
    }
}
