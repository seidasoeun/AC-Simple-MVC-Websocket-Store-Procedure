using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api4.Repository
{
    public class UserRepository
    {
        public DatabaseSimpleMvcEntities _context { get; set; }
        public UserRepository() {
            this._context = new DatabaseSimpleMvcEntities();
        }
    }
}