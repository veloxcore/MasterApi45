using MasterApi45.Data.Infrastracture;
using MasterApi45.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Data.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        User Find(string userName, string password);

    }
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }
        public User Find(string userName, string password)
        {
            return dbSet.Where(o => o.UserName.Equals(userName) && o.Password.Equals(password)).FirstOrDefault();
        }

        protected override object[] GetKey(User entity)
        {
            return new object[] { entity.ID };
        }
    }
}
