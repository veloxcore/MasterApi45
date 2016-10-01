using MasterApi45.Data.Infrastracture;
using MasterApi45.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Data.Repository
{
    public interface ITestRepository : IRepository<Test>
    {

    }

    public class TestRepository : Repository<Test>, ITestRepository
    {
        public TestRepository(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {

        }

        protected override object[] GetKey(Test enity)
        {
            throw new NotImplementedException();
        }
    }
}
