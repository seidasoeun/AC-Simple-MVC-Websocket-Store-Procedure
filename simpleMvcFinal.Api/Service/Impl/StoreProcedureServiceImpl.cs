using simpleMvcFinal.Api.Models;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using simpleMvcFinal.Api.Dto;
using System.Linq;

namespace simpleMvcFinal.Api.Service.Impl
{
    public class StoreProcedureServiceImpl : IStoreProcedureService
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
                    cmd.Parameters.AddWithValue("@roleId", model.RoleId);
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
                message = "SUCCESS";
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
                    cmd.Parameters.AddWithValue("@roleId", model.RoleId);
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

        public UserResponse GetUserFromDataTable(DataTable data)
        {
            UserResponse user = new UserResponse();
            user.Roles = new List<RoleResponse>();
            user.Books = new List<BookResponse>();
            int index = 0;
            foreach (DataRow row in data.Rows)
            {
                if (index == 0)
                {
                    index++;
                    user.Email = row["email"].ToString();
                    user.Username = row["username"].ToString();
                    user.Email = row["email"].ToString();
                    user.Passcode = row["passcode"].ToString();
                    user.RefreshToken = row["refreshToken"].ToString();


                    if (!string.IsNullOrEmpty(row["roleName"].ToString()))
                    {
                        RoleResponse newRole = new RoleResponse
                        {
                            RoleName = row["roleName"].ToString()
                        };
                        user.Roles.Add(newRole);
                    }

                    if (!string.IsNullOrEmpty(row["bookTitle"].ToString()))
                    {
                        user.Books.Add(new BookResponse
                        {
                            Author = row["author"].ToString(),
                            BookTitle = row["bookTitle"].ToString()
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(row["roleName"].ToString()))
                    {
                        RoleResponse newRole = new RoleResponse
                        {
                            RoleName = row["roleName"].ToString()
                        };
                        user.Roles.Add(newRole);
                    }

                    if (!string.IsNullOrEmpty(row["bookTitle"].ToString()))
                    {
                        user.Books.Add(new BookResponse
                        {
                            Author = row["author"].ToString(),
                            BookTitle = row["bookTitle"].ToString()
                        });
                    }
                }
            }

            return user;
        }

        public List<UserResponse> GetUserListFromDataTable(DataTable data)
        {
            string currentUserId = String.Empty;
            List<UserResponse> users = new List<UserResponse>();

            foreach (DataRow row in data.Rows)
            {
                string userId = row["username"].ToString();
                if (currentUserId != userId)
                {
                    UserResponse newUser = new UserResponse
                    {
                        Username = row["username"].ToString(),
                        Email = row["email"].ToString(),
                        Passcode = row["passcode"].ToString(),
                        RefreshToken = row["refreshToken"].ToString(),
                        Roles = new List<RoleResponse>(),
                        Books = new List<BookResponse>()
                    };


                    if (!string.IsNullOrEmpty(row["roleName"].ToString()))
                    {
                        RoleResponse newRole = new RoleResponse
                        {
                            RoleName = row["roleName"].ToString()
                        };
                        newUser.Roles.Add(newRole);
                    }

                    if (!string.IsNullOrEmpty(row["bookTitle"].ToString()))
                    {
                        newUser.Books.Add(new BookResponse
                        {
                            Author = row["author"].ToString(),
                            BookTitle = row["bookTitle"].ToString()
                        });
                    }

                    users.Add(newUser);

                    currentUserId = userId;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row["roleName"].ToString()))
                    {
                        RoleResponse newRole = new RoleResponse
                        {
                            RoleName = row["roleName"].ToString()
                        };
                        users.Last().Roles.Add(newRole);
                    }

                    if (!string.IsNullOrEmpty(row["bookTitle"].ToString()))
                    {
                        users.Last().Books.Add(new BookResponse
                        {
                            Author = row["author"].ToString(),
                            BookTitle = row["bookTitle"].ToString()
                        });
                    }
                }
            }

            return users;
        }

        public List<RoleResponse> GetRoleFromDataTable(DataTable data)
        {
            int index = 0;
            List<RoleResponse> roles = new List<RoleResponse>();

            foreach (DataRow row in data.Rows)
            {
                if (index == 0)
                {
                    if (!string.IsNullOrEmpty(row["roleName"].ToString()))
                    {
                        RoleResponse newRole = new RoleResponse
                        {
                            RoleName = row["roleName"].ToString()
                        };
                        roles.Add(newRole);
                    }
                    index++;
                }
                else
                {
                    if (!string.IsNullOrEmpty(row["roleName"].ToString()))
                    {
                        RoleResponse newRole = new RoleResponse
                        {
                            RoleName = row["roleName"].ToString()
                        };
                        roles.Add(newRole);
                    }
                }
            }

            return roles;
        }

    }
}