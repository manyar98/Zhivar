//using System;
//using System.Data.Entity;
//using System.Data.Entity.Infrastructure;
//using System.Threading.Tasks;
//using Zhivar.DomainClasses.Accunting;
//using Zhivar.DomainClasses.Account;
//using Zhivar.DomainClasses.BaseInfo;
//using Zhivar.DomainClasses.Common;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Zhivar.DomainClasses.Contract;
//using Zhivar.DomainClasses.Accounting;
//using Zhivar.Access.MapConfiguration.Security;

//namespace Zhivar.DataLayer.Context
//{
//    public class ApplicationDBContext :IdentityDbContext<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>,IUnitOfWork
//    {

//        public DbSet<Saze> Sazes { get; set; }
//        public DbSet<SazeImage> SazeImages { get; set; }
//        public DbSet<GoroheSaze> GorheSazes { get; set; }
//        public DbSet<NoeSaze> NoeSazes { get; set; }
//        //public DbSet<Person> Pepole { get; set; }
//        //public DbSet<TarfeHesab> TarfeHesabs { get; set; }
//        public DbSet<Bank> Banks { get; set; }
//        public DbSet<Personel> Personels { get; set; }

//        public DbSet<Contract> Contracts { get; set; }
//        public DbSet<Contract_Saze> Contract_Sazes { get; set; }
//        public DbSet<Contract_Saze_Bazareab> Contarct_Saze_Bazareab { get; set; }
//        public DbSet<Contract_Saze_Chapkhane> Contract_Saze_Chapkhane { get; set; }
//        public DbSet<Contract_Saze_Nasab> Contract_Saze_Nasab { get; set; }
//        public DbSet<Contract_Saze_Tarah> Contract_Saze_Tarah { get; set; }

//        public DbSet<BaseAccount> BaseAccounts { get; set; }
//        public DbSet<Cash> Cashs { get; set; }
//        public DbSet<Account> Accounts { get; set; }
//        public DbSet<Invoice> Invoices { get; set; }
//        public DbSet<InvoiceItem> InvoiceItems { get; set; }
//        //public DbSet<Sahamdaran> Sahamdaran { get; set; }
//        public DbSet<PayRecevie> PayRecevies { get; set; }
//        public DbSet<FinanYear> FinanYears { get; set; }
//        public DbSet<ItemGroup> ItemGroups { get; set; }
//        public DbSet<Item> Items { get; set; }
//        public DbSet<VahedTol> VahedTols { get; set; }
//        public DbSet<NoeChap> NoeChaps { get; set; }
//        public DbSet<Location> Locations { get; set; }
//        public DbSet<Contact> Contacts { get; set; }
//        public DbSet<NoeEjare> NoeEjares { get; set; }
//        public DbSet<Document> Documents { get; set; }
//        public DbSet<Transaction> Transactions { get; set; }
//        public DbSet<Cheque> Cheques { get; set; }
//        public DbSet<ChequeBank> ChequeBanks { get; set; }
//        public DbSet<TransferMoney> TransferMoneies { get; set; }
//        public DbSet<Shareholder> Shareholders { get; set; }
//        public DbSet<Bussiness> Bussinesses { get; set; }
//        public DbSet<Cost> Costs { get; set; }
//        public DbSet<CostItem> CostItems { get; set; }

//        public DbSet<OMF.Security.Model.Operation> Operation { get; set; }
//        public ApplicationDBContext()
//            : base("ApplicationDbContext")
//        {
//            //this.Database.Log = data => System.Diagnostics.Debug.WriteLine(data);
//            this.Configuration.ProxyCreationEnabled = false;
//            this.Configuration.LazyLoadingEnabled = false;
//            this.Configuration.AutoDetectChangesEnabled = false;

//        }


//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Ignore<Entity,IActivityLoggable>();

//            base.OnModelCreating(modelBuilder);
//            modelBuilder.Entity<ApplicationUser>().ToTable("Users");
//            modelBuilder.Entity<CustomRole>().ToTable("Roles");
//            modelBuilder.Entity<CustomUserClaim>().ToTable("UserClaims");
//            modelBuilder.Entity<CustomUserRole>().ToTable("UserRoles");
//            modelBuilder.Entity<CustomUserLogin>().ToTable("UserLogins");
//            modelBuilder.Configurations.Add(new ZhivarUserInfoConfig());
//            //modelBuilder.Entity<Saze>().Property(a => a.X).HasPrecision(18, 9);
//            //modelBuilder.Entity<Saze>().Property(a => a.Y).HasPrecision(18, 9);

//            //modelBuilder.Entity<Contract_Saze>().HasOptional(t => t.Contarct_Saze_Bazareabs).WithRequired(t => t.Contract_Saze).WillCascadeOnDelete(true);
//            //modelBuilder.Entity<Contract_Saze>().HasOptional(t => t.Contract_Saze_Tarah).WithRequired(t => t.Contract_Saze).WillCascadeOnDelete(true);
//            //modelBuilder.Entity<Contract_Saze>().HasOptional(t => t.Contract_Saze_Nasab).WithRequired(t => t.Contract_Saze).WillCascadeOnDelete(true);
//            //modelBuilder.Entity<Contract_Saze>().HasOptional(t => t.Contract_Saze_Chapkhane).WithRequired(t => t.Contract_Saze).WillCascadeOnDelete(true);


//        }

//        public new IDbSet<TEntity> Set<TEntity>() where TEntity : class
//        {
//            return base.Set<TEntity>();
//        }

//        public int SaveAllChanges()
//        {
//            return base.SaveChanges();
//        }

//        public async Task<int> SaveAllChangesAsync()
//        {
        
//             return await base.SaveChangesAsync();
//        }
//        public void MarkAsAdded<TEntity>(TEntity entity) where TEntity : class
//        {
//            Entry(entity).State = EntityState.Added;
//        }

//        public void MarkAsChanged<TEntity>(TEntity entity) where TEntity : class
//        {
//            Entry(entity).State = EntityState.Modified;
//        }

//        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class
//        {
//            Entry(entity).State = EntityState.Deleted;
//        }
//    }
//}
