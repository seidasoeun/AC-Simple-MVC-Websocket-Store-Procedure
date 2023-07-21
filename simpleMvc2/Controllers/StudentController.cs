using simpleMvc1.Models;
using simpleMvc2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace simpleMvc1.Controllers
{
    public class StudentController : Controller
    {
        db_simplemvcEntities _context = new db_simplemvcEntities();
        public ActionResult Index()
        {
            var listData = _context.students.ToList();
            return View(listData);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(student student)
        {
            int lastId = _context.students
                .OrderByDescending(x => x.StudentId).First().StudentId + 1;
            student.StudentId = lastId;
            _context.students.Add(student);
            _context.SaveChanges();
            ViewBag.message = "Data have inserted!";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var data = _context.students.Where(x => x.StudentId == id).FirstOrDefault();
            return View(data);
        }

        [HttpPost]
        public ActionResult Edit(student student)
        {
            var data = _context.students.Where(x => x.StudentId == student.StudentId).FirstOrDefault();
            if (data != null)
            {
                data.StudentName = student.StudentName;
                data.age = student.age;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var data = _context.students.Where(x => x.StudentId == id).FirstOrDefault();
            return View(data);
        }

        public ActionResult Delete(int id)
        {
            var data = _context.students.Where(x => x.StudentId == id).FirstOrDefault();
            _context.students.Remove(data);
            _context.SaveChanges();
            ViewBag.message = "Data have deleted!";
            return RedirectToAction("Index");
        }
    }
}