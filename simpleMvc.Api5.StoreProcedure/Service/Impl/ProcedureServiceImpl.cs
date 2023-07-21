using System.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using StoreProcedure.Api1.Models;

namespace simpleMvc.Api5.StoreProcedure.Service.Impl
{
    public class ProcedureServiceImpl : ProcedureService
    {
        public string ProcessProcedureWithMessage(ProcedureModel model, out string message)
        {
            message = string.Empty;
            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["sqlConnection"]))
            {
                SqlCommand cmd = null;
                try
                {
                    cmd = new SqlCommand("SimpleMvcProcedure", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@bookId", model.BookId);
                    cmd.Parameters.AddWithValue("@bookTitle", model.BookTitle);
                    cmd.Parameters.AddWithValue("@author", model.Author);
                    cmd.Parameters.AddWithValue("@roleName", model.RoleName);
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@email", model.Email);
                    cmd.Parameters.AddWithValue("@password", model.Password);
                    cmd.Parameters.AddWithValue("@refreshToken", model.RefreshToken);
                    cmd.Parameters.AddWithValue("@action", model.Action);
                    sqlConnection.Open();
                }
                catch (Exception e)
                {
                    message = e.Message;
                }

                if (cmd != null)
                {
                    cmd.ExecuteNonQuery();
                }
                sqlConnection.Close();
                message = "Success!";
            }
            return message;
        }

        public DataTable ProcessProcedureWithDataTable(ProcedureModel model, out string message)
        {
            DataTable dataTable;
            DataSet dataSet = new DataSet();
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();

            using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.AppSettings["sqlConnection"]))
            {
                SqlCommand cmd = null;
                try
                {
                    cmd = new SqlCommand("SimpleMvcProcedure", sqlConnection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@bookId", model.BookId);
                    cmd.Parameters.AddWithValue("@bookTitle", model.BookTitle);
                    cmd.Parameters.AddWithValue("@author", model.Author);
                    cmd.Parameters.AddWithValue("@roleName", model.RoleName);
                    cmd.Parameters.AddWithValue("@username", model.Username);
                    cmd.Parameters.AddWithValue("@email", model.Email);
                    cmd.Parameters.AddWithValue("@password", model.Password);
                    cmd.Parameters.AddWithValue("@refreshToken", model.RefreshToken);
                    cmd.Parameters.AddWithValue("@action", model.Action);
                    sqlConnection.Open();
                }
                catch (Exception e)
                {
                    message = e.Message;
                }

                if (cmd != null)
                {
                    sqlDataAdapter.SelectCommand = cmd;
                }

                sqlDataAdapter.Fill(dataSet, "Books");
                dataTable = dataSet.Tables["Books"];
                sqlConnection.Close();
                message = "Success!";
            }
            return dataTable;
        }
    }
}