﻿using System.Web;
using System.Web.Mvc;

namespace simpleMvc.Api3
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
