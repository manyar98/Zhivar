using OMF.Common.Cache;

namespace OMF.Common.Security
{
    public class OperationKeysManager
    {
        private const string pattern = "__{0}_OperationKeys__";

        public static OperationKeys GetOperationKeys<TEntity>() where TEntity : class
        {
            string key = string.Format("__{0}_OperationKeys__", (object)typeof(TEntity).FullName);
            OperationKeys operationKeys = CacheManager.GetData(key) as OperationKeys;
            if (operationKeys == null)
            {
                operationKeys = new OperationKeys();
                CacheManager.Add(key, (object)operationKeys);
            }
            return operationKeys;
        }
    }
}
