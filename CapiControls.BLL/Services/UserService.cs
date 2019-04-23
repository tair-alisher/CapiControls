using AutoMapper;
using CapiControls.BLL.DTO.Account;
using CapiControls.BLL.Interfaces;
using CapiControls.DAL.Entities.Account;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Text;

namespace CapiControls.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly ILocalUnitOfWork _uow;

        public UserService(ILocalUnitOfWork uow)
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

        public string HashPassword(string password, string secret)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(secret),
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
