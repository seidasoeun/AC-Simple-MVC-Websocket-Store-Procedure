using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StoreProcedure.Api1.Models;

namespace simpleMvc.Api5.StoreProcedure.Service
{
    internal interface ProcedureService
    {
        string ProcessProcedureWithMessage(ProcedureModel model, out string message);
        DataTable ProcessProcedureWithDataTable(ProcedureModel model, out string message);
    }
}
