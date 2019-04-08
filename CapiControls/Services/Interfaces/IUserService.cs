using CapiControls.Models.Local.Account;
using CapiControls.ViewModels;
using System;
using System.Collections.Generic;

namespace CapiControls.Services.Interfaces
{
    public interface IUserService
    {
        int CountUsers();
        IEnumerable<User> GetUsers(int pageSize, int page);
        IEnumerable<Role> GetRoles();
        void AddUser(RegisterViewModel userVM, Guid[] roles);
        bool UserExists(string login);
        User GetUserById(Guid id);
        User GetUser(LoginViewModel model);
        void UpdateUser(User user, Guid[] roles);
        void DeleteUser(Guid id);
    }
}
