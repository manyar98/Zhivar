using OMF.Common;
using OMF.Common.Security;
using OMF.EntityFramework.UnitOfWork;
using System;

namespace OMF.Business
{
    public static class BusinessContext
    {
        public static IBusinessRuleBaseAsync<TEntity> GetBusinessRule<TEntity>() where TEntity : class, IEntity
        {
            return BusinessContext.GetBusinessRule<TEntity>((OperationAccess)null, (IUnitOfWorkAsync)null);
        }

        public static IBusinessRuleBaseAsync<TEntity> GetBusinessRule<TEntity>(
          IUnitOfWorkAsync unitOfWork)
          where TEntity : class, IEntity
        {
            return BusinessContext.GetBusinessRule<TEntity>((OperationAccess)null, unitOfWork);
        }

        public static IBusinessRuleBaseAsync<TEntity> GetBusinessRule<TEntity>(
          OperationAccess operationAccess)
          where TEntity : class, IEntity
        {
            return BusinessContext.GetBusinessRule<TEntity>(operationAccess, (IUnitOfWorkAsync)null);
        }

        public static IBusinessRuleBaseAsync<TEntity> GetBusinessRule<TEntity>(
          OperationAccess operationAccess,
          IUnitOfWorkAsync unitOfWork)
          where TEntity : class, IEntity
        {

            Type type = typeof(BusinessRuleBase<>);
            IBusinessRuleBaseAsync<TEntity> instance;
            if (unitOfWork == null)
                instance = Activator.CreateInstance(type.MakeGenericType(typeof(TEntity))) as IBusinessRuleBaseAsync<TEntity>;
            else
                instance = Activator.CreateInstance(type.MakeGenericType(typeof(TEntity)), (object)unitOfWork) as IBusinessRuleBaseAsync<TEntity>;
            if (operationAccess != null)
            {
                OperationAccess operationAccess1 = new OperationAccess()
                {
                    CanInsert = instance.OperationAccess.CanInsert || operationAccess.CanInsert,
                    CanView = instance.OperationAccess.CanView || operationAccess.CanView,
                    CanUpdate = instance.OperationAccess.CanUpdate || operationAccess.CanUpdate,
                    CanDelete = instance.OperationAccess.CanDelete || operationAccess.CanDelete,
                    CanExport = instance.OperationAccess.CanExport || operationAccess.CanExport,
                    CanPrint = instance.OperationAccess.CanPrint || operationAccess.CanPrint,
                    CanImport = instance.OperationAccess.CanImport || operationAccess.CanImport
                };
                instance.OperationAccess = operationAccess1;
            }
            return instance;
        }
    }
}
