using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Data.Infrastracture
{
    public interface IUnitOfWork: IDisposable
    {
        void CommitTransaction();
        void StartTransaction();
        void RollBackTransaction();
        void SaveChanges();
    }
}
