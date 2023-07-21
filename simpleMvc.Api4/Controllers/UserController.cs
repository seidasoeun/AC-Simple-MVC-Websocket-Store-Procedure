using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using simpleMvc.Api4.Dto;
using simpleMvc.Api4.Models;
using simpleMvc.Api4.Repository;
using simpleMvc.Api4.Service.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;

namespace simpleMvc.Api4.Controllers
{
    public class UserController : ApiController
    {
        private TokenServiceImpl _tokenService;
        private UserServiceImpl _userService;
        private UserRepository _userRepository;
        public UserController() {
            this._tokenService = new TokenServiceImpl();
            this._userService = new UserServiceImpl();
            this._userRepository = new UserRepository();
        }

        [HttpPost]
        public IHttpActionResult Login(TokenRequest req)
        {

            if (Authenticate(req.Email, req.Passcode))
            {
                var u = _userRepository._context.Users.FirstOrDefault(x => x.email == req.Email && x.passcode == req.Passcode);
                List<RoleRespone> roles = new List<RoleRespone>();
                foreach (var item in _userRepository._context.UserRoles)
                {
                    if(item.userId == u.userId)
                    {
                        roles.Add(new RoleRespone
                        {
                            roleName = _userRepository._context.Roles.FirstOrDefault(x => x.roleId == item.roleId).roleName
                        });
                    }
                }
                var accessToken = _tokenService.GenerateAccessToken(u.username, roles);
                var refreshToken = _tokenService.GenerateRefreshToken();
                _tokenService.StoreRefreshToken(refreshToken, u.username, u.userId);

                return Ok(new
                {
                    access_token = accessToken,
                    refresh_token = refreshToken
                });
            }

            return Unauthorized();
        }

        private bool Authenticate(string email, string password)
        {
            return _userRepository._context.Users.Any(x => x.email == email && x.passcode == password);
        }

        [HttpPost]
        [Route("api/user/refresh")]
        public IHttpActionResult RefreshToken(RefreshTokenRequestModel model)
        {
            var refreshToken = model.RefreshToken;
            var user = _tokenService.GetUserFromRefreshToken(refreshToken);

            if (user != null)
            {
                List<RoleRespone> roles = new List<RoleRespone>();
                foreach(var role in _userRepository._context.UserRoles)
                {
                    if (role.userId == user.userId)
                        roles.Add(new RoleRespone
                        {
                            roleName = _userRepository._context.Roles.FirstOrDefault(x => x.roleId == role.roleId).roleName
                        });
                }
                var accessToken = _tokenService.GenerateAccessToken(user.username, roles);
                return Ok(new
                {
                    access_token = accessToken,
                    username = user.username,
                });
            }

            return Unauthorized();
        }


        [HttpPost]
        public IEnumerable<string> SignUp(RegisterRequest req)
        {
            User user = new User();
            if (_userRepository._context.Users.Any())
            {
                int lastId = _userRepository._context.Users.OrderByDescending(x => x.userId).FirstOrDefault().userId;
                user.userId = lastId + 1;
            }
            else
                user.userId = 1;
            user.passcode = req.passcode;
            user.email = req.email;
            user.username = req.username;

            List<RoleRespone> roles = new List<RoleRespone>();
            foreach (var item in req.Roles)
            {
                var isExistRole = _userRepository._context.Roles.FirstOrDefault(x => x.roleName == item.roleName);
                if(isExistRole != null)
                {
                    UserRole role = new UserRole();
                    role.roleId = isExistRole.roleId;
                    role.userId = user.userId;
                    _userRepository._context.UserRoles.Add(role);
                    roles.Add(new RoleRespone { roleName = isExistRole.roleName});
                }
            }

            _userRepository._context.Users.Add(user);
            _userRepository._context.SaveChanges();

            var accessToken = _tokenService.GenerateAccessToken(req.username, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();
            _tokenService.StoreRefreshToken(refreshToken, req.username, user.userId);

            return new string[] { 
                HttpStatusCode.Created.ToString(), 
                "Register successfully!",
                "access_token = " + accessToken,
                "refresh_token = " + refreshToken
            };
        }

        [HttpPost]
        public HttpResponseMessage getUser(UserRequest req)
        {
            var secret = ConfigurationManager.AppSettings["JwtSecret"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];
            var handler = new JwtSecurityTokenHandler();
            var validations = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                UserRespone respone = new UserRespone();
                var claims = handler.ValidateToken(req.accessToken, validations, out var tokenSecure);
                var user = _userRepository._context.Users
                    .FirstOrDefault(x => x.username == claims.Identity.Name);

                if(user == null)
                    return Request.CreateResponse(HttpStatusCode.NotFound, "User Not found");

                respone.email = user.email;
                respone.userId = user.userId;
                respone.username = user.username;
                respone.passcode = user.passcode;
                respone.refreshToken = user.refreshToken;
                respone.Books = _userService.GetBookFromUserId(user.userId);
                respone.Roles = _userService.GetRoleFromUserId(user.userId);

                return new HttpResponseMessage() { Content = new StringContent(JsonConvert.SerializeObject(respone)) };
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,ex.Message);
            }
        }

        [HttpPost]
        public string Logout()
        {
            return "";
        }        
    }
}