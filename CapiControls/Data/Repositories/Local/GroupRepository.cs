using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Data.Repositories.Local
{
    public class GroupRepository : BaseRepository, IPaginatedRepository<Group>
    {
        public GroupRepository(IConfiguration configuration) : base(configuration, "local.main") { }

        public void Create(Group item)
        {
            using (var connection = Connection)
            {
                item.Id = Guid.NewGuid();
                connection.Execute("INSERT INTO main.groups (id, title) VALUES(@Id, @Title)", item);
            }
        }

        public Group Get(Guid id)
        {
            using (var connection = Connection)
            {
                return connection.Query<Group>(
                    "SELECT id as Id, title as Title FROM main.groups WHERE id = @id",
                    new { id }
                ).FirstOrDefault();
            }
        }

        public IEnumerable<Group> GetAll()
        {
            using (var connection = Connection)
            {
                return connection.Query<Group>("SELECT id as Id, title as Title FROM main.groups ORDER BY title").ToList();
            }
        }

        public IEnumerable<Group> GetAll(int itemsPerPage = 10, int page = 1)
        {
            using (var connection = Connection)
            {
                int offset = (page - 1) * itemsPerPage;
                string query = @"SELECT id as Id, title as Title FROM main.groups ORDER BY title OFFSET @Offset LIMIT @Limit";

                return connection.Query<Group>(query, new { Offset = offset, Limit = itemsPerPage }).ToList();
            }
        }

        public void Update(Group item)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "UPDATE main.groups SET title = @Title WHERE id = @Id",
                    item
                );
            }
        }

        public void Delete(Guid id)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "DELETE FROM main.groups WHERE id = @id",
                    new { id }
                );
            }
        }

        public int CountAll()
        {
            using (var connection = Connection)
            {
                return connection.Query<int>("SELECT COUNT(id) FROM main.groups").FirstOrDefault();
            }
        }
    }
}
