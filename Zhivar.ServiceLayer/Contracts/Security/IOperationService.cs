using OMF.Common.Security;
using OMF.Security.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace Zhivar.ServiceLayer.Contracts.Security
{
    public interface IOperationService
    {
        List<Operation> GetUserOperations();
        List<Operation> GetOperationsByRoleCode(string roleCode, int? applicationId = null);
        List<Operation> GetUserOperations(OperationType operationType);
        bool ExistsUser(string userName, int? applicationId = null);
        bool UserInRole(string roleCode);
        bool HasAccess(string operationCode);
        bool HasAccess(int userId, string operationCode);
        bool HasPageAccess(string operationCode);
        UserContext Login(string userName, string password);
        UserContext Logoff(string userToken);
        event Action<string> AfterLogoffHandler;
        UserInfo GetUser(int userId);
        UserInfo GetUser(string userName, int? applicationId = null);
        Task<UserContext> LoginAsync(string userName, string password);
        Task<bool> HasAccessAsync(int userId, string operationCode);
        Task<bool> HasAccessAsync(string operationCode);
        Task<bool> UserInRoleAsync(string roleCode);
        Task<bool> HasPageAccessAsync(string operationCode);
        Task<List<Operation>> GetUserOperationsAsync();
        Task<List<Operation>> GetOperationsByRoleCodeAsync(string roleCode);
        Task<List<Operation>> GetAllOperationsAsync();
        Task<List<Operation>> GetUserOperationsAsync(OperationType operationType);
        Task<UserInfo> GetUserAsync(int userId);
        Task<UserInfo> GetUserAsync(string userName, int? applicationId = null);
        Task<bool> ExistsUserAsync(string userName, int? applicationId = null);
        UserContext InitiateUserContext(UserInfo uInfo);
        RoleDataCollection GetUserRoles(int userId);
        void ThrowIfUserContextNull();
    }
}
