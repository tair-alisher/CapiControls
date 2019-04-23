using CapiControls.BLL.DTO.Account;
using System;
using System.Collections.Generic;

namespace CapiControls.BLL.Interfaces
{
    public interface IUserService : IBaseService
    {
        int CountUsers();
        IEnumerable<UserDTO> GetUsers(int page, int pageSize);
        IEnumerable<RoleDTO> GetRoles();
        void AddUser(RegisterDTO registerData, Guid[] roles);
        bool UserExists(string login);
        UserDTO GetUserById(Guid id);
        UserDTO GetUser(LoginDTO loginData);
        void UpdateUser(UserDTO userData, Guid[] roles);
        void DeleteUser(Guid id);
        void UpdatePassword(string login, ChangePasswordDTO changePasswordData);
    }
}
