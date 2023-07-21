using simpleMvc.Api4.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleMvc.Api4.Service
{
    internal interface TokenService
    {
        string GenerateAccessToken(string username, List<RoleRespone> roles);
        string GenerateRefreshToken();
        void StoreRefreshToken(string refreshToken, string username, int userId);
        User GetUserFromRefreshToken(string refreshToken);
    }
}
