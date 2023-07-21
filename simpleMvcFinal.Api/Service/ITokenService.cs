using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using simpleMvcFinal.Api.Dto;

namespace simpleMvcFinal.Api.Service
{
    internal interface ITokenService
    {
        string GetUsernameWithToken(string token);
        string GenerateAccessToken(string username, List<RoleResponse> roles);
        bool IsAdmin(string token);
        string GenerateRefreshToken();
    }
}
