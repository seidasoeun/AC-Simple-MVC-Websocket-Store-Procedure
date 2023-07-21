using Microsoft.IdentityModel.Tokens;
using simpleMvc4.Dto;
using simpleMvc4.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace simpleMvc4.Controllers
{
    public class UserController : Controller
    {
        DatabaseSimpleMvcEntities _context = new DatabaseSimpleMvcEntities();
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpGet]
        [Route("/api/user/my-profile/edit")]
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPatch]
        public ActionResult Edit(int id, LoginModel req)
        {
            if (req == null)
            {
                return RedirectToAction("Show", "User", new string[] {"Please provide infomation"});
            }
            if (_context.Users.Any(x => x.userId == id))
            {
                var user = _context.Users.FirstOrDefault(x => x.userId == id);
                user.email = req.Email ?? user.email;
                user.username = req.username ?? user.username;
                user.passcode = req.Passcode ?? user.passcode;
                string accessToken = GenerateAccessToken(user.username);
                string refreshToken = GenerateRefreshToken();
                StoreTokenInCookieAndSetAuthCookie(accessToken, refreshToken, user.username);
                StoreRefreshToken(refreshToken, user.username, user.userId);
                AddDefaultRoleToTable(user.userId);
                _context.SaveChanges();
                return RedirectToAction("Show", "User", new string[]
                {
                    accessToken, refreshToken
                });
            }
            return RedirectToAction("Show", "User", new string[] {"User not found!"});
        }

        [HttpPost]
        public ActionResult Login(LoginModel userInfo)
        {
            if (_context.Users.Any(x => x.email == userInfo.Email && x.passcode == userInfo.Passcode))
            {
                var u = _context.Users.FirstOrDefault(x => x.email == userInfo.Email && x.passcode == userInfo.Passcode);
                string accessToken = GenerateAccessToken(u.username);
                string refreshToken = GenerateRefreshToken();
                StoreTokenInCookieAndSetAuthCookie(accessToken, refreshToken, u.username);
                StoreRefreshToken(refreshToken, u.username, u.userId);
                return RedirectToAction("Index", "Home", new { accessToken, refreshToken });
            }
            ModelState.AddModelError("[UserController::Login]", "Invalid Email or Password");
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (_context.Users.Any())
            {
                int lastId = _context.Users.OrderByDescending(x => x.userId).FirstOrDefault().userId;
                user.userId = lastId + 1;
            }
            _context.Users.Add(user);
            _context.SaveChanges();
            string accessToken = GenerateAccessToken(user.username);
            string refreshToken = GenerateRefreshToken();
            AddDefaultRoleToTable(user.userId);
            StoreTokenInCookieAndSetAuthCookie(accessToken, refreshToken, user.username);
            StoreRefreshToken(refreshToken, user.username, user.userId);

            return RedirectToAction("Index", "Home", new {accessToken, refreshToken});
        }
        [HttpGet]
        [Route("/user/my-profile")]
        [Authorize]
        public ActionResult Show()
        {
            if (Request.Cookies["refresh-token"] == null)
                return RedirectToAction("Login", "User");
            var user = getUserFromToken();
            if(user == null)
                return RedirectToAction("Login", "User", new string [] {"Unauthorized! Token invalid" });
            UserRespone respone = new UserRespone();
            respone.passcode = user.passcode;
            respone.username = user.username;
            respone.email = user.email;
            if(_context.UserRoles.Any(x => x.userId == user.userId))
            {
                List<RoleRespone> roles = new List<RoleRespone>();
                foreach (var item in _context.UserRoles)
                    if (user.userId == item.userId)
                        roles.Add(new RoleRespone{ 
                            roleName = _context.Roles.FirstOrDefault(x => x.roleId == item.roleId).roleName });
                respone.Roles = roles;
            }
            if(_context.Books.Any(x => x.userId == user.userId))
            {
                List<BookRespone> books = new List<BookRespone>();
                foreach (var item in _context.Books)
                    if (user.userId == item.userId)
                        books.Add(new BookRespone { author = item.author, bookTitle = item.bookTitle });
                respone.Books = books;
            }
            return View(respone);
        }

        [HttpGet]
        public ActionResult Logout()
        {
            HttpCookie cookieAccessToken = new HttpCookie("access-token");
            HttpCookie cookieRefreshToken = new HttpCookie("refresh-token");
            cookieAccessToken.Value = null;
            cookieRefreshToken.Value = null;
            cookieAccessToken.Expires = DateTime.Now.AddDays(-10);
            cookieRefreshToken.Expires = DateTime.Now.AddDays(-10);
            Response.Cookies.Add(cookieRefreshToken);
            Response.Cookies.Add(cookieAccessToken);
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        private void StoreTokenInCookieAndSetAuthCookie(string accessToken,string refreshToken, string username)
        {
            var expires = ConfigurationManager.AppSettings["JwtExpires"];
            HttpCookie cookieAccessToken = new HttpCookie("access-token");
            HttpCookie cookieRefreshToken = new HttpCookie("refresh-token");
            cookieAccessToken.Value = accessToken;
            cookieRefreshToken.Value = refreshToken;
            cookieAccessToken.Expires = DateTime.UtcNow.AddMinutes(int.Parse(expires));
            cookieRefreshToken.Expires = DateTime.UtcNow.AddMinutes(int.Parse(expires));
            Response.Cookies.Add(cookieRefreshToken);
            Response.Cookies.Add(cookieAccessToken);

            FormsAuthentication.SetAuthCookie(username, false);
        }

        private string GenerateAccessToken(string username)
        {
            var secret = ConfigurationManager.AppSettings["JwtSecret"];
            var issuer = ConfigurationManager.AppSettings["JwtIssuer"];
            var audience = ConfigurationManager.AppSettings["JwtAudience"];
            var expires = ConfigurationManager.AppSettings["JwtExpires"];
            var role = ConfigurationManager.AppSettings["JwtDefaultRole"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(expires)),
                signingCredentials: signingCredentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return accessToken;
        }

        private string GenerateRefreshToken()
        {
            return Guid.NewGuid().ToString();
        }

        private void StoreRefreshToken(string refreshToken, string username, int userId)
        {
            var user = _context.Users.Where(x => x.userId == userId).FirstOrDefault();
            if (user != null)
            {
                user.refreshToken = refreshToken;
                _context.SaveChanges();
            }
        }
        private User getUserFromToken()
        {
            var token = Request.Cookies["access-token"].Value;
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
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                var user = _context.Users.Where(x => x.username == claims.Identity.Name).FirstOrDefault();
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private void AddDefaultRoleToTable(int userId)
        {
            var defaultRole = ConfigurationManager.AppSettings["JwtDefaultRole"];
            var role = _context.Roles.FirstOrDefault(x => x.roleName == defaultRole);
            if (role != null)
            {
                _context.UserRoles.Add(new UserRole { roleId = role.roleId, userId = userId });
            }
        }

    }
}