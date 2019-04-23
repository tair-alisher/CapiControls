using CapiControls.DAL.Entities.Account;
using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Repositories.Base;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapiControls.DAL.Repositories
{
    internal class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IDbTransaction transaction) : base(transaction) { }

        public void Add(User item)
        {
            Connection.Execute(
                @"INSERT INTO main.users (id, login, username, password)
                VALUES(@Id, @Login, @UserName, @Password)",
                param: item,
                transaction: Transaction
            );
        }

        public int CountAll()
        {
            return Connection.QueryFirstOrDefault<int>(
                "SELECT COUNT(id) FROM main.users",
                transaction: Transaction
            );
        }

        public void Delete(Guid id)
        {
            Connection.Execute(
                "DELETE FROM main.userroles WHERE user_id = @id",
                param: new { id },
                transaction: Transaction
            );

            Connection.Execute(
                "DELETE from main.users WHERE id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public void Delete(User item)
        {
            Delete(item.Id);
        }

        public User Find(Guid id)
        {
            var rawUserData = Connection.Query<RawUserData>(
                @"SELECT
                    user_t.id as Id,
                    user_t.login as Login,
                    user_t.username as UserName,
                    role_t.id as RoleId,
                    role_t.name as RoleName,
                    role_t.title as RoleTitle
                FROM main.users as user_t
                JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                JOIN main.roles as role_t on userrole_t.role_id = role_t.id
                WHERE user_t.id = @id",
                param: new { id },
                transaction: Transaction
            ).ToList();

            return CollectUser(rawUserData);
        }

        private User CollectUser(List<RawUserData> rawUserData)
        {
            User user = null;
            var firstRow = rawUserData.FirstOrDefault();
            if (firstRow != null)
            {
                user = new User()
                {
                    Id = firstRow.Id,
                    Login = firstRow.Login,
                    UserName = firstRow.UserName
                };

                foreach (var row in rawUserData)
                {
                    user.Roles.Add(new Role()
                    {
                        Id = row.RoleId,
                        Name = row.RoleName,
                        Title = row.RoleTitle
                    });
                }
            }

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            var rawUserData = Connection.Query<RawUserData>(
                @"SELECT
                    user_t.id as Id,
                    user_t.login as Login,
                    user_t.username as UserName,
                    user_t.password as Password,
                    role_t.id as RoleId,
                    role_t.name as RoleName,
                    role_t.title as RoleTitle
                FROM main.users as user_t
                JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                JOIN main.roles as role_t on userrole_t.role_id = role_t.id",
                transaction: Transaction
            ).ToList();

            return CollectUsers(rawUserData);
        }

        private IEnumerable<User> CollectUsers(List<RawUserData> rawUserData)
        {
            var users = new List<User>();
            foreach (var rawUser in rawUserData)
            {
                if (users.Where(u => u.Id == rawUser.Id).Count() <= 0)
                {
                    users.Add(new User
                    {
                        Id = rawUser.Id,
                        Login = rawUser.Login,
                        UserName = rawUser.UserName
                    });
                }
            }

            foreach (var user in users)
            {
                var userRoles = rawUserData.Where(u => u.Id == user.Id).ToList();
                foreach (var role in userRoles)
                {
                    user.Roles.Add(new Role
                    {
                        Id = role.RoleId,
                        Name = role.RoleName,
                        Title = role.RoleTitle
                    });
                }
            }

            return users;
        }

        public IEnumerable<User> GetAll(int pageSize, int page)
        {
            int offset = (page - 1) * pageSize;
            var rawUserData = Connection.Query<RawUserData>(
                @"SELECT
                    user_t.id as Id,
                    user_t.login as Login,
                    user_t.username as UserName,
                    role_t.id as RoleId,
                    role_t.name as RoleName,
                    role_t.title as RoleTitle
                FROM main.users as user_t
                JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                JOIN main.roles as role_t on userrole_t.role_id = role_t.id
                OFFSET @offset
                LIMIT @pageSize",
                param: new { offset, pageSize },
                transaction: Transaction
            ).ToList();

            return CollectUsers(rawUserData);
        }

        public User GetUserByLoginAndPassword(string login, string passwordHash)
        {
            var rawUserData = Connection.Query<RawUserData>(
                @"SELECT
                    user_t.id as Id,
                    user_t.login as Login,
                    user_t.username as UserName,
                    role_t.id as RoleId,
                    role_t.name as RoleName,
                    role_t.title as RoleTitle
                FROM main.users as user_t
                JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                JOIN main.roles as role_t on userrole_t.role_id = role_t.id
                WHERE user_t.login = @login and user_t.password = @passwordHash",
                param: new { login, passwordHash },
                transaction: Transaction
            ).ToList();

            return CollectUser(rawUserData);
        }

        public void Update(User item)
        {
            Connection.Execute(
                "UPDATE main.users SET login = @Login, username = @UserName WHERE id = @Id",
                param: item,
                transaction: Transaction
            );
        }

        public List<Role> GetRoles()
        {
            return Connection.Query<Role>(
                "SELECT id as Id, name as Name, title as Title FROM main.roles ORDER BY title",
                transaction: Transaction
            ).ToList();
        }

        public void AddRoleToUser(Guid roleId, Guid userId)
        {
            Connection.Execute(
                @"INSERT INTO main.userroles (id, user_id, role_id)
                VALUES(@id, @userId, @roleId)",
                param: new { id = Guid.NewGuid(), userId, roleId },
                transaction: Transaction
            );
        }

        public void RemoveRoleFromUser(Guid roleId, Guid userId)
        {
            Connection.Execute(
                "DELETE FROM main.userroles WHERE user_id = @userId and role_id = @roleId",
                param: new { userId, roleId },
                transaction: Transaction
            );
        }

        public void UpdatePassword(Guid userId, string newPassword)
        {
            Connection.Execute(
                "UPDATE main.users SET password = @newPassword WHERE id = @userId",
                param: new { userId, newPassword },
                transaction: Transaction
            );
        }

        public bool UserExists(string login)
        {
            int userCount = Connection.QueryFirstOrDefault<int>(
                "SELECT COUNT(id) FROM main.users WHERE login = @login",
                param: new { login },
                transaction: Transaction
            );

            return userCount > 0;
        }
    }
}
