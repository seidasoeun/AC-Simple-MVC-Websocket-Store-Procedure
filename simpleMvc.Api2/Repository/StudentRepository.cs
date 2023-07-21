using simpleMvc1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleMvc.Api2.Repository
{
    internal interface StudentRepository
    {

        Task<Student> GetStudent(int studentId);
        Task<IEnumerable<Student>> GetAllStudents();
        Task<Student> AddStudent(Student student);

        Task<Student> UpdateStudent(Student student);
        Task DeleteStudent(int studentId);
    }
}
