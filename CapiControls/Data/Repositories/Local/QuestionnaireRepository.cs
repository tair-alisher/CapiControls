using CapiControls.Data.Interfaces;
using CapiControls.Models.Local;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CapiControls.Data.Repositories.Local
{
    public class QuestionnaireRepository : BaseRepository, IPaginatedRepository<Questionnaire>
    {
        public QuestionnaireRepository(IConfiguration configuration) : base(configuration, "local.main") { }

        public void Create(Questionnaire item)
        {
            using (var connection = Connection)
            {
                item.Id = Guid.NewGuid();
                connection.Execute(
                    "INSERT INTO main.questionnaires (id, group_id, identifier, title) VALUES(@Id, @GroupId, @Identifier, @Title)",
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
                        , group_t.title as Group
                        , quest.identifier as Identifier
                        , quest.title as Title
                    FROM main.questionnaires as quest
                    JOIN main.groups as group_t ON quest.group_id = group_t.id
                    WHERE quest.id = @id
                ";

                return connection.Query<Questionnaire>(query, new { id }).FirstOrDefault();
            }
        }

        public IEnumerable<Questionnaire> GetAll()
        {
            using (var connection = Connection)
            {
                string query = @"
                    SELECT
                        quest.id as Id
                        , quest.group_id as GroupId
                        , group_t.title as Group
                        , quest.identifier as Identifier
                        , quest.title as Title
                    FROM main.questionnaires as quest
                    JOIN main.groups as group_t ON quest.group_id = group_t.id
                ";

                return connection.Query<Questionnaire>(query).ToList();
            }
        }

        public IEnumerable<Questionnaire> GetAll(int pageSize = 10, int page = 1)
        {
            using (var connection = Connection)
            {
                int offset = (page - 1) * pageSize;
                string query = @"
                    SELECT
                        quest.id as Id
                        , quest.group_id as GroupId
                        , group_t.title as Group
                        , quest.title as Title
                    FROM main.questionnaires as quest
                    JOIN main.groups as group_t ON quest.group_id = group_t.id
                    OFFSET @Offset
                    LIMIT @Limit
                ";

                return connection.Query<Questionnaire>(query, new { Offset = offset, Limit = pageSize }).ToList();
            }
        }

        public void Update(Questionnaire item)
        {
            using (var connection = Connection)
            {
                connection.Execute(
                    "UPDATE main.questionnaires SET group_id = @GroupId, identifier = @Identifier, title = @Title WHERE id = @Id",
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
