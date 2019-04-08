using CapiControls.Models.Local.Account;
using System;
using System.Collections.Generic;

namespace CapiControls.Data.Interfaces
{
    public interface IUserRepository : IPaginatedRepository<User>
    {
        List<Role> GetRoles();
        void AddRoleToUser(Guid roleId, Guid userId);
        void RemoveRoleFromUser(Guid roleId, Guid userId);
        bool UserExists(string login);
        User GetUserByLoginAndPassword(string login, string passwordHash);
        void UpdatePassword(Guid userId, string newPassword);
    }
}
