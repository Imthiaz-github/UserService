using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Core.Models;

namespace UserService.Core.Interfaces.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetUsers();
        Task<User> GetUserById(int id);
        Task<bool> DeleteUser(int id);
        Task<User> CreateUser(User user);
        Task<bool> UserExists(string userName);
        Task<bool> UpdateUser(int id, User user);
    }
}
