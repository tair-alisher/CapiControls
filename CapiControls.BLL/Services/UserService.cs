using AutoMapper;
using CapiControls.BLL.DTO.Account;
using CapiControls.BLL.Exceptions;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities.Account;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CapiControls.BLL.Services
{
    public class UserService : BaseService, IUserService
    {
        private readonly ILocalUnitOfWork _uow;

        public UserService(ILocalUnitOfWork uow, IMapper mapper) : base(mapper)
        {
            _uow = uow;
        }

        public void AddUser(RegisterDTO registerData, Guid[] roles)
        {
            var user = new UserDTO
            {
                Id = Guid.NewGuid(),
                Login = registerData.Login,
                UserName = registerData.UserName,
                Password = HashPassword(registerData.Password, registerData.Secret)
            };

            _uow.UserRepository.Add(Mapper.Map<UserDTO, User>(user));

            foreach (var roleId in roles)
                _uow.UserRepository.AddRoleToUser(roleId, user.Id);

            _uow.Commit();
        }

        private string HashPassword(string password, string secret)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(secret),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }

        public int CountUsers()
        {
            return _uow.UserRepository.CountAll();
        }

        public IEnumerable<RoleDTO> GetRoles()
        {
            var roles = _uow.UserRepository.GetRoles();
            return Mapper.Map<IEnumerable<Role>, IEnumerable<RoleDTO>>(roles);
        }

        public IEnumerable<UserDTO> GetUsers(int page, int pageSize)
        {
            var users = _uow.UserRepository.GetAll(page, pageSize);
            return Mapper.Map<IEnumerable<User>, IEnumerable<UserDTO>>(users);
        }

        public bool UserExists(string login)
        {
            return _uow.UserRepository.UserExists(login);
        }

        public UserDTO GetUser(LoginDTO loginData)
        {
            var user = _uow.UserRepository.GetUserByLoginAndPassword(
                loginData.Login,
                HashPassword(loginData.Password, loginData.Secret)
            );

            return Mapper.Map<User, UserDTO>(user);
        }

        public UserDTO GetUserById(Guid id)
        {
            return Mapper.Map<User, UserDTO>(_uow.UserRepository.Find(id));
        }

        public void UpdateUser(UserDTO user, Guid[] roles)
        {
            var userToUpdate = GetUserById(user.Id);
            userToUpdate.Login = user.Login;
            userToUpdate.UserName = user.UserName;

            _uow.UserRepository.Update(Mapper.Map<UserDTO, User>(userToUpdate));

            foreach (var roleId in roles)
                if (userToUpdate.Roles.Where(r => r.Id == roleId).Count() <= 0)
                    _uow.UserRepository.AddRoleToUser(roleId, userToUpdate.Id);

            foreach (var role in userToUpdate.Roles)
                if (!roles.Contains(role.Id))
                    _uow.UserRepository.RemoveRoleFromUser(role.Id, userToUpdate.Id);

            _uow.Commit();
        }

        public void DeleteUser(Guid id)
        {
            _uow.UserRepository.Delete(id);
            _uow.Commit();
        }

        public void UpdatePassword(string login, ChangePasswordDTO changePasswordData)
        {
            var user = _uow.UserRepository.GetUserByLoginAndPassword(
                login,
                HashPassword(changePasswordData.OldPassword, changePasswordData.Secret)
            );

            if (user == null)
                throw new WrongOldPasswordException();
            else
            {
                _uow.UserRepository.UpdatePassword(
                    user.Id,
                    HashPassword(changePasswordData.NewPassword, changePasswordData.Secret)
                );
                _uow.Commit();
            }
        }
    }
}
