using System.Collections.Generic;
using simpleMvc.Api5.StoreProcedure.Dto;

namespace simpleMvc.Api5.StoreProcedure.Service
{
    internal interface TokenService
    {
        string GetUsernameWithToken(string token);
        string GenerateAccessToken(string username, List<RoleResponse> roles);
        string GenerateRefreshToken();
    }
}
