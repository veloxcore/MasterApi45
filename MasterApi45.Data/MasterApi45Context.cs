using MasterApi45.Model.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Data
{
    public class MasterApi45Context : DbContext
    {
        /// <summary>
        ///Reason for enabling proxy creation. 
        ///Proxies are necessary for two features:
        ///
        ///Lazy loading - navigation properties are loaded once accessed first time
        ///Dynamic change tracking - if you modify any property in the entity the context is notified about this change and set the state of the entity.If dynamic change tracking is not used, context must use snapshot change tracking which means discovery all changes before saving takes place = exploring all properties even if they were not changed.
        ///Both these techniques have other requirements:
        ///
        ///Lazy loading - all navigation properties in the entity must be virtual. Lazy loading must be enabled.
        ///Dynamic change tracking - all mapped properties must be virtual.
        /// </summary>
        public MasterApi45Context()
            :base("name=ApplicationDbContext")
        {
            this.Configuration.ProxyCreationEnabled = true;
        }
        public virtual DbSet<Test> Tests { get; set; }

    }
}
