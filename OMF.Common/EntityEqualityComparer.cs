using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OMF.Common
{
    public class EntityEqualityComparer
    {
        public static bool Equals<TEntity>(TEntity originalEntity, TEntity currentEntity) where TEntity : class, IEntity
        {
            if ((object)originalEntity == null)
                return false;
            foreach (PropertyInfo property in typeof(TEntity).GetProperties())
            {
                if (!((IEnumerable<ParameterInfo>)property.GetIndexParameters()).Any<ParameterInfo>())
                {
                    object obj1 = currentEntity[property.Name];
                    object obj2 = originalEntity[property.Name];
                    if ((obj1 != null || obj2 != null) && obj1 != obj2)
                    {
                        if (obj1 == null || obj2 == null)
                            return false;
                        if (obj1 is ICollection)
                        {
                            if (((ICollection)obj1).Count != ((ICollection)obj2).Count)
                                return false;
                        }
                        else if (!EqualityComparer<string>.Default.Equals(obj1.ToString(), obj2.ToString()))
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
