using System.Collections.Generic;
using System.Data;
using simpleMvcFinal.Api.Dto;
using simpleMvcFinal.Api.Models;

namespace simpleMvcFinal.Api.Service
{
    internal interface IStoreProcedureService
    {
        string ProcessProcedureWithMessage(ProcedureModel model, out string message);
        DataTable ProcessProcedureWithDataTable(ProcedureModel model, out string message);
        UserResponse GetUserFromDataTable(DataTable data);

        List<UserResponse> GetUserListFromDataTable(DataTable data);

        List<RoleResponse> GetRoleFromDataTable(DataTable data);

    }
}
