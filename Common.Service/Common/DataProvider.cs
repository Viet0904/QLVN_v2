using AutoMapper;
using Common.Library.Helper;
using Common.Model.Common;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using Microsoft.Data.SqlClient;

namespace Common.Service.Common
{
    public class DataProvider
    {
        #region Properties
        private string _dbClientConnectionString = string.Empty;

        public IMapper Mapper { get; set; } = null!;

        public BaseEntity DB { get; private set; } = null!;

        private IDbContextTransaction? transaction { get; set; }

        #endregion Properties

        #region Contructor
        public DataProvider(SQLConnectionStringModel client)
        {
            if (client == null)
                throw new Exception("SQLConnectionString cannot be null");

            _dbClientConnectionString = BuildConnectionString(client.Ip, client.Database, client.UserName, client.Password);
            DB = new BaseEntity(_dbClientConnectionString);

            transaction = null;
            BuildMapper(DB);
        }

        #endregion Contructor

        #region Method

        public void RenewDB()
        {
            // Check transaction is opening or not.
            if (DB.Database.CurrentTransaction == null)
            {
                DB = new BaseEntity(_dbClientConnectionString);
                transaction = null;
            }
        }

        public void TransactionOpen()
        {
            transaction = DB.Database.BeginTransaction();
        }

        public void TransactionCommit()
        {
            if (transaction != null)
                transaction.Commit();
        }

        public void TransactionRollback()
        {
            if (transaction != null)
                transaction.Rollback();
        }

        public void TransactionDispose()
        {
            if (transaction != null)
                transaction.Dispose();
        }

        #endregion Method

        #region Private

        private string BuildConnectionString(string server, string database, string userName, string password)
        {
            server = CryptorEngineHelper.Decrypt(server);
            database = CryptorEngineHelper.Decrypt(database);
            userName = CryptorEngineHelper.Decrypt(userName);
            password = CryptorEngineHelper.Decrypt(password);


            string connection = string.Format("data source={0};initial catalog={1};user id={2};password={3}", server, database, userName, password);
            // Build standard ADO.NET connection string for SQL Server
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = server,
                InitialCatalog = database,
                UserID = userName,
                Password = password,
                MultipleActiveResultSets = true,
                TrustServerCertificate = true
            };
            return builder.ToString();
            //var entityBuilder = new EntityConnectionStringBuilder();
            //entityBuilder.Provider = "System.Data.SqlClient";
            //entityBuilder.ProviderConnectionString = connection + ";MultipleActiveResultSets=True;App=EntityFramework;";
            //entityBuilder.Metadata = @"res://*/SystemDB.csdl|res://*/SystemDB.ssdl|res://*/SystemDB.msl";
            //return entityBuilder.ToString();
        }

        private void BuildMapper(BaseEntity baseEntity)
        {
            Mapper = MapperConfig.BuildMapper(baseEntity);
        }
        #endregion Private
    }
}
