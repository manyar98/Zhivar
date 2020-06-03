//using Zhivar.DataLayer.Context;
//using Zhivar.DomainClasses;
//using Zhivar.ServiceLayer.Contracts;
//using Microsoft.AspNet.Identity.EntityFramework;
//using Zhivar.DomainClasses.Account;

//namespace Zhivar.ServiceLayer.Contracts.Account
//{
//    public class CustomUserStore :
//        UserStore<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>,
//        ICustomUserStore
//    {
//        //private readonly IDbSet<ApplicationUser> _myUserStore;
//        //public CustomUserStore(ApplicationDBContext context)
//        //    : base(context)
//        //{
//        //    //_myUserStore = context.Set<ApplicationUser>();
//        //}

//        //public override Task<ApplicationUser> FindByIdAsync(int userId)
//        //{
//        //   return Task.FromResult(_myUserStore.Find(userId));
//        //}
//    }
//}