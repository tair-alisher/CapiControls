﻿using CapiControls.DAL.Entities;
using CapiControls.DAL.Interfaces.Repositories;
using CapiControls.DAL.Repositories.Base;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CapiControls.DAL.Repositories
{
    internal class QuestionnaireRepository : BaseRepository, IQuestionnaireRepository
    {
        public QuestionnaireRepository(IDbTransaction transaction) : base(transaction) { }

        public void Add(Questionnaire item)
        {
            item.Id = Guid.NewGuid();
            Connection.Execute(
                @"INSERT INTO main.questionnaires (id, group_id, identifier, title)
                VALUES (@Id, @GroupId, @Identifier, @Title)",
                param: item,
                transaction: Transaction
            );
        }

        public int CountAll()
        {
            return Connection.QueryFirstOrDefault<int>(
                "SELECT COUNT(id) FROM main.questionnaires",
                transaction: Transaction
            );
        }

        public void Delete(Guid id)
        {
            Connection.Execute(
                "DELETE FROM main.questionnaires WHERE id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public void Delete(Questionnaire item)
        {
            Delete(item.Id);
        }

        public Questionnaire Find(Guid id)
        {
            return Connection.QueryFirstOrDefault<Questionnaire>(
                @"SELECT
                    quest.id as Id
                    , quest.group_id as GroupId
                    , group_t.title as Group
                    , quest.identifier as Identifier
                    , quest.title as Title
                FROM main.questionnaires as quest
                JOIN main.groups as group_t ON quest.group_id = group_t.id
                WHERE quest.id = @id",
                param: new { id },
                transaction: Transaction
            );
        }

        public IEnumerable<Questionnaire> GetAll()
        {
            return Connection.Query<Questionnaire>(
                @"SELECT
                    quest.id as Id
                    , quest.group_id as GroupId
                    , group_t.title as Group
                    , quest.identifier as Identifier
                    , quest.title as Title
                FROM main.questionnaires as quest
                JOIN main.groups as group_t ON quest.group_id = group_t.id
                ORDER BY quest.title",
                transaction: Transaction
            ).ToList();
        }

        public IEnumerable<Questionnaire> GetAll(int pageSize, int page)
        {
            int offset = (page - 1) * pageSize;
            return Connection.Query<Questionnaire>(
                @"SELECT
                    quest.id as Id
                    , quest.group_id as GroupId
                    , group_t.title as Group
                    , quest.identifier as Identifier
                    , quest.title as Title
                FROM main.questionnaires as quest
                JOIN main.groups as group_t ON quest.group_id = group_t.id
                ORDER BY quest.title
                OFFSET @offset
                LIMIT @pageSize",
                param: new { offset, pageSize },
                transaction: Transaction
            ).ToList();
        }

        public void Update(Questionnaire item)
        {
            Connection.Execute(
                @"UPDATE main.questionnaires
                SET group_id = @GroupId, identifier = @Identifier, title = @Title
                WHERE id = @Id",
                param: item,
                transaction: Transaction
            );
        }
    }
}