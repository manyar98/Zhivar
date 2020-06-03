using OMF.Common.Cache;
using System;

namespace OMF.Common.Validation
{
    public class ValidationManager
    {
        private const string pattern = "__{0}_EntityValidator__";

        public static void SetEntityValidator<TEntity>(EntityValidator<TEntity> validator) where TEntity : class, IEntity
        {
            ValidationManager.SetEntityValidator(typeof(TEntity), (IEntityValidator)validator);
        }

        public static void SetEntityValidator(Type entityType, IEntityValidator validator)
        {
            CacheManager.Add(string.Format("__{0}_EntityValidator__", (object)entityType.FullName), (object)validator);
        }

        public static EntityValidator<TEntity> GetEntityValidator<TEntity>() where TEntity : class, IEntity
        {
            return ValidationManager.GetEntityValidator(typeof(TEntity)) as EntityValidator<TEntity>;
        }

        public static IEntityValidator GetEntityValidator(Type entityType)
        {
            return CacheManager.GetData(string.Format("__{0}_EntityValidator__", (object)entityType.FullName)) as IEntityValidator;
        }
    }
}
