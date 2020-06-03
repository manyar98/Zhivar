using System.Data.Entity;
//using Zhivar.Access.MapConfiguration.Security;
//using OMF.EntityFramework.Common.Mappings;
using OMF.EntityFramework.Ef6;
using OMF.Security.Model.MappingConfiguration;
using OMF.Workflow.Model.MapConfiguration;
//using Zhivar.Access.MapConfiguration.Security;
using Zhivar.DomainClasses.BaseInfo;
using Zhivar.DomainClasses.Accunting;
using Zhivar.DomainClasses.Accounting;
using Zhivar.DomainClasses.Common;
using Zhivar.DomainClasses.Contract;
using System.Data.Entity.ModelConfiguration;
using OMF.Security.Model;
using OMF.EntityFramework.Common;
using Zhivar.DomainClasses.Test;
using Zhivar.DataLayer.MapConfiguration;
using Zhivar.DataLayer.MapConfiguration.Accounting;
using Zhivar.DataLayer.MapConfiguration.Common;
using Zhivar.DataLayer.MapConfiguration.BaseInfo;
using Zhivar.Access.MapConfiguration.Security;
using Zhivar.DataLayer.MapConfiguration.Contract;

namespace Zhivar.DataLayer.Context
{
    public class ZhivarContext : DataContext
    {
        public ZhivarContext()
        {
            Database.Log = Logger.Log;
            Database.SetInitializer(new NullDatabaseInitializer<ZhivarContext>());
            Configuration.ProxyCreationEnabled = false;
            Configuration.UseDatabaseNullSemantics = true;
            Configuration.LazyLoadingEnabled = false;
        }

        //public DbSet<ForTest> ForTests { get; set; }
        //public DbSet<UserInfo> Users { get; set; }

        //public DbSet<Operation> Operations { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var factory = System.Data.Common.DbProviderFactories.GetFactory(this.Database.Connection);
      

            if (factory is System.Data.SqlClient.SqlClientFactory)
            {
                System.Data.SqlClient.SqlClientFactory sqlFactory = (System.Data.SqlClient.SqlClientFactory)factory;
                var connectionStringBuilder = sqlFactory.CreateConnectionStringBuilder() as System.Data.SqlClient.SqlConnectionStringBuilder;
                connectionStringBuilder.ConnectionString = "Data Source= 45.159.196.12,1437;Initial Catalog=smbtir_ZhivarDB;Persist Security Info=True;User ID=smbtir_omid;Password=M7a1_4bg";// this.Database.Connection.ConnectionString;
 
            }

            #region Infrastructure

            //modelBuilder.Configurations.Add<UserInfo>((EntityTypeConfiguration<UserInfo>)new UserInfoConfig());
            modelBuilder.Configurations.Add<Operation>((EntityTypeConfiguration<Operation>)new OperationConfig());

            // modelBuilder.Configurations.Add(new ContentInfoConfig());
            modelBuilder.Configurations.Add(new UserOperationConfig());
            //modelBuilder.Configurations.Add(new OperationConfig());
            modelBuilder.Configurations.Add(new UserInfoConfig());
            modelBuilder.Configurations.Add(new RoleBaseConfig());
            modelBuilder.Configurations.Add(new RoleConfig());
            //modelBuilder.Configurations.Add(new OrganizationConfig());
            //modelBuilder.Configurations.Add(new OrganizationUnitChartConfig());
            modelBuilder.Configurations.Add(new PositionConfig());
            modelBuilder.Configurations.Add(new RoleOperationConfig());
            modelBuilder.Configurations.Add<UserRole>((EntityTypeConfiguration<UserRole>)new UserRoleConfig());
           // modelBuilder.Configurations.Add(new UserRoleConfig());



            modelBuilder.Configurations.Add(new WorkflowInfoConfig());
            modelBuilder.Configurations.Add(new WFActionTypeConfig());
            modelBuilder.Configurations.Add(new WorkflowInstanceConfig());
            modelBuilder.Configurations.Add(new WorkflowInstanceStateConfig());
            modelBuilder.Configurations.Add(new WorkflowStepActionConfig());
            modelBuilder.Configurations.Add(new WorkflowStepConfig());

            #endregion


            modelBuilder.Configurations.Add(new LoggableEntityNameAndIDTypeConfig());
            modelBuilder.Configurations.Add(new LoggableEntityNameTypeConfig());
            modelBuilder.Configurations.Add(new LoggableEntityIDTypeConfig());

        

            modelBuilder.Entity<BaseAccount>().ToTable("BaseAccounts");

            modelBuilder.Configurations.Add(new PersonelConfig());
            modelBuilder.Configurations.Add(new PersonConfig());
            modelBuilder.Configurations.Add(new BankConfig());
            modelBuilder.Configurations.Add(new CashConfig());
            modelBuilder.Configurations.Add(new AccountConfig());
            modelBuilder.Configurations.Add(new InvoiceConfig());
            modelBuilder.Configurations.Add(new InvoiceItemConfig());
            modelBuilder.Configurations.Add(new PayRecevieConfig());
            modelBuilder.Configurations.Add(new ItemGroupConfig());
            modelBuilder.Configurations.Add(new ItemConfig());
            modelBuilder.Configurations.Add(new ContactConfig());
            modelBuilder.Configurations.Add(new DocumentConfig());
            modelBuilder.Configurations.Add(new TransactionConfig());
            modelBuilder.Configurations.Add(new ChequeConfig());
            modelBuilder.Configurations.Add(new ChequeBankConfig());
            modelBuilder.Configurations.Add(new TransferMoneyConfig());
            modelBuilder.Configurations.Add(new ShareholderConfig());
            modelBuilder.Configurations.Add(new CostConfig());
            modelBuilder.Configurations.Add(new CostItemConfig());
   
            modelBuilder.Configurations.Add(new FinanYearConfig());
            modelBuilder.Configurations.Add(new BussinessConfig());

            modelBuilder.Configurations.Add(new NoeEjareConfig());
            modelBuilder.Configurations.Add(new SazeConfig());
            modelBuilder.Configurations.Add(new SazeImageConfig());
            modelBuilder.Configurations.Add(new GoroheSazeConfig());
            modelBuilder.Configurations.Add(new NoeSazeConfig());
            modelBuilder.Configurations.Add(new ContractConfig());
            modelBuilder.Configurations.Add(new Contract_SazeConfig());
            modelBuilder.Configurations.Add(new VahedTolConfig());
            modelBuilder.Configurations.Add(new NoeChapConfig());

            modelBuilder.Configurations.Add(new Contract_Saze_BazareabConfig());
            modelBuilder.Configurations.Add(new Contract_Saze_ChapkhaneConfig());
            modelBuilder.Configurations.Add(new Contract_Saze_NasabConfig());
            modelBuilder.Configurations.Add(new Contract_Saze_TarahConfig());
            modelBuilder.Configurations.Add(new ContractSazeImagesConfig());
            modelBuilder.Configurations.Add(new Contract_PayReceviesConfig());
            modelBuilder.Configurations.Add(new Contract_DetailPayReceviesConfig());

            modelBuilder.Configurations.Add(new MapItemSazeConfig());

            modelBuilder.Configurations.Add(new DocTypeConfig());
            modelBuilder.Configurations.Add(new MadarekPayvastConfig());
            // modelBuilder.Configurations.Add(new TasvirBlobConfig());

            //modelBuilder.Configurations.Add(new ZhivarUserInfoConfig());
            modelBuilder.Configurations.Add(new ReservationConfig());
            modelBuilder.Configurations.Add(new Reservation_DetailConfig());

            modelBuilder.Configurations.Add(new ContractStopsConfig());
            modelBuilder.Configurations.Add(new ContractStopDetailsConfig());


            modelBuilder.Configurations.Add(new BaseColorConfig());
            modelBuilder.Configurations.Add(new Organ_ColorConfig());

            modelBuilder.Configurations.Add(new ItemUnitConfig());
            
        }
    }

    internal static class Logger
    {
        public static void Log(string msg)
        {
        }
    }
}