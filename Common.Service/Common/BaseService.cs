using AutoMapper;
using Common.Database.Data;
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

        // Nhận từ constructor thay vì từ UnitOfWork
        protected readonly QLVN_DbContext DbContext;
        protected readonly IMapper Mapper;

        #endregion Properties

        #region ConstructorMapperProfile

        //  Inject DbContext và IMapper qua constructor
        public BaseService(QLVN_DbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            Mapper = mapper;
        }

        #endregion Constructor

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
            return DbContext.Database.ExecuteSqlRaw(query);
        }

        #endregion Store procedure

        #region Query

        public IList<T> SqlQuery<T>(string query) where T : class
        {
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
                table = new SysIdGenerated
                {
                    Table = tableName,
                    TotalRows = 1
                };
                DbContext.SysIdGenerateds.Add(table);
                DbContext.SaveChanges();
            }

            return table.TotalRows.ToString().PadLeft(length, '0');
        }

        #endregion Method
    }
}