using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Data.Repositories.Local
{
    public class QuestionnaireRepository : BaseRepository, IRepository<Questionnaire>
    {
        public QuestionnaireRepository(IConfiguration configuration) : base(configuration, "local.main") { }

        public void Create(Questionnaire item)
        {
            using (var connection = Connection)
            {
                item.Id = Guid.NewGuid();
                connection.Execute(
                    "INSERT INTO main.questionnaires (id, group_id, title) VALUES(@Id, @GroupId, @Title)",
                    item
                );
            }
        }

        public Questionnaire Get(Guid id)
        {
            using (var connection = Connection)
            {
                string query = @"
                    SELECT
                        quest.id as Id
                        , quest.group_id as GroupId
                        , group.title as Group
                        , quest.title as Title
                    FROM main.questionnaires as quest
                    JOIN main.groups as group ON quest.group_id = group.id
                    WHERE id = @id
                ";

                return connection.Query<Questionnaire>(query, new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<Questionnaire> GetAll(int itemsPerPage = 10, int page = 1)
        {
            using (var connection = Connection)
            {
                int offset = (page - 1) * itemsPerPage;
                string query = @"
                    SELECT
                        quest.id as Id
                        , quest.group_id as GroupId
                        , group.title as Group
                        , quest.title as Title
                    FROM main.questionnaires as quest
                    JOIN main.groups as group ON quest.group_id = group.id
                    OFFSET @Offset
                    LIMIT @Limit
                ";

                return connection.Query<Questionnaire>(query, new { Offset = offset, Limit = itemsPerPage }).ToList();
            }
        }

        public void Update(Questionnaire item)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "UPDATE main.questionnaires SET group_id = @GroupId, title = @Title where id = @Id",
                    item
                );
            }
        }

        public void Delete(Guid id)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "DELETE FROM main.questionnaires WHERE id = @id",
                    new { id }
                );
            }
        }

        public int CountAll()
        {
            using (var connection = Connection)
            {
                return connection.Query<int>("SELECT COUNT(id) FROM main.questionnaires").FirstOrDefault();
            }
        }
    }
}
