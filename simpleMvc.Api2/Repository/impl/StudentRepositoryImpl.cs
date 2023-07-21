using simpleMvc1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace simpleMvc.Api2.Repository.impl
{
    public class StudentRepositoryImpl : StudentRepository
    {
        Task<Student> StudentRepository.AddStudent(Student student)
        {
            throw new NotImplementedException();
        }

        Task StudentRepository.DeleteStudent(int studentId)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Student>> StudentRepository.GetAllStudents()
        {
            throw new NotImplementedException();
        }

        Task<Student> StudentRepository.GetStudent(int studentId)
        {
            throw new NotImplementedException();
        }

        Task<Student> StudentRepository.UpdateStudent(Student student)
        {
            throw new NotImplementedException();
        }
    }
}