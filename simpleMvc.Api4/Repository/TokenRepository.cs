using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace simpleMvc.Api4.Repository
{
    public class TokenRepository
    {
        public DatabaseSimpleMvcEntities _context { get; set; }
        public TokenRepository() { 
            this._context = new DatabaseSimpleMvcEntities();
        }

    }
}