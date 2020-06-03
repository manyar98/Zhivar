using System.Threading.Tasks;

namespace OMF.Common.Security
{
    public interface ISecurityProviderAsync : ISecurityProvider
    {
        Task<UserContext> LoginAsync(string userName, string password);

        Task<bool> HasAccessAsync(int userId, string operationCode);

        Task<bool> HasAccessAsync(string operationCode);

        Task<bool> UserInRoleAsync(string roleCode);

        Task<bool> HasPageAccessAsync(string operationCode);

        Task<bool> ExistsUserAsync(string userName, int? applicationId = null);
    }
}
