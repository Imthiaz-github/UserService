using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Core.Interfaces.Repositories;
using UserService.Core.Models;
using UserService.Infrastructure.Context;

namespace UserService.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _userDbContext;
        private readonly IMapper _mapper;
        public UserRepository(UserDbContext userDbContext, IMapper mapper)
        {
            _userDbContext = userDbContext ?? throw new ArgumentNullException(nameof(userDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<User> CreateUser(User user)
        {
           var dbUser = _mapper.Map<Entities.User>(user);
           await _userDbContext.Users.AddAsync(dbUser);
           await _userDbContext.SaveChangesAsync();
            user.UserId = dbUser.UserId;
            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _userDbContext.Users.FindAsync(id);
            if(user != null)
            {
                _userDbContext.Users.Remove(user);
                await _userDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<User> GetUserById(int id)
        {
            var dbUser = await _userDbContext.Users.FindAsync(id);
            if (dbUser != null)
                return _mapper.Map<User>(dbUser);
            return null;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var dbUsers = await _userDbContext.Users.OrderBy(x=>x.UserId).ToListAsync().ConfigureAwait(false);
            if (dbUsers.Count == 0)
                return null;
            return _mapper.Map<IEnumerable<User>>(dbUsers);
        }

        public async Task<bool> UpdateUser(int id, User user)
        {
            var dbUser = await _userDbContext.Users.FindAsync(id);
            if (dbUser == null || dbUser.UserId != id)
            {
                return false;
            }
            if(user !=null)
            {
                dbUser.UserName = user.UserName;
                _userDbContext.Users.Update(dbUser);
                await _userDbContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> UserExists(string userName)
        {
            return await _userDbContext.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        }
    }
}
