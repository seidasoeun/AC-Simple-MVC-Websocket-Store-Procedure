using simpleMvc.Api5.Websocket.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleMvc.Api5.Websocket.Service
{
    internal interface TokenService
    {
        string GetUsernameWithToken(string token);
        string GenerateAccessToken(string username, List<RoleResponse> roles);
        string GenerateRefreshToken();
    }
}
