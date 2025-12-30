using AutoMapper;
using Common.Database.Entities;
using Common.Model.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Collections.Generic;

namespace Common.Service.Common
{
    public class BaseService
    {
        #region Properties

        public BaseEntity DbContext
        {
            get
            {
                return UnitOfWork.Ins.DB;
            }
        }

        public IMapper Mapper
        {
            get
            {
                return UnitOfWork.Ins.Mapper;
            }
        }

        #endregion Properties

        #region Contructor

        public BaseService()
        {
            UnitOfWork.Ins.RenewDB();
        }

        #endregion Contructor

        #region Store procedure

        public T ExecuteReader<T>(SqlCommandModel model, Func<DbDataReader, T> customReader)
        {
            var dbConnection = DbContext.Database.GetDbConnection();
            using (var command = dbConnection.CreateCommand())
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                command.CommandText = model.CommandText;
                command.CommandType = model.CommandType;

                foreach (var item in model.Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    return customReader(reader);
                }
            }
        }

        public DataTable ExecuteAndGetDynamicTable(SqlCommandModel model)
        {
            DataTable result = new DataTable();

            var dbConnection = DbContext.Database.GetDbConnection();
            using (var command = dbConnection.CreateCommand())
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                command.CommandText = model.CommandText;
                command.CommandType = model.CommandType;

                foreach (var item in model.Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    result.Load(reader);
                }
            }

            return result;
        }

        public bool ExecuteStoreNoneReaderWithTransactionr(SqlCommandModel model)
        {
            var dbConnection = DbContext.Database.GetDbConnection();
            using (var command = dbConnection.CreateCommand())
            {
                // attach existing EF Core transaction if any
                var current = DbContext.Database.CurrentTransaction;
                if (current != null)
                {
                    var dbTrans = current.GetDbTransaction();
                    command.Transaction = dbTrans;
                }

                command.CommandText = model.CommandText;
                command.CommandType = model.CommandType;
                bool result = true;

                foreach (var item in model.Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    result = reader != null;
                }

                return result;
            }
        }

        public bool ExecuteNoneReader(SqlCommandModel model)
        {
            var dbConnection = DbContext.Database.GetDbConnection();
            using (var command = dbConnection.CreateCommand())
            {
                if (dbConnection.State != ConnectionState.Open)
                {
                    dbConnection.Open();
                }

                command.CommandText = model.CommandText;
                command.CommandType = model.CommandType;
                bool result = true;

                foreach (var item in model.Parameters)
                {
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = item.Key;
                    parameter.Value = item.Value ?? DBNull.Value;
                    command.Parameters.Add(parameter);
                }

                using (var reader = command.ExecuteReader())
                {
                    result = reader != null;
                }

                return result;
            }
        }

        public int ExecuteSqlCommand(string query)
        {
            // EF Core replacement for ExecuteSqlCommand (EF6)
            return DbContext.Database.ExecuteSqlRaw(query);
        }

        #endregion Store procedure

        #region Query

        public IList<T> SqlQuery<T>(string query) where T : class
        {
            // Use FromSqlRaw on the DbSet for EF Core
            return DbContext.Set<T>().FromSqlRaw(query).ToList();
        }

        #endregion Query

        #region Method

        public string GenerateId(string tableName, int length)
        {
            var table = DbContext.SysIdGenerateds.Where(x => x.Table.Equals(tableName)).FirstOrDefault();
            if (table != null)
            {
                table.TotalRows += 1;
                DbContext.SaveChanges();
            }
            else
            {
                table = new SysIdGenerated();
                table.Table = tableName;
                table.TotalRows = 1;
                DbContext.SysIdGenerateds.Add(table);
                DbContext.SaveChanges();
            }

            return table.TotalRows.ToString().PadLeft(length, '0');
        }

        #endregion Method
    }
}