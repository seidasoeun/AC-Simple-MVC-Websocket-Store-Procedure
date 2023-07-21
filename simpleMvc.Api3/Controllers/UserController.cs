using simpleMvc.Api3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace simpleMvc.Api3.Controllers
{
    public class UserController : ApiController
    {
        db_simplemvc3Entities _context = new db_simplemvc3Entities();
        [HttpGet]
        public user GetUser()
        {
            var userName = Request.Headers.GetCookies("cookie").FirstOrDefault()?["cookie"].Value;
            var user = _context.users.Where(x => x.UserName == userName).FirstOrDefault();
            return user;
        }

        [HttpPost]
        public HttpResponseMessage Login(LoginModel userInfo)
        {
            bool isUserExist = _context.users.Any(x => x.Email == userInfo.Email && x.Passcode == userInfo.Passcode);
            user u = _context.users.FirstOrDefault(x => x.Email == userInfo.Email && x.Passcode == userInfo.Passcode);

            var resp = new HttpResponseMessage();

            var cookie = new CookieHeaderValue("cookie", u.UserName);
            cookie.Expires = DateTimeOffset.Now.AddDays(1);
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";

            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            return resp;
        }

        [HttpPost]
        public HttpResponseMessage Register(user user)
        {
            if (_context.users.Any())
            {
                user.UserId = _context.users.OrderByDescending(x => x.UserId).FirstOrDefault().UserId + 1;
            }
            _context.users.Add(user);
            _context.SaveChanges();
            var resp = new HttpResponseMessage();

            var cookie = new CookieHeaderValue("cookie", user.UserName);
            cookie.Expires = DateTimeOffset.Now.AddDays(1);
            cookie.Domain = Request.RequestUri.Host;
            cookie.Path = "/";

            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            return resp;
        }

        [HttpDelete]
        public IEnumerable<string> Logout()
        {

            var resp = new HttpResponseMessage();

            var cookie = Request.Headers.GetCookies("cookie").FirstOrDefault();
            cookie.Expires = DateTimeOffset.Now.AddDays(-1);
            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });

            return new string[] { 
                HttpStatusCode.OK.ToString(), "Logout successfully!" ,
            };
        }
    }
}