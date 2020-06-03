using OMF.Business;
using OMF.Common;

namespace OMF.Enterprise.MVC
{
    public class WebApiBusinessManager<TEntity> : BusinessManager<TEntity> where TEntity : class, IEntity, new()
    {
        public WebApiBusinessManager(IBusinessRuleBaseAsync<TEntity> businessRule)
          : base(businessRule)
        {
        }
    }
}
