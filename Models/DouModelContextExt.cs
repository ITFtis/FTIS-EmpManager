using System;
using System.Data.Entity;
using System.Linq;

namespace DouImp.Models
{
    public class DouModelContextExt : Dou.Models.ModelContextBase<User, Role>
    {
        public DouModelContextExt() : base("name=DouModelContextExt")
        {
            Database.SetInitializer<DouModelContextExt>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<F22cmmEmpData>().HasOptional(s=>s.Seat);
            base.OnModelCreating(modelBuilder);
        }

    }

    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}
}