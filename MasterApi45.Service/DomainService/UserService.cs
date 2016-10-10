using MasterApi45.Data.Repository;
using MasterApi45.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterApi45.Service.DomainService
{
    public interface IUserService
    {
        User Find(string userName, string password);
    }
    public class UserService : IUserService
    {
        IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public User Find(string userName, string password)
        {
            return _userRepository.Find(userName, password);
        }
    }
}
