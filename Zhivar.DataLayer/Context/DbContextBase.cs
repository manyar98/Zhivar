using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zhivar.DomainClasses.Common;

namespace Zhivar.DataLayer.Context
{
    public class DbContextBase : DbContext
    {
        public override int SaveChanges()
        {
          //  applyCorrectYeKe();
            //auditFields();
            return base.SaveChanges();
        }

        //private void applyCorrectYeKe()
        //{
        //    //پیدا کردن موجودیت‌های تغییر کرده
        //    var changedEntities = this.ChangeTracker
        //                              .Entries()
        //                              .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

        //    foreach (var item in changedEntities)
        //    {
        //        if (item.Entity == null) continue;

        //        //یافتن خواص قابل تنظیم و رشته‌ای این موجودیت‌ها
        //        var propertyInfos = item.Entity.GetType().GetProperties(
        //            BindingFlags.Public | BindingFlags.Instance
        //            ).Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

        //        var pr = new PropertyReflector();

        //        //اعمال یکپارچگی نهایی
        //        foreach (var propertyInfo in propertyInfos)
        //        {
        //            var propName = propertyInfo.Name;
        //            var val = pr.GetValue(item.Entity, propName);
        //            if (val != null)
        //            {
        //                var newVal = val.ToString().Replace("ی", "ی").Replace("ک", "ک");
        //                if (newVal == val.ToString()) continue;
        //                pr.SetValue(item.Entity, propName, newVal);
        //            }
        //        }
        //    }
        //}

        //private void auditFields()
        //{
        //    // var auditUser = User.Identity.Name; // in web apps
        //    var auditDate = DateTime.Now;
        //    foreach (var entry in this.ChangeTracker.Entries<BaseEntity>())
        //    {
        //        // Note: You must add a reference to assembly : System.Data.Entity
        //        switch (entry.State)
        //        {
        //            case EntityState.Added:
        //               // entry.Entity.CreatedOn = auditDate;
        //              //  entry.Entity.ModifiedOn = auditDate;
        //              //  entry.Entity.CreatedBy = "auditUser";
        //               // entry.Entity.ModifiedBy = "auditUser";
        //                break;

        //            case EntityState.Modified:
        //              //  entry.Entity.ModifiedOn = auditDate;
        //             //   entry.Entity.ModifiedBy = "auditUser";
        //                break;
        //        }
        //    }
        //}

    }
}

