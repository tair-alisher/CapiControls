using CapiControls.Data.Interfaces;
using CapiControls.Models.Local.Account;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Data.Repositories.Local
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration, "local.main") { }

        public void Create(User item)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "INSERT INTO main.users (id, login, username, password) VALUES(@Id, @Login, @UserName, @Password)",
                    item
                );
            }
        }

        public User Get(Guid id)
        {
            User user = null;
            using (var connection = Connection)
            {
                string query = @"
                    SELECT
                        user_t.id as Id,
                        user_t.login as Login,
                        user_t.username as UserName,
                        role_t.id as RoleId,
                        role_t.name as RoleName,
                        role_t.title as RoleTitle
                    FROM main.users as user_t
                    JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                    JOIN main.roles as role_t on userrole_t.role_id = role_t.id
                    WHERE user_t.id = @id
                ";
                
                var rawUserData = connection.Query<RawUserData>(query, new { id }).ToList();

                user = CollectUser(rawUserData);

                return user;
            }
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

        public User GetUserByLoginAndPassword(string login, string passwordHash)
        {
            User user = null;
            using (var connection = Connection)
            {
                string query = @"
                    SELECT
                        user_t.id as Id,
                        user_t.login as Login,
                        user_t.username as UserName,
                        role_t.id as RoleId,
                        role_t.name as RoleName,
                        role_t.title as RoleTitle
                    FROM main.users as user_t
                    JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                    JOIN main.roles as role_t on userrole_t.role_id = role_t.id
                    WHERE user_t.login = @login and user_t.password = @passwordHash
                ";

                var rawUserData = connection.Query<RawUserData>(query, new { login, passwordHash }).ToList();
                user = CollectUser(rawUserData);
            }

            return user;
        }

        public IEnumerable<User> GetAll()
        {
            List<RawUserData> rawUserData = null;
            using (var connection = Connection)
            {
                string query = @"
                    SELECT
                        user_t.id as Id,
                        user_t.login as Login,
                        user_t.username as UserName,
                        user_t.password as Password,
                        role_t.id as RoleId,
                        role_t.name as RoleName,
                        role_t.title as RoleTitle
                    FROM main.users as user_t
                    JOIN main.userroles as userrole_t on user_t.id = userrole_t.user_id
                    JOIN main.roles as role_t on userrole_t.role_id = role_t.id
                ";

                rawUserData = connection.Query<RawUserData>(query).ToList();
            }

            List<User> users = new List<User>();
            foreach (var rawUser in rawUserData)
            {
                if (users.Where(u => u.Id == rawUser.Id).Count() <= 0)
                {
                    users.Add(new User()
                    {
                        Id = rawUser.Id,
                        Login = rawUser.Login,
                        UserName = rawUser.UserName,
                        Password = rawUser.Password
                    });
                }
            }

            foreach (var user in users)
            {
                var userRoles = rawUserData.Where(u => u.Id == user.Id).ToList();
                foreach (var role in userRoles)
                {
                    user.Roles.Add(new Role()
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
            List<RawUserData> rawUserData = null;
            using (var connection = Connection)
            {
                int offset = (page - 1) * pageSize;
                string query = @"
                    SELECT
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
                    LIMIT @pageSize
                ";

                rawUserData = connection.Query<RawUserData>(query, new { offset, pageSize }).ToList();
            }

            List<User> users = new List<User>();
            foreach (var rawUser in rawUserData)
            {
                if (users.Where(u => u.Id == rawUser.Id).Count() <= 0)
                {
                    users.Add(new User()
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
                    user.Roles.Add(new Role()
                    {
                        Id = role.RoleId,
                        Name = role.RoleName,
                        Title = role.RoleTitle
                    });
                }
            }

            return users;
        }

        public void Update(User item)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "UPDATE main.users SET login = @Login, username = @UserName WHERE id = @Id",
                    item
                );
            }
        }

        public void Delete(Guid id)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "DELETE FROM main.userroles WHERE user_id = @id",
                    new { id }
                );

                connection.Execute(
                    "DELETE FROM main.users WHERE id = @id",
                    new { id }
                );
            }
        }

        public int CountAll()
        {
            using (var connection = Connection)
            {
                return connection.Query<int>("SELECT COUNT(id) FROM main.users").FirstOrDefault();
            }
        }

        public List<Role> GetRoles()
        {
            using (var connection = Connection)
            {
                return connection.Query<Role>("SELECT id as Id, name as Name, title as Title FROM main.roles").ToList();
            }
        }

        public void AddRoleToUser(Guid roleId, Guid userId)
        {
            using (var connection = Connection)
            {
                connection.Execute("INSERT INTO main.userroles (id, user_id, role_id) VALUES(@id, @userId, @roleId)", new { id = Guid.NewGuid(), userId, roleId });
            }
        }

        public void RemoveRoleFromUser(Guid roleId, Guid userId)
        {
            using (var connection = Connection)
            {
                connection.Execute("DELETE FROM main.userroles WHERE user_id = @userId and role_id = @roleId", new { userId, roleId });
            }
        }

        public bool UserExists(string login)
        {
            int userCount = 0;
            using (var connection = Connection)
            {
                userCount = connection.Query<int>("SELECT COUNT(id) FROM main.users WHERE login = @login", new { login }).FirstOrDefault();
            }

            return userCount > 0;
        }

        public void UpdatePassword(Guid userId, string newPassword)
        {
            using (var connection = Connection)
            {
                connection.Execute("UPDATE main.users SET password = @newPassword WHERE id = @userId", new { userId, newPassword });
            }
        }
    }
}
