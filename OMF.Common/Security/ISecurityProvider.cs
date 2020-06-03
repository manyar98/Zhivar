namespace OMF.Common.Security
{
    public interface ISecurityProvider
    {
        UserContext Login(string userName, string password);

        UserContext Logoff(string token);

        bool HasAccess(string operationCode);

        bool HasAccess(int userId, string operationCode);

        bool UserInRole(string roleCode);

        bool HasPageAccess(string operationCode);

        bool ExistsUser(string userName, int? applicationId = null);

        RoleDataCollection GetUserRoles(int userId);

        void ThrowIfUserContextNull();
    }
}
