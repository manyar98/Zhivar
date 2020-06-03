using System.Collections;

namespace OMF.Common.Cache
{
    public interface ISessionProvider : ICacheProvider, IEnumerable
    {
        void ChangeSessionId();

        string SessionId { get; }
    }
}
