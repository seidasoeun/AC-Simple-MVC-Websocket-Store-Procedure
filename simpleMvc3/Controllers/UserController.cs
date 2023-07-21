using simpleMvc3.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace simpleMvc3.Controllers
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

        [HttpPost]
        public ActionResult Login(UserViewModel userInfo)
        {
            bool isUserExist = _context.users.Any(x => x.Email == userInfo.Email && x.Passcode == userInfo.Passcode);
            user u = _context.users.FirstOrDefault(x => x.Email == userInfo.Email && x.Passcode == userInfo.Passcode);

            if(isUserExist)
            {
                FormsAuthentication.SetAuthCookie(u.UserName, false);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("[UserController::Login]", "Email or Password is wrong!");
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(user user)
        {
            if(_context.users.Any())
            {
                int lastId = _context.users.OrderByDescending(x => x.UserId).FirstOrDefault().UserId;
                user.UserId = lastId + 1;
            }
            _context.users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Login", "User");
        }

        public ActionResult Logout() {

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }
    }
}