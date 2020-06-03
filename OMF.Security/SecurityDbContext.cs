using OMF.Common;
using OMF.Common.Configuration;
using OMF.EntityFramework;
using OMF.EntityFramework.Common;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model;
using OMF.Security.Model.MappingConfiguration;
using OMF.Security.Model.MappingConfiguration.Test;
using OMF.Security.Model.Test;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Threading.Tasks;
using static OMF.Common.Enums;

namespace OMF.Security
{
    public class SecurityDbContext : DataContext
    {
        public SecurityDbContext()
          : base(ExceptionDbConnectionManager.CreateConnection(), true)
        {
           
            this.Database.Log = (Action<string>)(str => { });
        
            Database.SetInitializer(new NullDatabaseInitializer<SecurityDbContext>());
            //Database.SetInitializer<SecurityDbContext>((IDatabaseInitializer<SecurityDbContext>)new
            //    NullDatabaseInitializer<SecurityDbContext>());
        }

        //public DbSet<ForTest2> ForTest2s { get; set; }
        public DbSet<UserInfo> Users { get; set; }

        public DbSet<Operation> Operations { get; set; }

        public DbSet<RoleBase> Roles { get; set; }

        public DbSet<ForgotPasswordEntity> ForgotPasswordEntities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.HasDefaultSchema(ConfigurationController.SecurityDbSchema);

            modelBuilder.Configurations.Add(new OrganizationConfig());
            modelBuilder.Configurations.Add(new OrganizationUnitChartConfig());
            modelBuilder.Configurations.Add(new UserOperationConfig());
            modelBuilder.Configurations.Add(new UserInfoConfig());
            modelBuilder.Configurations.Add(new OperationConfig());
            modelBuilder.Configurations.Add(new RoleBaseConfig());
            modelBuilder.Configurations.Add(new RoleConfig());
            modelBuilder.Configurations.Add(new PositionConfig());
            modelBuilder.Configurations.Add(new RoleOperationConfig());
           // modelBuilder.Configurations.Add(new UserRoleConfig());
            modelBuilder.Configurations.Add<UserRole>((EntityTypeConfiguration<UserRole>)new UserRoleConfig());
            modelBuilder.Configurations.Add(new ForgotPasswordConfig());
            modelBuilder.Configurations.Add(new LoggableEntityIDTypeConfig());
            modelBuilder.Configurations.Add(new LoggableEntityNameTypeConfig());
        }

        internal bool CheckSecurityAccess(int userId, string operationCode)
        {
            return this.Database.SqlQuery<int>(string.Format("select count(Code) \r\n                                                 from VW_USER_OPERATION_LIST \r\n                                                 where UserID = {0}p0 and Code = {1}p1", (object)OMFAppContext.ParameterPrefix, (object)OMFAppContext.ParameterPrefix), (object)userId, (object)operationCode).FirstOrDefault<int>() > 0;
        }

        internal async Task<bool> CheckSecurityAccessAsync(int userId, string operationCode)
        {
            DbRawSqlQuery<int> query = this.Database.SqlQuery<int>(string.Format("select count(Code) \r\n                                                 from VW_USER_OPERATION_LIST \r\n                                                 where UserID = {0}p0 and Code = {1}p1", (object)OMFAppContext.ParameterPrefix, (object)OMFAppContext.ParameterPrefix), (object)userId, (object)operationCode);
            int result = await query.FirstOrDefaultAsync();
            return result > 0;
        }

        internal List<Operation> GetOperationsByUserID(int userId)
        {
            return this.Database.SqlQuery<SecurityDbContext.OperationV>(string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_USER_OPERATION_LIST \r\n                                                       where UserID = {0}", userId)).ToList<SecurityDbContext.OperationV>().ConvertAll<Operation>((Converter<SecurityDbContext.OperationV, Operation>)(oprV =>
            {
                return new Operation()
                {
                    ApplicationId = oprV.ApplicationId,
                    Code = oprV.Code,
                    ID = oprV.ID,
                    IsActive = Convert.ToBoolean(oprV.IsActive),
                    IsDeleted = Convert.ToBoolean(oprV.IsDeleted),
                    Name = oprV.Name,
                    OperationType = (OperationType)oprV.OperationType,
                    OrderNo = oprV.OrderNo,
                    ParentId = oprV.ParentId,
                    Tag1 = oprV.Tag1,
                    Tag2 = oprV.Tag2,
                    Tag3 = oprV.Tag3,
                    Tag4 = oprV.Tag4
                };
            }));
        }

        internal async Task<List<Operation>> GetOperationsByUserIDAsync(int userId)
        {
            try
            {
                var result2 =
      await this.Database.SqlQuery<OperationV>
      (string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_USER_OPERATION_LIST \r\n                                                       where UserID = {0}", userId)).ToListAsync();


                List<SecurityDbContext.OperationV> result =
            await this.Database.SqlQuery<SecurityDbContext.OperationV>
            (string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_USER_OPERATION_LIST \r\n                                                       where UserID = {0}", userId)).ToListAsync();
                return result.ConvertAll<Operation>((Converter<SecurityDbContext.OperationV, Operation>)(oprV =>
                {
                    return new Operation()
                    {
                        ApplicationId = oprV.ApplicationId,
                        Code = oprV.Code,
                        ID = oprV.ID,
                        IsActive = Convert.ToBoolean(oprV.IsActive),
                        IsDeleted = Convert.ToBoolean(oprV.IsDeleted),
                        Name = oprV.Name,
                        OperationType = (OperationType)oprV.OperationType,
                        OrderNo = oprV.OrderNo,
                        ParentId = oprV.ParentId,
                        Tag1 = oprV.Tag1,
                        Tag2 = oprV.Tag2,
                        Tag3 = oprV.Tag3,
                        Tag4 = oprV.Tag4
                    };
                }));
            }
            catch (Exception ex)
            {

                throw;
            }
        
        }

        internal List<Operation> GetOperationsByRoleID(int roleId)
        {
            return this.Database.SqlQuery<SecurityDbContext.OperationV>
                (string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_ROLE_OPERATION_LIST \r\n                                                       where RoleID = {0}", roleId)).ToList<SecurityDbContext.OperationV>().ConvertAll<Operation>((Converter<SecurityDbContext.OperationV, Operation>)(oprV =>
            {
                return new Operation()
                {
                    ApplicationId = oprV.ApplicationId,
                    Code = oprV.Code,
                    ID = oprV.ID,
                    IsActive = Convert.ToBoolean(oprV.IsActive),
                    IsDeleted = Convert.ToBoolean(oprV.IsDeleted),
                    Name = oprV.Name,
                    OperationType = (OperationType)oprV.OperationType,
                    OrderNo = oprV.OrderNo,
                    ParentId = oprV.ParentId,
                    Tag1 = oprV.Tag1,
                    Tag2 = oprV.Tag2,
                    Tag3 = oprV.Tag3,
                    Tag4 = oprV.Tag4
                };
            }));
        }

        internal async Task<List<Operation>> GetOperationsByRoleIDAsync(int roleId)
        {
            List<SecurityDbContext.OperationV> result = await this.Database.SqlQuery<SecurityDbContext.OperationV>
                (string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_ROLE_OPERATION_LIST \r\n                                                       where RoleID = {0}", roleId)).ToListAsync();
            return result.ConvertAll<Operation>((Converter<SecurityDbContext.OperationV, Operation>)(oprV =>
            {
                return new Operation()
                {
                    ApplicationId = oprV.ApplicationId,
                    Code = oprV.Code,
                    ID = oprV.ID,
                    IsActive = Convert.ToBoolean(oprV.IsActive),
                    IsDeleted = Convert.ToBoolean(oprV.IsDeleted),
                    Name = oprV.Name,
                    OperationType = (OperationType)oprV.OperationType,
                    OrderNo = oprV.OrderNo,
                    ParentId = oprV.ParentId,
                    Tag1 = oprV.Tag1,
                    Tag2 = oprV.Tag2,
                    Tag3 = oprV.Tag3,
                    Tag4 = oprV.Tag4
                };
            }));
        }

        internal List<Operation> GetOperationsByRoleCode(
          string roleCode,
          int? applicationId)
        {
            List<SecurityDbContext.OperationV> operationVList = new List<SecurityDbContext.OperationV>();
            List<SecurityDbContext.OperationV> list;
            if (applicationId.HasValue)
                list = this.Database.SqlQuery<SecurityDbContext.OperationV>(string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_ROLE_OPERATION_LIST \r\n                                                       where RoleCode = {0} and ApplicationId = {1}", roleCode, applicationId)).ToList<SecurityDbContext.OperationV>();
            else
                list = this.Database.SqlQuery<SecurityDbContext.OperationV>(string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_ROLE_OPERATION_LIST \r\n                                                       where RoleCode = {0}", roleCode)).ToList<SecurityDbContext.OperationV>();
            return list.ConvertAll<Operation>((Converter<SecurityDbContext.OperationV, Operation>)(oprV =>
            {
                return new Operation()
                {
                    ApplicationId = oprV.ApplicationId,
                    Code = oprV.Code,
                    ID = oprV.ID,
                    IsActive = Convert.ToBoolean(oprV.IsActive),
                    IsDeleted = Convert.ToBoolean(oprV.IsDeleted),
                    Name = oprV.Name,
                    OperationType = (OperationType)oprV.OperationType,
                    OrderNo = oprV.OrderNo,
                    ParentId = oprV.ParentId,
                    Tag1 = oprV.Tag1,
                    Tag2 = oprV.Tag2,
                    Tag3 = oprV.Tag3,
                    Tag4 = oprV.Tag4
                };
            }));
        }

        internal async Task<List<Operation>> GetOperationsByRoleCodeAsync(
          string roleCode,
          int? applicationId)
        {
            List<SecurityDbContext.OperationV> result = new List<SecurityDbContext.OperationV>();
            if (applicationId.HasValue)
                result = await this.Database.SqlQuery<SecurityDbContext.OperationV>(string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_ROLE_OPERATION_LIST \r\n                                                       where RoleCode = {0} and ApplicationId = {1}", roleCode, applicationId)).ToListAsync();
            else
                result = await this.Database.SqlQuery<SecurityDbContext.OperationV>(string.Format("select ID,ParentID,Name,Code,OperationType,IsActive,IsDeleted,Tag1,Tag2,Tag3,Tag4,OrderNo,ApplicationId \r\n                                                       from VW_ROLE_OPERATION_LIST \r\n                                                       where RoleCode = {0}", roleCode)).ToListAsync();
            return result.ConvertAll<Operation>((Converter<SecurityDbContext.OperationV, Operation>)(oprV =>
            {
                return new Operation()
                {
                    ApplicationId = oprV.ApplicationId,
                    Code = oprV.Code,
                    ID = oprV.ID,
                    IsActive = Convert.ToBoolean(oprV.IsActive),
                    IsDeleted = Convert.ToBoolean(oprV.IsDeleted),
                    Name = oprV.Name,
                    OperationType = (OperationType)oprV.OperationType,
                    OrderNo = oprV.OrderNo,
                    ParentId = oprV.ParentId,
                    Tag1 = oprV.Tag1,
                    Tag2 = oprV.Tag2,
                    Tag3 = oprV.Tag3,
                    Tag4 = oprV.Tag4
                };
            }));
        }

        private class OperationV
        {
            public int ID { get; set; }

            public int? ParentId { get; set; }

            public string Name { get; set; }

            public string Code { get; set; }

            public int ApplicationId { get; set; }

            public int OperationType { get; set; }

            public bool IsActive { get; set; }

            public bool IsDeleted { get; set; }

            public int OrderNo { get; set; }

            public string Tag1 { get; set; }

            public string Tag2 { get; set; }

            public string Tag3 { get; set; }

            public string Tag4 { get; set; }
        }
    }
}
