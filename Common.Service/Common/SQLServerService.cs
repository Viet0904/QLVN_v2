using Common.Library.Helper;
using Common.Model.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Service.Common
{
    public class SQLServerService
    {
        //public ResModel<bool> CreateDatabase(SQLConnectionStringModel data, string sqlFilePath)
        //{
        //    ResModel<bool> res = new ResModel<bool>();
        //    try
        //    {
        //        string connectionString = BuildConnectionString(data.Ip, "master", data.UserName, data.Password);
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            try
        //            {
        //                connection.Open();

        //                var command = connection.CreateCommand();
        //                command.CommandText = "IF DB_ID('" + data.Database + "') IS NOT NULL BEGIN DROP DATABASE " + data.Database + " END";
        //                command.ExecuteNonQuery();

        //                command.CommandText = "CREATE DATABASE " + data.Database;
        //                command.ExecuteNonQuery();

        //                connection.Close();

        //                List<string> listFilePaths = Directory.EnumerateFiles(sqlFilePath, "*.sql").ToList();
        //                listFilePaths = SortAlphanumericText(listFilePaths);

        //                if (listFilePaths != null && listFilePaths.Count > 0)
        //                {
        //                    res = CreateTable(data, listFilePaths);
        //                }
        //            }
        //            catch (SqlException ex)
        //            {
        //                connection.Close();
        //                ExceptionHelper.HandleException(ex);
        //                res.ErrorMessage = "Lỗi khi tạo CSDL.";
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        ExceptionHelper.HandleException(ex);
        //        res.ErrorMessage = "Lỗi khi tạo CSDL.";
        //    }

        //    return res;
        //}

        //private ResModel<bool> CreateTable(SQLConnectionStringModel data, List<string> listFilePaths)
        //{
        //    ResModel<bool> res = new ResModel<bool>();
        //    try
        //    {
        //        string connectionString = BuildConnectionString(data.Ip, data.Database, data.UserName, data.Password);
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            try
        //            {
        //                connection.Open();

        //                foreach (string path in listFilePaths)
        //                {
        //                    string sqlScript = File.ReadAllText(path);
        //                    try
        //                    {
        //                        var command = connection.CreateCommand();
        //                        command.CommandText = sqlScript;
        //                        command.ExecuteNonQuery();
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        sqlScript = File.ReadAllText(path, Encoding.GetEncoding(1252));

        //                        var command = connection.CreateCommand();
        //                        command.CommandText = sqlScript;
        //                        command.ExecuteNonQuery();
        //                    }
        //                }

        //                connection.Close();
        //            }
        //            catch (SqlException ex)
        //            {
        //                connection.Close();
        //                ExceptionHelper.HandleException(ex);
        //                res.ErrorMessage = "Lỗi khi tạo CSDL.";
        //            }
        //        }
        //    }
        //    catch (SqlException ex)
        //    {
        //        ExceptionHelper.HandleException(ex);
        //        res.ErrorMessage = "Lỗi khi tạo CSDL.";
        //    }

        //    return res;
        //}

        //private string BuildConnectionString(string server, string database, string userName, string password)
        //{
        //    string connectionString = $@"Server={server};Database={database};
        //                            User Id={userName};Password={password};";
        //    return connectionString;
        //}

        //private List<string> SortAlphanumericText(List<string> unsortedText)
        //{
        //    return unsortedText.OrderBy(x => Regex.Replace(x, "[0-9]+", match => match.Value.PadLeft(10, '0'))).ToList();
        //}
    }
}
