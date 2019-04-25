using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Repositories.Base;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapiControls.DAL.Repositories
{
    internal class RegionRepository : BaseRepository, IRegionRepository
    {
        public RegionRepository(IDbTransaction transaction) : base(transaction) { }

        public void Add(Region item)
        {
            Connection.Execute(
               @"INSERT INTO regions (id, name, title)
                VALUES(@Id, @Name, @Title)",
                param: item,
                transaction: Transaction
            );
        }

        public void Delete(Guid id)
        {
            Connection.Execute(
                "DELETE FROM regions WHERE id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public void Delete(Region item)
        {
            throw new NotImplementedException();
        }

        public Region Find(Guid id)
        {
            return Connection.QueryFirstOrDefault<Region>(
                @"SELECT id as Id, name as Name, title as Title
                FROM regions
                WHERE id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public IEnumerable<Region> GetAll()
        {
            return Connection.Query<Region>(
                @"SELECT id as Id, name as Name, title as Title
                FROM regions.
                ORDER BY title",
                transaction: Transaction
            ).ToList();
        }

        public void Update(Region item)
        {
            Connection.Execute(
                "UPDATE regions SET name = @Name, title = @Title WHERE id = @Id",
                param: item,
                transaction: Transaction
            );
        }
    }
}
