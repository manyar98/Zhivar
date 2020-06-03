using OMF.Common;
using System;
using System.Data.Entity;
using static OMF.Common.Enums;

namespace OMF.EntityFramework.Ef6
{
    public class StateHelper
    {
        public static EntityState ConvertState(ObjectState state)
        {
            switch (state)
            {
                case ObjectState.Unchanged:
                    return EntityState.Unchanged;
                case ObjectState.Added:
                    return EntityState.Added;
                case ObjectState.Modified:
                    return EntityState.Modified;
                case ObjectState.Deleted:
                    return EntityState.Deleted;
                default:
                    return EntityState.Detached;
            }
        }

        public static ObjectState ConvertState(EntityState state)
        {
            switch (state)
            {
                case EntityState.Detached:
                    return ObjectState.Unchanged;
                case EntityState.Unchanged:
                    return ObjectState.Unchanged;
                case EntityState.Added:
                    return ObjectState.Added;
                case EntityState.Deleted:
                    return ObjectState.Deleted;
                case EntityState.Modified:
                    return ObjectState.Modified;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state));
            }
        }
    }
}
