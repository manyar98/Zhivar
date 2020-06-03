using System.Threading.Tasks;

namespace OMF.Common.ExceptionManagement
{
    public interface IExceptionLoggerAsync : IExceptionLogger
    {
        Task LogExceptionAsync(ExceptionData exData);
    }
}
