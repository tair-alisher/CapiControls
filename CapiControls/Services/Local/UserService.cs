using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CapiControls.Data.Interfaces;
using CapiControls.Exceptions;
using CapiControls.Models.Local.Account;
using CapiControls.Services.Interfaces;
using CapiControls.ViewModels;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Configuration;

namespace CapiControls.Services.Local
{
    public class UserService : IUserService
    {
        private readonly IUserRepository UserRepository;
        private readonly IConfiguration Configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            UserRepository = userRepository;
            Configuration = configuration;
        }

        public void AddUser(RegisterViewModel userVM, Guid[] roles)
        {
            var user = new User()
            {
                Id = Guid.NewGuid(),
                Login = userVM.Login,
                UserName = userVM.UserName,
                Password = HashPassword(userVM.Password)
            };

            UserRepository.Create(user);

            foreach (Guid roleId in roles)
                UserRepository.AddRoleToUser(roleId, user.Id);
        }

        public int CountUsers()
        {
            return UserRepository.CountAll();
        }

        public IEnumerable<Role> GetRoles()
        {
            return UserRepository.GetRoles();
        }

        public IEnumerable<User> GetUsers(int pageSize, int page)
        {
            return UserRepository.GetAll(pageSize, page);
        }

        public bool UserExists(string login)
        {
            return UserRepository.UserExists(login);
        }

        public User GetUser(LoginViewModel model)
        {
            model.Password = HashPassword(model.Password);
            User user = UserRepository.GetUserByLoginAndPassword(model.Login, model.Password);

            return user;
        }

        private string HashPassword(string password)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(Configuration.GetValue<string>("Secrets:Salt")),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public User GetUserById(Guid id)
        {
            return UserRepository.Get(id);
        }

        public void UpdateUser(User user, Guid[] roles)
        {
            var userToUpdate = GetUserById(user.Id);
            userToUpdate.Login = user.Login;
            userToUpdate.UserName = user.UserName;

            UserRepository.Update(userToUpdate);

            foreach (Guid roleId in roles)
                if (userToUpdate.Roles.Where(r => r.Id == roleId).Count() <= 0)
                    UserRepository.AddRoleToUser(roleId, userToUpdate.Id);

            foreach (var role in userToUpdate.Roles)
                if (!roles.Contains(role.Id))
                    UserRepository.RemoveRoleFromUser(role.Id, userToUpdate.Id);
        }

        public void DeleteUser(Guid id)
        {
            UserRepository.Delete(id);
        }

        public void UpdatePassword(string login, ChangePasswordViewModel model)
        {
            string oldPasswordHash = HashPassword(model.OldPassword);
            var user = UserRepository.GetUserByLoginAndPassword(login, oldPasswordHash);

            if (user == null)
                throw new WrongOldPasswordException();
            else
            {
                string newPasswordHash = HashPassword(model.NewPassword);
                UserRepository.UpdatePassword(user.Id, newPasswordHash);
            }
        }
    }
}
