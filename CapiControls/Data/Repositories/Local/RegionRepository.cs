using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Data.Repositories.Local
{
    public class RegionRepository : BaseRepository, IRepository<Region>
    {
        public RegionRepository(IConfiguration configuration) : base(configuration, "local.main") { }

        public void Create(Region item)
        {
            using (var connection = Connection)
            {
                item.Id = Guid.NewGuid();
                connection.Execute("INSERT INTO main.regions (id, name, title) VALUES(@Id, @Name, @Title)", item);
            }
        }

        public Region Get(Guid id)
        {
            using (var connection = Connection)
            {
                return connection.Query<Region>(
                    "SELECT id as Id, name as Name, title as Title FROM main.regions WHERE id = @id",
                    new { id }
                ).FirstOrDefault();
            }
        }

        public IEnumerable<Region> GetAll()
        {
            using (var connection = Connection)
            {
                return connection.Query<Region>("SELECT id as Id, name as Name, title as Title from main.regions order by title").ToList();
            }
        }

        public void Update(Region item)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "UPDATE main.regions SET name = @Name, title = @Title WHERE id = @Id",
                    item
                );
            }
        }

        public void Delete(Guid id)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "DELETE FROM main.regions WHERE id = @id",
                    new { id }
                );
            }
        }
    }
}
