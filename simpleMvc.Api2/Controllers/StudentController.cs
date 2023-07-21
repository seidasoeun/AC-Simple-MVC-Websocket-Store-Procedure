using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace simpleMvc.Api2.Controllers
{
    public class StudentController : ApiController
    {
        DatabaseSimpleMvcApiEntities databaseSimpleMvcApiEntities = new DatabaseSimpleMvcApiEntities();

        [HttpGet]
        public IEnumerable<student> GetAllStudent()
        {
            return databaseSimpleMvcApiEntities.students;
        }

        [HttpGet]
        public student GetStudent(int id)
        {
            var Getstudent = databaseSimpleMvcApiEntities.students.Where(x => x.StudentId == id).FirstOrDefault();
            return Getstudent;
        }

        [HttpPost]
        // POST: api/student
        public IEnumerable<string> CreateStudent([FromBody] student student)
        {
            int lastId = databaseSimpleMvcApiEntities.students
                .OrderByDescending(x => x.StudentId).First().StudentId + 1;
            student.StudentId = lastId;
            databaseSimpleMvcApiEntities.students.Add(student);
            databaseSimpleMvcApiEntities.SaveChanges();
            return new string[] {HttpStatusCode.Created.ToString(), "Record Created!" };
        }

        [HttpPut]
        // PUT: api/student/5
        public IEnumerable<string> UpdateStudent(int id, [FromBody] student student)
        {
            var UpdateStudent = databaseSimpleMvcApiEntities.students.Where(x => x.StudentId == id).FirstOrDefault();
            if (UpdateStudent != null)
            {
                UpdateStudent.StudentName = student.StudentName;
                UpdateStudent.age = student.age;
                databaseSimpleMvcApiEntities.SaveChanges();
            }
            return new string[] { HttpStatusCode.OK.ToString(), "Record Updated!" };
        }

        [HttpPost]
        // DELETE: api/student/5
        public IEnumerable<string> DeleteStudent(int id)
        {
            var DeleteStudent = databaseSimpleMvcApiEntities.students.Where(x => x.StudentId == id).FirstOrDefault();
            databaseSimpleMvcApiEntities.students.Remove(DeleteStudent);
            databaseSimpleMvcApiEntities.SaveChanges();
            return new string[] { HttpStatusCode.OK.ToString(), "Record Deleted!" };
        }
    }
}