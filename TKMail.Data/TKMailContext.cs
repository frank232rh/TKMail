using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKMail.Data
{
    public class TKMailContext : DbContext
    {
        public TKMailContext() : base("name=MailConnectionString")
        {

        }
        public DbSet Set(string name)
        {
            return base.Set(Type.GetType(name));
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<TKMailContext>(null);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
