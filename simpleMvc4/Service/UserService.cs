using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleMvc4.Service
{
    internal interface UserService
    {
        Task SignUp(string username, string email, string password);
        Task<string> Login(string email, string password);
    }
}
