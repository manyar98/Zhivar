using AutoMapper;

namespace OMF.Common
{
    public static class EntityTranslator
    {
        public static TToEntity Translate<TToEntity>(this object fromEntity) where TToEntity : class, new()
        {
            if (fromEntity == null)
                return default(TToEntity);
            return Mapper.Map<TToEntity>(fromEntity);
        }
    }
}
