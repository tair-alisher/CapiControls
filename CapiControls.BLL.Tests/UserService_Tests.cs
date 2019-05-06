using AutoMapper;
using CapiControls.BLL.DTO.Account;
using CapiControls.BLL.Interfaces;
using CapiControls.BLL.Services;
using CapiControls.Common.Mapping.Profiles;
using CapiControls.DAL.Entities.Account;
using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Interfaces.Units;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace CapiControls.BLL.Tests
{
    public class UserService_Tests
    {
        IUserService UserService;
        int commits = 0;
        List<Role> RolesData;
        List<User> UsersData;

        public UserService_Tests()
        {
            RolesData = new List<Role>()
            {
                new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    Title = "Role User"
                },
                new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Title = "Role Admin"
                }
            };

            UsersData = new List<User>()
            {
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "UserLogin_1",
                    UserName = "UserName_1",
                    Password = "UserPassword_1",
                    Roles = new List<Role>()
                    {
                        RolesData.First()
                    }
                },
                new User()
                {
                    Id = Guid.NewGuid(),
                    Login = "UserLogin_2",
                    UserName = "UserName_2",
                    Password = "UserPassword_2",
                    Roles = new List<Role>()
                }
            };

            var userRepository = new Mock<IUserRepository>();
            userRepository
                .Setup(r => r.Add(It.IsAny<User>()))
                .Callback((User user) => UsersData.Add(user));
            userRepository
                .Setup(r => r.Find(It.IsAny<Guid>()))
                .Returns((Guid id) => UsersData.Where(u => u.Id == id).First());
            userRepository
                .Setup(r => r.CountAll())
                .Returns(() => UsersData.Count());
            userRepository
                .Setup(r => r.AddRoleToUser(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Callback((Guid roleId, Guid userId) =>
                {
                    var role = RolesData.Find(r => r.Id == roleId);
                    UsersData.Where(u => u.Id == userId).First().Roles.Add(role);
                });
            userRepository
                .Setup(r => r.GetRoles())
                .Returns(() => RolesData);
            userRepository
                .Setup(r => r.GetAll(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(
                    (int page, int pageSize) => UsersData.Skip(pageSize * page).Take(pageSize)
                );
            userRepository
                .Setup(r => r.UserExists(It.IsAny<string>()))
                .Returns((string login) => UsersData.Where(u => u.Login == login).Count() > 0);
            userRepository
                .Setup(r => r.GetUserByLoginAndPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string login, string password) => UsersData.Where(u => u.Login == login && u.Password == password).First());
            userRepository
                .Setup(r => r.Update(It.IsAny<User>()))
                .Callback((User item) =>
                {
                    var user = UsersData.Where(u => u.Id == item.Id).First();
                    user.Login = item.Login;
                    user.UserName = item.UserName;
                });
            userRepository
                .Setup(r => r.RemoveRoleFromUser(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .Callback((Guid roleId, Guid userId) =>
                {
                    UsersData.Where(u => u.Id == userId).First().Roles.Remove(RolesData.Where(r => r.Id == roleId).First());
                });
            userRepository
                .Setup(r => r.UpdatePassword(It.IsAny<Guid>(), It.IsAny<string>()))
                .Callback((Guid userId, string password) =>
                {
                    UsersData.Where(u => u.Id == userId).First().Password = password;
                });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new BLLProfile());
            }).CreateMapper();

            var mockUnitOfWork = new Mock<ILocalUnitOfWork>();
            mockUnitOfWork
                .Setup(uow => uow.UserRepository)
                .Returns(userRepository.Object);
            mockUnitOfWork
                .Setup(uow => uow.Commit())
                .Callback(() => commits++);

            UserService = new UserService(mockUnitOfWork.Object, mapper);
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

        [Fact]
        public void AddUser_WithoutRoles_AddsUser()
        {
            // Arrange
            int commitsBeforeAct = this.commits;
            int usersAmoutBeforeAct = UsersData.Count();
            var registerData = new RegisterDTO()
            {
                Login = $"NewUserLogin{DateTime.Now}",
                UserName = $"NewUserName{DateTime.Now}",
                Password = $"NewUserPassword{DateTime.Now}",
                Secret = "Secret"
            };

            // Act
            UserService.AddUser(registerData, new Guid[0]);

            // Assert
            Assert.True(this.commits > commitsBeforeAct);
            Assert.True(UsersData.Count() > usersAmoutBeforeAct);
            Assert.True(UsersData.Where(u => u.Login == registerData.Login).First().Roles.Count() == 0);
        }

        [Fact]
        public void AddUser_HashesPassword()
        {
            // Arrange
            var registerData = new RegisterDTO()
            {
                Login = $"NewUserLogin{DateTime.Now}",
                UserName = $"NewUserName{DateTime.Now}",
                Password = $"NewUserPassword{DateTime.Now}",
                Secret = "Secret"
            };

            // Act
            UserService.AddUser(registerData, new Guid[0]);

            // Assert
            var hashedPassword = UsersData.Where(u => u.Login == registerData.Login).First().Password;
            Assert.NotEqual(registerData.Password, hashedPassword);
        }

        [Fact]
        public void AddUser_WithRoles()
        {
            // Arrange
            int commitsBeforeAct = this.commits;
            var userRoleId = RolesData.Where(r => r.Name == "User").First().Id;
            var registerData = new RegisterDTO()
            {
                Login = $"NewUserLogin{DateTime.Now}",
                UserName = $"NewUserName{DateTime.Now}",
                Password = $"NewUserPassword{DateTime.Now}",
                Secret = "Secret"
            };

            // Act
            UserService.AddUser(registerData, new Guid[1] { userRoleId });

            // Assert
            Assert.True(this.commits > commitsBeforeAct);
            Assert.True(UsersData.Where(u => u.Login == registerData.Login).First().Roles.Count() == 1);
            Assert.True(UsersData.Where(u => u.Login == registerData.Login).First().Roles.First().Name == "User");
        }

        [Fact]
        public void CountUsers_ReturnsUsersAmount()
        {
            // Arrange
            int trueUsersAmount = UsersData.Count();

            // Act
            int usersAmount = UserService.CountUsers();

            // Assert
            Assert.Equal(trueUsersAmount, usersAmount);
        }

        [Fact]
        public void GetRoles_ReturnsRolesList()
        {
            // Arrange
            var rolesAmount = RolesData.Count();

            // Act
            var roles = UserService.GetRoles();

            // Assert
            Assert.Equal(rolesAmount, roles.Count());
            Assert.IsType<List<RoleDTO>>(roles);
        }

        [Fact]
        public void GetUsers_Page0PageSize10()
        {
            // Act
            var users = UserService.GetUsers(0, 10);

            // Assert
            Assert.True(users.Count() <= 10);
            Assert.True(users.Count() > 0);
        }

        [Fact]
        public void GetUsers_Page10000PageSize10()
        {
            // Act
            var users = UserService.GetUsers(10000, 10);

            // Assert
            Assert.True(users.Count() <= 0);
        }

        [Fact]
        public void UserExists_ReturnsTrue_IfUserExists()
        {
            // Arrange
            string existedUserLogin = UsersData.First().Login;

            // Act
            var isExists = UserService.UserExists(existedUserLogin);

            // Assert
            Assert.IsType<bool>(isExists);
            Assert.True(isExists);
        }

        [Fact]
        public void UserExists_ReturnsFalse_IfUserDoesNotExist()
        {
            // Arrange
            string nonExistentUserLogin = $"userLogin{DateTime.Now}";

            // Act
            var isExists = UserService.UserExists(nonExistentUserLogin);

            // Assert
            Assert.False(isExists);
        }

        [Fact]
        public void GetUser_ReturnsUserDTO()
        {
            // Arrange
            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                Login = $"NewUserLogin{DateTime.Now}",
                UserName = $"NewUserName{DateTime.Now}",
                Password = HashPassword("NewUserPassword", "Secret")
            };
            UsersData.Add(newUser);

            var loginData = new LoginDTO()
            {
                Login = $"NewUserLogin{DateTime.Now}",
                Password = "NewUserPassword",
                Secret = "Secret"
            };

            // Act
            var user = UserService.GetUser(loginData);

            // Assert
            Assert.IsType<UserDTO>(user);
            Assert.Equal(newUser.Id, user.Id);
            Assert.Equal(newUser.Login, user.Login);
        }

        [Fact]
        public void GetUserById_ReturnsUserDTO()
        {
            // Arrange
            var expectedUser = UsersData.Last();

            // Act
            var actualUser = UserService.GetUserById(expectedUser.Id);

            // Assert
            Assert.IsType<UserDTO>(actualUser);
            Assert.Equal(expectedUser.Id, actualUser.Id);
        }

        [Fact]
        public void UpdateUser_UpdatesUserLoginAndName()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateUser_DoesNotUpdatePassword()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateUser_AddsRoles_IfUserDoesNotHave()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void UpdateUser_RemovesRole_IfItIsNotInList()
        {

        }
    }
}
