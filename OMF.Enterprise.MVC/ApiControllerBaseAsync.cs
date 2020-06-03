using OMF.Common;

namespace OMF.Enterprise.MVC
{
    public class ApiControllerBaseAsync<TEntity, TEntityVM> : ApiControllerCustomQueryBaseAsync<TEntity, TEntity, TEntityVM>
      where TEntity : class, IEntity, new()
      where TEntityVM : class, new()
    {
    }
}
