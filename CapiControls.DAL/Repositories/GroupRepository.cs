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
    internal class GroupRepository : BaseRepository, IGroupRepository
    {
        public GroupRepository(IDbTransaction transaction) : base(transaction) { }

        public void Add(Group item)
        {
            item.Id = Guid.NewGuid();
            Connection.Execute(
                "INSERT INTO main.groups (id, title) VALUES(@Id, @Title)",
                param: item,
                transaction: Transaction
            );
        }

        public int CountAll()
        {
            return Connection.QueryFirstOrDefault<int>(
                "SELECT COUNT(id) FROM main.groups",
                transaction: Transaction
            );
        }

        public void Delete(Guid id)
        {
            Connection.Execute(
                "DELETE FROM main.groups WHERE id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public void Delete(Group item)
        {
            Delete(item.Id);
        }

        public Group Find(Guid id)
        {
            return Connection.QueryFirstOrDefault<Group>(
                "SELECT id as Id, title as Title FROM main.groups WHERE id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public IEnumerable<Group> GetAll()
        {
            return Connection.Query<Group>(
                "SELECT id as Id, title as Title FROM main.groups ORDER BY title",
                transaction: Transaction
            ).ToList();
        }

        public IEnumerable<Group> GetAll(int pageSize, int page)
        {
            int offset = (page - 1) * pageSize;

            return Connection.Query<Group>(
                @"SELECT id as Id, title as Title
                FROM main.groups
                ORDER BY title
                OFFSET @offset
                LIMIT @pageSize",
                param: new { offset, pageSize },
                transaction: Transaction
            ).ToList();
        }

        public void Update(Group item)
        {
            Connection.Execute(
                "UPDATE main.groups SET title = @title WHERE id = @Id",
                param: item,
                transaction: Transaction
            );
        }
    }
}
