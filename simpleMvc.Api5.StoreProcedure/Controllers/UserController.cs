using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using simpleMvc.Api5.StoreProcedure.Dto;
using simpleMvc.Api5.StoreProcedure.Service.Impl;
using StoreProcedure.Api1.Models;

namespace simpleMvc.Api5.StoreProcedure.Controllers
{
    public class UserController : ApiController
    {
        private readonly ProcedureServiceImpl _procedureService = new ProcedureServiceImpl();
        private readonly TokenServiceImpl _tokenService = new TokenServiceImpl();


        [HttpGet]
        [Route("api/user/get-all")]
        public List<UserResponse> GetAllUser()
        {
            DataTable result = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-ALL-USER",
            }, out _);

            string currentUserId = String.Empty;
            List<UserResponse> users = new List<UserResponse>();

            foreach (DataRow row in result.Rows)
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

        [HttpGet]
        [Route("api/user/profile")]
        public UserResponse GetUser(UserRequest req)
        {
            string username = _tokenService.GetUsernameWithToken(req.Token);
            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USER-WITH-USERNAME",
                Username = username
            }, out _);

            UserResponse user = new UserResponse();
            user.Roles = new List<RoleResponse>();
            user.Books = new List<BookResponse>();
            int index = 0;
            foreach (DataRow row in res.Rows)
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

        [HttpPost]
        [Route("api/user/login")]
        public TokenResponse Login(LoginRequest req)
        {
            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USER-WITH-USERNAME-AND-PASSWORD",
                Username = req.Username,
                Password = req.Password,
            }, out _);

            int index = 0;
            string username = String.Empty;
            List<RoleResponse> roles = new List<RoleResponse>();

            foreach (DataRow row in res.Rows)
            {
                if (index == 0)
                {
                    username = row["username"].ToString();
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

            string refreshToken = _tokenService.GenerateRefreshToken();
            _procedureService.ProcessProcedureWithMessage(new ProcedureModel
            {
                Action = "ADD-REFRESH-TOKEN-WITH-USERNAME",
                Username = username,
                RefreshToken = refreshToken,
            }, out _);

            return new TokenResponse
            {
                RefreshToken = refreshToken,
                AccessToken = _tokenService.GenerateAccessToken(username, roles)
            };
        }

        [HttpPost]
        [Route("api/user/refresh-token")]
        public TokenResponse RefreshToken(TokenRequest req)
        {
            DataTable dataTable = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USERNAME-WITH-REFRESH-TOKEN",
                RefreshToken = req.RefreshToken,
            }, out _);
            string username = string.Empty;
            string refreshToken = String.Empty;
            
            foreach (DataRow row in dataTable.Rows)
            {
                username = row["username"].ToString();
            }

            DataTable res = _procedureService.ProcessProcedureWithDataTable(new ProcedureModel
            {
                Action = "SELECT-USER-WITH-USERNAME",
                Username = username,
            }, out _);

            List<RoleResponse> roles = new List<RoleResponse>();
            int index = 0;
            foreach (DataRow row in res.Rows)
            {
                if (index == 0)
                {
                    refreshToken = row["refreshToken"].ToString();
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

            return new TokenResponse
            {
                RefreshToken = refreshToken,
                AccessToken = _tokenService.GenerateAccessToken(username, roles)
            };
        }
    }
}
